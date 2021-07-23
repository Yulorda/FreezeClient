using UniRx;
using UnityEngine;

namespace MVP
{
    public class ReactiveBinder<T> : MonoBehaviour
    {
        public Component component;
        private ReactiveProperty<T> reactiveProperty;
       
        protected void Get()
        {
            Debug.Log("name " + component.name + " type " + component.GetType() + " basetype " + component.GetType().BaseType);
            var result = component.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var result2 = component.GetType().GetProperties();
            Debug.Log(result.Length);
            Debug.Log(result2.Length);
            foreach (var fi in result)
            {
                if(fi.FieldType == reactiveProperty.GetType())
                {
                    try
                    {
                        System.Object obj = (System.Object)component;
                        Debug.Log("fi name " + fi.Name + " val " + fi.GetValue(obj));
                    }
                    catch
                    {
                        Debug.Log("Not supported");
                    }
                }
            }
        }
    }
}