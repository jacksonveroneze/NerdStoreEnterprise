using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSE.Identidade.API.Extensions;
using NSE.Identidade.API.Models;
using NSE.Identidade.API.Models.Requests;
using NSE.Identidade.API.Models.Responses;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace NSE.Identidade.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : MainController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppSettings _appSettings;

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        // GET api/user/userdata
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
                return CustomResponseCreated("api/v1/auth/user-data", await GerarJwt(usuarioRegistro.Email));

            foreach (IdentityError error in result.Errors)
                AdicionarErroProcessamento(error.Description);

            return CustomResponse();
        }

        [HttpPost("autenticar")]
        public async Task<ActionResult> Login(UsuarioLogin usuarioLogin)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            SignInResult result = await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, false, true);

            if (result.Succeeded)
                return CustomResponse(await GerarJwt(usuarioLogin.Email));

            if (result.IsLockedOut)
            {
                AdicionarErroProcessamento("Usuário temporariamente bloqueado por tentativas inválidas");

                return CustomResponse();
            }

            AdicionarErroProcessamento("Usuário ou Senha incorretos");

            return CustomResponse();
        }

        private async Task<UsuarioRespostaLogin> GerarJwt(string email)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            IList<Claim> claims = await _userManager.GetClaimsAsync(user);

            ClaimsIdentity identityClaims = await ObterClaimsUsuario(claims, user);
            string encodedToken = CodificarToken(identityClaims);

            return ObterRespostaToken(encodedToken, user, claims);
        }

        private async Task<ClaimsIdentity> ObterClaimsUsuario(ICollection<Claim> claims, ApplicationUser user)
        {
            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (var userRole in userRoles)
                claims.Add(new Claim("role", userRole));

            ClaimsIdentity identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            return identityClaims;
        }

        private string CodificarToken(ClaimsIdentity identityClaims)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            SecurityToken token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            return tokenHandler.WriteToken(token);
        }

        private UsuarioRespostaLogin ObterRespostaToken(string encodedToken, MongoUser user, IEnumerable<Claim> claims)
        {
            return new UsuarioRespostaLogin
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds,
                UsuarioToken = new UsuarioToken
                {
                    Id = user.Id.ToString(),
                    Email = user.Email,
                    Claims = claims.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value })
                }
            };
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}