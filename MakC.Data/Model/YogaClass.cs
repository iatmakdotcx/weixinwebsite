using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data
{
    [SugarTable("yogaclass")]
    public class YogaClass
    {
        public YogaClass()
        {
        }

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        public string name { get; set; }
        public string tags { get; set; }
        public bool disabled { get; set; }
        public string avatar { get; set; }
        public string description { get; set; }
        public int teacherid { get; set; }
        public string teacher { get; set; }
        public string address { get; set; }
        public DateTime rdate { get; set; }
        public string rtimeRange { get; set; }
        public int star { get; set; }
        /// <summary>
        /// 可预约数量
        /// </summary>
        public int kyyzs { get; set; }
        /// <summary>
        /// 已预约数量
        /// </summary>
        public int yysl { get; set; }
        /// <summary>
        /// 实际签到数量
        /// </summary>
        public int qdsl { get; set; }
    } 
}
