using System.Collections.Generic;
using ETLLibrary.Database.Utils;
using Microsoft.Data.SqlClient;

namespace ETLLibrary.Interfaces
{
    public interface ISqlServerSerializer
    {
        public List<List<string>> Serialize(DatasetInfo info);
        public List<List<string>> GetContent(SqlDataReader reader);
    }
}