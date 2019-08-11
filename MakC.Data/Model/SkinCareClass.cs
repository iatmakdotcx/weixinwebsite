using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data
{
    [SugarTable("skincareclass")]
    public class SkinCareClass
    {
        public SkinCareClass()
        {
        }

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        public string name { get; set; }
        public string tags { get; set; }
        public bool enabled { get; set; }
        public string avatar { get; set; }
        public string description { get; set; }
        public string teacher { get; set; }
        public string address { get; set; }
    } 
}
