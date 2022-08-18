using System.ComponentModel.DataAnnotations;

namespace ETLLibrary.Database.Models
{
    public class DbConnection
    {
        [Key]
        public int Id { get; set; }
        public string DbName { get; set; }
        public string DbUsername { get; set; }
        public string DbPassword { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Table { get; set; }
        
        public User User { get; set; }
        public int UserId { get; set; }
    }
}