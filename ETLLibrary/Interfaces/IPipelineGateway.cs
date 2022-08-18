using System.Collections.Generic;
using ETLLibrary.Database.Models;

namespace ETLLibrary.Interfaces
{
    public interface IPipelineGateway
    {
        List<string> GetUserPipelines(string username);
        string AddPipeline(string username, string name, string content);
        public bool DeletePipeline(string pipelineName, int userId);
        public DbPipeline GetPipeline(string name, int userId);
    }
}