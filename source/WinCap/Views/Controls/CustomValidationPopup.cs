using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using WinCap.Interop.Win32;

namespace WinCap.Views.Controls
{
    /// <summary>
    /// このカスタムポップアップは、検証エラーテンプレートによって使用されます。
    /// このカスタムポップアップは、下記の追加機能を提供します。
    /// 　・ホストウィンドウのサイズまたは位置が変更された際に再配置。
    /// 　・ホストウィンドウが最大化された際に再配置。
    ///   ・ホストウィンドウがアクティブになっている場合は最上位に表示。
    /// </summary>
    public class CustomValidationPopup : Popup
    {
        private const int TopmostFlags = (int)SWP.NOACTIVATE | (int)SWP.NOOWNERZORDER | (int)SWP.NOSIZE | (int)SWP.NOMOVE | (int)SWP.NOREDRAW | (int)SWP.NOSENDCHANGING;

        private Window hostWindow;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public CustomValidationPopup()
        {
            this.Loaded += this.CustomValidationPopup_Loaded;
            this.Opened += this.CustomValidationPopup_Opened;
        }

        #region CloseOnMouseLeftButtonDown 依存関係プロパティ
        /// <summary>
        /// Gets/sets if the popup can be closed by left mouse button down.
        /// </summary>
        public bool CloseOnMouseLeftButtonDown
        {
            get { return (bool)GetValue(CloseOnMouseLeftButtonDownProperty); }
            set { SetValue(CloseOnMouseLeftButtonDownProperty, value); }
        }

        public static readonly DependencyProperty CloseOnMouseLeftButtonDownProperty =
            DependencyProperty.Register("CloseOnMouseLeftButtonDown", typeof(bool), typeof(CustomValidationPopup), new PropertyMetadata(true));
        #endregion

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (CloseOnMouseLeftButtonDown)
            {
                this.IsOpen = false;
                Console.WriteLine("");
            }
        }

        private void CustomValidationPopup_Loaded(object sender, RoutedEventArgs e)
        {
            var target = this.PlacementTarget as FrameworkElement;
            if (target == null)
            {
                return;
            }

            this.hostWindow = Window.GetWindow(target);
            if (this.hostWindow == null)
            {
                return;
            }

            this.hostWindow.LocationChanged -= this.HostWindow_SizeOrLocationChanged;
            this.hostWindow.LocationChanged += this.HostWindow_SizeOrLocationChanged;
            this.hostWindow.SizeChanged -= this.HostWindow_SizeOrLocationChanged;
            this.hostWindow.SizeChanged += this.HostWindow_SizeOrLocationChanged;
            target.SizeChanged -= this.HostWindow_SizeOrLocationChanged;
            target.SizeChanged += this.HostWindow_SizeOrLocationChanged;
            this.hostWindow.StateChanged -= this.HostWindow_StateChanged;
            this.hostWindow.StateChanged += this.HostWindow_StateChanged;
            this.hostWindow.Activated -= this.HostWindow_Activated;
            this.hostWindow.Activated += this.HostWindow_Activated;
            this.hostWindow.Deactivated -= this.HostWindow_Deactivated;
            this.hostWindow.Deactivated += this.HostWindow_Deactivated;

            this.Unloaded -= this.CustomValidationPopup_Unloaded;
            this.Unloaded += this.CustomValidationPopup_Unloaded;
        }

        private void CustomValidationPopup_Opened(object sender, EventArgs e)
        {
            this.SetTopmostState(true);
        }

        private void HostWindow_Activated(object sender, EventArgs e)
        {
            this.SetTopmostState(true);
        }

        private void HostWindow_Deactivated(object sender, EventArgs e)
        {
            this.SetTopmostState(false);
        }

        private void CustomValidationPopup_Unloaded(object sender, RoutedEventArgs e)
        {
            var target = this.PlacementTarget as FrameworkElement;
            if (target != null)
            {
                target.SizeChanged -= this.HostWindow_SizeOrLocationChanged;
            }
            if (this.hostWindow != null)
            {
                this.hostWindow.LocationChanged -= this.HostWindow_SizeOrLocationChanged;
                this.hostWindow.SizeChanged -= this.HostWindow_SizeOrLocationChanged;
                this.hostWindow.StateChanged -= this.HostWindow_StateChanged;
                this.hostWindow.Activated -= this.HostWindow_Activated;
                this.hostWindow.Deactivated -= this.HostWindow_Deactivated;
            }
            this.Unloaded -= this.CustomValidationPopup_Unloaded;
            this.Opened -= this.CustomValidationPopup_Opened;
            this.hostWindow = null;
        }

        private void HostWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.hostWindow != null && this.hostWindow.WindowState != WindowState.Minimized)
            {
                var target = this.PlacementTarget as FrameworkElement;
                var holder = target != null ? target.DataContext as AdornedElementPlaceholder : null;
                if (holder != null && holder.AdornedElement != null)
                {
                    this.PopupAnimation = PopupAnimation.None;
                    this.IsOpen = false;
                    var errorTemplate = holder.AdornedElement.GetValue(Validation.ErrorTemplateProperty);
                    holder.AdornedElement.SetValue(Validation.ErrorTemplateProperty, null);
                    holder.AdornedElement.SetValue(Validation.ErrorTemplateProperty, errorTemplate);
                }
            }
        }

        private void HostWindow_SizeOrLocationChanged(object sender, EventArgs e)
        {
            var offset = this.HorizontalOffset;
            // "bump" the offset to cause the popup to reposition itself on its own
            this.HorizontalOffset = offset + 1;
            this.HorizontalOffset = offset;
        }

        private bool? appliedTopMost;
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        private void SetTopmostState(bool isTop)
        {
            if (this.appliedTopMost.HasValue && this.appliedTopMost == isTop)
            {
                return;
            }

            if (this.Child == null)
            {
                return;
            }

            var hwndSource = (PresentationSource.FromVisual(this.Child)) as HwndSource;

            if (hwndSource == null)
            {
                return;
            }
            var hwnd = hwndSource.Handle;

            RECT rect;
            if (!User32.GetWindowRect(hwnd, out rect))
            {
                return;
            }
            //Debug.WriteLine("setting z-order " + isTop);

            var left = rect.left;
            var top = rect.top;
            var width = rect.Width;
            var height = rect.Height;
            if (isTop)
            {
                User32.SetWindowPos(hwnd, HWND_TOPMOST, left, top, width, height, TopmostFlags);
            }
            else
            {
                // Z-Order would only get refreshed/reflected if clicking the
                // the titlebar (as opposed to other parts of the external
                // window) unless I first set the popup to HWND_BOTTOM
                // then HWND_TOP before HWND_NOTOPMOST
                User32.SetWindowPos(hwnd, HWND_BOTTOM, left, top, width, height, TopmostFlags);
                User32.SetWindowPos(hwnd, HWND_TOP, left, top, width, height, TopmostFlags);
                User32.SetWindowPos(hwnd, HWND_NOTOPMOST, left, top, width, height, TopmostFlags);
            }

            this.appliedTopMost = isTop;
        }
    }
}
