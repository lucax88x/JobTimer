using System.Collections.Generic;

namespace JobTimer.WebApplication.ViewModels.WebApi.Master.ViewModels
{
    public class BundleItem
    {
        public string Bundle { get; set; }
        public List<string> Scripts { get; set; }

        public BundleItem()
        {
            Scripts = new List<string>();
        }
    }
    public class GetBundlesViewModel
    {
        public List<BundleItem> Bundles { get; set; }
        public GetBundlesViewModel()
        {
            Bundles = new List<BundleItem>();
        }
    }
}
