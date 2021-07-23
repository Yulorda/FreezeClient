using System;
using System.Runtime.CompilerServices;

namespace MVP
{
    public class DisposableDictionaryModel<T> : IDisposable where T : class
    {
        public T Model { get; private set; }
        public DisposableDictionary disposableDictionary = new DisposableDictionary();

        public DisposableDictionaryModel(T model)
        {
            this.Model = model;
        }

        public void Dispose()
        {
            disposableDictionary.Dispose();
            Model = null;
        }
    }
}