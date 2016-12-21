using Codeer.Friendly.Windows;
using System.Diagnostics;

namespace WinCap.Driver
{
    /// <summary>
    /// アプリケーション操作機能を提供するインターフェイス。
    /// </summary>
    public interface IAppDriverCore
    {
        /// <summary>
        /// プロセスを取得します。
        /// </summary>
        Process Process { get; }

        /// <summary>
        /// アプリケーションをアタッチします。
        /// </summary>
        void Attach();

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        /// <param name="isContinue">処理を継続するかどうかを示す値</param>
        void Release(bool isContinue);

        /// <summary>
        /// アプリケーションを終了します。
        /// </summary>
        void EndProcess();

        /// <summary>
        /// 設定ウィンドウを表示します。
        /// </summary>
        /// <returns>設定ウィンドウドライバー</returns>
        SettingsWindowDriver ShowSettingsWindow();
    }
}
