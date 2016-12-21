namespace WinCap.Driver
{
    /// <summary>
    /// 対象アプリケーションを操作する機能を提供します。
    /// </summary>
    public class AppDriver
    {
        /// <summary>
        /// アプリケーション操作インターフェイス。
        /// </summary>
        private IAppDriverCore _core;

        /// <summary>
        /// デバッグ中かどうかを示す値を取得します。
        /// </summary>
        public bool IsDebug => this._core is AppDriverDebug;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public AppDriver()
        {
            this._core = AppDriverDebug.Exists ? (IAppDriverCore)new AppDriverDebug() : new AppDriverNormal();
        }

        /// <summary>
        /// プロセスをアタッチします。
        /// </summary>
        public void Attach()
        {
            this._core.Attach();
            this.InitApp();
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        /// <param name="isContinue">処理を継続するかどうかを示す値</param>
        public void Release(bool isContinue)
        {
            this._core.Release(isContinue);
        }

        /// <summary>
        /// アプリケーションを初期化します。
        /// </summary>
        private void InitApp()
        {
        }

        /// <summary>
        /// アプリケーションを終了します。
        /// </summary>
        public void EndProcess()
        {
            this._core.EndProcess();
        }

        /// <summary>
        /// 設定ウィンドウを表示します。
        /// </summary>
        /// <returns>設定ウィンドウドライバー</returns>
        public SettingsWindowDriver ShowSettingsWindow()
        {
            return this._core.ShowSettingsWindow();
        }
    }
}
