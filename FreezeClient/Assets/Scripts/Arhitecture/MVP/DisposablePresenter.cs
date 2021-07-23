using System;
using System.Collections.Generic;

namespace MVP
{
    public class DisposablePresenter<T> : IDisposablePresenter<T> where T : class 
    {
        public T Model { get; private set; }
        protected readonly List<IDisposable> disosables = new List<IDisposable>();

        public DisposablePresenter(T model)
        {
            this.Model = model;
        }

        public void AddToDisposable(IDisposable disposable)
        {
            disosables.Add(disposable);
        }

        public void AddToDisposable(IEnumerable<IDisposable> disposable)
        {
            disosables.AddRange(disposable);
        }

        public void Dispose()
        {
            disosables.ForEach(x => x.Dispose());
            disosables.Clear();
        }
    }
}