using System;
using ETLLibrary;
using ETLLibrary.Authentication;
using ETLLibrary.Database.Utils;
using ETLLibrary.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ETLWebApp.Controllers
{
    [ApiController]
    [Route("dataset/sqlserver/")]
    public class SqlServerDatasetController : Controller
    {
        private ISqlServerDatasetManager _manager;
        private ISqlServerSerializer _serializer;

        public SqlServerDatasetController(ISqlServerDatasetManager manager, ISqlServerSerializer serializer)
        {
            _manager = manager;
            _serializer = serializer;
        }
        
        [HttpPost("create/")]
        public ActionResult Create(string token, DatasetInfo model)
        {
            var user = Authenticator.GetUserFromToken(token);
            if (user == null)
            {
                return Unauthorized(new {Message = "First login."});
            }

            try
            {
                _manager.CreateDataset(user.Username, model);            }
            catch (Exception e)
            {
                return Conflict(new {Message = e.Message});
            }
            return Ok(new {Message = "Dataset added successfully."});
        }

        [HttpDelete("delete/{name}")]
        public ActionResult Delete(string token, string name)
        {
            var user = Authenticator.GetUserFromToken(token);
            if (user == null)
            {
                return Unauthorized(new {Message = "First login."});
            }

            if (_manager.DeleteDataset(name, user))
            {
                return Ok(new {Message = $"Dataset {name} deleted successfully."});
            }

            return NotFound(new {Message = "Dataset with this name not found."});
        }
        
        
        [HttpGet("tables/")]
        public ActionResult GetTables( string dbName, string dbUsername, string dbPassword, string url)
        {
            var result = _manager.GetTables(dbName, dbUsername, dbPassword, url);
            if (result == null)
            {
                return BadRequest(new {Message = "Database not found"});
            }
            else
            {
                return Ok(new {TableNames = result});
            }
        }
        
        
        
        [Route("{name}")]
        [HttpGet]
        public ActionResult GetSerializedContent(string token, string name)
        {
            var user = Authenticator.GetUserFromToken(token);
            if (user == null)
            {
                return Unauthorized(new {Message = "First login."});
            }

            var dbConnection = _manager.GetDbConnection(user, name);

            var info = new DatasetInfo()
            {
                DbName = dbConnection.DbName,
                DbPassword = dbConnection.DbPassword,
                DbUsername = dbConnection.DbPassword,
                Url = dbConnection.Url,
                Table = dbConnection.Table
            };
            
            
            var result = _serializer.Serialize(info);
            if (result == null)
            {
                return NotFound(new {Message = "Connection interrupted"});
            }
            else
            {
                return Ok(new {Content = result});
            }
        }
        
    }
}