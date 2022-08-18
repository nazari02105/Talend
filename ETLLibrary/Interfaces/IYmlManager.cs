using System.IO;

namespace ETLLibrary.Interfaces
{
    public interface IYmlManager
    {
        void SaveYml(Stream openReadStream, string modelName, string username, long fileLength);
        string GetYml(int userId, string name);
    }
}