using System.Collections.Generic;
using System.IO;
using ETLLibrary.Database;
using ETLLibrary.Database.Models;
using ETLLibrary.Database.Utils;

namespace ETLLibrary.Interfaces
{
    public interface ICsvDatasetManager
    {
        void SaveCsv(Stream stream, string username, string fileName, CsvInfo info, long fileLength);
        List<string> GetCsvFiles(string username);

        List<List<string>> GetCsvContent(User user, string fileName);

        bool DeleteCsv(User user, string fileName);
    }
}