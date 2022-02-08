using System;
using System.Threading.Tasks;
using ShopApi.Dtos;
using ShopApi.Interface;
using ShopApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Authorize;
using ShopApi.Entity;
using ShopApi.Helpers;

namespace ShopApi.Controllers
{
    //[Authorize(Role.SuperSu)]
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(UserTracker))]
    public class RegisterUsersController : ControllerBase
    {
        private readonly ICommonRepository<RegisterUser> _commonRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJwtUtils _jwtUtils;

        public RegisterUsersController(ICommonRepository<RegisterUser> commonRepository, IUserRepository userRepository, IJwtUtils jwtUtils)
        {
            _commonRepository = commonRepository;
            _userRepository = userRepository;
            _jwtUtils = jwtUtils;
        }

        // GET: api/RegisterUsers
        [HttpGet]
        [Authorize(new[] { Role.SuperSu })]
        public async Task<ActionResult> GetRegisterUsers()
        {
            return Ok(await _commonRepository.Get());
        }

        // GET: api/RegisterUsers/5
        [HttpGet("{id:int}")]
        [Authorize(new[] { Role.SuperSu, Role.Administrator, Role.User })]
        public async Task<ActionResult<RegisterUser>> GetRegisterUser(int id)
        {
            RegisterUser registerUser = await _commonRepository.GetSpecific(id);
            if (registerUser == null)
            {
                return NotFound();
            }

            return Ok(registerUser);
        }

        // PUT: api/RegisterUsers/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<RegisterUser>> PutRegisterUser(int id, RegisterUser registerUser)
        {
            if (registerUser.Id != id)
            {
                return BadRequest("User Id mismatch!.");
            }
            var userToUpdate = await _commonRepository.GetSpecific(id);
            if (userToUpdate == null)
            {
                return NotFound($"User with id:{id} not found");
            }
            return await _commonRepository.Update(registerUser);

        }

        // POST: api/RegisterUsers
        [HttpPost]
        public async Task<ActionResult<RegisterUser>> PostRegisterUser(RegisterUser registerUser)
        {
            if(registerUser == null)
            {
                return BadRequest();
            }
            var user = await _userRepository.GetUserByName(registerUser.UserName);
            if(user != null)
            {
                ModelState.AddModelError("Email","User already exist.");
                return BadRequest(ModelState);
            }
            var newUser = await _commonRepository.Add(registerUser);


            return CreatedAtAction(nameof(GetRegisterUser),
                new { id = newUser.Id },newUser);

        }

        // DELETE: api/RegisterUsers/5
        [HttpDelete("{id:int}")]
        [Authorize(new[] { Role.SuperSu })]
        public async Task<ActionResult> DeleteRegisterUser(int id)
        {
            RegisterUser registerUser = await _commonRepository.GetSpecific(id);
            if (registerUser == null)
            {
                return NotFound($"User with id:{id} not found.");
            }

            await _commonRepository.Delete(id);

            return Ok($"User with id:{id} Deleted.");

        }

        //User Id
        [HttpGet("{username}")]
        //[Route("api/registeruser/GetUserId")]
        public async Task<ActionResult<int>> GetUserId(string username)
        {
            var res = await _userRepository.GetUserId(username);
            if (res == 0)
            {
                return NotFound();
            }

            return Ok(res);
        }

        //POST: Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var user = await _userRepository.GetUserByName(login.Name);

            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            {
                return BadRequest("Invalid Credentials");
            }

            var jwt = _jwtUtils.GenerateJwtToken(user); 

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
            var jwt = Request.Cookies["jwt"];
            var userId = _jwtUtils.ValidateJwtToken(jwt);

            var user = await _commonRepository.GetSpecific((int)userId);

            return Ok(user);
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