using System;
using System.Collections.Generic;
using ETLLibrary.Database.Utils;
using ETLLibrary.Interfaces;
using Microsoft.Data.SqlClient;

namespace ETLLibrary.Serializers
{
    public class SqlServerSerializer : ISqlServerSerializer
    {
        public List<List<string>> Serialize(DatasetInfo info)
        {
            var queryString = $"SELECT * FROM {info.Table};";
            var connectionString =
                DatabaseConfigurator.GetConnectionString(info.DbName, info.DbUsername, info.DbPassword, info.Url);
            var connection = new SqlConnection(connectionString);
            var command = new SqlCommand(queryString, connection);
            try
            {
                command.Connection.Open();
            }
            catch (Exception e)
            {
                return null;
            }
            var reader = command.ExecuteReader();


            return GetContent(reader);
        }

        public List<List<string>> GetContent(SqlDataReader reader)
        {
            var result = new List<List<string>>();
            
            var columns = new List<string>();
            for(var i=0;i<reader.FieldCount;i++)
            {
                columns.Add(reader.GetName(i));
            }

            result.Add(columns);
            
            while (reader.Read())
            {
                var row = new List<string>();
                for (int i = 0; i < reader.FieldCount; ++i)
                {
                    row.Add((reader.GetValue(i)).ToString());
                }

                result.Add(row);
            }

            return result;
        }
    }
}