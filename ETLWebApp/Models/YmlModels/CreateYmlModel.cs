using Microsoft.AspNetCore.Http;

namespace ETLWebApp.Models.YmlModels
{
    public class CreateYmlModel
    {
        public IFormFile File { get; set; }
        public string Name { get; set; }
    }
}