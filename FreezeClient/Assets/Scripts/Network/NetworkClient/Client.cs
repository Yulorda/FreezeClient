using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;

namespace Telepathy
{
    public class Client : Common, IClient
    {
        TcpClient client;

        public Action<NetworkLog> actionLog;

        private Thread receiveThread;
        private Thread sendThread;

        public bool Connected => client != null &&
                                   client.Client != null &&
                                   client.Client.Connected;

        private volatile bool _Connecting;
        public bool Connecting => _Connecting;

        private SafeQueue<byte[]> sendQueue = new SafeQueue<byte[]>();

        private ManualResetEvent sendPending = new ManualResetEvent(false);


        private string ip;
        private int port;

        public Client(string ip, int port) 
        {
            this.ip = ip;
            this.port = port;
        }

        private void ReceiveThreadFunction(string ip, int port)
        {
            try
            {
                client.Connect(ip, port);
                _Connecting = false;

                client.NoDelay = NoDelay;
                client.SendTimeout = SendTimeout;

                sendThread = new Thread(() => { SendLoop(client, sendQueue, sendPending, actionLog); });
                sendThread.IsBackground = true;
                sendThread.Start();

                ReceiveLoop(client, receiveQueue, MaxMessageSize, actionLog);
            }
            catch (SocketException exception)
            {
                actionLog?.Invoke(new NetworkLog(EventType.Error, "Client Recv: failed to connect to ip=" + ip + " port=" + port + " reason=" + exception));
            }
            catch (ThreadInterruptedException)
            {
                // expected if Disconnect() aborts it
            }
            catch (ThreadAbortException)
            {
                // expected if Disconnect() aborts it
            }
            catch (Exception exception)
            {
                // something went wrong. probably important.
                actionLog?.Invoke(new NetworkLog(EventType.Error, "Client Recv Exception: " + exception));
            }

            actionLog?.Invoke(new NetworkLog(EventType.Disconnected));
            sendThread?.Interrupt();
            _Connecting = false;
            client?.Close();
        }

        public void Connect(string ip, int port)
        {
            if (Connecting || Connected)
            {
                actionLog?.Invoke(new NetworkLog(EventType.Error, "Telepathy Client can not create connection because an existing connection is connecting or connected"));
                return;
            }

            _Connecting = true;

            receiveQueue = new ConcurrentQueue<byte[]>();

            sendQueue.Clear();

            client = new TcpClient();
            client.Client = null;

            receiveThread = new Thread(() => { ReceiveThreadFunction(ip, port); });
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        public void Disconnect()
        {
            if (Connecting || Connected)
            {
                client.Close();
                receiveThread?.Interrupt();
                _Connecting = false;
                sendQueue.Clear();
                client = null;
            }
        }

        public bool Send(byte[] data)
        {
            if (Connected)
            {
                sendQueue.Enqueue(data);
                sendPending.Set();
                return true;
            }
            actionLog?.Invoke(new NetworkLog(EventType.Error, "Client.Send: not connected!"));
            return false;
        }

        public virtual void Connect()
        {
            Connect(ip, port);
        }

        public bool TryGetPackage(out byte[] networkPackage)
        {
            return GetNextMessage(out networkPackage);
        }
    }
}
