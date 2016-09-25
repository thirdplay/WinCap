using System;
using System.Collections.Generic;

namespace WinCap.Utility.Lifetime
{
    public interface IDisposableHolder : IDisposable
    {
        ICollection<IDisposable> CompositeDisposable { get; }
    }
}
