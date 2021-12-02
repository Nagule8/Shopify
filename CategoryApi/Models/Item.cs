using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ShopApi.Models
{
    public class Item
    {
        public int Id { get; set; }
        [Required, MinLength(2, ErrorMessage = "Minimum Length is 2")]
        public string Name { get; set; }
        public string Slug { get { return Name.ToLower().Replace(" ", "-"); } }
        [Required, MinLength(5, ErrorMessage = "Minimum Length is 2")]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }     
        public string ImageName { get; set; }
        public virtual Category Category { get; set; }

    }
}