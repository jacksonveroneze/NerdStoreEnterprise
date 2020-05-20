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
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace NSE.Identidade.API.Controllers
{
    [ApiController]
    [Route("api/v1/authenticate")]
    public class AuthController : BaseController
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJWTService _jwtService;

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
        //   signInManager:
        //     The signInManager param.
        //
        //   userManager:
        //     The userManager param.
        //
        //   jwtService:
        //     The jwtService param.
        //
        public AuthController(ILogger<AuthController> logger, IMapper mapper, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IJWTService jwtService)
        {
            _logger = logger;
            _mapper = mapper;
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtService = jwtService;
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
        [ProducesResponseType(typeof(UsuarioRespostaLogin), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Login(LoginRequest usuarioLogin)
        {
            _logger.LogInformation("Request: [auth]");

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            SignInResult result = await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, false, true);

            if (result.Succeeded)
                return CustomResponse(await _jwtService.GerarJwt(usuarioLogin.Email));

            if (result.IsLockedOut)
            {
                AdicionarErroProcessamento("Usuário temporariamente bloqueado por tentativas inválidas");

                return CustomResponse();
            }

            AdicionarErroProcessamento("Usuário ou Senha incorretos");

            return CustomResponse();
        }
    }
}