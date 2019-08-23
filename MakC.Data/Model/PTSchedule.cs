using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data
{
    [SugarTable("ptschedule")]
    public class PTSchedule
    {
        public PTSchedule()
        {
        }

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        public string name { get; set; }
        public int userId { get; set; }
        public string username { get; set; }
        public string avatar { get; set; }
        public string tags { get; set; }
        public DateTime rdate { get; set; }
        public string rtime { get; set; }
        public int rlen { get; set; }
        public string description { get; set; }
        public int kyyzs { get; set; }
        public int yysl { get; set; }
        
    } 
}
