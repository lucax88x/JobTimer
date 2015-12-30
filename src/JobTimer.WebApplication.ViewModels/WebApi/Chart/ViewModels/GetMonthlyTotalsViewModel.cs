using JobTimer.WebApplication.TypeScript;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.WebApi.Chart.ViewModels
{
    [TsClass(Module = Modules.ViewModels.Chart)]
    public class GetMonthlyTotalsViewModel : ChartViewModel<double?>
    {
    }
}