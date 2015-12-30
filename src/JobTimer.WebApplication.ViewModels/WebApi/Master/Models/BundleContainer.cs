using System.Collections.Generic;

namespace JobTimer.WebApplication.ViewModels.WebApi.Master.Models
{
    public class BundleItem
    {
        public string Bundle { get; set; }
        public List<string> Scripts { get; set; }
    }
    public class BundleContainer
    {
        public List<BundleItem> Bundles { get; set; }
    }
}
