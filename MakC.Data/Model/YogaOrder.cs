using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data
{
    [SugarTable("yogaorder")]
    public class YogaOrder
    {
        public YogaOrder()
        {
        }

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        public int classId { get; set; }
        public int userId { get; set; }
        public DateTime create_at { get; set; }
        public string tel { get; set; }
        public bool canceled { get; set; }
        public int? cancelUser { get; set; }
        public DateTime? cancelTime { get; set; }
        public string cancelMsg { get; set; }
    } 
}
