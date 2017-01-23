using Livet;
using System;
using System.Drawing;
using System.Windows;
using WinCap.Interop;
using WinCap.Models;
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
        public double Width { get; } = 240.0;

        /// <summary>
        /// ウィンドウの高さを取得します。
        /// </summary>
        public double Height { get; } = 100.0;

        /// <summary>
        /// ウィンドウのマージンを取得します。
        /// </summary>
        public System.Windows.Point Margin { get; } = new System.Windows.Point(12.0, 12.0);

        #region Left 変更通知プロパティ
        private double _Left;
        /// <summary>
        /// コントロール選択情報のX座標を取得または設定します。
        /// </summary>
        public double Left
        {
            get { return _Left; }
            set
            {
                if (_Left != value)
                {
                    _Left = value;
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
            get { return _Top; }
            set
            {
                if (_Top != value)
                {
                    _Top = value;
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
            get { return _ClassName; }
            set
            {
                if (_ClassName != value)
                {
                    _ClassName = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region Point 変更通知プロパティ
        private System.Drawing.Point _Point;
        /// <summary>
        /// 位置を取得または設定します。
        /// </summary>
        public System.Drawing.Point Point
        {
            get { return _Point; }
            set
            { 
                if (_Point != value)
                {
                    _Point = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region Size 変更通知プロパティ
        private System.Drawing.Size _Size;
        /// <summary>
        /// サイズを取得または設定します。
        /// </summary>
        public System.Drawing.Size Size
        {
            get { return _Size; }
            set
            { 
                if (_Size != value)
                {
                    _Size = value;
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
            this.ClassName = InteropExtensions.GetClassName(handle);
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
            var screen = ScreenHelper.GetCurrentScreen(point);

            // スクリーンの左端座標に設定する
            this.Left = screen.Bounds.Left + Margin.X;
            this.Top = screen.Bounds.Top + Margin.Y;
        }
    }
}
