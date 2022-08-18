using System.ComponentModel.DataAnnotations;

namespace ETLLibrary.Database.Models
{
    public class DbPipeline
    {
        [Key] public int Id{ get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public User User { get; set; }
        public string Content { get; set; }
    }
}