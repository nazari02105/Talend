using System;
using ETLLibrary.Authentication;
using ETLLibrary.Database.Utils;
using ETLLibrary.Interfaces;
using ETLWebApp.Models.CsvModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ETLWebApp.Controllers
{
    [ApiController]
    [Route("dataset/csv/")]
    public class CsvDatasetController : ControllerBase
    {
        private ICsvDatasetManager _manager;

        public CsvDatasetController(ICsvDatasetManager manager)
        {
            _manager = manager;
        }

        [HttpPost("create/")]
        public ActionResult Create([FromForm] CreateModel model, string token)
        {
            var user = Authenticator.GetUserFromToken(token);
            if (user == null)
            {
                return Unauthorized(new {Message = "First login."});
            }

            var details = GetCreateModelDetails(model.Details);
            var info = new CsvInfo()
            {
                Name = details.Name,
                ColDelimiter = details.ColDelimiter,
                RowDelimiter = details.RowDelimiter,
                HasHeader = details.HasHeader == "true"
            };
            try
            {
                _manager.SaveCsv(model.File.OpenReadStream(), user.Username, details.Name + ".csv", info,
                    model.File.Length);
            }
            catch (Exception e)
            {
                return Conflict(new {Message = e.Message});
            }

            return Ok(new {Message = "File uploaded successfully."});
        }

        private CreateModelDetails GetCreateModelDetails(string modelDetails)
        {
            return JsonConvert.DeserializeObject<CreateModelDetails>(modelDetails);
        }

        [Route("{name}")]
        [HttpGet]
        public ActionResult GetContent(string name, string token)
        {
            var user = Authenticator.GetUserFromToken(token);
            if (user == null)
            {
                return Unauthorized(new {Message = "First login."});
            }

            var response = _manager.GetCsvContent(user, name);
            if (response == null)
            {
                return NotFound(new {Message = "Not Found"});
            }

            return Ok(new {Content = response});
        }

        [HttpDelete("delete/{name}")]
        public ActionResult Delete(string name, string token)
        {
            var user = Authenticator.GetUserFromToken(token);
            if (user == null)
            {
                return Unauthorized(new {Message = "First login."});
            }

            if (_manager.DeleteCsv(user, name))
            {
                return Ok(new {Message = "File deleted successfully."});
            }

            return NotFound(new {Message = "Dataset with this name not found;"});
        }

        [HttpGet("download/{name}")]
        public ActionResult Download(string token, string name)
        {
            var user = Authenticator.GetUserFromToken(token);
            if (user == null)
            {
                return Unauthorized(new {Message = "First login."});
            }

            var path = CsvConfigurator.GetFilePath(user.Username, name + ".csv");
            try
            {
                var file = System.IO.File.ReadAllBytes(path);
                return new FileContentResult(file, "text/csv")
                {
                    FileDownloadName = name + ".csv"
                };
            }
            catch (Exception e)
            {
                return NotFound(new {Message = e.Message});
            }

            
        }
    }
}