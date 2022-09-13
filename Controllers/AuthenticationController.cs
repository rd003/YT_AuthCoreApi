using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthCoreApi.Models;
using AuthCoreApi.Models.Domain;
using AuthCoreApi.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthCoreApi.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this._configuration = configuration;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> Register(SignupModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Status { StatusCode = 0, Message = "Please pass all the details" });
            }
            var userExist = await userManager.FindByNameAsync(model.Username);
            if (userExist != null)
            {
                return Ok(new Status { StatusCode = 0, Message = "User is already exists" });
            }
            ApplicationUser user = new ApplicationUser
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Name = model.Name
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return Ok(new Status { StatusCode = 0, Message = "Failed to create user" });
            }
            // if you want to register as admin , replace  UserRoles.User with UserRoles.Admin
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }

            if (await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await userManager.AddToRoleAsync(user, UserRoles.User);
            }
            return Ok(new Status { StatusCode = 1, Message = "successfully registered" });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var response = new LoginResponse();
            var user = await userManager.FindByNameAsync(model.UserName);
            bool passwordMatched = await userManager.CheckPasswordAsync(user, model.Password);
            if(user!=null && passwordMatched)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                };
                foreach(var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                var token = new JwtSecurityToken(
                    issuer:_configuration["JWT:Issuer"],
                    audience:_configuration["JWT:Audience"],
                    expires:DateTime.Now.AddHours(4),
                    claims:authClaims,
                    signingCredentials:new SigningCredentials(authSigningKey,SecurityAlgorithms.HmacSha256)
                    );
                response.Name = user.Name;
                response.Username = user.UserName;
                response.Token = new JwtSecurityTokenHandler().WriteToken(token);
                response.Expiration = token.ValidTo;
                response.StatusCode = 1;
                response.Message = "Successfully logged in";
                return Ok(response);
            }
            else
            {
                response.StatusCode = 0;
                response.Message = "Invalid username";
                return Ok(response);
            }
        }
    }
}
