using System;
using System.Threading.Tasks;
using ShopApi.Models;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Authorize;
using ShopApi.Entity;
using System.Collections.Generic;
using System.Linq;
using ShopApi.Data;
using Microsoft.EntityFrameworkCore;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(new[] { Role.SuperSu })]
    public class UserActivitiesController : ControllerBase
    {
        private readonly CategoryApiContext _context;

        public UserActivitiesController(CategoryApiContext context)
        {
            _context = context;
        }

        // GET: api/UserActivities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserActivity>>> GetUserActivity()
        {
            return await _context.UserActivity.ToListAsync();
        }

        // GET: api/UserActivities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserActivity>> GetUserActivity(int id)
        {
            var userActivity = await _context.UserActivity.FindAsync(id);

            if (userActivity == null)
            {
                return NotFound();
            }

            return userActivity;
        }

        // PUT: api/UserActivities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserActivity(int id, UserActivity userActivity)
        {
            if (id != userActivity.Id)
            {
                return BadRequest();
            }

            _context.Entry(userActivity).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;

            await _context.SaveChangesAsync();

            if (!UserActivityExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/UserActivities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserActivity>> PostUserActivity(UserActivity userActivity)
        {
            _context.UserActivity.Add(userActivity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserActivity", new { id = userActivity.Id }, userActivity);
        }

        // DELETE: api/UserActivities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserActivity(int id)
        {
            var userActivity = await _context.UserActivity.FindAsync(id);
            if (userActivity == null)
            {
                return NotFound();
            }

            _context.UserActivity.Remove(userActivity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserActivityExists(int id)
        {
            return _context.UserActivity.Any(e => e.Id == id);
        }
    }
}
