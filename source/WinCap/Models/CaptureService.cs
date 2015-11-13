using Livet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows;
using WinCap.Utilities.Lifetime;

namespace WinCap.Models
{
    /// <summary>
    /// キャプチャサービスの状態を示す識別子
    /// </summary>
    public enum CaptureServiceStatus
    {
        /// <summary>
        /// 待機
        /// </summary>
        Wait,

        /// <summary>
        /// キャプチャ
        /// </summary>
        Capture,
    }

    /// <summary>
    /// キャプチャサービス
    /// </summary>
    public sealed class CaptureService : NotificationObject, IDisposableHolder
    {
        #region フィールド
        /// <summary>
        /// 基本CompositeDisposable
        /// </summary>
        private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();

        /// <summary>
        /// 現在のキャプチャサービスの状態
        /// </summary>
        private CaptureServiceStatus currentStatus = CaptureServiceStatus.Wait;
        #endregion

        #region プロパティ
        /// <summary>
        /// 現在のサービスを取得する。
        /// </summary>
        public static CaptureService Current { get; } = new CaptureService();

        /// <summary>
        /// キャプチャサービスの状態
        /// </summary>
        public CaptureServiceStatus Status
        {
            get { return this.currentStatus; }
            set
            {
                if (this.currentStatus != value)
                {
                    this.currentStatus = value;
                    if (this.currentStatus == CaptureServiceStatus.Wait)
                    {
                        // TODO:SE再生
                    }
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
        /// 初期化
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// 画面全体をキャプチャする。
        /// </summary>
        public void CaptureScreenWhole()
        {
            // 待機状態以外は処理しない
            if (this.Status != CaptureServiceStatus.Wait) return;
            this.Status = CaptureServiceStatus.Capture;

            // 画面全体をキャプチャする
            ScreenCapture capture = new ScreenCapture();
            using (Bitmap bitmap = capture.Capture())
            {
                // TODO:ImageService.Current.Save(bitmap);
                // キャプチャした画像をクリップボードに設定する
                Clipboard.SetDataObject(bitmap, true);
            }

            // 待機状態に戻す
            this.Status = CaptureServiceStatus.Wait;
        }

        //CaptureActiveWindow()
        //CaptureSelectControl()

        /// <summary>
        /// 選択コントロールをキャプチャする。
        /// </summary>
        public void CaptureSelectControl()
        {
            // 待機状態以外は処理しない
            if (this.Status != CaptureServiceStatus.Wait) return;
            this.Status = CaptureServiceStatus.Capture;

            // コントロール選択ウィンドウの取得
            var selectWindow = WindowService.Current.GetControlSelectWindow();
            Observable.FromEventPattern(
                handler => selectWindow.Closed += handler,
                handler => selectWindow.Closed -= handler
            )
            .Subscribe(x =>
            {
                // 選択コントロールをキャプチャする
                //ScreenCapture capture = new ScreenCapture();
                //using (Bitmap bitmap = capture.Capture())
                //{
                //    // キャプチャした画像をクリップボードに設定する
                //    Clipboard.SetDataObject(bitmap, true);
                //}

                // 待機状態に戻す
                this.Status = CaptureServiceStatus.Wait;
            });

            // 選択ウィンドウの表示
            selectWindow.Show();
        }

        //CaptureWholePage()

        #region IDisposableHoloder members
        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄する。
        /// </summary>
        public void Dispose()
        {
            this.compositeDisposable.Dispose();
        }
        #endregion
    }
}
