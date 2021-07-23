using UnityEngine;

namespace MVP
{
    public class ReactiveBinderInt : ReactiveBinder<int>
    {
        [ContextMenu("Get")]
        public void Get2()
        {
            this.Get();
        }
    }
}