using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data
{
    [SugarTable("cards")]
    public class Card
    {
        public Card()
        {
        }

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        public string name { get; set; }
        /// <summary>
        /// 周卡，月卡，年卡，次卡
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 卡片角标
        /// </summary>
        public string cornerMark { get; set; }
        /// <summary>
        /// 自购买之日起过期时间（天数）        
        /// </summary>
        public int expiry { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string img { get; set; }
        /// <summary>
        /// 卡片描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 次卡可使用次数
        /// </summary>
        public int canUseCount { get; set; }
        /// <summary>
        /// 卡片已禁用（不能
        /// </summary>
        public bool disabled { get; set; }
        /// <summary>
        /// 微信端可以购买卡片
        /// </summary>
        public bool canBuy { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public float price { get; set; }
        /// <summary>
        /// 商品原价
        /// </summary>
        public float price2 { get; set; }

    }
}
