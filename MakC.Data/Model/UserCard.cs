using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data
{
    [SugarTable("usercards")]
    public class UserCard
    {
        public UserCard()
        {
        }

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        public int userid { get; set; }
        public DateTime create_at { get; set; }
        public int cardsId { get; set; }
        public string cardname { get; set; }
        public string cardtype { get; set; }
        public DateTime cardexpiryDate { get; set; }
        public int canUseCnt { get; set; }
        public int usedCnt { get; set; }
        public string tradeNo { get; set; }

    }
}
