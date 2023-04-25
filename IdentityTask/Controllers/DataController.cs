using autheticationpart.Data.models;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace autheticationpart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly UserManager<Employee> _userManager;

        public DataController(UserManager<Employee> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetUserInfo()
        {
            var user = await _userManager.GetUserAsync(User);

            return Ok( new
            {
                UserName = user?.UserName,
                Email  = user?.Email,
                Department = user?.Department     
            });
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [Route("Manger")]
        public ActionResult GetInfoForManager() 
        {
            return Ok(new string[] {"Value For Manager"});
        }

        [HttpGet]
        [Authorize(Policy = "AdminAndUser")]
        [Route("User")]
        public ActionResult GetInfoForUser()
        {
            return Ok(new string[] { "Value For user" });

        }
    }
}
