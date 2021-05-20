using System;
using System.Threading.Tasks;
using TradeMap.Data.Repository;

namespace TradeMap.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        public IGenericRepository<T> Repository<T>()
          where T : class;

        int Commit();

        Task<int> CommitAsync();
    }
}