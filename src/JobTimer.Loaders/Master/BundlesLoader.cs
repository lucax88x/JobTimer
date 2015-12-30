using System.Web.Hosting;
using JobTimer.WebApplication.ViewModels.WebApi.Master.Models;
using Newtonsoft.Json;

namespace JobTimer.WebApplication.Loaders.Master
{
    public interface IBundlesLoader
    {
        BundleContainer Load();
    }
    public class BundlesLoader : IBundlesLoader
    {
        public BundleContainer Load()
        {
            var b = JsonConvert.DeserializeObject<BundleContainer>(System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/bundles/bundles.json")));
            return b;
        }
    }
}
