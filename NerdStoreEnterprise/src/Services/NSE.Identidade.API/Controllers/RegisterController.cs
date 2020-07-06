using System;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSE.Core.Integration;
using NSE.Identidade.API.Models;
using NSE.Identidade.API.Models.Requests;
using NSE.Identidade.API.Models.Responses;
using NSE.Identidade.API.Services;
using NSE.MessageBus;
using NSE.WebAPI.Core.Controllers;
using Polly;
using Polly.Retry;

namespace NSE.Identidade.API.Controllers
{
    [ApiController]
    [Route("api/v1/register")]
    public class RegisterController : MainController
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJWTService _jwtService;
        private readonly IMessageBus _bus;

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
        //   bus:
        //     The bus param.
        //
        public RegisterController(ILogger<RegisterController> logger, IMapper mapper,
            UserManager<ApplicationUser> userManager, IJWTService jwtService, IMessageBus bus)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _jwtService = jwtService;
            _bus = bus;
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
        public async Task<ActionResult> Register(RegisterRequest registerRequest)
        {
            _logger.LogInformation("Request: [register]");

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            ApplicationUser user = _mapper.Map<RegisterRequest, ApplicationUser>(registerRequest);

            IdentityResult result = await _userManager.CreateAsync(user, registerRequest.Senha);

            if (result.Succeeded)
            {
                ResponseMessage clienteResult = await RegisterClient(registerRequest);

                if (clienteResult.ValidationResult.IsValid is false)
                {
                    await _userManager.DeleteAsync(user);

                    return CustomResponse(clienteResult.ValidationResult);
                }

                return Created(new Uri($"{Request.Path}/{user.Id}", UriKind.Relative),
                    await _jwtService.GerarJwt(registerRequest.Email));
            }

            foreach (IdentityError error in result.Errors)
                AdicionarErroProcessamento(error.Description);

            return CustomResponse();
        }

        private async Task<ResponseMessage> RegisterClient(RegisterRequest registerRequest)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(registerRequest.Email);

            UsuarioRegistradoIntegrationEvent registeredUser = new UsuarioRegistradoIntegrationEvent(Guid.NewGuid(),
                registerRequest.Name, registerRequest.Email, registerRequest.Cpf);

            UsuarioRegistradoSendMailIntegrationEvent registeredUserSendMail =
                new UsuarioRegistradoSendMailIntegrationEvent(Guid.NewGuid(), registerRequest.Name,
                    registerRequest.Email, registerRequest.Cpf);

            try
            {
                RetryPolicy policy = Policy.Handle<Exception>()
                    .WaitAndRetry(15, retryAttempt =>
                            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (outcome, timespan, retryCount, context) =>
                        {
                            Console.WriteLine($"Tentando pela {retryCount} vez!");
                        });

                var result = await policy.Execute(async () =>
                    await _bus.RequestAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(registeredUser));

                if (result.ValidationResult.IsValid)
                    await policy.Execute(async () =>
                        await _bus.PublishAsync(registeredUserSendMail));

                return result;
            }
            catch (Exception e)
            {
                await _userManager.DeleteAsync(user);

                throw;
            }
        }
    }
}
