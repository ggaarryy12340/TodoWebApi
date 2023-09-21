using Microsoft.AspNetCore.Mvc;
using Todo.Models;
using System.Linq;
using Todo.Dtos;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly TodoContext _todoContext;
        private readonly IConfiguration _configuration;

        public LoginController(TodoContext todoContext, IConfiguration configuration)
        { 
            _todoContext = todoContext;
            _configuration = configuration;
        }

        [HttpPost]
        public string Login(LoginPostDto model)
        {
            var user = _todoContext.Employees.SingleOrDefault(x => x.Account == model.Account && x.Password == model.Password);

            if (user == null)
            {
                return "帳號密碼錯誤";
            }
            else
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Account),
                    new Claim("FullName", user.Name),
                    new Claim("EmployeeId", user.EmployeeId.ToString())
                };

                // 設定權限
                var roleList = GetRoleList(user.EmployeeId);

                if (roleList != null)
                {
                    foreach (var role in roleList)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return "登入成功";
            }
        }

        [HttpPost("JwtLogin")]
        public string JwtLogin(LoginPostDto model)
        {
            var user = _todoContext.Employees.SingleOrDefault(x => x.Account == model.Account && x.Password == model.Password);

            if (user == null)
            {
                return "帳號密碼錯誤";
            }
            else
            {
                 
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Email, user.Account),
                    new Claim("FullName", user.Name),
                    new Claim(JwtRegisteredClaimNames.NameId, user.EmployeeId.ToString())
                };

                // 設定權限
                var roleList = GetRoleList(user.EmployeeId);

                if (roleList != null)
                {
                    foreach (var role in roleList)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:KEY"]));

                var jwt = new JwtSecurityToken
                    (
                        issuer: _configuration["JWT:Issuer"],
                        audience: _configuration["JWT:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                    );

                var token = new JwtSecurityTokenHandler().WriteToken(jwt);

                return token;
            }
        }

        [HttpDelete]
        public void Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet("NotLogin")]
        public string NotLogin()
        {
            return "未登入";
        }

        [HttpGet("AccessDenied")]
        public string AccessDenied()
        {
            return "沒有權限";
        }

        private List<string> GetRoleList(Guid employeeId)
        {
            var roleList = _todoContext.Roles.Where(x => x.EmployeeId == employeeId).Select(x => x.Name).ToList();

            return roleList;
        }
    }
}
