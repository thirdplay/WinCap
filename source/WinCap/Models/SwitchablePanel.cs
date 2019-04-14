using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinCap.Interop;

namespace WinCap.Models
{
    /// <summary>
    /// マウスオーバー時に表示位置を切り替えるパネル。
    /// </summary>
    public class SwitchablePanel
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
        /// 位置を取得または設定します。
        /// </summary>
        public ReactiveProperty<Point> Location { get; } = new ReactiveProperty<Point>();

        /// <summary>
        /// サイズを取得または設定します。
        /// </summary>
        public ReactiveProperty<Size> Size { get; } = new ReactiveProperty<Size>();

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="width">横幅</param>
        /// <param name="height">高さ</param>
        public SwitchablePanel(int width, int height)
        {
            var point = Cursor.Position;

            this.Screen = GetScreen(point);
            this.Anchor = AnchorLeftTop;
            this.Size.Value = new Size(width, height);

            // 位置座標の更新
            UpdateLocation(this.Screen, this.Anchor, point);
        }

        /// <summary>
        /// 更新処理。
        /// </summary>
        /// <param name="point">マウス座標</param>
        public void Update(Point point)
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
            var bounds = screen.Bounds.ToLogicalPixel();
            var result = new Point();
            if (anchor == AnchorLeftTop)
            {
                result.X = bounds.Left + MarginWidth;
                result.Y = bounds.Top + MarginHeight;
            }
            else
            {
                result.X = bounds.Right - Size.Value.Width - MarginWidth;
                result.Y = bounds.Bottom - Size.Value.Height - MarginHeight;
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
            var area = new Rectangle(new Point(location.X, location.Y), new Size(Size.Value.Width, Size.Value.Height));
            if (area.Contains(point))
            {
                // マウスと重なる場合はアンカーを反転し再度座標を取得する
                this.Anchor ^= AnchorLeftTop;
                location = GetLocation(screen, anchor);
            }

            // スクリーン座標に変換し、座標を反映する
            Location.Value = location.PointToControl();
        }
    }
}
