using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CategoryApi.Data;
using CategoryApi.Models;

namespace CategoryApi.Services
{
    public class UserSecurity
    {
        public static bool Login(string username, string password)
        {
            using (CategoryApiContext db = new CategoryApiContext())
            {
                return (db.RegisterUsers.Any(user => user.UserName.Equals(username
                    ,StringComparison.OrdinalIgnoreCase) && user.Password == password));

            }
        }
    }
}