using System.ComponentModel.DataAnnotations;

namespace ETLLibrary.Database.Models
{
    public class Csv
    {
        [Key] public int Id{ get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
        public User User { get; set; }
        public string ColDelimiter { get; set; }
        public string RowDelimiter { get; set; }
        public bool HasHeader { get; set; }
    }
}