using Livet;
using Livet.Messaging.Windows;
using Reactive.Bindings;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using WinCap.Capturers;
using WinCap.Models;
using WinCap.ViewModels.Messages;
using WpfUtility.Mvvm;

namespace WinCap.ViewModels
{
    /// <summary>
    /// 領域選択ウィンドウViewModel。
    /// </summary>
    public class RegionSelectionWindowViewModel : WindowViewModel
    {
        /// <summary>
        /// ウィンドウを常に手前に表示するか否か返す。
        /// </summary>
        /// <remarks>デバッグ中は常に手前には表示しない</remarks>
        public bool TopMost => Debugger.IsAttached;

        /// <summary>
        /// 選択結果を取得します。
        /// </summary>
        public Rectangle? Result { get; private set; }

        /// <summary>
        /// 矩形トラッカー
        /// </summary>
        public RectangleTracker Tracker { get; }

        /// <summary>
        /// 選択した領域のViewModel
        /// </summary>
        public SelectedRegionViewModel SelectedViewModel { get; }

        /// <summary>
        /// 領域選択情報ViewModel
        /// </summary>
        public RegionSelectionInfoViewModel SelectionInfoViewModel { get; }

        /// <summary>
        /// マウスダウン
        /// </summary>
        public ReactiveProperty<MouseEventArgs> MouseDown { get; } = new ReactiveProperty<MouseEventArgs>(mode: ReactivePropertyMode.None);

        /// <summary>
        /// マウスアップ
        /// </summary>
        public ReactiveProperty<MouseEventArgs> MouseUp { get; } = new ReactiveProperty<MouseEventArgs>(mode: ReactivePropertyMode.None);

        /// <summary>
        /// マウス移動
        /// </summary>
        public ReactiveProperty<MouseEventArgs> MouseMove { get; } = new ReactiveProperty<MouseEventArgs>(mode: ReactivePropertyMode.None);

        /// <summary>
        /// キーダウン
        /// </summary>
        public ReactiveProperty<KeyEventArgs> KeyDown { get; } = new ReactiveProperty<KeyEventArgs>(mode: ReactivePropertyMode.None);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RegionSelectionWindowViewModel()
        {
            //this.Subscribe(nameof(MousePoint), () =>
            //{
            //    // 領域選択情報の更新
            //    this.RegionSelectionInfo.Update(this.MousePoint, this.startPoint, region);
            //}).AddTo(this);

            this.Tracker = new RectangleTracker();
            this.SelectedViewModel = new SelectedRegionViewModel(MouseDown, MouseUp, MouseMove, KeyDown, Tracker).AddTo(this);
            this.SelectionInfoViewModel = new RegionSelectionInfoViewModel().AddTo(this);

            // 選択完了時にウィンドウを非表示にする
            this.Tracker.Status
                .Where(x => x == RectangleTracker.SelectionStatus.Completed)
                .Delay(TimeSpan.FromMilliseconds(100))
                .ObserveOn(DispatcherHelper.UIDispatcher)
                .Subscribe(x => {
                    this.Result = Tracker.IsEmptySelectedRange
                        ? null
                        : (Rectangle?)Tracker.SelectedRange.Value;
                    this.Messenger.Raise(new SetVisibilityMessage
                    {
                        MessageKey = "Window.Visibility",
                        Visibility = Visibility.Hidden
                    });
                })
                .AddTo(this);
        }

        /// <summary>
        /// 初期化処理。
        /// </summary>
        protected override void InitializeCore()
        {
            // ウィンドウに画面全体の領域を設定する
            var screenBounds = ScreenHelper.GetFullScreenBounds();
            this.Messenger.Raise(new SetWindowBoundsMessage
            {
                MessageKey = "Window.Bounds",
                Left = screenBounds.Left,
                Top = screenBounds.Top,
                Width = screenBounds.Width,
                Height = screenBounds.Height
            });

            // 初期化
            Tracker.Initialize();
            this.SendWindowAction(WindowAction.Active);
            this.SelectedViewModel.Initialize();
            this.SelectionInfoViewModel.Initialize(new System.Drawing.Point());
        }
    }
}