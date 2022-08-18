using System.Collections.Generic;
using ETLLibrary.Database.Gataways;
using ETLLibrary.Database.Models;
using ETLLibrary.Interfaces;

namespace ETLLibrary.Database.Managers
{
    public class PipelineManager : IPipelineManager
    {
        private PipelineGateway _gateway;
        
        public PipelineManager(EtlContext context)
        {
            _gateway = new PipelineGateway(context);
        }
        
        public string CreatePipeline(string username, string name, string content)
        {
            return _gateway.AddPipeline(username, name, content);
        }
        
        public bool DeletePipeline(string name, User user)
        {
            return _gateway.DeletePipeline(name, user.Id);
        }
        
        public List<string> GetPipelines(string username)
        {
            return _gateway.GetUserPipelines(username);
        }
        
        public DbPipeline GetDbPipeline(User user, string name)
        {
            return _gateway.GetPipeline(name, user.Id);
        }
    }
}