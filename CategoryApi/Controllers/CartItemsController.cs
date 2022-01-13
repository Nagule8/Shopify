using System;
using System.Threading.Tasks;
using ShopApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ShopApi.Interface;
using ShopApi.Authorize;
using ShopApi.Entity;
using ShopApi.Helpers;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(UserTracker))]
    [Authorize(new[] { Role.User })]
    public class CartItemsController : ControllerBase
    {
        private readonly ICommonRepository<CartItem> _commonRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IJwtUtils _jwtUtils;
        public CartItemsController(ICommonRepository<CartItem> commonRepository,ICartItemRepository cartItemRepository, IJwtUtils jwtUtils)
        {
            _commonRepository = commonRepository;
            _cartItemRepository = cartItemRepository;
            _jwtUtils = jwtUtils;
        }

        // GET: api/CartItems
        [HttpGet]
        public async Task<ActionResult> GetCartItems()
        {
            var jwt = Request.Cookies["jwt"];
            var userId = _jwtUtils.ValidateJwtToken(jwt);

            return Ok(await _cartItemRepository.GetCartItems((int)userId));
        }

        // GET: api/CartItems/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CartItem>> GetCartItem(int id)
        {
            CartItem cartItem = await _commonRepository.GetSpecific(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return Ok(cartItem);
        }

        // PUT: api/CartItems/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<CartItem>> PutCartItem(int id, CartItem cartItem)
        {
            if (cartItem.Id != id)
            {
                return BadRequest("Cart Item Id mismatch!.");
            }
            var itemToUpdate = await _commonRepository.GetSpecific(id);
            if (itemToUpdate == null)
            {
                return NotFound($"Cart Item with id:{id} not found");
            }
            return await _commonRepository.Update(cartItem);
        }

        //PUT Increase Quantity
        [HttpPut("Increase/{id:int}")]
        public async Task<ActionResult<CartItem>> PUTIncreaseQuantity(int id,int quantity)
        {
            //var cartItem =await _commonRepository.GetSpecific(id);

            return await _cartItemRepository.IncreaseQuantity(id, quantity);
        }

        // POST: api/CartItems
        [HttpPost]
        public async Task<ActionResult<CartItem>> PostCartItem(CartItem cartItem)
        {
            if (cartItem == null)
            {
                ModelState.AddModelError("Cart", "Item is null.");
                return BadRequest(ModelState);
            }
            var item1 = await _cartItemRepository.GetCartItemByItemId(cartItem.ItemId);
            if (item1 != null)
            {
                ModelState.AddModelError("Cart", "Item already exist.");
                return BadRequest(ModelState);
            }
            var newitem = await _commonRepository.Add(cartItem);

            return CreatedAtAction(nameof(GetCartItem),
                new { id = newitem.Id }, newitem);
        }

        // DELETE: api/CartItems/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CartItem>> DeleteCartItem(int id)
        {
                CartItem cartItem = await _commonRepository.GetSpecific(id);
                if (cartItem == null)
                {
                    return NotFound($"Cart item with id:{id} not found.");
                }

                await _commonRepository.Delete(id);

                return Ok($"Cart Item with id:{id} Deleted.");
        }
    }
}