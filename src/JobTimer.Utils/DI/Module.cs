using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace JobTimer.Utils.DI
{
    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TimeGetter>().As<ITimeGetter>();
            builder.RegisterType<TimeBuilder>().As<ITimeBuilder>();
        }
    }
}
