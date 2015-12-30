using System.Collections.Generic;
using JobTimer.WebApplication.ViewModels.WebApi.Data.Models.Search;

namespace JobTimer.WebApplication.ViewModels.WebApi.Data.ViewModels
{
    public class SearchPagesViewModel : BaseViewModel
    {
        public List<SearchItem> Pages { get; set; }

        public SearchPagesViewModel()
        {
            Pages = new List<SearchItem>();
        }
    }
}