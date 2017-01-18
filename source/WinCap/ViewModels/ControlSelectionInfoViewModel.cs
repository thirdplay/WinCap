using Livet;
using System;
using System.Drawing;
using System.Windows;
using WinCap.Models;
using WinCap.ViewModels.Messages;

namespace WinCap.ViewModels
{
    /// <summary>
    /// コントロール選択情報ViewModel
    /// </summary>
    public class ControlSelectionInfoViewModel : ViewModel
    {
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
            SetMargin(12.0, 12.0);

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
        /// ウィンドウのマージンを設定します。
        /// </summary>
        private void SetMargin(double? left, double? top)
        {
            this.Messenger.Raise(new SetMarginMessage
            {
                MessageKey = "Window.Margin",
                Left = left,
                Top = top
            });
        }
    }
}
