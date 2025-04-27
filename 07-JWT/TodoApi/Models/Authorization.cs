using System;
using System.Collections.Generic;

namespace TodoWebApp.Models
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string User = "User";
    }

    public static class Permissions
    {
        public const string CreateTodo = "CreateTodo";
        public const string UpdateTodo = "UpdateTodo";
        public const string DeleteTodo = "DeleteTodo";
        public const string ViewTodo = "ViewTodo";
        public const string ManageUsers = "ManageUsers";
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = Roles.User;
        public List<string> Permissions { get; set; } = new List<string>();
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }

    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; } = string.Empty;
        public bool IsRevoked { get; set; }
    }

    public class RegistrationRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
} 