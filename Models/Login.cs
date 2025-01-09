using System.ComponentModel.DataAnnotations;
using CommunityManager.Models;

namespace CommunityManager.Models
{
    public class Login
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public Login() { }

        public Login(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public bool ValidateCredentials(string correctUsername, string correctPassword)
        {
            // Checks if the entered credentials match the correct values
            return Username == correctUsername && Password == correctPassword;
        }
    }
}
