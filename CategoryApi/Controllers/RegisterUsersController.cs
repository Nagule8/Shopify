using System;
using System.Threading.Tasks;
using ShopApi.Dtos;
using ShopApi.Interface;
using ShopApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Authorize;
using ShopApi.Entity;

namespace ShopApi.Controllers
{
    //[Authorize(Role.SuperSu)]
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterUsersController : ControllerBase
    {
        private readonly ICommonRepository<RegisterUser> commonRepository;
        private readonly IUserRepository userRepository;
        private readonly IJwtUtils jwtUtils;

        public RegisterUsersController(ICommonRepository<RegisterUser> commonRepository, IUserRepository userRepository, IJwtUtils jwtUtils)
        {
            this.commonRepository = commonRepository;
            this.userRepository = userRepository;
            this.jwtUtils = jwtUtils;
        }

        // GET: api/RegisterUsers
        [HttpGet]
        public async Task<ActionResult> GetRegisterUsers()
        {
            try
            {
                return Ok(await commonRepository.Get());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,"Error retrieving data from database.");
            }
        }

        // GET: api/RegisterUsers/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RegisterUser>> GetRegisterUser(int id)
        {
            try
            {
                RegisterUser registerUser = await commonRepository.GetSpecific(id);
                if (registerUser == null)
                {
                    return NotFound();
                }

                return Ok(registerUser);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from database.");
            }
        }

        // PUT: api/RegisterUsers/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<RegisterUser>> PutRegisterUser(int id, RegisterUser registerUser)
        {
            try
            {
                if (registerUser.Id != id)
                {
                    return BadRequest("User Id mismatch!.");
                }
                var userToUpdate = await commonRepository.GetSpecific(id);
                if (userToUpdate == null)
                {
                    return NotFound($"User with id:{id} not found");
                }
                return await commonRepository.Update(registerUser);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Updating User.");
            }

        }

        // POST: api/RegisterUsers
        [HttpPost]
        public async Task<ActionResult<RegisterUser>> PostRegisterUser(RegisterUser registerUser)
        {
            try
            {
                if(registerUser == null)
                {
                    return BadRequest();
                }
                var user = await userRepository.GetUserByName(registerUser.UserName);
                if(user != null)
                {
                    ModelState.AddModelError("Email","User already exist.");
                    return BadRequest(ModelState);
                }
                var newUser = await commonRepository.Add(registerUser);


                return CreatedAtAction(nameof(GetRegisterUser),
                    new { id = newUser.Id },newUser);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new User.");
            }

        }

        // DELETE: api/RegisterUsers/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteRegisterUser(int id)
        {
            try
            {
                RegisterUser registerUser = await commonRepository.GetSpecific(id);
                if (registerUser == null)
                {
                    return NotFound($"User with id:{id} not found.");
                }

                await commonRepository.Delete(id);

                return Ok($"User with id:{id} Deleted.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Deleting User.");
            }

        }

        //User Id
        [HttpGet("{username}")]
        //[Route("api/registeruser/GetUserId")]
        public async Task<ActionResult<int>> GetUserId(string username)
        {
            try
            {
                var res = await userRepository.GetUserId(username);
                if (res == 0)
                {
                    return NotFound();
                }

                return Ok(res);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from database.");
            }
        }

        //POST: Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var user = await userRepository.GetUserByName(login.Name);

            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            {
                return BadRequest("Invalid Credentials");
            }

            var jwt = jwtUtils.GenerateJwtToken(user); 

            Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true
            });

            return Ok(new { message = "Success." });
        }

        //GET: Authenticated User
        [HttpGet("User")]
        public async Task<IActionResult> User()
        {
            try
            {
                var jwt = Request.Cookies["jwt"];
                var userId = jwtUtils.ValidateJwtToken(jwt);

                var user = await commonRepository.GetSpecific((int)userId);

                return Ok(user);
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }

        //POST: Logout User
        [HttpPost("LogOut")]
        public IActionResult LogOut()
        {
            Response.Cookies.Delete("jwt", new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true
            });

            return Ok(new {
                message = "Successfully logged out!."
            });
        }
    }
}