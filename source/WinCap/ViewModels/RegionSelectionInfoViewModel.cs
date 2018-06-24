using System;
using System.Drawing;
using System.Windows.Forms;
using WinCap.ViewModels.Messages;
using Visibility = System.Windows.Visibility;

namespace WinCap.ViewModels
{
    /// <summary>
    /// 領域選択情報ViewModel
    /// </summary>
    public class RegionSelectionInfoViewModel : SwitchablePanelViewModel
    {
        #region StartPoint 変更通知プロパティ

        private Point _StartPoint;

        /// <summary>
        /// 始点を取得または設定します。
        /// </summary>
        public Point StartPoint
        {
            get { return this._StartPoint; }
            set
            {
                if (this._StartPoint != value)
                {
                    this._StartPoint = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region EndPoint 変更通知プロパティ

        private Point _EndPoint;

        /// <summary>
        /// 終点を取得または設定します。
        /// </summary>
        public Point EndPoint
        {
            get { return this._EndPoint; }
            set
            {
                if (this._EndPoint != value)
                {
                    this._EndPoint = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Size 変更通知プロパティ

        private Size _Size;

        /// <summary>
        /// サイズを取得または設定します。
        /// </summary>
        public Size Size
        {
            get { return this._Size; }
            set
            {
                if (this._Size != value)
                {
                    this._Size = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public RegionSelectionInfoViewModel() : base()
        {
        }

        /// <summary>
        /// 初期化。
        /// </summary>
        /// <param name="screenOrigin">スクリーンの原点</param>
        public override void Initialize(Point screenOrigin)
        {
            // 親メソッドの呼び出し
            base.Initialize(screenOrigin);

            // 領域情報のクリア
            this.StartPoint = Cursor.Position;
            this.EndPoint = new Point();
            this.Size = new Size();

            // 表示状態の設定
            this.Messenger.Raise(new SetVisibilityMessage
            {
                MessageKey = "Window.Visibility",
                Visibility = Visibility.Visible
            });
        }

        /// <summary>
        /// 更新処理。
        /// </summary>
        /// <param name="mousePoint">マウス座標</param>
        /// <param name="startPoint">始点</param>
        /// <param name="region">選択領域</param>
        public void Update(Point mousePoint, Point? startPoint, Rectangle? region)
        {
            base.Update(mousePoint);
            if (startPoint.HasValue && region.HasValue)
            {
                this.StartPoint = startPoint.Value;
                this.EndPoint = mousePoint;
                this.Size = region.Value.Size;
            }
            else
            {
                this.StartPoint = mousePoint;
                this.EndPoint = new Point();
                this.Size = new Size();
            }
        }
    }
}