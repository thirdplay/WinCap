using Reactive.Bindings;
using System;
using System.Drawing;

namespace WinCap.Models
{
    /// <summary>
    /// 矩形トラッカー機能を提供します。
    /// </summary>
    public class RectangleTracker
    {
        /// <summary>
        /// 矩形トラッカーの選択状態を表します。
        /// </summary>
        public enum SelectionStatus
        {
            /// <summary>
            /// 待機
            /// </summary>
            Wait,

            /// <summary>
            /// 選択中
            /// </summary>
            Selecting,

            /// <summary>
            /// 選択完了
            /// </summary>
            Completed,
        }

        /// <summary>
        /// 選択範囲
        /// </summary>
        public ReactiveProperty<Rectangle> SelectedRange { get; } = new ReactiveProperty<Rectangle>();

        /// <summary>
        /// 現在のマウス座標
        /// </summary>
        public ReactiveProperty<Point> CurrentPoint { get; } = new ReactiveProperty<Point>();

        /// <summary>
        /// 選択範囲の始点
        /// </summary>
        public ReactiveProperty<Point> StartPoint { get; } = new ReactiveProperty<Point>();

        /// <summary>
        /// 状態
        /// </summary>
        public ReactiveProperty<SelectionStatus> Status { get; } = new ReactiveProperty<SelectionStatus>();

        /// <summary>
        /// 選択範囲が空かどうか判定します。
        /// </summary>
        public bool IsEmptySelectedRange => this.SelectedRange.Value.Width == 0 || this.SelectedRange.Value.Height == 0;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public RectangleTracker()
        {
        }

        /// <summary>
        /// 初期化。
        /// </summary>
        public void Initialize()
        {
            this.SelectedRange.Value = new Rectangle();
            this.CurrentPoint.Value = new Point();
            this.StartPoint.Value = new Point();
            this.SelectedRange.Value = new Rectangle();
            this.Status.Value = SelectionStatus.Wait;
        }

        /// <summary>
        /// 矩形選択を開始します。
        /// </summary>
        /// <param name="point">現在の座標</param>
        public void Start(Point point)
        {
            this.StartPoint.Value = point;
            this.SelectedRange.Value = new Rectangle();
            this.Status.Value = SelectionStatus.Selecting;
        }

        /// <summary>
        /// 矩形選択を終了します。
        /// </summary>
        /// <param name="point">現在の座標</param>
        public void Stop(Point point)
        {
            // 選択範囲がない場合、待機中へ
            // 選択範囲がある場合、選択完了へ
            this.Update(point);
            if (this.IsEmptySelectedRange)
            {
                this.Status.Value = SelectionStatus.Wait;
            }
            else
            {
                this.Status.Value = SelectionStatus.Completed;
            }
        }

        /// <summary>
        /// 選択範囲を更新します。
        /// </summary>
        /// <param name="point">現在のマウス座標</param>
        public void Update(Point point)
        {
            this.CurrentPoint.Value = point;
            if (this.Status.Value == SelectionStatus.Selecting)
            {
                this.SelectedRange.Value = CreateRange(this.StartPoint.Value, this.CurrentPoint.Value);
            }
        }

        /// <summary>
        /// 矩形選択を中断します。
        /// </summary>
        public void Susspend()
        {
            this.SelectedRange.Value = new Rectangle();
            this.Status.Value = SelectionStatus.Completed;
        }

        /// <summary>
        /// 始点と終点から選択範囲を作成します。
        /// </summary>
        /// <param name="p1">始点座標</param>
        /// <param name="p2">終点座標</param>
        /// <returns>選択範囲</returns>
        private Rectangle CreateRange(Point p1, Point p2)
        {
            return new Rectangle()
            {
                X = Math.Min(p1.X, p2.X),
                Y = Math.Min(p1.Y, p2.Y),
                Width = Math.Abs(p1.X - p2.X),
                Height = Math.Abs(p1.Y - p2.Y),
            };
        }
    }
}
