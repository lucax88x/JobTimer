using System.Collections.Generic;

namespace JobTimer.WebApplication.ViewModels.WebApi.Master.ViewModels
{
    public class GetLastVisitedsViewModel : BaseViewModel
    {        
        public List<string> Visiteds { get; set; }

        public GetLastVisitedsViewModel()
        {            
            Visiteds = new List<string>(); 
        }
    }
}