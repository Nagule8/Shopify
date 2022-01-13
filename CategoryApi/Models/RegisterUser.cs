using ShopApi.Entity;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ShopApi.Models
{
    public class RegisterUser
    {
        public int Id { get; set; }
        [Required]
        [MinLength(5,ErrorMessage ="Username must contain atleast 5 character long.")]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        //[JsonConverter(typeof(StringEnumConverter))]
        public Role Role { get; set; }
        [Required]
        public string Password { get; set; }
    }
}