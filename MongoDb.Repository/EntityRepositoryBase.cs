using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Repository
{
    public abstract class EntityRepositoryBase<T> : IEntityRepository<T> where T : IEntity
    {
        private MongoServer m_Server;
        private MongoDatabase m_Index;

        private MongoCollection<T> m_Entities;
        public MongoCollection<T> Entities
        {
            get
            {
                return m_Entities;
            }
        }

        private string m_Collection;

        //public EntityRepositoryBase(string collection) : this(collection, null, null)
        //{

        //}

        public EntityRepositoryBase(string collection, string connectionString, string database)
        {
            if (String.IsNullOrEmpty(collection))
            {
                throw new ArgumentNullException("collection");
            }

            if (String.IsNullOrEmpty(database))
            {
                throw new ArgumentNullException("database");
            }

            m_Collection = collection;

            if (String.IsNullOrEmpty(connectionString))
            {
                connectionString = "mongodb://localhost:27017";
            }

            m_Server = new MongoClient(connectionString).GetServer();
            m_Index = m_Server.GetDatabase(database);
            m_Entities = m_Index.GetCollection<T>(m_Collection);
        }

        public long Count()
        {
            return m_Entities.Count();
        }

        public IEnumerable<T> All()
        {
            return this.m_Entities.AsQueryable<T>().ToList();
        }

        public IQueryable<T> All(int page, int pageSize)
        {
            return this.m_Entities.AsQueryable<T>().Skip((page - 1) * pageSize).Take(pageSize);
        }

        public IEnumerable<D> AllAs<D>()
        {
            return m_Entities.AsQueryable<T>().OfType<D>().ToList();
        }

        public T Get(string id)
        {
            //IMongoQuery query = Query.EQ("_id", ObjectId.Parse(id));
            IMongoQuery query = Query.EQ("_id", id);

            return this.m_Entities.Find(query).FirstOrDefault();
        }

        public IQueryable<T> GetFunc(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return this.m_Entities.AsQueryable<T>().Where(expression);
        }

        public IQueryable<S> GetFunc<S>(System.Linq.Expressions.Expression<Func<S, bool>> expression) where S : T
        {
            return this.m_Entities.AsQueryable<S>().Where(expression);
        }

        public Tuple<IQueryable<S>, int> GetFunc<S>(System.Linq.Expressions.Expression<Func<S, bool>> expression, int page, int pageSize) where S : T
        {
            var entities = this.m_Entities.AsQueryable<S>().Where(expression);
            int pages = entities.Count();

            return new Tuple<IQueryable<S>, int>(entities.Skip((page - 1) * pageSize).Take(pageSize), pages);
        }

        public IQueryable<T> GetAs<D>(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return m_Entities.FindAllAs<D>().Cast<T>().ToList().AsQueryable().Where(expression);
        }

        public virtual T Add(T entity)
        {
            try
            {
                IEntity oEntity = (entity as IEntity);

                oEntity.Id = String.IsNullOrEmpty(oEntity.Id) ?
                    GetRepositoryPrefix() + ObjectId.GenerateNewId().ToString() :
                    oEntity.Id;

                var timeStamp = DateTime.Now;

                oEntity.Created = timeStamp;
                oEntity.LastModified = timeStamp;

                m_Entities.Insert(entity);

                return entity;
            }
            catch (Exception mongoException)
            {
                if (mongoException.HResult == -2146233088)
                {
                    throw new MongoEntityUniqueIndexException("Unique Index violation", mongoException);
                }
                else
                {
                    throw mongoException;
                }
            }

            return default(T);
        }

        public virtual int Add(IEnumerable<T> entities)
        {
            int addCount = 0;

            entities.ToList().ForEach(entity =>
            {
                if (Add(entity) != null)
                {
                    addCount++;
                }
            });

            return addCount;
        }

        public virtual void AddBatch(IEnumerable<T> entities)
        {
            int addCount = 0;

            entities.ToList().ForEach(entity =>
            {
                IEntity oEntity = (entity as IEntity);

                //oEntity.Id = GetRepositoryPrefix() + ObjectId.GenerateNewId().ToString();
                oEntity.Id = String.IsNullOrEmpty(oEntity.Id) ?
                    GetRepositoryPrefix() + ObjectId.GenerateNewId().ToString() :
                    oEntity.Id;

                var timeStamp = DateTime.Now;

                oEntity.Created = timeStamp;
                oEntity.LastModified = timeStamp;
            });

            try
            {
                m_Entities.InsertBatch(entities);
            }
            catch (Exception addBatchException)
            {

            }
        }

        private string GetRepositoryPrefix()
        {
            Type type = this.GetType();
            RepositoryAttribute attribute = type.GetCustomAttributes(typeof(RepositoryAttribute), true).Cast<Attribute>().SingleOrDefault() as RepositoryAttribute;

            if (attribute != null)
            {
                return attribute.Prefix;
            }

            return String.Empty;
        }

        public virtual void Remove(T entity)
        {
            Remove(entity.Id);
        }

        public virtual bool Remove(string id)
        {
            try
            {
                //IMongoQuery query = Query.EQ("_id", ObjectId.Parse(id));
                IMongoQuery query = Query.EQ("_id", id);

                var result = m_Entities.Remove(query);

                return result.DocumentsAffected == 1;
            }
            catch (Exception mongoException)
            {
            }

            return false;
        }

        public virtual bool RemoveAll()
        {
            try
            {
                var result = m_Entities.RemoveAll();
                return result.DocumentsAffected == 1;
            }
            catch (Exception mongoException)
            {

            }

            return false;
        }

        public virtual int Remove(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            int removeCount = 0;
            List<T> entitiesToRemove = this.m_Entities.AsQueryable<T>().Where(expression).ToList();

            entitiesToRemove.ForEach(entity =>
            {
                if (Remove((entity as IEntity).Id))
                {
                    removeCount++;
                }
            });

            return removeCount;
        }

        //public virtual bool Update(string id, T updatedEntity)
        //{
        //    return Update(id, updatedEntity, false);
        //}

        //public virtual bool Update(string id, T updatedEntity, bool systemUpdate)
        //{
        //    try
        //    {
        //        if (!systemUpdate)
        //        {
        //            updatedEntity.LastModified = DateTime.Now;
        //        }
                
        //        m_Entities.Save(updatedEntity);
        //        return true;
        //    }
        //    catch (Exception mongoException)
        //    {
        //        throw mongoException;
        //    }

        //    return false;
        //}

        public virtual T Update(T updatedEntity)
        {
            return Update(updatedEntity, false);
        }


        public virtual T Update(T updatedEntity, bool systemUpdate)
        {
            try
            {
                if (!systemUpdate)
                {
                    updatedEntity.LastModified = DateTime.Now;
                }

                m_Entities.Save(updatedEntity);

                return updatedEntity;
                //return true;
            }
            catch (Exception mongoException)
            {
                throw mongoException;
            }

            //return false;
        }

        public T Upsert(T entity, bool systemUpdate)
        {
            bool exists = false;

            if (!String.IsNullOrEmpty(entity.Id))
            {
                var existing = this.Get(entity.Id);

                exists = (existing != null);
            }

            if (exists)
            {
                return Update(entity, systemUpdate);
            }
            else
            {
                return Add(entity);
            }

        }

        public T Upsert(T entity)
        {
            return Upsert(entity, false);
        }
    }
}
