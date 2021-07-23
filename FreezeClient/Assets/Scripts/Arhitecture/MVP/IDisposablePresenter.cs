using System;
using System.Collections.Generic;

namespace MVP
{
    public interface IDisposablePresenter<T> : IDisposable where T : class
    {
        T Model { get; }
        void AddToDisposable(IDisposable disposable);
        void AddToDisposable(IEnumerable<IDisposable> disposable);
    }
}