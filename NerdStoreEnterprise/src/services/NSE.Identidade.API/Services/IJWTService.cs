using System.Threading.Tasks;
using NSE.Identidade.API.Models.Responses;

namespace NSE.Identidade.API.Services
{
    public interface IJWTService
    {
        Task<UsuarioRespostaLogin> GerarJwt(string email);
    }
}