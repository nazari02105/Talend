using System.ComponentModel.DataAnnotations;

namespace ETLWebApp.Models.AuthenticationModels
{
    public class RegisterModel
    {
        public string Username { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}