using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data
{
    [SugarTable("setting")]
    public class Setting
    {
        public Setting()
        {
        }

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        public string key { get; set; }
        public string value { get; set; }

    }
}
