using Livet;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WinCap.Interop;
using WinCap.ViewModels.Messages;
using Visibility = System.Windows.Visibility;

namespace WinCap.ViewModels
{
    /// <summary>
    /// コントロール選択情報ViewModel
    /// </summary>
    public class ControlSelectionInfoViewModel : ViewModel
    {
        /// <summary>
        /// アンカーの左上を表します。
        /// </summary>
        private const AnchorStyles AnchorLeftTop = AnchorStyles.Left | AnchorStyles.Top;

        /// <summary>
        /// ウィンドウのマージン横幅
        /// </summary>
        private const int MarginWidth = 12;

        /// <summary>
        /// ウィンドウのマージン高さ
        /// </summary>
        private const int MarginHeight = 12;

        /// <summary>
        /// コントロールの位置
        /// </summary>
        private Point controlLocation;

        /// <summary>
        /// 現在ウィンドウを表示しているスクリーン
        /// </summary>
        private Screen screen;

        /// <summary>
        /// ウィンドウのアンカー
        /// </summary>
        private AnchorStyles anchor;

        #region Left 変更通知プロパティ

        private double _Left;

        /// <summary>
        /// コントロール選択情報のX座標を取得または設定します。
        /// </summary>
        public double Left
        {
            get { return this._Left; }
            set
            {
                if (this._Left != value)
                {
                    this._Left = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Top 変更通知プロパティ

        private double _Top;

        /// <summary>
        /// コントロール選択情報のY座標を取得または設定します。
        /// </summary>
        public double Top
        {
            get { return this._Top; }
            set
            {
                if (this._Top != value)
                {
                    this._Top = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Width 変更通知プロパティ

        private double _Width = 240.0;

        /// <summary>
        /// ウィンドウの横幅を取得します。
        /// </summary>
        public double Width
        {
            get { return this._Width; }
            set
            {
                if (this._Width != value)
                {
                    this._Width = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Width 変更通知プロパティ

        private double _Height = 100.0;

        /// <summary>
        /// ウィンドウの高さを取得します。
        /// </summary>
        public double Height
        {
            get { return this._Height; }
            set
            {
                if (this._Height != value)
                {
                    this._Height = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

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
        public ControlSelectionInfoViewModel()
        {
        }

        /// <summary>
        /// 初期化。
        /// </summary>
        /// <param name="controlLocation">コントロールの位置</param>
        /// <param name="point">マウス座標</param>
        public void Initialize(Point controlLocation, Point point)
        {
            this.controlLocation = controlLocation;
            this.screen = GetScreen(point);
            this.anchor = AnchorLeftTop;

            // 位置座標の更新
            UpdateLocation(this.screen, this.anchor, point);

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

        /// <summary>
        /// マウス座標を更新します。
        /// </summary>
        /// <param name="point">マウス座標</param>
        public void UpdateMousePoint(Point point)
        {
            // スクリーン、アンカー更新
            var nextScreen = GetScreen(point);
            if (this.screen != nextScreen)
            {
                this.screen = nextScreen;
                this.anchor = AnchorLeftTop;
            }

            // 位置座標の更新
            UpdateLocation(this.screen, this.anchor, point);
        }

        /// <summary>
        /// 指定座標上のスクリーンを取得します。
        /// </summary>
        /// <param name="point">座標</param>
        /// <returns>スクリーン</returns>
        private Screen GetScreen(Point point)
        {
            return Screen.AllScreens
                .Where(x => x.Bounds.Contains(point))
                .FirstOrDefault() ?? Screen.PrimaryScreen;
        }

        /// <summary>
        /// 指定 <see cref="Screen"/> の <see cref="AnchorStyles"/> 位置の座標を取得します。
        /// </summary>
        /// <param name="screen">スクリーン</param>
        /// <param name="anchor">アンカー</param>
        /// <returns>位置座標</returns>
        private Point GetLocation(Screen screen, AnchorStyles anchor)
        {
            var result = new Point();
            if (anchor == AnchorLeftTop)
            {
                result.X = screen.Bounds.Left + MarginWidth;
                result.Y = screen.Bounds.Top + MarginHeight;
            }
            else
            {
                result.X = screen.Bounds.Right - (int)this.Width - MarginWidth;
                result.Y = screen.Bounds.Bottom - (int)this.Height - MarginHeight;
            }
            return result;
        }

        /// <summary>
        /// 自身の座標を更新します。
        /// </summary>
        /// <param name="screen">スクリーン</param>
        /// <param name="anchor">アンカー</param>
        /// <param name="point">マウス座標</param>
        private void UpdateLocation(Screen screen, AnchorStyles anchor, Point point)
        {
            // 現在の座標を取得
            var location = GetLocation(screen, anchor);

            // マウスオーバーチェック
            var area = new Rectangle(new Point(location.X, location.Y), new Size((int)this.Width, (int)this.Height));
            if (area.Contains(point))
            {
                // マウスと重なる場合はアンカーを反転し再度座標を取得する
                this.anchor ^= AnchorLeftTop;
                point = GetLocation(screen, anchor);
            }

            // スクリーン座標に変換し、座標を反映する
            this.Left = location.X - this.controlLocation.X;
            this.Top = location.Y - this.controlLocation.Y;
        }
    }
}