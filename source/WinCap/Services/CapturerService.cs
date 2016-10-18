using Livet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Reactive.Linq;
using System.Windows;
using WinCap.Capturers;
using WinCap.Interop;
using WinCap.Util.Lifetime;
using WinCap.ViewModels;
using WinCap.Views;

namespace WinCap.Services
{
    /// <summary>
    /// 画面やコントロールをキャプチャする機能を提供します。
    /// </summary>
    public sealed class CapturerService : NotificationObject, IDisposableHolder
    {
        /// <summary>
        /// 基本CompositeDisposable
        /// </summary>
        private readonly LivetCompositeDisposable _compositeDisposable = new LivetCompositeDisposable();

        /// <summary>
        /// フックサービス
        /// </summary>
        private readonly HookService _hookService;

        /// <summary>
        /// 画面キャプチャ
        /// </summary>
        private readonly ScreenCapturer _screenCapturer = new ScreenCapturer();

        /// <summary>
        /// コントロールキャプチャ
        /// </summary>
        private readonly ControlCapturer _controlCapturer = new ControlCapturer();

        /// <summary>
        /// IEキャプチャ
        /// </summary>
        private readonly IWebBrowserCapturer _ieCapturer = new InternetExplorerCapturer();

        /// <summary>
        /// コントロール選択ウィンドウのViewModel
        /// </summary>
        private ControlSelectionWindowViewModel _controlSelectionWindowViewModel;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CapturerService(HookService hookService)
        {
            _hookService = hookService;
            _controlSelectionWindowViewModel = new ControlSelectionWindowViewModel().AddTo(this);
        }

        /// <summary>
        /// デスクトップ全体をキャプチャします。
        /// </summary>
        public void CaptureDesktop()
        {
            using (_hookService.Suspend())
            {
                // デスクトップ全体をキャプチャ
                using (Bitmap bitmap = _screenCapturer.CaptureFullScreen())
                {
                    // TODO:save(bitmap); => Clipboard or Bitmap(ファイル名選定込み）
                    // キャプチャした画像をクリップボードに設定
                    Clipboard.SetDataObject(bitmap, true);
                    SystemSounds.Asterisk.Play();
                }
            }
        }

        /// <summary>
        /// アクティブコントロールをキャプチャします。
        /// </summary>
        public void CaptureActiveControl()
        {
            using (_hookService.Suspend())
            {
                // アクティブウィンドウをキャプチャ
                using (Bitmap bitmap = _controlCapturer.CaptureActiveControl())
                {
                    // TODO:save(bitmap); => Clipboard or Bitmap(ファイル名選定込み）
                    // キャプチャした画像をクリップボードに設定
                    Clipboard.SetDataObject(bitmap, true);
                    SystemSounds.Asterisk.Play();
                }
            }
        }

        /// <summary>
        /// 選択コントロールをキャプチャします。
        /// </summary>
        public void CaptureSelectionControl()
        {
            var susspendReq = this._hookService.Suspend();
            var viewModel = this._controlSelectionWindowViewModel;
            var window = new ControlSelectionWindow { DataContext = viewModel };
            Observable.FromEventPattern<EventArgs>(window, nameof(window.Closed))
            .Subscribe(x =>
            {
                using (susspendReq)
                {
                    if (viewModel.SelectedHandle != IntPtr.Zero)
                    {
                        // 選択コントロールをキャプチャ
                        using (Bitmap bitmap = _controlCapturer.CaptureControl(viewModel.SelectedHandle))
                        {
                            // キャプチャした画像をクリップボードに設定
                            Clipboard.SetDataObject(bitmap, true);
                            SystemSounds.Asterisk.Play();
                        }
                    }
                }
            });

            // 選択ウィンドウの表示
            window.Show();
            window.Activate();
        }

        /// <summary>
        /// ウェブページ全体をキャプチャします。
        /// </summary>
        public void CaptureWebPage()
        {
            var susspendReq = this._hookService.Suspend();
            var viewModel = this._controlSelectionWindowViewModel;
            var window = new ControlSelectionWindow { DataContext = viewModel };
            Observable.FromEventPattern<EventArgs>(window, nameof(window.Closed))
            .Subscribe(x =>
            {
                using (susspendReq)
                {
                    if (viewModel.SelectedHandle != IntPtr.Zero)
                    {
                        // キャプチャ可能か判定
                        string className = NativeMethods.GetClassName(viewModel.SelectedHandle);
                        if (_ieCapturer.CanCapture(className))
                        {
                            // ウェブページ全体をキャプチャ
                            using (Bitmap bitmap = _ieCapturer.Capture(viewModel.SelectedHandle))
                            {
                                // キャプチャした画像をクリップボードに設定
                                Clipboard.SetDataObject(bitmap, true);
                                SystemSounds.Asterisk.Play();
                            }
                        }
                        else
                        {
                            // 選択コントロールをキャプチャ
                            using (Bitmap bitmap = _controlCapturer.CaptureControl(viewModel.SelectedHandle))
                            {
                                // キャプチャした画像をクリップボードに設定
                                Clipboard.SetDataObject(bitmap, true);
                                SystemSounds.Asterisk.Play();
                            }
                        }
                    }
                }
            });


            // 選択ウィンドウの表示
            window.Show();
            window.Activate();
        }

        #region IDisposableHoloder members
        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this._compositeDisposable;

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        public void Dispose()
        {
            this._compositeDisposable.Dispose();
        }
        #endregion
    }
}
