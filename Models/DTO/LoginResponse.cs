using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthCoreApi.Models.DTO
{
    public class LoginResponse:Status
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
