using System;
using System.Threading.Tasks;
using ShopApi.Interface;
using ShopApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Authorize;
using ShopApi.Entity;
using ShopApi.Helpers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(UserTracker))]
    public class CategoriesController : ControllerBase
    {
        private readonly ICommonRepository<Category> commonRepository;
        private readonly ICategoryRepository categoryRepository;

        public CategoriesController(ICommonRepository<Category> commonRepository,ICategoryRepository categoryRepository)
        {
            this.commonRepository = commonRepository;
            this.categoryRepository = categoryRepository;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            try
            {
                return Ok(await commonRepository.Get());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from database.");
            }
        }

        // GET: api/Categories/5
        [HttpGet("{id:int}")]
        
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
                Category category = await commonRepository.GetSpecific(id);
                if(category == null)
                {
                throw new KeyNotFoundException();
                }
                return Ok(category);
        }

        // PUT: api/Categories/5
        [HttpPut("{id:int}")]
        [Authorize(new[] { Role.SuperSu, Role.Administrator })]
        //[CustomExceptionFiler]
        public async Task<ActionResult<Category>> PutCategory(int id, Category category)
        {

                var categoryToUpdate = await commonRepository.GetSpecific(id);

                return await commonRepository.Update(category);
        }

        // POST: api/Categories
        [HttpPost]
        [Authorize(new[] { Role.SuperSu, Role.Administrator })]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            try
            {

                if (category == null)
                {
                    return BadRequest();
                }

                var cate = await categoryRepository.GetCategoryByName(category.Name);
                if (cate != null)
                {
                    ModelState.AddModelError("category", "Category already exist.");
                    return BadRequest(ModelState);
                }
                var newCategory = await commonRepository.Add(category);

                return CreatedAtAction(nameof(GetCategory),
                    new { id = newCategory.Id }, newCategory);
            }

            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new Category/ Unauthorized Access.");
            }
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id:int}")]
        [Authorize(new[] { Role.SuperSu, Role.Administrator })]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            try
            {
                Category category = await commonRepository.GetSpecific(id);
                if (category == null)
                {
                    return NotFound($"User with id:{id} not found.");
                }

                await commonRepository.Delete(id);

                return Ok($"User with id:{id} Deleted.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Deleting Category.");
            }
        }
    }
}