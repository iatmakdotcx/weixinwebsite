using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data
{
    [SugarTable("tickets")]
    public class Ticket
    {
        public Ticket()
        {
        }

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        public string name { get; set; }
        /// <summary>
        /// 满减、折扣
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 到手之后的过期时间
        /// </summary>
        public int expiryDay { get; set; }
        /// <summary>
        /// 折扣数，或满减金额数
        /// </summary>
        public float value { get; set; }
        /// <summary>
        /// 使用门槛
        /// </summary>
        public float threshold { get; set; }
        public string description { get; set; }
    }
}
