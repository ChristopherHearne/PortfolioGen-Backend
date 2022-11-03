using System;
using System.Collections.Generic;

namespace API_Test.Models
{
    public partial class Token
    {
        public int Id { get; set; }
        public string? AccessToken { get; set; }
        public string? TokenType { get; set; }
        public string? Scope { get; set; }
        public int? ProfileId { get; set; }

    }
}
