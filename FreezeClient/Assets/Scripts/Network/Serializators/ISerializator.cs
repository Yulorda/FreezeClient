namespace Serializator
{
    public interface ISerializator
    {
        bool TryDeserialize(byte[] message, out object networkPackage);
        byte[] Serialize(object networkPackage);
    }
}