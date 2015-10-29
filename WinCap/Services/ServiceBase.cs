using Livet;
using System;
using WinCap.Lifetime;
using System.Collections.Generic;

namespace WinCap.Services
{
    /// <summary>
    /// Serviceの基底クラス
    /// </summary>
    public abstract class ServiceBase : IDisposableHolder
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected ServiceBase() { }

        /// <summary>
        /// このServiceクラスの基本CompositeDisposable。
        /// </summary>
        protected readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();
        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄する。
        /// </summary>
        public virtual void Dispose()
        {
            //ViewModel vm;
            //Console.WriteLine("Dispose:" + this.GetType().FullName);
            this.compositeDisposable.Dispose();
        }
    }
}
