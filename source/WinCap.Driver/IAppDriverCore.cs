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
        /// アプリケーション操作クラスを取得します。
        /// </summary>
        WindowsAppFriend App { get; }

        /// <summary>
        /// タイムアウト時間を設定します。
        /// </summary>
        /// <param name="time">タイムアウト時間</param>
        void SetTimeout(int time);

        /// <summary>
        /// アプリケーションをアタッチします。
        /// </summary>
        /// <returns>アプリケーション操作クラス</returns>
        WindowsAppFriend Attach();

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        /// <param name="isContinue">処理を継続するかどうかを示す値</param>
        void Release(bool isContinue);

        /// <summary>
        /// アプリケーションを終了します。
        /// </summary>
        void EndProcess();
    }
}
