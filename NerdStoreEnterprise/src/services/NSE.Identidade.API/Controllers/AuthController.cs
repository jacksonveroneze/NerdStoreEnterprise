using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSE.Identidade.API.Extensions;
using NSE.Identidade.API.Models;
using System.Threading.Tasks;

namespace NSE.Identidade.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;

        public AuthController(SignInManager<IdentityUser> signInManager,
                      UserManager<IdentityUser> userManager,
                      IOptions<AppSettings> appSettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        [HttpPost("nova-conta")]
        public async Task<IActionResult> Registrar(UsuarioRegistro command)
        {
            if (ModelState.IsValid is false) return BadRequest();

            IdentityUser identityUser = new IdentityUser
            {
                UserName = command.Email,
                Email = command.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, command.Senha);

            if (result.Succeeded)
                return Ok();

            return BadRequest();
        }

        [HttpPost("autenticar")]
        public async Task<IActionResult> Login(UsuarioLogin command)
        {
            if (ModelState.IsValid is false) return BadRequest();

            var result = await _signInManager.PasswordSignInAsync(command.Email, command.Senha, false, true);

            if (result.Succeeded)
                return Ok();

            return BadRequest();
        }
    }
}
