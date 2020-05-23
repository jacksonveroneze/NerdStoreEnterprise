using System.Threading.Tasks;

namespace NSE.Identidade.API.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}