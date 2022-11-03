using System;
using System.Collections.Generic;

namespace API_Test.Models
{
    public partial class Token
    {
        public string? AccessToken { get; set; }
        public string? Scope { get; set; }
        public string? TokenType { get; set; }
        public int? UserId { get; set; }

        public virtual Profile? User { get; set; }
    }
}
