using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WinCap.Models;

namespace WinCap
{
    /// <summary>
    /// タスクトレイアイコンの表示する機能を提供します。
    /// </summary>
    public class TaskTrayIcon : IDisposable
    {
        /// <summary>
        /// アイコン
        /// </summary>
        private readonly Icon _icon;

        /// <summary>
        /// タスクトレイアイコン項目
        /// </summary>
        private readonly TaskTrayIconItem[] _items;

        /// <summary>
        /// 通知アイコン
        /// </summary>
        private NotifyIcon _notifyIcon;

        /// <summary>
        /// テキスト
        /// </summary>
        public string Text { get; set; } = ProductInfo.Title;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="icon">アイコン</param>
        /// <param name="items">タスクトレイ項目</param>
        public TaskTrayIcon(Icon icon, TaskTrayIconItem[] items)
        {
            this._icon = icon;
            this._items = items;
        }

        /// <summary>
        /// タスクトレイアイコンを表示します。
        /// </summary>
        public void Show()
        {
            var menus = this._items
                .Where(x => x.CanDisplay())
                .Select(x => x.GetMenuItem())
                .ToArray();

            this._notifyIcon = new NotifyIcon()
            {
                Text = this.Text,
                Icon = this._icon,
                Visible = true,
                ContextMenu = new ContextMenu(menus),
            };
        }

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        public void Dispose()
        {
            this._notifyIcon?.Dispose();
            this._icon?.Dispose();
        }
    }

    /// <summary>
    /// タスクトレイアイコンの項目
    /// </summary>
    public class TaskTrayIconItem
    {
        /// <summary>
        /// テキスト
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// クリック時のアクション
        /// </summary>
        public Action ClickAction { get; }

        /// <summary>
        /// 表示可否判定用のメソッド
        /// </summary>
        public Func<bool> CanDisplay { get; }

        /// <summary>
        /// タスクトレイアイコンのリスト
        /// </summary>
        public TaskTrayIconItem[] Items { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="clickAction">クリック時のアクション</param>
        public TaskTrayIconItem(string text, Action clickAction) : this(text, clickAction, () => true, null) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="clickAction">クリック時のアクション</param>
        public TaskTrayIconItem(string text, TaskTrayIconItem[] items) : this(text, null, () => true, items) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="clickAction">クリック時のアクション</param>
        /// <param name="canDisplay">表示可否判定用のメソッド</param>
        public TaskTrayIconItem(string text, Action clickAction, Func<bool> canDisplay, TaskTrayIconItem[] items)
        {
            this.Text = text;
            this.ClickAction = clickAction;
            this.CanDisplay = canDisplay;
            this.Items = items;
        }

        /// <summary>
        /// メニュー項目を取得します。
        /// </summary>
        /// <returns>メニュー項目</returns>
        public MenuItem GetMenuItem()
        {
            if (this.Items != null)
            {
                return new MenuItem(this.Text, this.Items.Select(x => x.GetMenuItem()).ToArray());
            }
            return new MenuItem(this.Text, (sender, args) => this.ClickAction());
        }
    }
}
