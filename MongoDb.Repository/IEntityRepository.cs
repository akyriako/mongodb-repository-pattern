
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Repository
{
    public interface IEntityRepository<T> where T : IEntity
    {
        IEnumerable<T> All();
        IQueryable<T> All(int page, int pageSize);
        T Get(string id);
        IQueryable<T> GetFunc(Expression<Func<T, bool>> expression);
        T Add(T entity);
        int Add(IEnumerable<T> entities);
        void Remove(T entity);
        bool Remove(string id);
        bool RemoveAll();
        int Remove(Expression<Func<T, bool>> expression);
        T Update(T updatedEntity);
        T Upsert(T entity);
    }
}
