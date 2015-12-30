using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JobTimer.Data.Model.JobTimer;
using LinqKit;

namespace JobTimer.Data.Access
{
    public interface IAccess<in TY, T> : IDisposable
        where T : Entity<TY>
    {
        DbSet<T> Set { get; }
        DbEntityEntry<T> Entry(T entity);
        void Attach(T entity);
        T TryAttach(T entity);
        Task<int> CountAsync(List<Expression<Func<T, bool>>> expressions = null);
        Task<T> GetFirstAsync();
        Task<T> LoadAsync(TY id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllAsync(int start, int limit, List<Expression<Func<T, bool>>> expressions = null);
        T UpdateState(T entity, EntityState state);
        T UpdateState(T entity);
        int SaveOrUpdate(T entity);
        Task<int> SaveOrUpdateAsync(T entity);
        Task<int> SaveAsync(T entity);
        Task<int> DeleteAsync(TY id);
        Task<int> DeleteAsync(T entity);
    }

    public class AccessWithInt<T> : AccessWithInt<JobTimerDbContext, T>
        where T : Entity<int>, new()
    {
        public AccessWithInt(JobTimerDbContext context) : base(context)
        {
        }
    }
    public class AccessWithInt<C, T> : Access<C, int, T>
        where T : Entity<int>, new()
        where C : DbContext
    {
        public AccessWithInt(C context) : base(context)
        {
        }

        protected override Expression<Func<T, bool>> Is(int expected)
        {
            return (item) => item.ID == expected;
        }
        protected override bool IsNew(int toCompare)
        {
            return toCompare == 0;
        }
    }

    public class AccessWithGuid<T> : AccessWithGuid<JobTimerDbContext, T>
        where T : Entity<Guid>, new()
    {
        public AccessWithGuid(JobTimerDbContext context) : base(context)
        {
        }
    }

    public class AccessWithGuid<C, T> : Access<Guid, T>
        where T : Entity<Guid>, new()
        where C : DbContext
    {
        public AccessWithGuid(JobTimerDbContext context) : base(context)
        {
        }

        protected override Expression<Func<T, bool>> Is(Guid expected)
        {
            return (item) => item.ID == expected;
        }
        protected override bool IsNew(Guid toCompare)
        {
            return toCompare == Guid.Empty;
        }
    }

    public abstract class Access<TY, T> : Access<JobTimerDbContext, TY, T>
        where T : Entity<TY>, new()
    {
        protected Access(JobTimerDbContext context) : base(context)
        {

        }
    }

    public abstract class Access<C, TY, T> : IAccess<TY, T>
        where T : Entity<TY>, new()
        where C : DbContext
    {
        readonly C _context;
        protected Access(C context)
        {
            _context = context;
        }

        protected virtual Expression<Func<T, bool>> Is(TY expected)
        {
            return (item) => true;
        }
        protected virtual bool IsNew(TY toCompare)
        {
            return false;
        }

        private DbSet<T> _set;
        public DbSet<T> Set
        {
            get
            {
                if (_set == null)
                {
                    _set = _context.Set<T>();
                }
                return _set;
            }
        }

        private T TryAdd(T entity)
        {
            T result = null;

            IQueryable<T> query = Set.Local.AsQueryable();
            var predicate = PredicateBuilder.False<T>();
            var filter = Is(entity.ID);
            predicate = predicate.Or(filter);
            query = query.AsExpandable().Where(predicate);

            var alreadyPresentEntity = query.FirstOrDefault();
            if (alreadyPresentEntity == null)
            {
                result = Entry(entity).Entity;
            }
            else
            {
                result = alreadyPresentEntity;
            }
            return result;
        }
        public DbEntityEntry<T> Entry(T entity)
        {
            return _context.Entry<T>(entity);
        }
        public void Attach(T entity)
        {
            Set.Attach(entity);
        }
        public T TryAttach(T entity)
        {
            T result = entity;
            if (!IsNew(entity.ID))
            {
                if (Entry(entity).State == EntityState.Detached)
                {
                    IQueryable<T> query = Set.Local.AsQueryable();
                    var predicate = PredicateBuilder.False<T>();
                    var filter = Is(entity.ID);
                    predicate = predicate.Or(filter);
                    query = query.AsExpandable().Where(predicate);

                    var alreadyPresentEntity = query.FirstOrDefault();
                    if (alreadyPresentEntity == null)
                    {
                        Set.Attach(entity);
                    }
                    else
                    {
                        result = alreadyPresentEntity;
                    }
                }
            }
            else
            {
                Set.Add(entity);
            }
            return result;
        }
        public async Task<T> GetFirstAsync()
        {
            return await Set.FirstOrDefaultAsync();
        }
        public async Task<T> LoadAsync(TY id)
        {
            IQueryable<T> query = Set.AsQueryable();

            var predicate = PredicateBuilder.False<T>();
            var filter = Is(id);
            predicate = predicate.Or(filter);
            query = query.AsExpandable().Where(predicate);

            return await query.FirstOrDefaultAsync();
        }
        public async Task<List<T>> GetAllAsync()
        {
            return await Set.OrderBy(x => x.ID).ToListAsync();
        }
        public async Task<List<T>> GetAllAsync(int start, int limit, List<Expression<Func<T, bool>>> expressions = null)
        {
            IQueryable<T> query = Set.OrderBy(x => x.ID);

            DynamicQuery(expressions, ref query);

            return await query.Skip(start).Take(limit).ToListAsync();
        }
        private void DynamicQuery(List<Expression<Func<T, bool>>> expressions, ref IQueryable<T> query)
        {
            if (expressions != null && expressions.Any())
            {
                var predicate = PredicateBuilder.False<T>();

                foreach (var expr in expressions)
                {
                    predicate = predicate.Or(expr);
                }

                query = query.AsExpandable().Where(predicate);
            }
            return;
        }

        public async Task<int> CountAsync(List<Expression<Func<T, bool>>> expressions = null)
        {
            IQueryable<T> query = Set.AsQueryable();

            DynamicQuery(expressions, ref query);

            return await query.CountAsync();
        }
        public T UpdateState(T entity, EntityState state)
        {
            entity = TryAdd(entity);
            var entry = Entry(entity);
            entry.State = state;
            return entity;
        }
        public T UpdateState(T entity)
        {
            if (!IsNew(entity.ID))
            {
                entity = UpdateState(entity, EntityState.Modified);
            }
            else
            {
                Entry(entity).State = EntityState.Added;
            }
            return entity;
        }
        public int SaveOrUpdate(T entity)
        {
            var entry = Entry(entity);

            if (!IsNew(entity.ID))
            {
                entry.State = EntityState.Modified;
            }
            else
            {
                entry.State = EntityState.Added;
            }

            return _context.SaveChanges();
        }
        public async Task<int> SaveOrUpdateAsync(T entity)
        {
            var entry = Entry(entity);

            if (!IsNew(entity.ID))
            {
                entry.State = EntityState.Modified;
            }
            else
            {
                entry.State = EntityState.Added;
            }

            return await _context.SaveChangesAsync();
        }
        public async Task<int> SaveAsync(T entity)
        {
            var entry = Entry(entity);

            entry.State = EntityState.Added;

            return await _context.SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(T entity)
        {
            Set.Attach(entity);
            Set.Remove(entity);

            return await _context.SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(TY id)
        {
            T entity = new T { ID = id };
            return await DeleteAsync(entity);
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
