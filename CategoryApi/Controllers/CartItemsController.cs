using System;
using System.Threading.Tasks;
using ShopApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ShopApi.Interface;
using ShopApi.Authorize;
using ShopApi.Entity;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(new[] { Role.SuperSu, Role.Administrator })]
    public class CartItemsController : ControllerBase
    {
        private readonly ICommonRepository<CartItem> commonRepository;
        private readonly ICartItemRepository cartItemRepository;
        private readonly IJwtUtils jwtUtils;
        public CartItemsController(ICommonRepository<CartItem> commonRepository,ICartItemRepository cartItemRepository, IJwtUtils jwtUtils)
        {
            this.commonRepository = commonRepository;
            this.cartItemRepository = cartItemRepository;
            this.jwtUtils = jwtUtils;
        }

        // GET: api/CartItems
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetCartItems()
        {
            try
            {
                var jwt = Request.Cookies["jwt"];
                var userId = jwtUtils.ValidateJwtToken(jwt);

                return Ok(await cartItemRepository.GetCartItems((int)userId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from database.");
            }
        }

        // GET: api/CartItems/5
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<CartItem>> GetCartItem(int id)
        {
            try
            {
                CartItem cartItem = await commonRepository.GetSpecific(id);
                if (cartItem == null)
                {
                    return NotFound();
                }

                return Ok(cartItem);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from database.");
            }
        }

        // PUT: api/CartItems/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<CartItem>> PutCartItem(int id, CartItem cartItem)
        {
            try
            {
                if (cartItem.Id != id)
                {
                    return BadRequest("Cart Item Id mismatch!.");
                }
                var itemToUpdate = await commonRepository.GetSpecific(id);
                if (itemToUpdate == null)
                {
                    return NotFound($"Cart Item with id:{id} not found");
                }
                return await commonRepository.Update(cartItem);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Updating Cart item.");
            }
        }

        //PUT Increase Quantity
        [HttpPut("Increase/{id:int}")]
        public async Task<ActionResult<CartItem>> PUTIncreaseQuantity(int id,int quantity)
        {
            var cartItem =await commonRepository.GetSpecific(id);

            try
            {
                return await cartItemRepository.IncreaseQuantity(id, quantity);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Increasing Quantity."); ;
            }
        }

        // POST: api/CartItems
        [HttpPost]
        public async Task<ActionResult<CartItem>> PostCartItem(CartItem cartItem)
        {
            try
            {
                if (cartItem == null)
                {
                    return BadRequest();
                }
                var item1 = await cartItemRepository.GetCartItemByItemId(cartItem.ItemId);
                if (item1 != null)
                {
                    ModelState.AddModelError("Cart", "Item already exist.");
                    return BadRequest(ModelState);
                }
                var newitem = await commonRepository.Add(cartItem);


                return CreatedAtAction(nameof(GetCartItem),
                    new { id = newitem.Id }, newitem);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Adding new item to cart.");
            }
        }

        // DELETE: api/CartItems/5
        [HttpDelete]
        public async Task<ActionResult<Item>> DeleteCartItem(int id)
        {
            try
            {
                CartItem cartItem = await commonRepository.GetSpecific(id);
                if (cartItem == null)
                {
                    return NotFound($"Cart item with id:{id} not found.");
                }

                await commonRepository.Delete(id);

                return Ok($"Cart Item with id:{id} Deleted.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Deleting Cart Item.");
            }
        }
    }
}