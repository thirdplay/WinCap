using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using WinCap.Interop.Win32;

namespace WinCap.ShortcutKeys
{
    /// <summary>
    /// ホットキーを検出する機能を提供します。
    /// ([modifier key(s)] + [key] style)
    /// </summary>
    public class HotkeyDetector : IDisposable
    {
        /// <summary>
        /// ホットキーIDの最大値
        /// </summary>
        private const int HotkeyIdMax = 0xc000;

        /// <summary>
        /// ウィンドウハンドル
        /// </summary>
        private IntPtr windowHandle;

        /// <summary>
        /// ホットキーIDに対応するアクション。
        /// </summary>
        private readonly Dictionary<int, Action> hotkeyActions = new Dictionary<int, Action>();

        /// <summary>
        /// 次に登録するホットキーID。
        /// </summary>
        private int nextHotkeyId = 0;

        /// <summary>
        /// 停止状態
        /// </summary>
        private bool suspended;

        /// <summary>
        /// 停止状態
        /// </summary>
        public bool IsSuspended => this.suspended;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="window">メインウィンドウ</param>
        public HotkeyDetector(Window window)
        {
            // WindowのHandleを取得
            WindowInteropHelper host = new WindowInteropHelper(window);
            this.windowHandle = host.Handle;

            // ホットキーのイベントハンドラを設定
            ComponentDispatcher.ThreadPreprocessMessage += this.ThreadPreprocessMessage;
        }

        /// <summary>
        /// ショートカットの監視を開始します。
        /// </summary>
        public void Start()
        {
            this.suspended = false;
        }

        /// <summary>
        /// ショートカットの監視を停止します。
        /// </summary>
        public void Stop()
        {
            this.suspended = true;
        }

        /// <summary>
        /// HotKeyを登録します。
        /// </summary>
        /// <param name="shortcutKey">ショートカットキー</param>
        /// <param name="action">アクション</param>
        /// <returns>成功の場合はtrue、それ以外はfalseを返します。</returns>
        public bool Register(ShortcutKey shortcutKey, Action action)
        {
            var modifier = (int)shortcutKey.ModifierKeys;
            var trigger = (int)shortcutKey.Key.ToVirtualKey();

            // HotKey登録時に指定するIDを決定する
            // 0x0000～0xbfff はIDとして使用可能
            if (this.nextHotkeyId < HotkeyIdMax && User32.RegisterHotKey(this.windowHandle, this.nextHotkeyId, modifier, trigger))
            {
                this.hotkeyActions.Add(this.nextHotkeyId, action);
                this.nextHotkeyId++;
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// HotKeyを全て解放します。
        /// </summary>
        public void Unregister()
        {
            foreach (var hotkeyId in this.hotkeyActions.Keys)
            {
                User32.UnregisterHotKey(this.windowHandle, hotkeyId);
            }
            this.hotkeyActions.Clear();
            this.nextHotkeyId = 0;
        }

        /// <summary>
        /// HotKeyの動作を設定します。
        /// </summary>
        /// <param name="msg">メッセージ情報</param>
        /// <param name="handled">メッセージを処理したかどうか</param>
        private void ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (this.suspended) { return; }
            if (msg.message != (int)WM.HOTKEY) { return; }

            // 自分が登録したホットキーか否か
            var hotkeyID = msg.wParam.ToInt32();
            if (!this.hotkeyActions.ContainsKey(hotkeyID))
            {
                return;
            }

            // 両方を満たす場合は登録してあるホットキーのイベントを実行
            this.hotkeyActions[hotkeyID]?.Invoke();
            handled = true;
        }

        #region IDisposable members

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        public void Dispose()
        {
            this.Stop();
            this.Unregister();
        }

        #endregion
    }
}
