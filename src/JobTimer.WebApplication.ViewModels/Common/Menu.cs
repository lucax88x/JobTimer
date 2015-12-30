using System.Collections.Generic;
using JobTimer.WebApplication.TypeScript;
using TypeLite;

namespace JobTimer.WebApplication.ViewModels.Common
{
    public class Menu
    {
        public string Name { get; set; }
        public List<MenuItem> Items { get; set; }

        public Menu()
        {
            Items = new List<MenuItem>();
        }
    }
    [TsClass(Module = Modules.Models.Menu)]
    public class MenuItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Roles { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public List<MenuItem> Items { get; set; }

        public MenuItem()
        {
            Roles = new List<string>();
            Items = new List<MenuItem>();            
        }
    }
}
