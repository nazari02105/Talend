using System.Collections.Generic;
using System.Linq;
using ETLLibrary.Database.Models;
using ETLLibrary.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ETLLibrary.Database.Gataways
{
    public class PipelineGateway : IPipelineGateway
    {
        public readonly EtlContext Context;

        public PipelineGateway(EtlContext context)
        {
            Context = context;
        }

        public List<string> GetUserPipelines(string username)
        {
            var user = Context.Users.Where(w => w.Username == username).Include(w => w.DbPipelines).Single();
            return user.DbPipelines.Select(document => document.Name).ToList();
        }
        
        public string AddPipeline(string username, string name, string content)
        {
            var user = Context.Users.Include(x => x.DbPipelines).Single(u => u.Username == username);

            var pipeline = GetPipeline(name, user.Id);
            if (pipeline == null)
            {
                var dbPipeline = new DbPipeline()
                {
                    Name = name,
                    User = user,
                    Content = content
                };
                user.DbPipelines.Add(dbPipeline);
                Context.SaveChanges();
                return "Pipeline created successfully.";
            }
            else
            {
                pipeline.Content = content;
                Context.SaveChanges();
                return "Pipeline updated successfully.";
            }
        }

        public bool DeletePipeline( string pipelineName, int userId)
        {
            var pipeline = Context.DbPipelines.SingleOrDefault(x => x.Name == pipelineName && x.UserId == userId);
            if (pipeline == null) return false;
            Context.DbPipelines.Remove(pipeline);
            Context.SaveChanges();
            return true;
        }
        
        public DbPipeline GetPipeline(string name, int userId)
        {
            var pipeline = Context.DbPipelines.SingleOrDefault(w => w.Name == name && w.UserId == userId);
            return pipeline;
        }
    }
}