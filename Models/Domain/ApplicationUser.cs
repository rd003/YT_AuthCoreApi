using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthCoreApi.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        // If you need other fields you can add here
    }
}
