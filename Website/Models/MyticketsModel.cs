using App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{
    public class MyticketsModel
    {
        public string name { get; set; }
        public string type { get; set; }
        public DateTime expiryDate { get; set; }
        public float value { get; set; }
        public float threshold { get; set; }
   
    }

}
