using System;
using System.Drawing;

namespace WinCap.Components
{
    /// <summary>
    /// 常駐アイコン
    /// </summary>
    public sealed class ResidentIcon : IDisposable
    {
        /// <summary>
        /// 通知アイコン
        /// </summary>
        private System.Windows.Forms.NotifyIcon notifyIcon;

        /// <summary>
        /// コンテキストメニュー
        /// </summary>
        public System.Windows.Forms.ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return this.notifyIcon.ContextMenuStrip;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="iconUri">アイコンURI</param>
        /// <param name="text">常駐アイコンのテキスト</param>
        /// <param name="visible">表示状態</param>
        public ResidentIcon(string iconUri, string text, bool visible = true)
        {
            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon.Icon = new Icon(System.Windows.Application.GetResourceStream(new Uri(iconUri)).Stream);
            this.notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            this.notifyIcon.Visible = visible;
        }

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        public void Dispose()
        {
            this.notifyIcon.Dispose();
        }
    }
}
