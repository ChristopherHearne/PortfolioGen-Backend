using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 
namespace API_Test.Models
{
    public partial class Profile
    {
        public int Id { get; set; }
        public string? ProfileName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Title { get; set; }
        public string? Email { get; set; }
        public string? Linkedin { get; set; }
        public string? About { get; set; }
        public string? Interests { get; set; }
        public string? Avatar { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Github { get; set; }
        public string? Website { get; set; }
        public string? GithubUsername { get; set; }
    }
}
