using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using JobTimer.Data.Model.Identity;

namespace JobTimer.Data.Access.Identity
{
    public interface IUserAccess
    {
        Task<int> CountAsync();
        Task<List<ApplicationUser>> GetAllAsync(int start, int limit);
        Task<int> DeleteAsync(string id);
        Task<int> DeleteAsync(ApplicationUser entity);
    }
    public class UserAccess : IUserAccess
    {
        private DbSet<ApplicationUser> _set;
        public DbSet<ApplicationUser> Set
        {
            get
            {
                if (_set == null)
                {
                    _set = _context.Set<ApplicationUser>();
                }
                return _set;
            }
        }

        private readonly JIdentityDbContext _context;

        public UserAccess(JIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountAsync()
        {
            return await Set.CountAsync();
        }
        public async Task<List<ApplicationUser>> GetAllAsync(int start, int limit)
        {
            IQueryable<ApplicationUser> query = Set.OrderBy(x => x.Id);

            return await query.Skip(start).Take(limit).ToListAsync();
        }
        public async Task<int> DeleteAsync(string id)
        {
            var entity = new ApplicationUser { Id = id };
            return await DeleteAsync(entity);
        }
        public async Task<int> DeleteAsync(ApplicationUser entity)
        {
            Set.Attach(entity);
            Set.Remove(entity);

            return await _context.SaveChangesAsync();
        }
    }
}
