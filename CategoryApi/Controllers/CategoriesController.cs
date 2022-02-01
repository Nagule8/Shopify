using System;
using System.Threading.Tasks;
using ShopApi.Interface;
using ShopApi.Models;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Authorize;
using ShopApi.Entity;
using ShopApi.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(UserTracker))]
    public class CategoriesController : ControllerBase
    {
        private readonly ICommonRepository<Category> _commonRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICommonRepository<Category> commonRepository,ICategoryRepository categoryRepository)
        {
            _commonRepository = commonRepository;
            _categoryRepository = categoryRepository;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            return Ok(await _commonRepository.Get());
        }

        // GET: api/Categories/5
        [HttpGet("{id:int}")]
        
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            Category category = await _commonRepository.GetSpecific(id);
            if(category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // PUT: api/Categories/5
        [HttpPut("{id:int}")]
        [Authorize(new[] { Role.SuperSu, Role.Administrator })]
        //[CustomExceptionFiler]
        public async Task<ActionResult<Category>> PutCategory(int id, Category category)
        {

            //var categoryToUpdate = await _commonRepository.GetSpecific(id);
            var result = await _commonRepository.Update(category);

            return result;
        }

        // POST: api/Categories
        [HttpPost]
        [Authorize(new[] { Role.SuperSu, Role.Administrator })]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            if (category == null)
            {
                return BadRequest();
            }

            var cate = await _categoryRepository.GetCategoryByName(category.Name);
            if (cate != null)
            {
                ModelState.AddModelError("category", "Category already exist.");
                return BadRequest(ModelState);
            }
            var newCategory = await _commonRepository.Add(category);

            return CreatedAtAction(nameof(GetCategory),
                new { id = newCategory.Id }, newCategory);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id:int}")]
        [Authorize(new[] { Role.SuperSu, Role.Administrator })]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
                Category category = await _commonRepository.GetSpecific(id);
                if (category == null)
                {
                    return NotFound($"User with id:{id} not found.");
                }

                await _commonRepository.Delete(id);

                return Ok($"Category with id:{id} Deleted.");
        }

        //TDD purpose
        //get matching product name
        [HttpGet("{nameToMatch}")]
        public async Task<IEnumerable<Category>> GetCategoryMatchingName(string nameToMatch = null)
        {
            var categories = (await _commonRepository.Get());

            if (!string.IsNullOrWhiteSpace(nameToMatch))
            {
                categories = categories.Where(category => category.Name.Contains(nameToMatch, StringComparison.OrdinalIgnoreCase));
            }

            return categories;
        }
    }
}