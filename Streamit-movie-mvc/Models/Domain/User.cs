﻿namespace Streamit_movie_mvc.Models.Domain
{
    public class User
    {
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public byte[]? Password { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public string? DisplayName { get; set; }
        public string? Avatar { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
