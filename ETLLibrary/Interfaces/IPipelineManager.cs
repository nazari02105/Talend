using System.Collections.Generic;
using ETLLibrary.Database.Models;

namespace ETLLibrary.Interfaces
{
    public interface IPipelineManager
    {
        public string CreatePipeline(string username, string name, string content);
        public bool DeletePipeline(string name, User user);
        public List<string> GetPipelines(string username);
        public DbPipeline GetDbPipeline(User user, string name);
    }
}