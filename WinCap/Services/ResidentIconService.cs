using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using WinCap.Components;
using WinCap.Lifetime;
using WinCap.Properties;

namespace WinCap.Services
{
    /// <summary>
    /// 常駐アイコンサービス
    /// </summary>
    public class ResidentIconService : ServiceBase
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static ResidentIconService Current { get; } = new ResidentIconService();

        /// <summary>
        /// 常駐アイコン
        /// </summary>
        private ResidentIcon notifyIcon;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            // 常駐アイコンの生成
            this.notifyIcon = new ResidentIcon(Settings.Default.IconUri, Assembly.GetExecutingAssembly().GetName().Name).AddTo(this);

            // コンテキストメニューの設定
            var contextMenu = this.notifyIcon.ContextMenuStrip;
            contextMenu.ShowImageMargin = false;
            var contextMenuCapture = contextMenu.Items.Add(Resources.ContextMenu_Capture) as System.Windows.Forms.ToolStripMenuItem;
            contextMenuCapture.DropDownItems.Add(Resources.ContextMenu_ScreenCapture);
            contextMenuCapture.DropDownItems.Add(Resources.ContextMenu_ControlCapture);
            contextMenuCapture.DropDownItems.Add(Resources.ContextMenu_PageCapture);
            contextMenu.Items.Add(Resources.ContextMenu_Option, null, ContextMenuOption_Click);
            contextMenu.Items.Add(Resources.ContextMenu_Help, null, ContextMenuHelp_Click);
            contextMenu.Items.Add(Resources.ContextMenu_Exit, null, ContextMenuClose_Click);
        }

        /// <summary>
        /// オプションクリックイベント。
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void ContextMenuOption_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// ヘルプクリックイベント。
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void ContextMenuHelp_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 閉じるクリックイベント。
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void ContextMenuClose_Click(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
