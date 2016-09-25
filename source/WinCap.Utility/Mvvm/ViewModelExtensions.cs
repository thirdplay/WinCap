using Livet;
using System;

namespace WinCap.Utility.Mvvm
{
    public static class ViewModelExtensions
    {
        /// <summary>
        /// <see cref="IDisposable"/> オブジェクトを、指定した <see cref="ViewModel"/> の <see cref="ViewModel.CompositeDisposable"/> に追加します。
        /// </summary>
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
