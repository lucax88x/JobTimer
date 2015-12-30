using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using JobTimer.Data.Model.JobTimer;

namespace JobTimer.Data.Access.JobTimer
{
    public interface ILastVisitedAccess : IAccess<int, LastVisited>
    {
        Task<LastVisited> LoadAsync(string username);
        LastVisited Load(string username);
    }
    public class LastVisitedAccess : AccessWithInt<LastVisited>, ILastVisitedAccess
    {
        public LastVisitedAccess(JobTimerDbContext context)
            : base(context)
        {
        }

        public async Task<LastVisited> LoadAsync(string username)
        {
            return await Set.FirstOrDefaultAsync(x => x.UserName == username);
        }

        public LastVisited Load(string username)
        {
            return Set.FirstOrDefault(x => x.UserName == username);
        }
    }
}
