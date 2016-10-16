using Livet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Reactive.Linq;
using System.Windows;
using WinCap.Capturers;
using WinCap.Interop;
using WinCap.Models;
using WinCap.Util.Lifetime;
using WinCap.Views;

namespace WinCap.Services
{
    /// <summary>
    /// 画面やウィンドウをキャプチャし、クリップボードや画像ファイルに出力する機能を提供します。
    /// </summary>
    public sealed class CapturableService : NotificationObject, IDisposableHolder
    {
        #region フィールド
        /// <summary>
        /// 基本CompositeDisposable
        /// </summary>
        private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();

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
        /// 現在の状態
        /// </summary>
        private CapturableServiceStatus currentStatus = CapturableServiceStatus.Waiting;
        #endregion

        #region プロパティ
        /// <summary>
        /// 現在のウィンドウサービス
        /// </summary>
        public static CapturableService Current { get; } = new CapturableService();

        /// <summary>
        /// 現在の状態を取得します。
        /// </summary>
        public CapturableServiceStatus Status
        {
            get { return this.currentStatus; }
            private set
            {
                if (this.currentStatus != value)
                {
                    this.currentStatus = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private CapturableService()
        {
            //this.ObserveProperty(nameof(this.Status))
            //    .Where(_ => this.currentStatus == CaptureServiceStatus.CaptureCompletion)
            //    .Subscribe(_ => SystemSounds.Asterisk.Play())
            //    .AddTo(this);
        }

        /// <summary>
        /// デスクトップ全体をキャプチャします。
        /// </summary>
        public void CaptureDesktop()
        {
            // 待機状態以外は処理しない
            if (this.Status != CapturableServiceStatus.Waiting) return;
            this.Status = CapturableServiceStatus.Capturing;

            // デスクトップ全体をキャプチャ
            using (Bitmap bitmap = _screenCapturer.CaptureFullScreen())
            {
                // TODO:save(bitmap); => Clipboard or Bitmap(ファイル名選定込み）
                // キャプチャした画像をクリップボードに設定
                Clipboard.SetDataObject(bitmap, true);
                SystemSounds.Asterisk.Play();
            }

            // 待機状態に戻す
            this.Status = CapturableServiceStatus.Waiting;
        }

        /// <summary>
        /// アクティブコントロールをキャプチャします。
        /// </summary>
        public void CaptureActiveControl()
        {
            // 待機状態以外は処理しない
            if (this.Status != CapturableServiceStatus.Waiting) return;
            this.Status = CapturableServiceStatus.Capturing;

            // アクティブウィンドウをキャプチャ
            using (Bitmap bitmap = _controlCapturer.CaptureActiveControl())
            {
                // TODO:save(bitmap); => Clipboard or Bitmap(ファイル名選定込み）
                // キャプチャした画像をクリップボードに設定
                Clipboard.SetDataObject(bitmap, true);
                SystemSounds.Asterisk.Play();
            }

            // 待機状態に戻す
            this.Status = CapturableServiceStatus.Waiting;
        }

        /// <summary>
        /// 選択コントロールをキャプチャします。
        /// </summary>
        public void CaptureSelectionControl()
        {
            // 待機状態以外は処理しない
            if (this.Status != CapturableServiceStatus.Waiting) return;
            this.Status = CapturableServiceStatus.Capturing;

            // コントロール選択ウィンドウの取得
            var window = WindowService.Current.GetControlSelectionWindow();
            Observable.FromEventPattern<SelectedEventArgs>(window, nameof(window.Selected))
                .Subscribe(x =>
                {
                    window.Close();
                    if (x.EventArgs.Handle != IntPtr.Zero)
                    {
                        // 選択コントロールをキャプチャ
                        using (Bitmap bitmap = _controlCapturer.CaptureControl(x.EventArgs.Handle))
                        {
                            // キャプチャした画像をクリップボードに設定
                            Clipboard.SetDataObject(bitmap, true);
                            SystemSounds.Asterisk.Play();
                        }
                    }

                    // 待機状態に戻す
                    this.Status = CapturableServiceStatus.Waiting;
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
            // 待機状態以外は処理しない
            if (this.Status != CapturableServiceStatus.Waiting) return;
            this.Status = CapturableServiceStatus.Capturing;

            // コントロール選択ウィンドウの取得
            var window = WindowService.Current.GetControlSelectionWindow();
            Observable.FromEventPattern<SelectedEventArgs>(window, nameof(window.Selected))
                .Subscribe(x =>
                {
                    window.Close();
                    if (x.EventArgs.Handle != IntPtr.Zero)
                    {
                        // キャプチャ可能か判定
                        string className = NativeMethods.GetClassName(x.EventArgs.Handle);
                        if (_ieCapturer.CanCapture(className))
                        {
                            // ウェブページ全体をキャプチャ
                            using (Bitmap bitmap = _ieCapturer.Capture(x.EventArgs.Handle))
                            {
                                // キャプチャした画像をクリップボードに設定
                                Clipboard.SetDataObject(bitmap, true);
                                SystemSounds.Asterisk.Play();
                            }
                        }
                        else
                        {
                            // 選択コントロールをキャプチャ
                            using (Bitmap bitmap = _controlCapturer.CaptureControl(x.EventArgs.Handle))
                            {
                                // キャプチャした画像をクリップボードに設定
                                Clipboard.SetDataObject(bitmap, true);
                                SystemSounds.Asterisk.Play();
                            }
                        }
                    }

                    // 待機状態に戻す
                    this.Status = CapturableServiceStatus.Waiting;
                });

            // 選択ウィンドウの表示
            window.Show();
            window.Activate();
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

    /// <summary>
    /// キャプチャ機能の状態を示す識別子。
    /// </summary>
    public enum CapturableServiceStatus
    {
        /// <summary>
        /// 待機中
        /// </summary>
        Waiting,

        /// <summary>
        /// キャプチャ中
        /// </summary>
        Capturing,
    }
}
