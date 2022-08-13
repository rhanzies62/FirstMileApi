using firstmile.domain;
using firstmile.domain.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.data
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private FirstMileEntities context;

        public UnitOfWork()
        {
            this.context = new FirstMileEntities();
        }

        public UnitOfWork(FirstMileEntities context)
        {
            this.context = context;

        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            //context.Configuration.LazyLoadingEnabled = false; // don't disable lazy loading for now
            return new GenericRepository<TEntity>(context);
        }



        public void Save()
        {
            context.SaveChanges();
        }

        Dictionary<string, object> GetPrimaryKeyValue(DbEntityEntry entry)
        {
            var objectStateEntry = ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            var entityValues = objectStateEntry.EntityKey.EntityKeyValues;

            return entityValues != null ? entityValues.ToDictionary(x => x.Key, x => x.Value) : new Dictionary<string, object>();
        }


        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
