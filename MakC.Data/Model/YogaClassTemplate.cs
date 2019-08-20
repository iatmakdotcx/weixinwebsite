using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data
{
    [SugarTable("yoga_template")]
    public class YogaClassTemplate
    {
        public YogaClassTemplate()
        {
        }

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int star { get; set; }
        public int duration { get; set; }
        public string tags { get; set; }
        public string teacher { get; set; }
        public string address { get; set; }
        public string avatar { get; set; }
        public int kyyzs { get; set; }
        public string rtimeRange { get; set; }
    } 
}
