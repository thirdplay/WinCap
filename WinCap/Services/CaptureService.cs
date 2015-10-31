using Livet;
using System;
using System.Collections.Generic;
using WinCap.Utilities.Lifetime;

namespace WinCap.Services
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
        /// キャプチャ中
        /// </summary>
        UnderCapture,
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
        private CaptureServiceStatus currentSatus = (CaptureServiceStatus)(-1);
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
            get { return this.currentSatus; }
            set
            {
                if (this.currentSatus != value)
                {
                    this.currentSatus = value;
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
        public void CaptureScreen()
        {
            /*
                : [一連の流れ]
                : Service：状態確認 ⇒ 待機状態以外は処理終了
                : Service：キャプチャ生成
                : Service：共通設定の反映
                :
                : Service：キャプチャ処理の呼び出し
                : ?      ：画像出力
            */
        }

        /// <summary>
        /// 選択コントロールをキャプチャする。
        /// </summary>
        public void CaptureControl()
        {
            /*
                : [一連の流れ]
                : Service：状態確認 ⇒ 待機状態以外は処理終了
                :
                : ?      ：コントロール選択
                :            
                : Service：キャプチャ生成
                : Service：共通設定の反映
                :
                : Service：キャプチャ処理の呼び出し
                : ?      ：画像出力
                :
                : ※コントロール選択処理をキャプチャに寄せるorサービスとの間に機能をかますか検討。
            */
        }

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
