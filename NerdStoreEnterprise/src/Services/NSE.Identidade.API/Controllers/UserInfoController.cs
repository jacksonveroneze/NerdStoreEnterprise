using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSE.Identidade.API.Models;
using NSE.Identidade.API.Models.Responses;
using NSE.WebAPI.Core.Controllers;

namespace NSE.Identidade.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/userinfo")]
    public class UserInfoController : MainController
    {
        private readonly ILogger<UserInfoController> _logger;
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
        public UserInfoController(ILogger<UserInfoController> logger, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        //
        // Summary:
        //     /// Method responsible for action: UserInfo (GET). ///
        //
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> UserInfo()
        {
            _logger.LogInformation("Request: [user-info]");

            ApplicationUser user = await _userManager.GetUserAsync(User);

            UserInfoResponse userInfo = _mapper.Map<ApplicationUser, UserInfoResponse>(user);

            return Ok(userInfo);
        }
    }
}
