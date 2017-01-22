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
        /// コンストラクタ
        /// </summary>
        public CapturerService(HookService hookService)
        {
            this.hookService = hookService;
        }

        /// <summary>
        /// デスクトップ全体をキャプチャします。
        /// </summary>
        public void CaptureDesktop()
        {
            using (this.hookService.Suspend())
            using (var bitmap = executeCapture(() => this.screenCapturer.CaptureFullScreen()))
            {
                saveCaptureImage(bitmap);
            }
        }

        /// <summary>
        /// アクティブコントロールをキャプチャします。
        /// </summary>
        public void CaptureActiveControl()
        {
            using (this.hookService.Suspend())
            using (var bitmap = executeCapture(() => this.controlCapturer.CaptureActiveControl()))
            {
                saveCaptureImage(bitmap);
            }
        }

        /// <summary>
        /// 選択コントロールをキャプチャします。
        /// </summary>
        public void CaptureSelectionControl()
        {
            using (this.hookService.Suspend())
            using (var viewModel = new ControlSelectionWindowViewModel())
            {
                var window = new ControlSelectionWindow { DataContext = viewModel };
                viewModel.Initialized += (s, e) => window.Activate();
                viewModel.Selected += (s, e) =>
                {
                    using (var bitmap = executeCapture(() => this.controlCapturer.CaptureControl(viewModel.SelectedHandle)))
                    {
                        saveCaptureImage(bitmap);
                    }
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
            using (var viewModel = new ControlSelectionWindowViewModel())
            {
                var window = new ControlSelectionWindow { DataContext = viewModel };
                viewModel.Initialized += (s, e) => window.Activate();
                viewModel.Selected += (s, e) =>
                {
                    // キャプチャ可能か判定
                    string className = InteropHelper.GetClassName(viewModel.SelectedHandle);
                    var capturer = this.webBrowserCapturers.Where(_ => _.CanCapture(className)).FirstOrDefault();
                    if (capturer != null)
                    {
                        // ウェブページ全体をキャプチャ
                        capturer.IsScrollWindowPageTop = Settings.General.IsWebPageCaptureStartWhenPageFirstMove.Value;
                        capturer.ScrollDelayTime = Settings.General.ScrollDelayTime.Value;
                        using(var bitmap = executeCapture(() => capturer.Capture(viewModel.SelectedHandle)))
                        {
                            saveCaptureImage(bitmap);
                        }
                    }
                    else
                    {
                        // 選択コントロールをキャプチャ
                        using(var bitmap = executeCapture(() => this.controlCapturer.CaptureControl(viewModel.SelectedHandle)))
                        {
                            saveCaptureImage(bitmap);
                        }
                    }
                };
                window.ShowDialog();
                window.Close();
            }
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
                Clipboard.Clear();
                Clipboard.SetDataObject(bitmap.ToBitmapSource(), false);
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
