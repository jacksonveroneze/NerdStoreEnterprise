using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSE.Identidade.API.Models.Requests;
using NSE.Identidade.API.Models.Responses;
using NSE.Identidade.API.Services;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace NSE.Identidade.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : MainController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWTService _jwtService;

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, JWTService jwtService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [Authorize]
        [HttpGet("user-data")]
        public async Task<ActionResult> UserData()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            UserDataResponse userData = new UserDataResponse
            {
                Name = user.UserName,
                LastName = user.LastName,
                City = user.City,
                Email = user.Email
            };

            return Ok(userData);
        }

        [HttpPost("nova-conta")]
        public async Task<ActionResult> Registrar(UsuarioRegistro usuarioRegistro)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            ApplicationUser user = new ApplicationUser
            {
                UserName = usuarioRegistro.Email,
                Email = usuarioRegistro.Email,
                EmailConfirmed = true,
                Name = usuarioRegistro.Name,
                LastName = usuarioRegistro.LastName,
                Birthdate = usuarioRegistro.Birthdate,
                Country = usuarioRegistro.Country,
                State = usuarioRegistro.State,
                City = usuarioRegistro.City,
            };

            IdentityResult result = await _userManager.CreateAsync(user, usuarioRegistro.Senha);

            if (result.Succeeded)
            {
                var result2 = await _userManager.AddToRoleAsync(user, "Visitor2");

                return CustomResponseCreated("api/v1/auth/user-data", await _jwtService.GerarJwt(usuarioRegistro.Email));
            }

            foreach (IdentityError error in result.Errors)
                AdicionarErroProcessamento(error.Description);

            return CustomResponse();
        }

        [HttpPost("autenticar")]
        public async Task<ActionResult> Login(UsuarioLogin usuarioLogin)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            SignInResult result = await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, false, true);

            if (result.RequiresTwoFactor)
            {

            }

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