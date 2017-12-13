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

        /// <summary>
        /// ウィンドウハンドル取得時に除外するクラス名
        /// </summary>
        private List<string> ignoreClassNames = new List<string>
        {
            "Intermediate D3D Window"
        };

        /// <summary>
        /// Initializeメソッドが呼ばれたかどうかを示す値を取得します。
        /// </summary>
        public bool IsInitialized { get; private set; }

        #region SelectedHandle 変更通知プロパティ
        private IntPtr _SelectedHandle;
        /// <summary>
        /// 選択コントロールのハンドルを取得します。
        /// </summary>
        public IntPtr SelectedHandle
        {
            get { return this._SelectedHandle; }
            set
            { 
                if (this._SelectedHandle != value)
                {
                    this._SelectedHandle = value;
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
            this.IsInitialized = true;
        }

        /// <summary>
        /// マウス座標を更新します。
        /// </summary>
        /// <param name="point">マウス座標</param>
        public void UpdateMousePoint(Point point)
        {
            if (!this.IsInitialized) return;

            var selectedHandle = IntPtr.Zero;
            foreach (var handle in this.handles)
            {
                Rectangle bounds = InteropHelper.GetWindowSize(handle);
                if (bounds != Rectangle.Empty && bounds.Contains(point))
                {
                    selectedHandle = handle;
                    break;
                }
            }
            this.SelectedHandle = selectedHandle;
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

            // 除外対象のクラス名のハンドルを除外する
            list.RemoveAll(x =>
            {
                var className = InteropHelper.GetClassName(x);
                return (className.IndexOf(ProductInfo.Product) >= 0 || this.ignoreClassNames.Contains(className));
            });

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
        /// 指定ウィンドウハンドルがキャプチャ対象か返します。
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
                Rectangle rect = InteropHelper.GetWindowSize(handle);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
