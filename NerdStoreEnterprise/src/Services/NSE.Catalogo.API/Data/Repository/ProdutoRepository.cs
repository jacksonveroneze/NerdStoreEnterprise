using NSE.Catalogo.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using NSE.Core.Data;

namespace NSE.Catalogo.API.Data.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly IMongoCollection<Produto> _produtos;

        public ProdutoRepository(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _produtos = database.GetCollection<Produto>(settings.CollectionName);
        }

        public IUnitOfWork UnitOfWork { get; }

        public Task<List<Produto>> ObterTodos()
            => _produtos.Find(x => x.Ativo == true).ToListAsync();

        public Task<Produto> ObterPorId(Guid id)
            => _produtos.FindAsync<Produto>(book => book.Id == id).Result.FirstAsync();

        public void Adicionar(Produto produto)
            => _produtos.InsertOneAsync(produto);

        public void Atualizar(Produto produto)
            => _produtos.ReplaceOneAsync(x => x.Id == produto.Id, produto);

        public void Dispose() { }
    }
}
