using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobTimer.WebApplication.TypeScript;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi.Chart.ViewModels
{
    [TsClass(Module = Modules.ViewModels.Chart)]
    public class ChartSerie<T>
    {
        public List<T> data { get; set; }

        public ChartSerie()
        {
            data = new List<T>();
        }
    }
    [TsClass(Module = Modules.ViewModels.Chart)]
    public class ChartData<T>
    {
        public string name { get; set; }
        public List<ChartSerie<T>> series { get; set; }

        public ChartData()
        {
            series = new List<ChartSerie<T>>();
        }
    }
    [TsClass(Module = Modules.ViewModels.Chart)]
    public class ChartViewModel<T> : BaseViewModel
    {
        public ChartData<T> Data { get; set; }
        
        public ChartViewModel()
        {
            Data = new ChartData<T>();
        }
    }
}
