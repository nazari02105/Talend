using System.Dynamic;
using System.Linq;
using ETLLibrary.Database.Models;
using ETLLibrary.Model.Pipeline.Nodes.Destinations.Csv;
using ETLLibrary.Model.Pipeline.Nodes.Sources.Csv;
using Microsoft.EntityFrameworkCore;

namespace ETLLibrary.Database.Utils
{
    public static class PipelineConfigurator
    {
        public static string GetCsvPath(string username, string name)
        {
            using var context = new EtlContext();
            var user = context.Users.SingleOrDefault(u => u.Username == username);
            var csv = context.CsvFiles.SingleOrDefault(x => x.Name == name && x.UserId == user.Id);
            return CsvConfigurator.GetFilePath(username, csv.FileName);
        }

        public static ConnectionInfo GetConnectionString(string username, string name)
        {
            using var context = new EtlContext();
            var user = context.Users.SingleOrDefault(u => u.Username == username);
            var dbConnection = context.DbConnections.SingleOrDefault(x => x.Name == name && x.UserId == user.Id);
            var connectionString = DatabaseConfigurator.GetConnectionString(dbConnection.DbName,
                dbConnection.DbUsername, dbConnection.DbPassword, dbConnection.Url);
            return new ConnectionInfo()
            {
                ConnectionString = connectionString,
                TableName = dbConnection.Table
            };
        }

        public static string GetCsvDelimiter(string username, string datasetName)
        {
            using var context = new EtlContext();
            var csv = GetCsv(username, datasetName, context);
            return csv.ColDelimiter ?? ",";
        }
        


        private static Csv GetCsv(string username, string datasetName, EtlContext context)
        {
            var csv = context.CsvFiles
                .Include(c => c.User)
                .Single(x => x.Name == datasetName && x.User.Username == username);
            return csv;
        }
    }

    public class ConnectionInfo
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
    }
}