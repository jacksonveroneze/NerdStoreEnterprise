using System.Net.Mime;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Model;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSE.Identidade.API.Models.Requests;

namespace NSE.Identidade.API.Controllers
{
    [ApiController]
    [Route("api/v1/role")]
    public class RoleController : BaseController
    {
        private readonly ILogger<AuthenticateController> _logger;
        private readonly IMapper _mapper;
        private readonly RoleManager<MongoRole> _roleManager;

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
        //   roleManager:
        //     The roleManager param.
        //
        public RoleController(ILogger<AuthenticateController> logger, IMapper mapper, RoleManager<MongoRole> roleManager)
        {
            _logger = logger;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        //
        // Summary:
        //     /// Method responsible for action: Login (POST). ///
        //
        // Parameters:
        //   usuarioLogin:
        //     The usuarioLogin param.
        //
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(RoleRequest), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Create(RoleRequest roleRequest)
        {
            _logger.LogInformation("Request: [role]");

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            IdentityResult result = await _roleManager.CreateAsync(new MongoRole(roleRequest.Name));

            if (result.Succeeded)
                return Ok(result);

            foreach (IdentityError error in result.Errors)
                AdicionarErroProcessamento(error.Description);

            return CustomResponse();
        }
    }
}
