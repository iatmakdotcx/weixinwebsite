using App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{
    public class TocResultModel
    {
        private string _type;
        public string typeMc { get; private set; }
        public string type {
            get { return _type; }
            set {
                _type = value;
                switch (_type)
                {
                    case "0": typeMc = "瑜伽";break;
                    case "1": typeMc = "私教";break;
                    default:
                        typeMc = "瑜伽"; break;
                }
            }
        }
        
        public DateTime qrydate;
        public List<YogaClass> yoga;
        public List<PTSchedule> PT;

  

    }

}
