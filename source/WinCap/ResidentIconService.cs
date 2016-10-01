//using Livet;
//using System;
//using System.Collections.Generic;
//using WinCap.Properties;
//using WinCap.Models;
//using WinCap.Utility.Lifetime;

//namespace WinCap
//{
//    /// <summary>
//    /// 常駐アイコンの表示する機能を提供します。
//    /// </summary>
//    public sealed class ResidentIconService : IDisposableHolder
//    {
//        #region フィールド
//        /// <summary>
//        /// 基本CompositeDisposable
//        /// </summary>
//        private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();
//        #endregion

//        #region プロパティ
//        /// <summary>
//        /// 現在のサービスを取得する。
//        /// </summary>
//        public static ResidentIconService Current { get; } = new ResidentIconService();
//        #endregion

//        /// <summary>
//        /// 常駐アイコン
//        /// </summary>
//        private ResidentIcon notifyIcon;

//        /// <summary>
//        /// コンストラクタ
//        /// </summary>
//        private ResidentIconService() { }

//        /// <summary>
//        /// 初期化
//        /// </summary>
//        public void Initialize()
//        {
//            // 常駐アイコンの生成
//            this.notifyIcon = new ResidentIcon(Settings.Default.IconUri, ProductInfo.Title).AddTo(this);

//            // コンテキストメニューの設定
//            var contextMenu = this.notifyIcon.ContextMenuStrip;
//            var contextMenuCapture = contextMenu.Items.Add(Resources.ContextMenu_Capture) as System.Windows.Forms.ToolStripMenuItem;
//            contextMenuCapture.DropDownItems.Add(Resources.ContextMenu_ScreenCapture).Click += (sender, e) => CaptureService.Current.CaptureScreenWhole();
//            contextMenuCapture.DropDownItems.Add(Resources.ContextMenu_ControlCapture).Click += (sender, e) => CaptureService.Current.CaptureSelectControl();
//            contextMenuCapture.DropDownItems.Add(Resources.ContextMenu_PageCapture);
//            contextMenu.Items.Add(Resources.ContextMenu_Setting).Click += (sender, e) => WindowService.Current.GetSettingWindow().Show();
//            contextMenu.Items.Add(Resources.ContextMenu_Help).Click += (sender, e) => CaptureService.Current.CaptureSelectControl();
//            contextMenu.Items.Add(Resources.ContextMenu_Exit).Click += (sender, e) => Application.Current.Shutdown();
//        }

//        #region IDisposableHoloder members
//        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

//        /// <summary>
//        /// このインスタンスによって使用されているリソースを全て破棄します。
//        /// </summary>
//        public void Dispose()
//        {
//            this.compositeDisposable.Dispose();
//        }
//        #endregion
//    }
//}
