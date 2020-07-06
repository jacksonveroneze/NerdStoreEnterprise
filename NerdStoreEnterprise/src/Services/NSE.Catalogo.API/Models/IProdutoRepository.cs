using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSE.Core.Data;

namespace NSE.Catalogo.API.Models
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        Task<List<Produto>> ObterTodos();

        Task<Produto> ObterPorId(Guid id);

        void Adicionar(Produto produto);

        void Atualizar(Produto produto);
    }
}
