using Livet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using WinCap.Capturers;
using WinCap.Models;
using WinCap.Properties;
using WinCap.Util.Lifetime;
using WinCap.ViewModels;
using WinCap.Views;
using Settings = WinCap.Serialization.Settings;

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
                using (Bitmap bitmap = executeCapture(() => this.screenCapturer.CaptureFullScreen()))
                {
                    saveCaptureImage(bitmap);
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
                using (Bitmap bitmap = executeCapture(() => this.controlCapturer.CaptureActiveControl()))
                {
                    saveCaptureImage(bitmap);
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
                        using (Bitmap bitmap = executeCapture(() => this.controlCapturer.CaptureControl(viewModel.SelectedHandle)))
                        {
                            saveCaptureImage(bitmap);
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
                            using (Bitmap bitmap = executeCapture(() => this.ieCapturer.Capture(viewModel.SelectedHandle)))
                            {
                                saveCaptureImage(bitmap);
                            }
                        }
                        else
                        {
                            // 選択コントロールをキャプチャ
                            using (Bitmap bitmap = executeCapture(() => this.controlCapturer.CaptureControl(viewModel.SelectedHandle)))
                            {
                                saveCaptureImage(bitmap);
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
        /// キャプチャを実行します。
        /// </summary>
        /// <param name="action">キャプチャ処理メソッド</param>
        /// <returns>ビットマップ</returns>
        private Bitmap executeCapture(Func<Bitmap> action)
        {
            if (Settings.General.CaptureDelayTime > 0)
            {
                Thread.Sleep(Settings.General.CaptureDelayTime);
            }
            return action();
        }

        /// <summary>
        /// キャプチャ画像を保存します。
        /// </summary>
        /// <param name="bitmap">画像</param>
        private void saveCaptureImage(Bitmap bitmap)
        {
            var settings = Settings.Output;
            if (settings.OutputMethodType == OutputMethodType.Clipboard)
            {
                // 画像をクリップボードに設定する
                Clipboard.SetDataObject(bitmap, false);
            }
            else if (settings.OutputMethodType == OutputMethodType.ImageFile)
            {
                // 出力形式とファイルパスの設定
                OutputFormatType outputFormatType = settings.OutputFormatType;
                string fileExtension = outputFormatType.GetExtension();
                string fileName = FileHelper.CreateFileName(settings.OutputFolder, fileExtension, settings.OutputFileNamePattern);
                string filePath = Path.Combine(settings.OutputFolder, fileName) + fileExtension;

                // 自動保存の場合
                if (settings.IsAutoSaveImage)
                {
                    // 出力フォルダがない場合は作成する
                    if (!Directory.Exists(settings.OutputFolder))
                    {
                        Directory.CreateDirectory(settings.OutputFolder);
                    }
                }
                else
                {
                    // ファイルの保存場所を選択する
                    using (var dialog = new System.Windows.Forms.SaveFileDialog()
                    {
                        Filter = Resources.Services_SaveImageFileDialog_Filter,
                        InitialDirectory = settings.OutputFolder,
                        FilterIndex = (int)outputFormatType + 1,
                        Title = Resources.Services_SaveImageFileDialog_Title,
                        FileName = Path.GetFileName(fileName)
                    })
                    {
                        if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        {
                            return;
                        }
                        filePath = dialog.FileName;
                        outputFormatType = (OutputFormatType)dialog.FilterIndex;
                    }
                }

                // 画像ファイルに保存
                bitmap.Save(filePath, settings.OutputFormatType.Value.ToImageFormat());
            }

            // SE再生
            if (Settings.General.IsPlaySeWhenCapture)
            {
                SystemSounds.Asterisk.Play();
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
