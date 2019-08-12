using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data
{
    [SugarTable("skincareorder")]
    public class SkinCareOrder
    {
        public SkinCareOrder()
        {
        }

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }
        public DateTime create_at { get; set; }
        public DateTime reserveDate { get; set; }
        public string tel { get; set; }
        public bool canceled { get; set; }
        public int? cancelUser { get; set; }
        public DateTime? cancelTime { get; set; }
    } 
}
