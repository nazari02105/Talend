using System.Collections.Generic;
using System.Linq;
using ETLLibrary.Database;
using ETLLibrary.Database.Models;
using ETLLibrary.Interfaces;

namespace ETLLibrary.Authentication
{
    public class Authenticator : IAuthenticator
    {
        private EtlContext _context;
        public static Dictionary<string, User> Tokens = new Dictionary<string, User>();

        public static User GetUserFromToken(string token)
        {
            return !Tokens.ContainsKey(token) ? null : Tokens[token];
        }

        public Authenticator(EtlContext context)
        {
            _context = context;
        }

        public User ValidateUser(string usernameOrEmail, string password)
        {
            return _context.Users.FirstOrDefault(x => (x.Username == usernameOrEmail || x.Email == usernameOrEmail) && x.Password == password);
        }

        public string Login(User user)
        {
            var token = TokenGenerator.Generate(16);
            Tokens.Add(token, user);
            return token;
        }

        public void Logout(string token)
        {
            Tokens.Remove(token);
        }

        public bool UserExists(string username)
        {
            return _context.Users.FirstOrDefault(x => x.Username == username) != null;
        }
    }
}