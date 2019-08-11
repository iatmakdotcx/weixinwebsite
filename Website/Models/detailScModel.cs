using App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{
    public class detailScModel
    {
        public SkinCareClass data;

        public List<AppointmentInfo> dayinfo = new List<AppointmentInfo>();

        public class AppointmentInfo
        {
            public DateTime date;
            public int remain = 0;
        }
    }

}
