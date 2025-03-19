using BLL.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPut("logout/{id}")]
        public async Task<ActionResult> DeactivateUserAsync()
        {
            await _userService.LogoutAsync();
            return NoContent();
        }


    }
}
