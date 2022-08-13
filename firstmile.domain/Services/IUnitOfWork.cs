using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.domain.Services
{
    public interface IUnitOfWork
    {
        void Dispose();
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        void Save();
    }
}
