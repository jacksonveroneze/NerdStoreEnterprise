using System;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSE.Identidade.API.Models.Requests;
using NSE.Identidade.API.Models.Responses;
using NSE.Identidade.API.Services;

namespace NSE.Identidade.API.Controllers
{
    [ApiController]
    [Route("api/v1/register")]
    public class RegisterController : MainController
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWTService _jwtService;

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
        //   jwtService:
        //     The jwtService param.
        //
        public RegisterController(ILogger<RegisterController> logger, IMapper mapper,
            UserManager<ApplicationUser> userManager, JWTService jwtService)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _jwtService = jwtService;
        }

        //
        // Summary:
        //     /// Method responsible for action: Register(POST). ///
        //
        // Parameters:
        //   usuarioRegistro:
        //     The usuarioRegistro param.
        //
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UsuarioRespostaLogin), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Register(UsuarioRegistro usuarioRegistro)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            ApplicationUser user = _mapper.Map<UsuarioRegistro, ApplicationUser>(usuarioRegistro);

            IdentityResult result = await _userManager.CreateAsync(user, usuarioRegistro.Senha);

            if (result.Succeeded)
                return Created(new Uri($"{Request.Path}/{user.Id}", UriKind.Relative),
                    await _jwtService.GerarJwt(usuarioRegistro.Email));

            foreach (IdentityError error in result.Errors)
                AdicionarErroProcessamento(error.Description);

            return CustomResponse();
        }
    }
}