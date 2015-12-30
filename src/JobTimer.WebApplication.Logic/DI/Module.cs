using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace JobTimer.WebApplication.Logic.DI
{
    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new Data.Access.DI.Module());
            builder.RegisterType<TimerCalculator>().As<ITimerCalculator>();
        }
    }
}
