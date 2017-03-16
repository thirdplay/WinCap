using Livet;
using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using WinCap.Interop;
using WinCap.ViewModels.Messages;

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
        public System.Windows.Point Margin => new System.Windows.Point(12.0, 12.0);

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

        #endregion Left 変更通知プロパティ

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

        #endregion Top 変更通知プロパティ

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

        #endregion ClassName 変更通知プロパティ

        #region Point 変更通知プロパティ

        private System.Drawing.Point _Point;

        /// <summary>
        /// 位置を取得または設定します。
        /// </summary>
        public System.Drawing.Point Point
        {
            get { return this._Point; }
            set
            {
                if (this._Point != value)
                {
                    this._Point = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion Point 変更通知プロパティ

        #region Size 変更通知プロパティ

        private System.Drawing.Size _Size;

        /// <summary>
        /// サイズを取得または設定します。
        /// </summary>
        public System.Drawing.Size Size
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

        #endregion Size 変更通知プロパティ

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
            this.Point = bounds.Location;
            this.Size = bounds.Size;
        }

        /// <summary>
        /// マウス座標を更新します。
        /// </summary>
        /// <param name="point">マウス座標</param>
        public void UpdateMousePoint(System.Drawing.Point point)
        {
            // スクリーン取得
            var screen = Screen.AllScreens
                .Where(x => x.Bounds.Contains(point))
                .FirstOrDefault();

            // スクリーンの左端座標に設定する
            this.Left = screen.Bounds.Left + this.Margin.X;
            this.Top = screen.Bounds.Top + this.Margin.Y;
        }
    }
}