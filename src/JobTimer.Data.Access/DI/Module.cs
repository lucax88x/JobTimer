using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using JobTimer.Data.Access.JobTimer;

namespace JobTimer.Data.Access.DI
{
    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new Utils.DI.Module());
            
            builder.RegisterType<JobTimerDbContext>().As<JobTimerDbContext>().InstancePerRequest();
            builder.RegisterType<ShortcutAccess>().As<IShortcutAccess>().InstancePerRequest();
            builder.RegisterType<LastVisitedAccess>().As<ILastVisitedAccess>().InstancePerRequest();
            builder.RegisterType<TimerAccess>().As<ITimerAccess>().InstancePerRequest();
            builder.RegisterType<TimerTypeAccess>().As<ITimerTypeAccess>().InstancePerRequest();
            builder.RegisterType<OvertimeAccess>().As<IOvertimeAccess>().InstancePerRequest();            
        }
    }
}
