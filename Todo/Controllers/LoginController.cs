using Microsoft.AspNetCore.Mvc;
using Todo.Models;
using System.Linq;
using Todo.Dtos;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly TodoContext _todoContext;

        public LoginController(TodoContext todoContext)
        { 
            _todoContext = todoContext;
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
                   // new Claim(ClaimTypes.Role, "Administrator")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return "登入成功";
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
    }
}
