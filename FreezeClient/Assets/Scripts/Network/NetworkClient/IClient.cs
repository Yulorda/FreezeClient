using System;

public interface IClient
{
    bool Connected { get; }
    void Connect();
    void Disconnect();
    bool TryGetPackage(out byte[] networkPackage);
    bool Send(byte[] networkPackage);
}
