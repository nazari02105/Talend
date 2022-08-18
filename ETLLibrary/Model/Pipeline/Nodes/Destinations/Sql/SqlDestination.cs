using System.Dynamic;
using ETLBox.Connection;
using ETLBox.DataFlow.Connectors;

namespace ETLLibrary.Model.Pipeline.Nodes.Destinations.Sql
{
    public class SqlDestination : DestinationNode
    {
        private string _connectionString;
        private string _tableName;

        public SqlDestination(string id, string name, string connectionString, string tableName)
        {
            Id = id;
            Name = name;
            _connectionString = connectionString;
            _tableName = tableName;
            CreateDataFlow();
        }

        private void CreateDataFlow()
        {
            SqlConnectionManager sqlConnectionManager = new SqlConnectionManager(_connectionString);
            DbDestination<ExpandoObject> dbDestination =
                new DbDestination<ExpandoObject>(sqlConnectionManager, _tableName);
            DataFlow = dbDestination;
        }
    }
}