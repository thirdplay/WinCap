using Livet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using WinCap.Capturers;
using WinCap.Interop;
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
        /// 画面キャプチャ
        /// </summary>
        private readonly ScreenCapturer screenCapturer = new ScreenCapturer();

        /// <summary>
        /// コントロールキャプチャ
        /// </summary>
        private readonly ControlCapturer controlCapturer = new ControlCapturer();

        /// <summary>
        /// WebBrowserキャプチャ
        /// </summary>
        private readonly IWebBrowserCapturer[] webBrowserCapturers = {
            new InternetExplorerCapturer()
        };

        /// <summary>
        /// コントロール選択ウィンドウのViewModel
        /// </summary>
        private readonly ControlSelectionWindowViewModel controlSelectionWindowViewModel;

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
                ExecuteCapture(() => this.screenCapturer.CaptureFullScreen());
            }
        }

        /// <summary>
        /// アクティブコントロールをキャプチャします。
        /// </summary>
        public void CaptureActiveControl()
        {
            using (this.hookService.Suspend())
            {
                ExecuteCapture(() => this.controlCapturer.CaptureActiveControl());
            }
        }

        /// <summary>
        /// 選択コントロールをキャプチャします。
        /// </summary>
        public void CaptureSelectionControl()
        {
            using (this.hookService.Suspend())
            {
                var viewModel = this.controlSelectionWindowViewModel;
                var window = new ControlSelectionWindow { DataContext = viewModel };
                viewModel.Initialized = () => window.Activate();
                viewModel.Selected = () =>
                {
                    ExecuteCapture(() => this.controlCapturer.CaptureControl(viewModel.SelectedHandle));
                };
                window.ShowDialog();
                window.Close();
            }
        }

        /// <summary>
        /// ウェブページ全体をキャプチャします。
        /// </summary>
        public void CaptureWebPage()
        {
            using (this.hookService.Suspend())
            {
                var viewModel = this.controlSelectionWindowViewModel;
                var window = new ControlSelectionWindow { DataContext = viewModel };
                viewModel.Initialized = () => window.Activate();
                viewModel.Selected = () =>
                {
                    // キャプチャ可能か判定
                    string className = InteropExtensions.GetClassName(viewModel.SelectedHandle);
                    var capturer = this.webBrowserCapturers.Where(_ => _.CanCapture(className)).FirstOrDefault();
                    if (capturer != null)
                    {
                        // ウェブページ全体をキャプチャ
                        capturer.IsScrollWindowPageTop = Settings.General.IsWebPageCaptureStartWhenPageFirstMove.Value;
                        capturer.ScrollDelayTime = Settings.General.ScrollDelayTime.Value;
                        ExecuteCapture(() => capturer.Capture(viewModel.SelectedHandle));
                    }
                    else
                    {
                        // 選択コントロールをキャプチャ
                        ExecuteCapture(() => this.controlCapturer.CaptureControl(viewModel.SelectedHandle));
                    }
                };
                window.ShowDialog();
                window.Close();
            }
        }

        /// <summary>
        /// キャプチャ処理を実行します。
        /// </summary>
        /// <param name="action">キャプチャ処理アクション</param>
        private void ExecuteCapture(Func<Bitmap> action)
        {
            try
            {
                if (Settings.General.CaptureDelayTime > 0)
                {
                    Thread.Sleep(Settings.General.CaptureDelayTime);
                }
                using (Bitmap bitmap = action())
                {
                    SaveCaptureImage(bitmap);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.ReportException(this, ex, false);
            }
        }

        /// <summary>
        /// キャプチャ画像を保存します。
        /// </summary>
        /// <param name="bitmap">画像</param>
        private void SaveCaptureImage(Bitmap bitmap)
        {
            var settings = Settings.Output;
            if (settings.OutputMethodType == OutputMethodType.Clipboard)
            {
                // 画像をクリップボードに設定する
                Clipboard.SetImage(bitmap.ToBitmapSource());
            }
            else if (settings.OutputMethodType == OutputMethodType.ImageFile)
            {
                // 出力形式とファイルパスの設定
                OutputFormatType outputFormatType = settings.OutputFormatType;
                string fileExtension = outputFormatType.GetExtension();
                string fileName = FileHelper.CreateFileName(settings.OutputFolder, fileExtension, settings.OutputFileNamePattern);
                string filePath = fileName + fileExtension;
                if (!string.IsNullOrEmpty(settings.OutputFolder))
                {
                    filePath = Path.Combine(settings.OutputFolder, filePath);
                }

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
                bitmap.Save(filePath, outputFormatType.ToImageFormat());
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
