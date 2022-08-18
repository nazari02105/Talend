using System.Collections.Generic;
using System.Linq;
using ETLLibrary.Authentication;
using ETLLibrary.Database;
using ETLLibrary.Database.Models;
using ETLLibrary.Interfaces;
using ETLWebApp.Models.AuthenticationModels;
using Microsoft.AspNetCore.Mvc;

namespace ETLWebApp.Controllers
{
    [ApiController]
    [Route("users/")]
    public class UsersController : Controller
    {
        private EtlContext _context;
        private IAuthenticator _authenticator;
        private ICsvDatasetManager _csvManager;
        private ISqlServerDatasetManager _sqlServerManager;
        private IPipelineManager _pipelineManager;

        public UsersController(EtlContext context, IAuthenticator authenticator, ICsvDatasetManager csvManager,
            ISqlServerDatasetManager sqlServerManager, IPipelineManager pipelineManager)
        {
            _context = context;
            _authenticator = authenticator;
            _csvManager = csvManager;
            _sqlServerManager = sqlServerManager;
            _pipelineManager = pipelineManager;
        }

        [HttpPost("signup")]
        public ActionResult SignUp(RegisterModel model)
        {
            if (_authenticator.UserExists(model.Username))
            {
                return Conflict(new {Message = "User with this username already exists."});
            }
            var user = new User()
            {
                Username = model.Username,
                Password = model.Password,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(new {Message = "User registered successfully!"});
        }

        [HttpPost("login")]
        public ActionResult Login(LoginModel model)
        {
            var user = _authenticator.ValidateUser(model.Username, model.Password);
            if (user != null)
            {
                var token = _authenticator.Login(user);
                return Ok(new {Token = token});
            }

            return Unauthorized("Authentication failed");
        }

        [HttpPost("logout")]
        public ActionResult Logout(LogoutModel model)
        {
            _authenticator.Logout(model.Token);
            return Ok(new {Message = "User logout successfully!"});
        }

        [HttpGet("{username}/csvs")]
        public ActionResult GetCsvFiles(string username)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound(new {Message = "User with this username not found"});
            }

            return Ok(new {CsvFiles = _csvManager.GetCsvFiles(username)});
        }

        [HttpGet("{username}/sqlservers")]
        public ActionResult GetSqlServerDbNames(string username)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound(new {Message = "User with this username not found"});
            }

            return Ok(new {DbNames = _sqlServerManager.GetDatasets(username)});
        }
        
        [HttpGet("{username}/datasets")]
        public ActionResult GetDatasets(string username)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound(new {Message = "User with this username not found"});
            }

            var csvDatasets = _csvManager.GetCsvFiles(username);
            var sqlServers = _sqlServerManager.GetDatasets(username);

            var result = new List<Dictionary<string, string>>();
            
            csvDatasets.ForEach(csv => result.Add(new Dictionary<string, string>(){{"name", csv}, {"type", "csv"}}));
            sqlServers.ForEach(sqlServer => result.Add(new Dictionary<string, string>(){{"name", sqlServer}, {"type", "sqlserver"}}));

            return Ok(new {Datasets = result});
        }
        
        [HttpGet("{username}/pipelines")]
        public ActionResult GetPipelines(string username)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound(new {Message = "User with this username not found"});
            }

            var res = _pipelineManager.GetPipelines(user.Username);
            return Ok(new {Pipelines = res});
        }


        [HttpGet("{token}")]
        public ActionResult GetUserByToken(string token)
        {
            var user = Authenticator.GetUserFromToken(token);
            if (user == null)
            {
                return Unauthorized(new {Message = "First login."});
            }

            return Ok(user);
        }
    }
}