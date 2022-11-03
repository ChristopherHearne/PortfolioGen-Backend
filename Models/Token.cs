using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Test.Models
{
    public partial class Token
    {
        public int Id { get; set; }
        public string? AccessToken { get; set; }
        public string? Scope { get; set; }
        public string? TokenType { get; set; }
        [ForeignKey("UserId")]
        public int? UserId { get; set; }
    }
}
