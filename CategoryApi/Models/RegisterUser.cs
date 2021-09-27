using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CategoryApi.Models
{
    public class RegisterUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
    }
}