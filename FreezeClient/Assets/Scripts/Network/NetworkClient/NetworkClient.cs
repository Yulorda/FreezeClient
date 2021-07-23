using Serializator;
using System;
using UniRxMessageBroker;
using UnityEngine;
using UniRx;
using System.Collections.Generic;

public class NetworkClient : IDisposable
{
    public event Action OnConnected;
    public event Action OnDisconnected;

    protected IClient client;
    protected ISerializator serializator;
    protected Broker messageBroker = new Broker();
    private List<IDisposable> disposables = new List<IDisposable>();

    public NetworkClient(IClient client, ISerializator serializator)
    {
        this.client = client;
        this.serializator = serializator;
    }

    public IDisposable AddListener<T>(Action<T> handler) where T : class
    {
        return messageBroker.Receive<T>().Subscribe(handler);
    }

    public virtual void Connect()
    {
        client.Connect();
        disposables.Add(client.ObserveEveryValueChanged(x => x.State, FrameCountType.EndOfFrame).Subscribe(ConnectionStateChange));
    }

    private void ConnectionStateChange(IClient.Status state)
    {
        switch (state)
        {
            case IClient.Status.Disconnect:
                OnDisconnected?.Invoke();
                break;
            case IClient.Status.Connectind:
                break;
            case IClient.Status.Connected:
                OnConnected?.Invoke();
                break;
        }
    }

    public virtual void Send(object value)
    {
        client.Send(serializator.Serialize(value));
    }

    public virtual void Update()
    {
        if (client.State == IClient.Status.Connected)
        {
            while (client.TryGetPackage(out var package))
            {
                if (serializator.TryDeserialize(package, out var type, out var value))
                {
                    messageBroker.Publish(value, type);
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

    public void Dispose()
    {
        messageBroker.Dispose();
        client.Dispose();
        disposables.Clear();
    }
}