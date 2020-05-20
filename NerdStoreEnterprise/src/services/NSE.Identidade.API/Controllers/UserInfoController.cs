using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSE.Identidade.API.Models.Requests;
using NSE.Identidade.API.Models.Responses;

namespace NSE.Identidade.API.Controllers
{
    [ApiController]
    [Route("api/v1/user-info")]
    public class UserInfoController : MainController
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        //
        // Summary:
        //     /// Method responsible for initializing the controller. ///
        //
        // Parameters:
        //   logger:
        //     The logger param.
        //
        //   mapper:
        //     The mapper param.
        //
        //   userManager:
        //     The userManager param.
        //
        public UserInfoController(ILogger<RegisterController> logger, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(UserDataResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> UserInfo()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            UserDataResponse userInfo = _mapper.Map<ApplicationUser, UserDataResponse>(user);

            return Ok(userInfo);
        }
    }
}