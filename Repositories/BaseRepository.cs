using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Logging;
using LinqKit;
using Core;
using Core.Interfaces;
using Repositories.DBContext;
using Utilities;
using Numero3.EntityFramework.Interfaces;

namespace Repositories
{
    public abstract class BaseRepository<TEntity, TPK>
        where TEntity : class, IEntity<TPK>
    {

        private static readonly string GENERIC_GET_ALL_LOG_MESSAGE = "Base repository GetAll failed for Entity: {0}. ";
        private static readonly string GENERIC_GET_ALL_WHERE_LOG_MESSAGE = "Base repository GetAllWhere failed for Entity: {0}. ";
        private static readonly string GENERIC_GET_ALL_WHERE_ORDERED_LOG_MESSAGE = "Base repository GetAllWhereOrdererd failed for Entity: {0}. ";
        private static readonly string GENERIC_PAGED_GET_ALL_WHERE_ORDERED_LOG_MESSAGE = "Base repository PagedGetAllWhereOrdered failed for Entity: {0}. ";
        private static readonly string GENERIC_GET_LOG_MESSAGE = "Base repository Get failed for Entity: {0}. ";
        private static readonly string GENERIC_GET_WHERE_LOG_MESSAGE = "Base repository GetWhere failed for Entity: {0}. ";
        private static readonly string GENERIC_ADD_LOG_MESSAGE = "Base repository Add failed for Entity: {0}. ";
        private static readonly string GENERIC_UPDATE_LOG_MESSAGE = "Base repository Update failed for Entity: {0}. ";
        private static readonly string GENERIC_DELETE_LOG_MESSAGE = "Base repository Delete failed for Entity: {0}. ";

        protected ILog Logger;
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;

        private static readonly string MissingAmbientDbContextLocatorExceptionMessage =
            "No ambient DbContext of type DbContextWithoutProxies found. This means that this repository method has been called outside of the scope of a DbContextScope. A repository must only be accessed within the scope of a DbContextScope, which takes care of creating the DbContext instances that the repositories need and making them available as ambient contexts. This is what ensures that, for any given DbContext-derived type, the same instance is used throughout the duration of a business transaction. To fix this issue, use IDbContextScopeFactory in your top-level business logic service method to create a DbContextScope that wraps the entire business transaction that your service method implements. Then access this repository within that scope. Refer to the comments in the IDbContextScope.cs file for more details.";

        protected ApplicationDbContext DbContext
        {
            get
            {
                var dbContext = _ambientDbContextLocator.Get<ApplicationDbContext>();

                if (dbContext == null)
                    throw new InvalidOperationException(MissingAmbientDbContextLocatorExceptionMessage);

                return dbContext;
            }
        }

        protected DbSet<TEntity> MainEntity
        {
            get { return DbContext.Set<TEntity>(); }
        }

        protected BaseRepository(IAmbientDbContextLocator ambientDbContextLocator)
        {
            if (ambientDbContextLocator == null)
            {
                throw new ArgumentNullException("ambientDbContextLocator");
            }
            _ambientDbContextLocator = ambientDbContextLocator;
            Logger = LoggerFactory.GetLogger(GetType());
        }

        public virtual DataAccessResult<List<TEntity>> GetAll(params string[] includes)
        {
            var result = new DataAccessResult<List<TEntity>>();

            try
            {
                var allEntities = MainEntity.IncludeMany(includes).ToList();

                result.IsSuccessful = true;
                result.Data = allEntities;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_ALL_LOG_MESSAGE), ex);
               
            }

            return result;
        }

        public async virtual Task<DataAccessResult<List<TEntity>>> GetAllAsync(params string[] includes)
        {
            var result = new DataAccessResult<List<TEntity>>();

            try
            {
                var allEntities = await MainEntity.IncludeMany(includes).ToListAsync();

                result.IsSuccessful = true;
                result.Data = allEntities;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_ALL_LOG_MESSAGE));
            }

            return result;
        }

        public virtual DataAccessResult<List<TEntity>> GetAllWhere(Expression<Func<TEntity, bool>> predicate,
            params string[] includes)
        {
            var result = new DataAccessResult<List<TEntity>>();

            try
            {
                var builder = MainEntity.IncludeMany(includes).AsExpandable().Where(predicate);
                var allEntities = builder.ToList();

                result.IsSuccessful = true;
                result.Data = allEntities;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_ALL_WHERE_LOG_MESSAGE), ex);
            }

            return result;
        }

        public async virtual Task<DataAccessResult<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> predicate,
            params string[] includes)
        {
            var result = new DataAccessResult<List<TEntity>>();

            try
            {
                var builder = MainEntity.IncludeMany(includes).AsExpandable().Where(predicate);
                var allEntities = await builder.ToListAsync();

                result.Data = allEntities;
                result.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_ALL_WHERE_LOG_MESSAGE), ex);
            }

            return result;
        }

        public virtual DataAccessResult<List<TEntity>> GetAllWhere(IList<Expression<Func<TEntity, bool>>> predicates,
            params string[] includes)
        {
            var result = new DataAccessResult<List<TEntity>>();

            try
            {
                var builder = MainEntity.IncludeMany(includes);
                predicates.ForEach(p => builder = builder.Where(p));

                var allEntities = builder.ToList();

                result.IsSuccessful = true;
                result.Data = allEntities;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_ALL_WHERE_LOG_MESSAGE), ex);
            }

            return result;
        }

        public virtual DataAccessResult<List<TEntity>> GetAllWhereOrdered(
            Expression<Func<TEntity, bool>> predicate,
            string orderBy, bool descending,
            params string[] includes)
        {
            var result = new DataAccessResult<List<TEntity>>();

            try
            {
                var builder = MainEntity.IncludeMany(includes).AsExpandable().Where(predicate);
                builder = @descending ? builder.OrderBy("it." + orderBy + " descending") : builder.OrderBy("it." + orderBy);

                var allOrderedEntities = builder.ToList();

                result.IsSuccessful = true;
                result.Data = allOrderedEntities;

            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_ALL_WHERE_ORDERED_LOG_MESSAGE), ex);
            }
            return result;
        }


        public virtual PagedDataAccessResult<List<TEntity>> PagedGetAllWhereOrdered(
            List<Expression<Func<TEntity, bool>>> predicates,
            string orderBy, bool descending,
            int pageSize, int pageNumber,
            params string[] includes)
        {
            var result = new PagedDataAccessResult<List<TEntity>>();

            try
            {
                var builder = MainEntity.IncludeMany(includes);
                predicates.ForEach(p => builder = builder.Where(p));

                var skip = pageSize * pageNumber;
                var take = pageSize;
                var count = builder.Count();

                builder = descending
                    ? builder.OrderBy("it." + orderBy + " descending").Skip(skip).Take(take)
                    : builder.OrderBy("it." + orderBy).Skip(skip).Take(take);

                var pagedEntities = builder.ToList();

                result.IsSuccessful = true;
                result.Data = pagedEntities;
                result.HasNext = count - (skip + take) > 0;
                result.HasPrevious = skip > 0;
                result.Count = count;
                result.CurrentPage = pageNumber;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                Logger.Error(GetFormattedGenericLogMessage(GENERIC_PAGED_GET_ALL_WHERE_ORDERED_LOG_MESSAGE), ex);
            }

            return result;
        }

        public virtual DataAccessResult<TEntity> Get(TPK byId)
        {
            var result = new DataAccessResult<TEntity>();

            try
            {
                result.Data = GetEntity(byId);
                result.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_LOG_MESSAGE), ex);
            }

            return result;
        }

        public virtual DataAccessResult<TEntity> GetWhere(Expression<Func<TEntity, bool>> predicate,
            params string[] includes)
        {
            var result = new DataAccessResult<TEntity>();

            try
            {
                var subset =
                    MainEntity.IncludeMany(includes)
                        .AsExpandable()
                        .Where(predicate)
                        .SingleOrDefault();

                result.Data = subset;
                result.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_WHERE_LOG_MESSAGE), ex);
            }

            return result;
        }

        public async virtual Task<DataAccessResult<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate,
            params string[] includes)
        {
            var result = new DataAccessResult<TEntity>();

            try
            {
                var subset = await
                    MainEntity.IncludeMany(includes)
                        .AsExpandable()
                        .Where(predicate)
                        .SingleOrDefaultAsync();

                result.Data = subset;
                result.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                Logger.Error(GetFormattedGenericLogMessage(GENERIC_GET_WHERE_LOG_MESSAGE), ex);
            }

            return result;
        }

        public virtual DataAccessResult<TEntity> Add(TEntity entityToAdd,
            EntityState childrenEntityState = EntityState.Unchanged)
        {
            var result = new DataAccessResult<TEntity>();

            try
            {
                MainEntity.Add(entityToAdd);
                SetExistingChildrenStateTo(childrenEntityState, entityToAdd);

                result.IsSuccessful = true;
                result.Data = entityToAdd;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                Logger.Error(GetFormattedGenericLogMessage(GENERIC_ADD_LOG_MESSAGE), ex);
            }

            return result;
        }

        public virtual DataAccessResult<TEntity> Update(TEntity entityToUpdate,
            EntityState newChildrenState = EntityState.Unchanged)
        {
            var result = new DataAccessResult<TEntity>();

            try
            {
                //If entityToUpdate is already being tracked, then we want to use a different method for updating the entity, so check that here
                if (MainEntity.Local.Any(e => e.Id.Equals(entityToUpdate.Id)))
                {
                    var existingEntity = GetEntity(entityToUpdate.Id);
                    DbContext.Entry(existingEntity).CurrentValues.SetValues(entityToUpdate);
                }
                else
                {
                    DbContext.Entry(entityToUpdate).State = EntityState.Modified;
                }

                SetExistingChildrenStateTo(newChildrenState, entityToUpdate);

                result.IsSuccessful = true;
                result.Data = entityToUpdate;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                Logger.Error(GetFormattedGenericLogMessage(GENERIC_UPDATE_LOG_MESSAGE), ex);
            }

            return result;
        }

        public virtual DataAccessResult Delete(TPK id)
        {
            var result = new DataAccessResult();

            try
            {
                var entityToDelete = GetEntity(id);
                MainEntity.Attach(entityToDelete);
                MainEntity.Remove(entityToDelete);
                result.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                Logger.Error(GetFormattedGenericLogMessage(GENERIC_DELETE_LOG_MESSAGE), ex);
            }

            return result;
        }

        protected virtual TEntity GetEntity(TPK id)
        {
            return MainEntity.Find(id);
        }

        protected void SetExistingChildrenStateTo(EntityState newChildrenState, TEntity entityToModify)
        {
            foreach (var entity in DbContext.ChangeTracker.Entries())
            {
                try
                {
                    var id = entity.Property("Id");

                    if (entityToModify.Id != null && id != null && Equals(entityToModify.Id, id.CurrentValue))
                    {
                        continue;
                    }

                    if (id != null && id.CurrentValue != null)
                    {
                        entity.State = newChildrenState;
                    }
                }
                catch (ArgumentException)
                {
                    // Exception is thrown when Entity doesn't have an Id field, EG - IdentityUserRole
                    // However, we want to make sure the EntityState of those is Unchanged, so we'll do so here.
                    entity.State = newChildrenState;
                }
            }
        }

        protected string GetFormattedGenericLogMessage(string logMessage)
        {
            return String.Format(logMessage, typeof(TEntity));
        }
    }
}
