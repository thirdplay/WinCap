using System;

namespace Thirdplay.WinCap.Utilities.Lifetime
{
    /// <summary>
    /// Disposableの拡張
    /// </summary>
    public static class DisposableExtensions
    {
        /// <summary>
        /// <see cref="IDisposable"/>オブジェクトを指定した<see cref="IDisposableHolder.CompositeDisposable"/>に追加する。
        /// </summary>
        /// <typeparam name="T">IDisposableを継承したクラス</typeparam>
        /// <param name="disposable">IDisposableオブジェクト</param>
        /// <param name="obj">追加するオブジェクト</param>
        /// <returns>IDisposableオブジェクト</returns>
        public static T AddTo<T>(this T disposable, IDisposableHolder obj) where T : IDisposable
        {
            if (obj == null)
            {
                disposable.Dispose();
                return disposable;
            }

            obj.CompositeDisposable.Add(disposable);
            return disposable;
        }
    }
}
