using System.Data.Entity;
using NUnit.Framework;

namespace JobTimer.Data.Access.Test
{
    [SetUpFixture]
    public class AccessTestSetup
    {
        [SetUp]
        public void Setup()
        {
            Database.SetInitializer(new NullDatabaseInitializer<JobTimerDbContext>());
        }
    }    
}

