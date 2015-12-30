using System.Collections.Generic;

namespace JobTimer.WebApplication.ViewModels.WebApi.Common
{
    public class GridDataRequest
    {
        public int start { get; set; }
        public int limit { get; set; }
    }
    public class GridData<T>
    {
        public GridDataRequest request { get; set; }
        public List<T> results { get; set; }
        public int hits { get; set; }

        public GridData()
        {
            results = new List<T>();
            request = new GridDataRequest();
        }
    }
}