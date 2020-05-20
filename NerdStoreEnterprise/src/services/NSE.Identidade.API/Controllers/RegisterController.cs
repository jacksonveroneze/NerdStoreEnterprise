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
    public class RegisterController : BaseController
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJWTService _jwtService;
        private readonly IEmailSender _emailSender;

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
        //   emailSender:
        //     The emailSender param.
        //
        public RegisterController(ILogger<RegisterController> logger, IMapper mapper, UserManager<ApplicationUser> userManager, IJWTService jwtService, IEmailSender emailSender)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _jwtService = jwtService;
            _emailSender = emailSender;
        }

        //
        // Summary:
        //     /// Method responsible for action: Register (POST). ///
        //
        // Parameters:
        //   usuarioRegistro:
        //     The usuarioRegistro param.
        //
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Register(RegisterRequest usuarioRegistro)
        {
            _logger.LogInformation("Request: [register]");

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            ApplicationUser user = _mapper.Map<RegisterRequest, ApplicationUser>(usuarioRegistro);

            IdentityResult result = await _userManager.CreateAsync(user, usuarioRegistro.Senha);

            if (result.Succeeded)
            {
                await _emailSender.SendEmailAsync("jackson@jacksonveroneze.com", "Register confirmation", "Register confirmation");

                return Created(new Uri($"{Request.Path}/{user.Id}", UriKind.Relative), await _jwtService.GerarJwt(usuarioRegistro.Email));
            }

            foreach (IdentityError error in result.Errors)
                AdicionarErroProcessamento(error.Description);

            return CustomResponse();
        }
    }
}