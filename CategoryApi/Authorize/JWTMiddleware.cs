using ShopApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ShopApi.Interface;
using ShopApi.Models;

namespace ShopApi.Authorize
{
    public class JWTMiddleware
    {
        public readonly RequestDelegate _next;
        public readonly AppSettings _appSettings;

        public JWTMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }
        public async Task Invoke(HttpContext context, IUserRepository userRepository, ICommonRepository<RegisterUser> commonRepository, IJwtUtils jWTUtils)
        {
            var token = context.Request.Cookies["jwt"];
            var userId = jWTUtils.ValidateJwtToken(token);

            if (userId != null)
            {
                var res = await commonRepository.GetSpecific(userId.Value);
                context.Items["UserId"] = res.Id;
                context.Items["Username"] = res.UserName;
                context.Items["LoginDto"] = res;
            }
             await _next(context);
 
        }
    }
}
