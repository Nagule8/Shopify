using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopApi.Models;

namespace ShopApi.Interface
{
    public interface ICategoryRepository
    {
        Task<Category> GetCategoryByName(string name);
    }
}
