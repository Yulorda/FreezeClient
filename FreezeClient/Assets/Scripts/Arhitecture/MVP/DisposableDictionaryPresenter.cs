using System;
using System.Collections.Generic;

namespace MVP
{
    public class DisposableDictionaryPresenter<T> : IDisposablePresenter<T> where T : class
    {
        public T Model => disposableDictionaryModel.Model;
        public DisposableDictionaryModel<T> disposableDictionaryModel;

        public DisposableDictionaryPresenter(DisposableDictionaryModel<T> disposableDictionaryModel)
        {
            this.disposableDictionaryModel = disposableDictionaryModel;
        }

        public void AddToDisposable(IDisposable disposable)
        {
            disposableDictionaryModel.disposableDictionary.Add(this, disposable);
        }

        public void AddToDisposable(IEnumerable<IDisposable> disposable)
        {
            disposableDictionaryModel.disposableDictionary.Add(this, disposable);
        }

        public void Dispose()
        {
            disposableDictionaryModel.disposableDictionary.Dispose(this);
        }
    }
}