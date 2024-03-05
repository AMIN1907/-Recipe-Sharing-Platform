using System.Linq.Expressions;
using RecipeSharing.models;
using Microsoft.AspNetCore.Mvc;

namespace RecipeSharing.Repository
{
    public interface IRepository<T> where T : class
    {
        Task Create(T entity);
        public Task save();
        Task Remove(T entity);

        Task<List<TEntity>> GetAllTEntity<TEntity>(Expression<Func<TEntity, bool>> filter = null, bool tracked = true) where TEntity : class;
        

        Task<TEntity> GetSpecialEntity<TEntity>(Expression<Func<TEntity, bool>> filter = null, bool tracked = true) where TEntity : class;

        Task Remove<TEntity>(TEntity entity) where TEntity : class;
        Task updatat<TEntity>(TEntity entity) where TEntity : class;

        Task AddEntityAsync<TEntity>(TEntity entity) where TEntity : class;

    }
}