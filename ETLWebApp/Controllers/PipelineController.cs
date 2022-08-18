using System;
using System.Text;
using ETLLibrary.Authentication;
using ETLLibrary.Enums;
using ETLLibrary.Interfaces;
using ETLLibrary.Model.Pipeline;
using ETLLibrary.Processing;
using ETLWebApp.Models.PipelineModel;
using ETLWebApp.Models.YmlModels;
using Microsoft.AspNetCore.Mvc;
using MediaTypeHeaderValue = Microsoft.Net.Http.Headers.MediaTypeHeaderValue;

namespace ETLWebApp.Controllers
{
    [ApiController]
    [Route("pipeline/")]
    public class PipelineController : Controller
    {
        //ToDo: refactor validate user by token
        private IPipelineManager _pipelineManager;
        private IYmlManager _ymlManager;

        public PipelineController(IPipelineManager pipelineManager, IYmlManager ymlManager)
        {
            _pipelineManager = pipelineManager;
            _ymlManager = ymlManager;
        }

        [HttpPost("create/")]
        public ActionResult Create(string token, PipelineCreateModel model)
        {
            var user = Authenticator.GetUserFromToken(token);
            if (user == null)
            {
                return Unauthorized(new {Message = "First login."});
            }

            var res = _pipelineManager.CreatePipeline(user.Username, model.Name, model.Content);
            return Ok(new {Message = res});
        }

        [HttpDelete("delete/{name}")]
        public ActionResult Delete(string token, string name)
        {
            var user = Authenticator.GetUserFromToken(token);
            if (user == null)
            {
                return Unauthorized(new {Message = "First login."});
            }

            if (_pipelineManager.DeletePipeline(name, user))
            {
                return Ok(new {Message = $"Pipeline {name} deleted successfully."});
            }

            return NotFound(new {Message = "Pipeline with this name not found."});

        }

        [Route("{name}")]
        [HttpGet]
        public ActionResult GetPipelineContent(string token, string name)
        {
            var user = Authenticator.GetUserFromToken(token);
            if (user == null)
            {
                return Unauthorized(new {Message = "First login."});
            }

            var pipeline = _pipelineManager.GetDbPipeline(user, name);

            return Ok(new {Content = pipeline.Content});
        }

        [Route("yml/upload")]
        [HttpPost]
        public ActionResult Upload([FromForm] CreateYmlModel model, string token)
        {
            var user = Authenticator.GetUserFromToken(token);
            if (user == null)
            {
                return Unauthorized(new {Message = "First login."});
            }

            try
            {
                _ymlManager.SaveYml(model.File.OpenReadStream(), model.Name, user.Username, model.File.Length);
            }
            catch (Exception e)
            {
                return Conflict(new {Message = "File upload failed due to unknown error(s)."});
            }

            return Ok(new {Message = "File uploaded successfully."});
        }


        [HttpPost("run/")]
        public IActionResult Run(string token, RunModel model)
        {
            var user = Authenticator.GetUserFromToken(token);
            if (user == null)
            {
                return Unauthorized(new {Message = "First login."});
            }

            if (Process.RunningProcessExists(user.Username))
            {
                return Conflict(new {Message = "You can't run more than one process at the same time."});
            }
            
            var pipeline = _pipelineManager.GetDbPipeline(user, model.Name);
            if (pipeline == null)
            {
                return NotFound(new {Message = $"Pipeline with name {model.Name} not found;"});
            }
            try
            {
                var p = new PipelineConvertor(user.Username, pipeline.Content).Pipeline;
                var process = new Process(user.Username, model.Name, p);
                // var p = new PipelineConvertor(user.Username, pipeline.Content).Pipeline;
                // var process = new Process(user.Username, model.Name, null);
                Process.AddToProcesses(process);
                process.Start();
            }
            catch (Exception e)
            {
                return StatusCode(500, new {Message = e.Message});
            }
            
            return Ok(new {Messsage = "Process started successfully"});
        }

        [HttpGet("{name}/status/")]
        public IActionResult Status(string name, string token)
        {
            var user = Authenticator.GetUserFromToken(token);
            if (user == null)
            {
                return Unauthorized(new {Message = "First login."});
            }

            var process = Process.GetProcess(user.Username, name);
            if (process.Status == ETLLibrary.Enums.Status.Failed)
            {
                return Ok(new {Status = Enum.GetName(typeof(Status), process.Status), Message = process.ErrorMessage});
            }
            return Ok(new {Status = Enum.GetName(typeof(Status), process.Status)});
        }

        [HttpPost("cancel/")]
        public ActionResult Cancel(string token)
        {
            var user = Authenticator.GetUserFromToken(token);
            if (user == null)
            {
                return Unauthorized(new {Message = "First login."});
            }
            
            if (!Process.RunningProcessExists(user.Username))
            {
                return BadRequest(new {Message = "You don't have a running process."});
            }

            Process.CancelProcess(user.Username);
            return Ok(new {Message = "Process canceled successfully."});
        }
        

        [HttpGet("yml/download/{name}")]
        public ActionResult Download(string token, string name)
        {
            var user = Authenticator.GetUserFromToken(token);
            if (user == null)
            {
                return Unauthorized(new {Message = "First login."});
            }

            try
            {
                var yml = _ymlManager.GetYml(user.Id, name);
                return GenerateYmlFile(name, yml);
            }
            catch (Exception e)
            {
                return StatusCode(500, new {Message = e.Message});
            }
        }

        private FileContentResult GenerateYmlFile(string name, string yml)
        {
            return new(Encoding.UTF8.GetBytes(yml),
                new MediaTypeHeaderValue("text/yaml"))
            {
                FileDownloadName = $"{name}.yml"
            };
        }
    }
}