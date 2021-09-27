using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CategoryApi.Data;
using CategoryApi.Models;

namespace CategoryApi.Controllers
{
    public class CartItemsController : ApiController
    {
        private CategoryApiContext db = new CategoryApiContext();

        // GET: api/CartItems
        public async Task<IHttpActionResult> GetCartItems()
        {
            var cart = db.CartItems.OrderByDescending(x => x.Id);
            return Ok(await cart.ToListAsync());
        }

        // GET: api/CartItems/5
        [ResponseType(typeof(CartItem))]
        public async Task<IHttpActionResult> GetCartItem(int id)
        {
            CartItem cartItem = await db.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return Ok(cartItem);
        }

        // PUT: api/CartItems/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCartItem(int id, CartItem cartItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cartItem.Id)
            {
                return BadRequest();
            }

            db.Entry(cartItem).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartItemExists(id))
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

        // POST: api/CartItems
        [ResponseType(typeof(CartItem))]
        public async Task<IHttpActionResult> PostCartItem(CartItem cartItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CartItems.Add(cartItem);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = cartItem.Id }, cartItem);
        }

        // DELETE: api/CartItems/5
        [ResponseType(typeof(CartItem))]
        public async Task<IHttpActionResult> DeleteCartItem(int id)
        {
            CartItem cartItem = await db.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            db.CartItems.Remove(cartItem);
            await db.SaveChangesAsync();

            return Ok(cartItem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CartItemExists(int id)
        {
            return db.CartItems.Count(e => e.Id == id) > 0;
        }
    }
}