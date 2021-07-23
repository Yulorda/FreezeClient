using UnityEngine;

namespace MVP
{
    public abstract class PresenterBehaviour<T> : MonoBehaviour where T : class
    {
        protected IDisposablePresenter<T> dc;

        public void InjectModel(IDisposablePresenter<T> model)
        {
            if (this.dc != null)
                RemoveModel();

            this.dc = model;
            OnInjectModel();
        }

        public void RemoveModel()
        {
            if (dc != null)
            {
                OnRemoveModel();
                dc.Dispose();
            }
        }

        protected virtual void OnInjectModel()
        {
        }

        protected virtual void OnRemoveModel()
        {
        }

        protected virtual void OnDestroy()
        {
            RemoveModel();
        }
    }
}