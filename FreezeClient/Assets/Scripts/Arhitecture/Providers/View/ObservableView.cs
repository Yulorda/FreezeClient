using System;
using UniRx;
using UnityEngine;

namespace Inconspicuous.Framework {
    public abstract class ObservableView : MonoBehaviour, IView
    {
        private readonly CompositeDisposable disposable = new CompositeDisposable();

        public GameObject GameObject
        {
            get { return gameObject; }
        }

        protected virtual void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposable.IsDisposed)
            {
                if (disposing)
                {
                    // Do nothing;
                }
                disposable.Dispose();
            }
        }

        public static implicit operator CompositeDisposable(ObservableView view)
        {
            return view.disposable;
        }
    }
}
