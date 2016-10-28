using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IDataService<TEntity, TPK> where TEntity : class, IEntity<TPK>
    {
        ServiceProcessingResult<List<TEntity>> GetAll(params string[] includes);

        ServiceProcessingResult<TEntity> Get(TPK id);

        ServiceProcessingResult<TEntity> Add(TEntity entity);

        ServiceProcessingResult<TEntity> Update(TEntity entity);

        ServiceProcessingResult Delete(TPK id);
    }
}
