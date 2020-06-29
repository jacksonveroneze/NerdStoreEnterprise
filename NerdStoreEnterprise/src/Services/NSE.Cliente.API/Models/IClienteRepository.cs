using System.Collections.Generic;
using System.Threading.Tasks;
using NSE.Core.Data;

namespace NSE.Clientes.API.Models
{
    public interface IClienteRepository : IRepository<Client>
    {
        void Adicionar(Client cliente);

        Task<IEnumerable<Client>> ObterTodos();

        Task<Client> ObterPorCpf(string cpf);
    }
}
