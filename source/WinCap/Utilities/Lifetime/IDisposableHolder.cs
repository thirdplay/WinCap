using System;
using System.Collections.Generic;

namespace WinCap.Utilities.Lifetime
{
    /// <summary>
    /// IDisposableの持ち主
    /// </summary>
    public interface IDisposableHolder : IDisposable
    {
        ICollection<IDisposable> CompositeDisposable { get; }
    }
}
