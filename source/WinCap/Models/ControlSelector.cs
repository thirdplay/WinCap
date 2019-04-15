using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WinCap.Interop;
using WinCap.Interop.Win32;
using WinCap.Properties;

namespace WinCap.Models
{
    /// <summary>
    /// コントロール選択機能を提供します。
    /// </summary>
    public class ControlSelector
    {
        /// <summary>
        /// コントロール選択の選択状態を表します。
        /// </summary>
        public enum SelectionStatus
        {
            /// <summary>
            /// 選択中
            /// </summary>
            Selecting,

            /// <summary>
            /// 選択完了
            /// </summary>
            Completed,
        }

        /// <summary>
        /// ウィンドウハンドル取得時に除外するクラス名
        /// </summary>
        private readonly List<string> ignoreClassNames = new List<string>
        {
            "Intermediate D3D Window"
        };

        /// <summary>
        /// ウィンドウハンドルリスト
        /// </summary>
        private List<IntPtr> Handles { get; set; }

        /// <summary>
        /// 選択コントロールのハンドル
        /// </summary>
        public ReactiveProperty<IntPtr?> SelectedHandle { get; } = new ReactiveProperty<IntPtr?>();
        
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public ControlSelector()
        {
        }

        /// <summary>
        /// 初期化。
        /// </summary>
        public void Initialize()
        {
            Handles = GetHandles();
            SelectedHandle.Value = null;
        }

        /// <summary>
        /// 更新処理。
        /// </summary>
        /// <param name="point">マウス座標</param>
        public void Update(Point point)
        {
            SelectedHandle.Value = Handles?
                .Where(x =>
                {
                    var bounds = InteropHelper.GetWindowSize(x);
                    return bounds != Rectangle.Empty && bounds.Contains(point);
                })
                .Select(x => (IntPtr?)x)
                .FirstOrDefault();
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
                if (IsValidWindow(handle))
                {
                    // 子ウィンドウ判定
                    IntPtr hWndChild = User32.GetWindow(handle, GW.CHILD);
                    if (hWndChild != IntPtr.Zero)
                    {
                        GetHandles(hWndChild, ref list);
                    }
                    list.Add(handle);
                }
            } while ((handle = User32.GetWindow(handle, GW.HWNDNEXT)) != IntPtr.Zero);

            // 最後に除外対象のクラス名のハンドルを除外する
            list.RemoveAll(x =>
            {
                var className = InteropHelper.GetClassName(x);
                return (className.IndexOf(ProductInfo.Product) >= 0 || ignoreClassNames.Contains(className));
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
            if (IsValidWindow(handle))
            {
                IntPtr hWndChild = User32.GetWindow(handle, GW.CHILD);
                if (hWndChild != IntPtr.Zero)
                {
                    GetHandles(hWndChild, ref list);
                }
                list.Add(handle);
            }

            if ((handle = User32.GetWindow(handle, GW.HWNDNEXT)) != IntPtr.Zero)
            {
                GetHandles(handle, ref list);
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
