using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Hosting;
using JobTimer.WebApplication.ViewModels.Common;
using Newtonsoft.Json;

namespace JobTimer.WebApplication.Loaders.Navigation
{
    public interface IMenuLoader
    {
        Menu Load(IList<string> roles);
        List<MenuItem> GetAllChilds(IList<string> roles);
        List<MenuItem> GetAllChildsFiltered(IList<string> roles, string name, int limit);
    }
    public class MenuLoader : IMenuLoader
    {
        public Menu Load(IList<string> roles)
        {
            var result = new Menu();
            var path = HostingEnvironment.MapPath("~/sitemap/site.json");
            if (!string.IsNullOrEmpty(path))
            {
                var key = string.Format("Menu_{0}", string.Join("|", roles));                
                if (MemoryCache.Default[key] == null)
                {                    
                    var menu = JsonConvert.DeserializeObject<Menu>(System.IO.File.ReadAllText(path));
                    //menu.Items = menu.Items.Where(x => x.Roles.Intersect(roles).Any()).ToList();                
                    foreach (var child in menu.Items)
                    {
                        child.Items = child.Items.Where(x => x.Roles.Intersect(roles).Any()).ToList();
                    }
                    MemoryCache.Default[key] = menu;                    
                }

                result = (Menu)MemoryCache.Default[key];                
            }
            return result;
        }
        public List<MenuItem> GetAllChilds(IList<string> roles)
        {
            List<MenuItem> result = null;
            var menu = Load(roles);
            result = menu.Items.SelectMany(x => x.Items).ToList();
            return result;
        }
        public List<MenuItem> GetAllChildsFiltered(IList<string> roles, string name, int limit)
        {
            List<MenuItem> result = GetAllChilds(roles);

            return result.Where(x => x.Name.ToLower().Contains(name.ToLower())).Take(limit).ToList();            
        }
    }
}
