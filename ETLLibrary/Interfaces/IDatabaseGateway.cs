using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ETLLibrary.Database;
using ETLLibrary.Database.Models;
using ETLLibrary.Database.Utils;

namespace ETLLibrary.Interfaces
{
    public interface IDatabaseGateway
    {
        List<string> GetUserDatasets(string username);
        
        bool DeleteDataset(string datasetName, int userId);

        static bool DatasetExist(EtlContext context, string name, User user)
        {
            var csv =
                context.CsvFiles.SingleOrDefault(x => x.Name == name && x.UserId == user.Id);
            var dbConnection =
                context.DbConnections.SingleOrDefault(x => x.Name == name && x.UserId == user.Id);
            
            return csv != null || dbConnection != null;
        }
    }
}