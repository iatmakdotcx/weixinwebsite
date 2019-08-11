using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data
{
    [SugarTable("users")]
    public class UserInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string nickname { get; set; }
        public string tel { get; set; }
        public string address { get; set; }
        public DateTime? birthday { get; set; }
        public int? height { get; set; }
        public float? weight { get; set; }
        public string wxId { get; set; }
        public int VipLevel { get; set; }
        public decimal balance { get; set; }
        public DateTime createAt { get; set; }
        public string avatar { get; set; }
    }
}
