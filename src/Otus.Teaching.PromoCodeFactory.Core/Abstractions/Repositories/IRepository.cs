using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Otus.Teaching.PromoCodeFactory.Core.Domain;

namespace Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T>
        where T: BaseEntity
    {
        Task<T> AddAsync(T entity);
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<bool> RemoveAsync(Guid id);
        Task<T> UpdateAsync(T entity);
    }
}