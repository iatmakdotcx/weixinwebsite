using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data
{
    [SugarTable("usertickets")]
    public class UserTicket
    {
        public UserTicket()
        {
        }

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        public int userid { get; set; }
        public int ticketid { get; set; }
        public string name { get; set; }
        public DateTime create_at { get; set; }
        public DateTime expiryDate { get; set; }
        public bool deleted { get; set; }
        public DateTime deleteTime { get; set; }
        public string deleteuser { get; set; }
        public string useOrder { get; set; }
        public string description { get; set; }
    }
}
