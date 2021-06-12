using System;
using System.Collections.Generic;

public class GenericListeners
{
    private interface IGenericListener
    {
        void Invoke(object obj);
    }

    private class GenericListener<T> : IGenericListener where T : class
    {
        public Action<T> listeners;

        public void AddListener(Action<T> listener)
        {
            listeners -= listener;
            listeners += listener;
        }

        public void RemoveListener(Action<T> listener)
        {
            listeners -= listener;
        }

        public void Invoke(object obj)
        {
            listeners?.Invoke(obj as T);
        }
    }

    private Dictionary<Type, IGenericListener> listeners = new Dictionary<Type, IGenericListener>();

    public void AddListener<T>(Action<T> handler) where T : class
    {
        Type t = typeof(T);
        IGenericListener listener;

        if (listeners.ContainsKey(t))
        {
            listener = listeners[t];
        }
        else
        {
            listener = new GenericListener<T>();
            listeners.Add(t, listener);
        }

        (listener as GenericListener<T>).AddListener(handler);
    }

    public void RemoveListener<T>(Action<T> handler) where T : class
    {
        Type t = typeof(T);
        if (listeners.ContainsKey(t))
        {
            (listeners[t] as GenericListener<T>).RemoveListener(handler);
        }
    }

    public void RemoveAllListeners()
    {
        listeners.Clear();
    }

    public void Invoke<T>(T obj)
    {
        Invoke(obj, typeof(T));
    }

    public void Invoke(object obj, Type t)
    {
        IGenericListener listener = null;
        if (listeners.TryGetValue(t, out listener))
        {
            listener.Invoke(obj);
        }
    }

    public void InvokeAll(object obj)
    {
        foreach (var pair in listeners)
        {
            pair.Value?.Invoke(obj);
        }
    }
}
