using Livet;
using System;

namespace WinCap.Utilities.Mvvm
{
    /// <summary>
    /// ViewModelの拡張
    /// </summary>
    public static class ViewModelExtensions
    {
        /// <summary>
        /// <see cref="IDisposable"/>オブジェクトを指定した<see cref="ViewModel"/>の<see cref="ViewModel.CompositeDisposable"/>に追加します。
        /// </summary>
        /// <typeparam name="T">IDisposableを実装したクラス<see cref="IDisposable"/></typeparam>
        /// <param name="disposable">IDisposableオブジェクト</param>
        /// <param name="vm">ViewModel</param>
        /// <returns>IDisposableオブジェクト</returns>
        public static T AddTo<T>(this T disposable, ViewModel vm) where T : IDisposable
        {
            if (vm == null)
            {
                disposable.Dispose();
                return disposable;
            }

            vm.CompositeDisposable.Add(disposable);
            return disposable;
        }
    }
}
