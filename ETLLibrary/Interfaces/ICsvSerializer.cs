using System.Collections.Generic;
using ETLLibrary.Database.Models;
using ETLLibrary.Database.Utils;

namespace ETLLibrary.Interfaces
{
    public interface ICsvSerializer
    {
        List<List<string>> Serialize(Csv info, string path);
    }
}