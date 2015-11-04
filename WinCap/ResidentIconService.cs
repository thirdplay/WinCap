using Livet;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using WinCap.Components;
using WinCap.Utilities.Lifetime;
using WinCap.Properties;
using WinCap.Models;

namespace WinCap
{
    /// <summary>
    /// 常駐アイコンサービス
    /// </summary>
    public sealed class ResidentIconService : IDisposableHolder
    {
        #region フィールド
        /// <summary>
        /// 基本CompositeDisposable
        /// </summary>
        private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();
        #endregion

        #region プロパティ
        /// <summary>
        /// 現在のサービスを取得する。
        /// </summary>
        public static ResidentIconService Current { get; } = new ResidentIconService();
        #endregion

        /// <summary>
        /// 常駐アイコン
        /// </summary>
        private ResidentIcon notifyIcon;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private ResidentIconService() { }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            // 常駐アイコンの生成
            this.notifyIcon = new ResidentIcon(Settings.Default.IconUri, Assembly.GetExecutingAssembly().GetName().Name).AddTo(this);

            // コンテキストメニューの設定
            var contextMenu = this.notifyIcon.ContextMenuStrip;
            var contextMenuCapture = contextMenu.Items.Add(Resources.ContextMenu_Capture) as System.Windows.Forms.ToolStripMenuItem;
            contextMenuCapture.DropDownItems.Add(Resources.ContextMenu_ScreenCapture).Click += (sender, e) => CaptureService.Current.CaptureWholeScreen();
            contextMenuCapture.DropDownItems.Add(Resources.ContextMenu_ControlCapture);
            contextMenuCapture.DropDownItems.Add(Resources.ContextMenu_PageCapture);
            contextMenu.Items.Add(Resources.ContextMenu_Option);
            contextMenu.Items.Add(Resources.ContextMenu_Help).Click += (sender, e) => CaptureService.Current.CaptureSelectControl();
            contextMenu.Items.Add(Resources.ContextMenu_Exit).Click += (sender, e) => Application.Current.Shutdown();
        }

        #region IDisposableHoloder members
        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄する。
        /// </summary>
        public void Dispose()
        {
            this.compositeDisposable.Dispose();
        }
        #endregion
    }
}
