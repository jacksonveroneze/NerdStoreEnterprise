using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.Core.Integration;
using NSE.MessageBus;

namespace NSE.Mail.API.Services
{
    public class SendMailIntegrationHandler : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageBus _bus;

        public SendMailIntegrationHandler(IServiceProvider serviceProvider, IMessageBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }

        private void SetResponder()
        {
            _bus.SubscribeAsync<UsuarioRegistradoSendMailIntegrationEvent>("my_subscription_id", async request =>
                await SendMail(request));

            _bus.AdvancedBus.Connected += OnConnect;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetResponder();

            return Task.CompletedTask;
        }

        private void OnConnect(object sender, EventArgs e)
            => SetResponder();

        private Task SendMail(UsuarioRegistradoSendMailIntegrationEvent message)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IEmailSender mailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                mailSender.SendEmailAsync("jackson@jacksonveroneze.com", "Teste", "Teste");
            }

            Console.WriteLine(message.Email);

            return Task.CompletedTask;
        }
    }
}
