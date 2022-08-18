using System.Dynamic;
using ETLBox.Connection;
using ETLBox.DataFlow.Connectors;

namespace ETLLibrary.Model.Pipeline.Nodes.Sources.Sql
{
    public class SqlSource : SourceNode // only works with SqlServer
    {
        private string _connectionString;
        private string _tableName;
        public SqlSource(string id, string name, string connectionString, string tableName)
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
            DbSource<ExpandoObject> dbSource = new DbSource<ExpandoObject>(sqlConnectionManager , _tableName);
            DataFlow = dbSource;
        }
    }
}