using Microsoft.AspNetCore.Http;

namespace ETLWebApp.Models.CsvModels
{
    public class CreateModel
    {
        public string Details { get; set; }
        public IFormFile File { get; set; }
    }

    public class CreateModelDetails
    {
        public string Name { get; set; }
        public string ColDelimiter { get; set; }
        public string RowDelimiter { get; set; }
        public string HasHeader { get; set; }
    }
}