namespace WinCap.Models
{
    /// <summary>
    /// ホットキーの登録や解除の際に指定するID。
    /// 0x0000～0xbfff内の値を指定。
    /// </summary>
    public enum HotkeyId : int
    {
        /// <summary>
        /// 画面全体
        /// </summary>
        ScreenWhole = 0x0001,

        /// <summary>
        /// アクティブウィンドウ
        /// </summary>
        ActiveWindow,

        /// <summary>
        /// 選択コントロール
        /// </summary>
        SelectControl,

        /// <summary>
        /// ページ全体
        /// </summary>
        PageWhole,
    }
}
