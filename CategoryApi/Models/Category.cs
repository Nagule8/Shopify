using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ShopApi.Models
{
    public class Category
    {        
        public int Id { get; set; }
        [Required, MinLength(2, ErrorMessage = "Minimum length is 2")]
        [RegularExpression(@"^[a-zA-Z- ]+$", ErrorMessage = "Only Letters are allowed")]
        public string Name { get; set; }
        public string Slug { get { return Name.ToLower().Replace(" ", "-"); } }
        public int Sorting { get; set; }
    }
}