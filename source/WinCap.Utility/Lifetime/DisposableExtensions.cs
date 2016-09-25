﻿using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCap.Utility.Lifetime
{
    public static class DisposableExtensions
    {
        /// <summary>
        /// <see cref="IDisposable"/> オブジェクトを、指定した <see cref="IDisposableHolder.CompositeDisposable"/> に追加します。
        /// </summary>
        public static T AddTo<T>(this T disposable, IDisposableHolder obj) where T : IDisposable
        {
            if (obj == null)
            {
                disposable.Dispose();
            }
            else
            {
                obj.CompositeDisposable.Add(disposable);
            }

            return disposable;
        }

        public static T AddTo<T>(this T disposable, MultipleDisposable obj) where T : IDisposable
        {
            if (obj == null)
            {
                disposable.Dispose();
            }
            else
            {
                obj.Add(disposable);
            }

            return disposable;
        }
    }
}
