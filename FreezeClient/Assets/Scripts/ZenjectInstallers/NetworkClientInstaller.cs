using Serializator;
using System.Collections;
using UnityEngine;
using Zenject;

public class NetworkClientInstaller : MonoInstaller
{
    public string ip = "127.0.0.1";
    public int port = 4004;

    private NetworkClient networkClient;

    public override void InstallBindings()
    {
        var client = new Telepathy.Client(ip, port);
        networkClient = new NetworkClient(client, new JSONSerializator());
        Container.Bind<NetworkClient>().FromInstance(networkClient).AsSingle();
        client.actionLog = (x) => networkClient.NetworkLogger(x);
    }

    private void OnDestroy()
    {
        networkClient.Disconnect();
        //TODO networkClient.Dispose() ???
    }

    private IEnumerator UpdateInformation()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            networkClient.Update();
        }
    }

    [ContextMenu(nameof(ConnectToServer))]
    public void ConnectToServer()
    {
        networkClient.Connect();
        StartCoroutine(UpdateInformation());
    }
}