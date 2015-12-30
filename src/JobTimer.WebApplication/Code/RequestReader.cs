using System.Web;

namespace JobTimer.WebApplication.Code
{
    public interface IRequestReader
    {
        string Read(string key);
    }

    public class RequestReader : IRequestReader
    {        
        public string Read(string key)
        {
            return HttpContext.Current.Request.GetOwinContext().Request.Headers[key];
        }
    }    
}