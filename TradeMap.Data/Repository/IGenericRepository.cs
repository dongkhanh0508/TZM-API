﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TradeMap.Data.Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        //async
        Task InsertAsync(TEntity entity);

        Task InsertRangeAsync(IQueryable<TEntity> entities);

        DbSet<TEntity> GetAll();

        IQueryable<TEntity> FindAll(Func<TEntity, bool> predicate);

        TEntity Find(Func<TEntity, bool> predicate);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetById(int Id);

        Task<TEntity> GetByIdGuid(Guid Id);

        void Insert(TEntity entity);

        Task Update(TEntity entity, int Id);


        Task UpdateGuid(TEntity entity, Guid Id);

        void UpdateRange(IQueryable<TEntity> entities);

        Task HardDelete(int key);

        Task HardDeleteGuid(Guid key);

        void DeleteRange(IQueryable<TEntity> entities);

        void InsertRange(IQueryable<TEntity> entities);

        public EntityEntry<TEntity> Delete(TEntity entity);
    }
}