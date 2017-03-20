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
        /// ウィンドウの横幅を取得します。
        /// </summary>
        public double Width => 240.0;

        /// <summary>
        /// ウィンドウの高さを取得します。
        /// </summary>
        public double Height => 100.0;

        /// <summary>
        /// ウィンドウのマージンを取得します。
        /// </summary>
        public Point Margin => new Point(12, 12);

        /// <summary>
        /// ウィンドウの位置を取得します。
        /// </summary>
        public Point Location => new Point((int)this.Left, (int)this.Top);

        /// <summary>
        /// ウィンドウのサイズを取得します。
        /// </summary>
        public Size Size => new Size((int)this.Width, (int)this.Height);

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
        public void Initialize()
        {
            this.Left = this.Margin.X;
            this.Top = this.Margin.Y;

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
            // スクリーン取得
            var screen = Screen.AllScreens
                .Where(x => x.Bounds.Contains(point))
                .FirstOrDefault();

            // スクリーンの左端座標に設定する
            this.Left = screen.Bounds.Left + this.Margin.X;
            this.Top = screen.Bounds.Top + this.Margin.Y;

            // マウスオーバーチェック
            //var rect = new Rectangle(this.Location, this.Size);
            //if (rect.Contains(point))
            //{
            //    Console.WriteLine($"Hit!!:({this.Location.X}, {this.Location.Y}), {this.Width} x {this.Height}");
            //}
        }
    }
}