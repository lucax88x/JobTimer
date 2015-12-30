using JobTimer.WebApplication.TypeScript;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi.Common
{
    [TsClass(Module = Modules.Models.Name)]
    public class BaseJson : ReadJson<int>
    {        
    }
    [TsClass(Module = Modules.Models.Name)]
    public class ReadJson<T>
    {
        public T ID { get; set; }
    }
}