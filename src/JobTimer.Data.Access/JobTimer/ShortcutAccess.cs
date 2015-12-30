using System.Data.Entity;
using System.Threading.Tasks;
using JobTimer.Data.Model.JobTimer;

namespace JobTimer.Data.Access.JobTimer
{
    public interface IShortcutAccess : IAccess<int, Shortcut>
    {
        Task<Shortcut> LoadAsync(string username);
    }
    public class ShortcutAccess : AccessWithInt< Shortcut>, IShortcutAccess
    {
        public ShortcutAccess(JobTimerDbContext context)
            : base(context)
        {
        }

        public async Task<Shortcut> LoadAsync(string username)
        {
            return await Set.FirstOrDefaultAsync(x => x.UserName == username);
        }
    }
}
