using System;

namespace WinCap.Util.Serialization
{
    /// <summary>
    /// シリアル化機能を提供するインターフェイス。
    /// </summary>
    public interface ISerializationProvider
    {
        /// <summary>
        /// 読み込み状態
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// 設定を保存します。
        /// </summary>
        void Save();

        /// <summary>
        /// 設定を読み込みます。
        /// </summary>
        void Load();

        /// <summary>
        /// 設定をリセットします。
        /// </summary>
        void Reset();
        
        /// <summary>
        /// 設定のリセットが発生したときに発生します。
        /// </summary>
        event EventHandler Reseted;

        /// <summary>
        /// 設定のリロードが発生したときに発生します。
        /// </summary>
        event EventHandler Reloaded;

        /// <summary>
        /// 指定したキーに値を設定します。
        /// </summary>
        /// <typeparam name="T">任意のクラス</typeparam>
        /// <param name="key">キー</param>
        /// <param name="value">値</param>
        void SetValue<T>(string key, T value);

        /// <summary>
        /// 指定したキーに関連付けられている値を取得します。
        /// </summary>
        /// <typeparam name="T">任意のクラス</typeparam>
        /// <param name="key">キー</param>
        /// <param name="value">取得した値</param>
        /// <returns>取得結果</returns>
        bool TryGetValue<T>(string key, out T value);

        /// <summary>
        /// 指定したキーに関連付けられている値を削除します。
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>削除結果</returns>
        bool RemoveValue(string key);
    }
}
