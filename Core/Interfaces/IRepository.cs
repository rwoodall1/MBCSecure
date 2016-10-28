using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRepository<TEntity, TPK> where TEntity : class, IEntity<TPK>
    {
        DataAccessResult<TEntity> Get(TPK byId);

        DataAccessResult<TEntity> Upsert(TEntity entityToSave);

        DataAccessResult<List<TEntity>> GetAll();

        DataAccessResult Delete(TPK id);
    }
}
