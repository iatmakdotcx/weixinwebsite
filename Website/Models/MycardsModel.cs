using App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{
    public class MyCardsModel
    {
        public string name { get; set; }
        public string img { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string cornerMark { get; set; }
        public DateTime expiryDate { get; set; }
        public int canUseCnt { get; set; }
        public int usedCnt { get; set; }
   
    }

}
