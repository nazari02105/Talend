using System.Collections.Generic;
using ETLLibrary.Database;
using ETLLibrary.Database.Models;
using ETLLibrary.Database.Utils;

namespace ETLLibrary.Interfaces
{
    public interface ISqlServerDatasetManager
    {
        List<string> GetTables(string dbName, string dbUsername, string dbPassword, string url);
        void CreateDataset(string username, DatasetInfo info);
        bool DeleteDataset( string name, User user);
        List<string> GetDatasets(string username);

        DbConnection GetDbConnection(User user, string name);
    }
}