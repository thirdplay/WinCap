using Livet;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinCap.ViewModels
{
    /// <summary>
    /// マウスオーバー時に表示位置を切り替えるパネルのViewModel。
    /// </summary>
    public class SwitchablePanelViewModel : ViewModel
    {
        /// <summary>
        /// アンカーの左上を表します。
        /// </summary>
        private const AnchorStyles AnchorLeftTop = AnchorStyles.Left | AnchorStyles.Top;

        /// <summary>
        /// ウィンドウのマージン横幅
        /// </summary>
        public int MarginWidth { get; set; } = 12;

        /// <summary>
        /// ウィンドウのマージン高さ
        /// </summary>
        public int MarginHeight { get; set; } = 12;

        /// <summary>
        /// ウィンドウのアンカー
        /// </summary>
        public AnchorStyles Anchor { get; private set; }

        /// <summary>
        /// 現在ウィンドウを表示しているスクリーン
        /// </summary>
        public Screen Screen { get; private set; }

        /// <summary>
        /// スクリーンの原点
        /// </summary>
        protected Point screenOrigin;

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

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public SwitchablePanelViewModel()
        {
        }

        /// <summary>
        /// 初期化。
        /// </summary>
        /// <param name="screenOrigin">スクリーンの原点</param>
        /// <param name="point">マウス座標</param>
        public virtual void Initialize(Point screenOrigin, Point point)
        {
            this.screenOrigin = screenOrigin;
            this.Screen = GetScreen(point);
            this.Anchor = AnchorLeftTop;

            // 位置座標の更新
            UpdateLocation(this.Screen, this.Anchor, point);
        }

        /// <summary>
        /// 更新処理。
        /// </summary>
        /// <param name="point">マウス座標</param>
        public virtual void Update(Point point)
        {
            // スクリーン、アンカー更新
            var nextScreen = GetScreen(point);
            if (this.Screen != nextScreen)
            {
                this.Screen = nextScreen;
                this.Anchor = AnchorLeftTop;
            }

            // 位置座標の更新
            UpdateLocation(this.Screen, this.Anchor, point);
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
                this.Anchor ^= AnchorLeftTop;
                point = GetLocation(screen, anchor);
            }

            // スクリーン座標に変換し、座標を反映する
            this.Left = location.X - this.screenOrigin.X;
            this.Top = location.Y - this.screenOrigin.Y;
        }
    }
}