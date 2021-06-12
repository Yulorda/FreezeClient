using Serializator;
using UnityEngine;
using Zenject;

public class NetworkClientInstaller : MonoInstaller
{
    public string ip = "127.0.0.1";
    public int port = 4004;

    private NetworkClient networkClient;

    public override void InstallBindings()
    {
        networkClient = new NetworkClient(new Telepathy.Client(ip, port), new JSONSerializator());
        networkClient.Connect();

        Container.Bind<NetworkClient>().FromInstance(networkClient).AsSingle();
    }

    private void OnDestroy()
    {
        networkClient.Disconnect();
        //TODO networkClient.Dispose() ???
    }
}