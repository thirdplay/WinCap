using Livet;
using System;
using System.Collections.Generic;
using WinCap.Capturers;
using WpfUtility.Lifetime;

namespace WinCap.Services
{
    /// <summary>
    /// 画面やコントロールをキャプチャする機能を提供します。
    /// </summary>
    public sealed class CapturerService : IDisposableHolder
    {
        /// <summary>
        /// 基本CompositeDisposable
        /// </summary>
        private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();

        /// <summary>
        /// フックサービス
        /// </summary>
        private readonly HookService hookService;

        /// <summary>
        /// ウィンドウサービス
        /// </summary>
        private readonly WindowService windowService;

        #region Captures

        /// <summary>
        /// 画面キャプチャ
        /// </summary>
        private readonly ScreenCapturer screenCapturer;

        /// <summary>
        /// アクティブコントロールキャプチャ
        /// </summary>
        private readonly ActiveControlCapturer activeControlCapturer;

        /// <summary>
        /// コントロールキャプチャ
        /// </summary>
        private readonly ControlCapturer controlCapturer;

        /// <summary>
        /// ウェブブラウザキャプチャ
        /// </summary>
        private readonly WebBrowserCapturer webBrowserCapturer;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CapturerService(HookService hookService, WindowService windowService)
        {
            this.hookService = hookService;
            this.windowService = windowService;
            this.screenCapturer = new ScreenCapturer();
            this.activeControlCapturer = new ActiveControlCapturer();
            this.controlCapturer = new ControlCapturer(windowService);
            this.webBrowserCapturer = new WebBrowserCapturer(windowService);
        }

        /// <summary>
        /// デスクトップ全体をキャプチャします。
        /// </summary>
        public void CaptureDesktop()
        {
            using (this.hookService.Suspend())
            {
                this.screenCapturer.Capture();
            }
        }

        /// <summary>
        /// アクティブコントロールをキャプチャします。
        /// </summary>
        public void CaptureActiveControl()
        {
            using (this.hookService.Suspend())
            {
                this.activeControlCapturer.Capture();
            }
        }

        /// <summary>
        /// 選択コントロールをキャプチャします。
        /// </summary>
        public void CaptureSelectionControl()
        {
            using (this.hookService.Suspend())
            {
                this.controlCapturer.Capture();
            }
        }

        /// <summary>
        /// ウェブページ全体をキャプチャします。
        /// </summary>
        public void CaptureWebPage()
        {
            using (this.hookService.Suspend())
            {
                this.webBrowserCapturer.Capture();
            }
        }

        #region IDisposableHoloder members

        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        public void Dispose()
        {
            this.compositeDisposable.Dispose();
        }

        #endregion
    }
}
