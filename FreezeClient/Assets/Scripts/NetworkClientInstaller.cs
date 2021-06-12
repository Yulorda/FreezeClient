using Serializator;
using UnityEngine;
using Zenject;

public class NetworkClientInstaller : MonoInstaller
{
    public string ip = "127.0.0.1";
    public int port = 4004;

    public override void InstallBindings()
    {
        var networkClient = new NetworkClient(new Telepathy.Client(ip, port), new JSONSerializator());
        networkClient.Connect();

        Container.Bind<NetworkClient>().FromInstance(networkClient).AsSingle();
    }

}