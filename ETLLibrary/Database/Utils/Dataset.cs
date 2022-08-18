using System;
using System.Linq;
using ETLLibrary.Enums;

namespace ETLLibrary.Database.Utils
{
    public static class Dataset
    {
        public static DatasetType TypeOf(string username, string datasetName)
        {
            using var context = new EtlContext();
            var user = context.Users.Single(u => u.Username == username);
            var csv = context.CsvFiles.SingleOrDefault(x => x.Name == datasetName && x.UserId == user.Id);
            if (csv != null) return DatasetType.Csv;
            var dbConnection = context.DbConnections.SingleOrDefault(x => x.Name == datasetName && x.UserId == user.Id);
            if (dbConnection != null) return DatasetType.SqlServer;
            throw new Exception("Dataset not found");
        }
    }
}