using Livet;
using System;
using System.Collections.Generic;
using System.Drawing;
using WinCap.Interop;
using WinCap.Interop.Win32;
using WinCap.Properties;

namespace WinCap.Models
{
    /// <summary>
    /// コントロール選択ウィンドウのロジックを提供します。
    /// </summary>
    public class ControlSelection : NotificationObject
    {
        /// <summary>
        /// ウィンドウハンドルリスト。
        /// </summary>
        private List<IntPtr> handles;

        #region SelectedHandle 変更通知プロパティ
        private IntPtr _SelectedHandle;
        /// <summary>
        /// 選択コントロールのハンドルを取得します。
        /// </summary>
        public IntPtr SelectedHandle
        {
            get { return _SelectedHandle; }
            set
            { 
                if (_SelectedHandle != value)
                {
                    _SelectedHandle = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
        
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public ControlSelection()
        {
        }

        /// <summary>
        /// 初期化。
        /// </summary>
        public void Initialize()
        {
            this.handles = this.GetHandles();
            this._SelectedHandle = IntPtr.Zero;
        }

        /// <summary>
        /// マウス座標を更新します。
        /// </summary>
        /// <param name="point">マウス座標</param>
        public void UpdateMousePoint(Point point)
        {
            var selectedHandle = IntPtr.Zero;
            foreach (var handle in this.handles)
            {
                Rectangle bounds = handle.GetWindowBounds();
                if (bounds != Rectangle.Empty && bounds.Contains(point))
                {
                    selectedHandle = handle;
                    break;
                }
            }
            this.SelectedHandle = selectedHandle;
        }

        /// <summary>
        /// 指定座標上に表示されているウィンドウのハンドルを取得します。
        /// </summary>
        /// <param name="point">座標</param>
        /// <returns>ウィンドウハンドル</returns>
        public IntPtr GetWindowHandle(Point point)
        {
            foreach (var handle in this.handles)
            {
                Rectangle bounds = handle.GetWindowBounds();
                if (bounds != Rectangle.Empty && bounds.Contains(point))
                {
                    return handle;
                }
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// 表示順にソートしたウィンドウハンドルを取得します。
        /// </summary>
        /// <returns>ウィンドウハンドルリスト</returns>
        private List<IntPtr> GetHandles()
        {
            List<IntPtr> list = new List<IntPtr>();

            // 一番手前のウィンドウを取得
            IntPtr handle = User32.GetForegroundWindow();
            IntPtr hWndPrev;
            while ((hWndPrev = User32.GetWindow(handle, GW.HWNDPREV)) != IntPtr.Zero)
            {
                handle = hWndPrev;
            }

            do
            {
                // 対象ウィンドウか判定
                if (this.IsValidWindow(handle))
                {
                    // 子ウィンドウ判定
                    IntPtr hWndChild = User32.GetWindow(handle, GW.CHILD);
                    if (hWndChild != IntPtr.Zero)
                    {
                        this.GetHandles(hWndChild, ref list);
                    }
                    list.Add(handle);
                }
            } while ((handle = User32.GetWindow(handle, GW.HWNDNEXT)) != IntPtr.Zero);

            // クラス名にWinCapを含むウィンドウは除外する
            list.RemoveAll(x => x.GetClassName().IndexOf(ProductInfo.Product) >= 0);
            return list;
        }

        /// <summary>
        /// 指定ウィンドウとそれに紐付く子ウィンドウを全て取得します。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <param name="list">ウィンドウハンドルの格納先</param>
        private void GetHandles(IntPtr handle, ref List<IntPtr> list)
        {
            if (this.IsValidWindow(handle))
            {
                IntPtr hWndChild = User32.GetWindow(handle, GW.CHILD);
                if (hWndChild != IntPtr.Zero)
                {
                    this.GetHandles(hWndChild, ref list);
                }
                list.Add(handle);
            }

            if ((handle = User32.GetWindow(handle, GW.HWNDNEXT)) != IntPtr.Zero)
            {
                this.GetHandles(handle, ref list);
            }
        }

        /// <summary>
        /// 指定ウィンドウをキャプチャ対象ウィンドウとするか返します。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>対象ならtrue、それ以外はfalse</returns>
        private bool IsValidWindow(IntPtr handle)
        {
            // ウィンドウの表示状態を取得
            int visible = User32.IsWindowVisible(handle);
            if (visible != 0)
            {
                // 矩形情報を取得
                Rectangle rect = handle.GetWindowBounds();
                if (rect.Width > 0 && rect.Height > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
