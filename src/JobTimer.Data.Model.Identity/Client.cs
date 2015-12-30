using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTimer.Data.Model.Identity
{    
    public class Client
    {
        public string Id { get; set; }        
        public string Secret { get; set; }        
        public string Name { get; set; }        
        public bool Active { get; set; }        
        [MaxLength(100)]
        public string AllowedOrigin { get; set; }
    }
}
