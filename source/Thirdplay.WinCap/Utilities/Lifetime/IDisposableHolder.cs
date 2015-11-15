using System;
using System.Collections.Generic;

namespace Thirdplay.WinCap.Utilities.Lifetime
{
    /// <summary>
    /// IDisposableの持ち主
    /// </summary>
    public interface IDisposableHolder : IDisposable
    {
        ICollection<IDisposable> CompositeDisposable { get; }
    }
}
