using System;
using System.Drawing;
using WinCap.Interop;
using WinCap.ViewModels.Messages;
using Visibility = System.Windows.Visibility;

namespace WinCap.ViewModels
{
    /// <summary>
    /// コントロール選択情報ViewModel
    /// </summary>
    public class ControlSelectionInfoViewModel : SwitchablePanelViewModel
    {
        #region ClassName 変更通知プロパティ

        private string _ClassName;

        /// <summary>
        /// クラス名を取得または設定します。
        /// </summary>
        public string ClassName
        {
            get { return this._ClassName; }
            set
            {
                if (this._ClassName != value)
                {
                    this._ClassName = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region ControlLocation 変更通知プロパティ

        private Point _ControlLocation;

        /// <summary>
        /// コントロールの位置を取得または設定します。
        /// </summary>
        public Point ControlLocation
        {
            get { return this._ControlLocation; }
            set
            {
                if (this._ControlLocation != value)
                {
                    this._ControlLocation = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region ControlSize 変更通知プロパティ

        private Size _ControlSize;

        /// <summary>
        /// サイズを取得または設定します。
        /// </summary>
        public Size ControlSize
        {
            get { return this._ControlSize; }
            set
            {
                if (this._ControlSize != value)
                {
                    this._ControlSize = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public ControlSelectionInfoViewModel() : base()
        {
        }

        /// <summary>
        /// 初期化。
        /// </summary>
        /// <param name="screenOrigin">スクリーンの原点</param>
        /// <param name="point">マウス座標</param>
        public override void Initialize(Point screenOrigin, Point point)
        {
            // 親メソッドの呼び出し
            base.Initialize(screenOrigin, point);

            // 表示状態の設定
            this.Messenger.Raise(new SetVisibilityMessage
            {
                MessageKey = "Window.Visibility",
                Visibility = Visibility.Visible
            });
        }

        /// <summary>
        /// コントロール情報を設定します。
        /// </summary>
        /// <param name="handle">コントロールのハンドル</param>
        /// <param name="bounds">コントロールの範囲</param>
        public void SetInfo(IntPtr handle, Rectangle bounds)
        {
            this.ClassName = InteropHelper.GetClassName(handle);
            this.ControlLocation = bounds.Location;
            this.ControlSize = bounds.Size;
        }
    }
}