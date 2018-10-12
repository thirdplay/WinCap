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
        private readonly LivetCompositeDisposable compositeDisposable;

        /// <summary>
        /// キャプチャーを格納するコンテナ
        /// </summary>
        private readonly Dictionary<Type, ICapturer> container;

        /// <summary>
        /// フックサービス
        /// </summary>
        private readonly HookService hookService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CapturerService(HookService hookService, WindowService windowService)
        {
            this.hookService = hookService;
            this.compositeDisposable = new LivetCompositeDisposable();
            this.container = new Dictionary<Type, ICapturer>()
            {
                [typeof(ScreenCapturer)] = new ScreenCapturer(),
                [typeof(ActiveControlCapturer)] = new ActiveControlCapturer(),
                [typeof(ControlCapturer)] = new ControlCapturer(windowService),
                [typeof(RegionCapturer)] = new RegionCapturer(windowService),
                [typeof(WebBrowserCapturer)] = new WebBrowserCapturer(windowService),
            };
        }

        /// <summary>
        /// デスクトップ全体をキャプチャします。
        /// </summary>
        public void CaptureDesktop()
        {
            Capture<ScreenCapturer>();
        }

        /// <summary>
        /// アクティブコントロールをキャプチャします。
        /// </summary>
        public void CaptureActiveControl()
        {
            Capture<ActiveControlCapturer>();
        }

        /// <summary>
        /// 選択コントロールをキャプチャします。
        /// </summary>
        public void CaptureSelectionControl()
        {
            Capture<ControlCapturer>();
        }

        /// <summary>
        /// 選択範囲をキャプチャします。
        /// </summary>
        public void CaptureSelectionRegion()
        {
            Capture<RegionCapturer>();
        }

        /// <summary>
        /// ウェブページ全体をキャプチャします。
        /// </summary>
        public void CaptureWebPage()
        {
            Capture<WebBrowserCapturer>();
        }

        /// <summary>
        /// 指定キャプチャー型のキャプチャー処理を実行します。
        /// </summary>
        /// <typeparam name="TType">キャプチャー型</typeparam>
        private void Capture<TType>()
            where TType : ICapturer
        {
            using (this.hookService.Suspend())
            {
                this.container[typeof(TType)].Capture();
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
