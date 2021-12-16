
using Microsoft.AspNetCore.Mvc.Filters;
using ShopApi.Data;
using ShopApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace ShopApi.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UserTracker : ActionFilterAttribute
    {
        private readonly CategoryApiContext _context;

        public UserTracker(CategoryApiContext context)
        {
            _context = context;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            var request = filterContext.HttpContext.Request;

            //METHOD TYPE
            var Method = request.Method;
            var ContentType = request.ContentType;
            
            //Cuurent user,if no user logged in then Anonymous user.
            var CurrentUser = filterContext.HttpContext.Items["Username"] ?? "Anonymous";
            var UserId = filterContext.HttpContext.Items["UserId"] ?? 0;
            // The IP Address of the Request
            var IpAddress = filterContext.HttpContext.Connection.RemoteIpAddress.ToString();
            // The URL that was accessed
            var AreaAccessed = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(request);
            // Creates Timestamp
            var TimeStamp = DateTime.UtcNow;

            //Input Reader
            //string BodyData;
            //using (var reader = new StreamReader(request.Body))
            //{
            //    BodyData = reader.ReadToEnd();
            //}

            var res = JsonSerializer.Serialize(new {
                ContentType, IpAddress, AreaAccessed, TimeStamp
            });

            UserActivity userActivity = new UserActivity();

            userActivity.UserId = (int)UserId;
            userActivity.Username = (string)CurrentUser;
            userActivity.Method = Method;
            userActivity.Description = res;

            _context.UserActivity.Add(userActivity);
            _context.SaveChanges();

            base.OnActionExecuting(filterContext);
        }
    }
}
