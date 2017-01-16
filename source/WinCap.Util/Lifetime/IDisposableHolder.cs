using System;
using System.Collections.Generic;

namespace WinCap.Util.Lifetime
{
    public interface IDisposableHolder : IDisposable
    {
        ICollection<IDisposable> CompositeDisposable { get; }
    }
}
