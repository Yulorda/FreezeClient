using Serializator;
using UnityEngine;

public class NetworkConfiguration : MonoBehaviour
{
    public string ip;
    public int port;

    [HideInInspector]
    public NetworkClient networkClient;

    public void Start()
    {
        networkClient = new NetworkClient(new Telepathy.Client(ip, port), new JSONSerializator());
        networkClient.Connect();
    }
}
