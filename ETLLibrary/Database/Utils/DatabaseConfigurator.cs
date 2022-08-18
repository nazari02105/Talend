namespace ETLLibrary.Database.Utils
{
    public static class DatabaseConfigurator
    {
        public static string GetSqlQuery(string dbName)
        {
            return $"SELECT TABLE_NAME FROM {dbName}.INFORMATION_SCHEMA.TABLES;";
        }

        public static string GetConnectionString(string dbName, string dbUsername, string dbPassword, string url)
        {
            return $"Data Source={url};Initial Catalog={dbName};Integrated Security=True;User ID={dbUsername};Password={dbPassword};Pooling=False;Application Name=sqlops-connection-string";
        }
    }
}