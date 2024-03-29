﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MakC.Common
{
    public class mUtils
    {
        public static string getweekText(DayOfWeek weekIdx)
        {
            switch (weekIdx)
            {
                case DayOfWeek.Monday: return "周一";
                case DayOfWeek.Tuesday: return "周二";
                case DayOfWeek.Wednesday: return "周三";
                case DayOfWeek.Thursday: return "周四";
                case DayOfWeek.Friday: return "周五";
                case DayOfWeek.Saturday: return "周六";
                case DayOfWeek.Sunday: return "周日";
                default: return "";
            }
        }
        public static string MD5Hash(string input)
        {
            return MD5Hash(Encoding.UTF8.GetBytes(input));
        }
        public static string MD5Hash(byte[] input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(input);
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "").ToLower();
            }
        }
        public static List<int> idsToList(string ids)
        {
            List<int> intlist = new List<int>();
            foreach (var item in ids.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                int tmpint;
                if (int.TryParse(item, out tmpint))
                {
                    intlist.Add(tmpint);
                }
            }
            return intlist;
        }
        /// <summary>
        /// 过滤不安全的id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static string checkids(string ids)
        {            
            return string.Join(",", idsToList(ids));
        }
    }
}
