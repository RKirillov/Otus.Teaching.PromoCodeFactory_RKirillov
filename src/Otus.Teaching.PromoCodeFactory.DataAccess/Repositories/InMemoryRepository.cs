using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories;
using Otus.Teaching.PromoCodeFactory.Core.Domain;

namespace Otus.Teaching.PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>
        : IRepository<T> where T : BaseEntity
    {
        protected List<T> Data { get; set; }

        public InMemoryRepository(List<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data.AsEnumerable<T>());
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<T> AddAsync(T entity)
        {
            if (!Data.Where(x => x.Id == entity.Id).Any())
            {
                Data.Add(entity);
                return Task.FromResult(entity);
            }
            return default;
        }


        public Task<bool> RemoveAsync(Guid id)
        {
            return Task.FromResult(Data.RemoveAll(x => x.Id == id) > 0);
        }

        public Task<T> UpdateAsync(T entity)
        {
            var index = Data.FindIndex(x => x.Id == entity.Id);
            if (index != -1)
            {
                Data[index] = entity;
            }
            return Task.FromResult(Data[index]);
        }
    }
}