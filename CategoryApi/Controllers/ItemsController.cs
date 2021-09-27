using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using CategoryApi.Data;
using CategoryApi.Models;
using CategoryApi.Services;
using Microsoft.AspNetCore.Hosting;

namespace CategoryApi.Controllers
{
    public class ItemsController : ApiController
    {
        private CategoryApiContext db = new CategoryApiContext();

        [BasicAuthentication]
        // GET: api/Items
        public async Task<IHttpActionResult> GetItems()
        {
            string username = Thread.CurrentPrincipal.Identity.Name;

            RegisterUser registerUser = db.RegisterUsers.FirstOrDefault(x=>x.UserName == username);
            var item = db.Items.OrderByDescending(x => x.Id);
            if(registerUser.Role == "Administrator")
            {
                return Ok(await item.ToListAsync());
            }
            return NotFound();
            
        }

        // GET: api/Items/5
        [ResponseType(typeof(Item))]
        public async Task<IHttpActionResult> GetItem(int id)
        {
            Item item= await db.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        // PUT: api/Items/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutItem(int id, Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != item.Id)
            {
                return BadRequest();
            }
            db.Entry(item).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Items
        [ResponseType(typeof(Item))]
        public async Task<IHttpActionResult> PostItem(Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Items.Add(item);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = item.Id }, item);
        }

        // DELETE: api/Items/5
        [ResponseType(typeof(Item))]
        public async Task<IHttpActionResult> DeleteItem(int id)
        {
            Item item = await db.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            db.Items.Remove(item);
            await db.SaveChangesAsync();

            return Ok(item);
        }

        //Save Image
        [Route("api/items/SaveFile")]
        public string SaveFile()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var uploadedFile = httpRequest.Files[0];
                string fileName = uploadedFile.FileName;
                var physicalPath = HttpContext.Current.Server.MapPath("~/Photos/"+fileName);

                uploadedFile.SaveAs(physicalPath);

                return (fileName);
            }
            catch(Exception)
            {
                return("noimage.png");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ItemExists(int id)
        {
            return db.Items.Count(e => e.Id == id) > 0;
        }
    }
}