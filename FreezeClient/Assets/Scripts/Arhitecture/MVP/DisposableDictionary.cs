using System;
using System.Collections.Generic;

public class DisposableDictionary : IDisposable
{
    private Dictionary<object, List<IDisposable>> disposables = new Dictionary<object, List<IDisposable>>();

    public void Add(object value, IDisposable disposable)
    {
        CreateKey(value);
        disposables[value].Add(disposable);
    }

    public void Add(object value, IEnumerable<IDisposable> disposable)
    {
        CreateKey(value);
        disposables[value].AddRange(disposable);
    }

    private void CreateKey(object value)
    {
        if (disposables.ContainsKey(value))
            return;

        disposables.Add(value, new List<IDisposable>());
    }

    public void Dispose(object value)
    {
        if (disposables.TryGetValue(value, out var result))
        {
            result.ForEach(x => x.Dispose());
        }
        disposables.Remove(value);
    }

    public void Dispose()
    {
        foreach (var value in disposables.Values)
        {
            value.ForEach(x => x.Dispose());
        }
        disposables.Clear();
    }
}