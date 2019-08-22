using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data
{
    [SugarTable("pttemplate")]
    public class PTTemplate
    {
        public PTTemplate()
        {
        }

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        public string name { get; set; }
        public int userId { get; set; }
        public string tags { get; set; }
        public string rtime { get; set; }
        public int rlen { get; set; }
        public string description { get; set; }
        public int kyyzs { get; set; }
        
    } 
}
