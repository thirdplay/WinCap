using Livet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Reactive.Linq;
using System.Windows;
using WinCap.Capturers;
using WinCap.Models;
using WinCap.Serialization;
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
        private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();

        /// <summary>
        /// フックサービス
        /// </summary>
        private readonly HookService hookService;

        /// <summary>
        /// 画面キャプチャ
        /// </summary>
        private readonly ScreenCapturer screenCapturer = new ScreenCapturer();

        /// <summary>
        /// コントロールキャプチャ
        /// </summary>
        private readonly ControlCapturer controlCapturer = new ControlCapturer();

        /// <summary>
        /// IEキャプチャ
        /// </summary>
        private readonly IWebBrowserCapturer ieCapturer = new InternetExplorerCapturer();

        /// <summary>
        /// コントロール選択ウィンドウのViewModel
        /// </summary>
        private ControlSelectionWindowViewModel controlSelectionWindowViewModel;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CapturerService(HookService hookService)
        {
            this.hookService = hookService;
            this.controlSelectionWindowViewModel = new ControlSelectionWindowViewModel().AddTo(this);
        }

        /// <summary>
        /// デスクトップ全体をキャプチャします。
        /// </summary>
        public void CaptureDesktop()
        {
            using (this.hookService.Suspend())
            {
                // デスクトップ全体をキャプチャ
                using (Bitmap bitmap = this.screenCapturer.CaptureFullScreen())
                {
                    saveImage(bitmap);
                }
            }
        }

        /// <summary>
        /// アクティブコントロールをキャプチャします。
        /// </summary>
        public void CaptureActiveControl()
        {
            using (this.hookService.Suspend())
            {
                // アクティブウィンドウをキャプチャ
                using (Bitmap bitmap = this.controlCapturer.CaptureActiveControl())
                {
                    saveImage(bitmap);
                }
            }
        }

        /// <summary>
        /// 選択コントロールをキャプチャします。
        /// </summary>
        public void CaptureSelectionControl()
        {
            var suspended = this.hookService.Suspend();
            var viewModel = this.controlSelectionWindowViewModel;
            var window = new ControlSelectionWindow { DataContext = viewModel };
            Observable.FromEventPattern<EventArgs>(window, nameof(window.Closed))
            .Subscribe(x =>
            {
                using (suspended)
                {
                    if (viewModel.SelectedHandle != IntPtr.Zero)
                    {
                        // 選択コントロールをキャプチャ
                        using (Bitmap bitmap = this.controlCapturer.CaptureControl(viewModel.SelectedHandle))
                        {
                            saveImage(bitmap);
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
            var suspended = this.hookService.Suspend();
            var viewModel = this.controlSelectionWindowViewModel;
            var window = new ControlSelectionWindow { DataContext = viewModel };
            Observable.FromEventPattern<EventArgs>(window, nameof(window.Closed))
            .Subscribe(x =>
            {
                using (suspended)
                {
                    if (viewModel.SelectedHandle != IntPtr.Zero)
                    {
                        // キャプチャ可能か判定
                        string className = InteropHelper.GetClassName(viewModel.SelectedHandle);
                        if (this.ieCapturer.CanCapture(className))
                        {
                            // ウェブページ全体をキャプチャ
                            this.ieCapturer.IsScrollWindowPageTop = Settings.General.IsWebPageCaptureStartWhenPageFirstMove.Value;
                            this.ieCapturer.ScrollDelayTime = Settings.General.ScrollDelayTime.Value;
                            using (Bitmap bitmap = this.ieCapturer.Capture(viewModel.SelectedHandle))
                            {
                                saveImage(bitmap);
                            }
                        }
                        else
                        {
                            // 選択コントロールをキャプチャ
                            using (Bitmap bitmap = this.controlCapturer.CaptureControl(viewModel.SelectedHandle))
                            {
                                saveImage(bitmap);
                            }
                        }
                    }
                }
            });


            // 選択ウィンドウの表示
            window.Show();
            window.Activate();
        }

        /// <summary>
        /// キャプチャ画像を保存します。
        /// </summary>
        /// <param name="image">画像</param>
        private void saveImage(Bitmap image)
        {
            if (Settings.Output.OutputMethodType == OutputMethodType.Clipboard)
            {
                // 画像をクリップボードに設定する
                Clipboard.SetDataObject(image, true);
            }
            else if(Settings.Output.OutputMethodType == OutputMethodType.ImageFile)
            {
                // TODO:
                // ファイルパス確定
                // 画像ファイルに保存
            }
            // SE再生
            SystemSounds.Asterisk.Play();
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
