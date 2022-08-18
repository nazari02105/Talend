using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ETLLibrary.Database.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public List<Csv> CsvFiles { get; set; }
        
        public List<DbConnection> DbConnections { get; set; }
        public List<DbPipeline> DbPipelines { get; set; }
    }
}