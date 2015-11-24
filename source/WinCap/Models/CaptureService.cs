using Livet;
using MetroTrilithon.Lifetime;
using MetroTrilithon.Mvvm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Reactive.Linq;
using System.Windows;
using WinCap.Utilities.Mvvm;

namespace WinCap.Models
{
    /// <summary>
    /// キャプチャ機能の状態を示す識別子。
    /// </summary>
    public enum CaptureServiceStatus
    {
        /// <summary>
        /// 待機
        /// </summary>
        Wait,

        /// <summary>
        /// キャプチャ中
        /// </summary>
        DuringCapture,
    }

    /// <summary>
    /// 画面やウィンドウをキャプチャし、クリップボードや画像ファイルに出力する機能を提供します。
    /// </summary>
    public sealed class CaptureService : NotificationObject, IDisposableHolder
    {
        #region フィールド
        /// <summary>
        /// 基本CompositeDisposable
        /// </summary>
        private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();

        /// <summary>
        /// 現在の状態
        /// </summary>
        private CaptureServiceStatus currentStatus = CaptureServiceStatus.Wait;
        #endregion

        #region プロパティ
        /// <summary>
        /// 現在のサービスを取得します。
        /// </summary>
        public static CaptureService Current { get; } = new CaptureService();

        /// <summary>
        /// 現在の状態を取得します。
        /// </summary>
        public CaptureServiceStatus Status
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
        private CaptureService() { }

        /// <summary>
        /// キャプチャ機能を使用できる状態にします。
        /// </summary>
        public void Initialize()
        {
            //this.ObserveProperty(nameof(this.Status))
            //    .Where(_ => this.currentStatus == CaptureServiceStatus.CaptureCompletion)
            //    .Subscribe(_ => SystemSounds.Asterisk.Play())
            //    .AddTo(this);
        }

        /// <summary>
        /// 画面全体をキャプチャします。
        /// </summary>
        public void CaptureScreenWhole()
        {
            // 待機状態以外は処理しない
            if (this.Status != CaptureServiceStatus.Wait) return;
            this.Status = CaptureServiceStatus.DuringCapture;

            // 画面全体をキャプチャ
            ScreenCapture capture = new ScreenCapture();
            using (Bitmap bitmap = capture.Capture())
            {
                // TODO:save(bitmap); => Clipboard or Bitmap(ファイル名選定込み）
                // キャプチャした画像をクリップボードに設定
                Clipboard.SetDataObject(bitmap, true);
            }

            // 待機状態に戻す
            this.Status = CaptureServiceStatus.Wait;
        }

        /// <summary>
        /// アクティブウィンドウをキャプチャします。
        /// </summary>
        public void CaptureActiveWindow()
        {
            // 待機状態以外は処理しない
            if (this.Status != CaptureServiceStatus.Wait) return;
            this.Status = CaptureServiceStatus.DuringCapture;

            // アクティブウィンドウをキャプチャ
            WindowCapture capture = new WindowCapture();
            using (Bitmap bitmap = capture.Capture())
            {
                // TODO:save(bitmap); => Clipboard or Bitmap(ファイル名選定込み）
                // キャプチャした画像をクリップボードに設定
                Clipboard.SetDataObject(bitmap, true);
            }

            // 待機状態に戻す
            this.Status = CaptureServiceStatus.Wait;
        }

        /// <summary>
        /// 選択コントロールをキャプチャします。
        /// </summary>
        public void CaptureSelectControl()
        {
            // 待機状態以外は処理しない
            if (this.Status != CaptureServiceStatus.Wait) return;
            this.Status = CaptureServiceStatus.DuringCapture;

            // コントロール選択ウィンドウの取得
            var window = WindowService.Current.GetControlSelectWindow();
            Observable.FromEventPattern<SelectedEventArgs>(window, nameof(window.Selected))
                .Subscribe(x =>
                {
                    window.Close();
                    if (x.EventArgs.Handle != IntPtr.Zero)
                    {
                        // 選択コントロールをキャプチャ
                        WindowCapture capture = new WindowCapture();
                        using (Bitmap bitmap = capture.Capture(x.EventArgs.Handle))
                        {
                            // キャプチャした画像をクリップボードに設定
                            Clipboard.SetDataObject(bitmap, true);
                        }
                    }

                    // 待機状態に戻す
                    this.Status = CaptureServiceStatus.Wait;
                });

            // 選択ウィンドウの表示
            window.Show();
        }

        /// <summary>
        /// ページ全体をキャプチャします。
        /// </summary>
        public void CapturePageWhole()
        {
            // 待機状態以外は処理しない
            if (this.Status != CaptureServiceStatus.Wait) return;
            this.Status = CaptureServiceStatus.DuringCapture;

            // コントロール選択ウィンドウの取得
            var window = WindowService.Current.GetControlSelectWindow();
            Observable.FromEventPattern<SelectedEventArgs>(window, nameof(window.Selected))
                .Subscribe(x =>
                {
                    window.Close();
                    if (x.EventArgs.Handle != IntPtr.Zero)
                    {
                        // ブラウザのページ全体をキャプチャ
                    }

                    // 待機状態に戻す
                    this.Status = CaptureServiceStatus.Wait;
                });

            // 選択ウィンドウの表示
            window.Show();
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
