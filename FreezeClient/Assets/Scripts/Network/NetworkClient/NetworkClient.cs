using Serializator;
using System;
using UnityEngine;

//TODO Disposable ?
public class NetworkClient
{
    public event Action OnConnected;

    public event Action OnDisconnected;

    protected IClient client;
    protected ISerializator serializator;
    protected GenericListeners listeners = new GenericListeners();

    public bool Connected
    {
        get
        {
            return client.Connected;
        }
    }

    public NetworkClient(IClient client, ISerializator serializator)
    {
        this.client = client;
        this.serializator = serializator;
    }

    public void AddListener<T>(Action<T> handler) where T : class
    {
        listeners.AddListener(handler);
    }

    public void RemoveListener<T>(Action<T> handler) where T : class
    {
        listeners.RemoveListener(handler);
    }

    public virtual void Connect()
    {
        client.Connect();
    }

    public virtual void Send(object value)
    {
        client.Send(serializator.Serialize(value));
    }

    public virtual void Update()
    {
        if (client.Connected)
        {
            while (client.TryGetPackage(out var package))
            {
                if (serializator.TryDeserialize(package, out var T))
                {
                    listeners.Invoke(T, T.GetType());
                }
                else
                {
                    NetworkLogger(new NetworkLog(EventType.Error, "SerializeError"));
                }
            }
        }
    }

    public void NetworkLogger(NetworkLog networkLog)
    {
        switch (networkLog.eventType)
        {
            case EventType.Connected:
                Debug.Log(networkLog.message);
                break;

            case EventType.Disconnected:
                Debug.Log(networkLog.message);
                break;

            case EventType.Error:
                Debug.LogError(networkLog.message);
                break;

            case EventType.Data:
                Debug.LogWarning(networkLog.message);
                break;
        }
    }

    public void Disconnect()
    {
        client.Disconnect();
    }
}