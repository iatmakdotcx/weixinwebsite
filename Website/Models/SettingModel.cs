﻿using App.Data;
using MakC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{
    public class SettingModel
    {
        private static SettingModel instance = null;
        private static object locker = new object();
        public static SettingModel getInstance()
        {
            if (instance == null)
                lock (locker)
                    if (instance == null)
                    {
                        instance = new SettingModel();
                        var settings = DbContext.Get().GetEntityDB<Setting>().GetList();
                        if (settings != null)
                        {
                            var instype = instance.GetType();
                            foreach (var item in settings)
                            {
                                var mbr = instype.GetProperty(item.key);
                                if (mbr != null)
                                {
                                    if (mbr.PropertyType == typeof(bool))
                                    {
                                        mbr.SetValue(instance, item.value.Asbool());
                                    }
                                    else if (mbr.PropertyType == typeof(int))
                                    {
                                        mbr.SetValue(instance, item.value.AsInt());
                                    }
                                    else
                                    mbr.SetValue(instance, item.value);
                                }
                            }
                        }
                    }
            return instance;
        }
        public static void clearCache()
        {
            instance = null;
        }

        /// <summary>
        /// 允许注册
        /// </summary>
        public bool reg_open { get; set; }
        /// <summary>
        /// 注册会员条款
        /// </summary>
        public string reg_agreement { get; set; }
        /// 预约规则协议
        /// </summary>
        public string skincare_apc_agreement { get; set; }
        /// <summary>
        /// 美肤场馆电话
        /// </summary>
        public string skincare_Tel { get; set; }
        /// <summary>
        /// 瑜伽场馆电话
        /// </summary>
        public string Yoga_Tel { get; set; }

        /// <summary>
        /// 短信接口地址 
        /// </summary>
        public string SMS_APIHost { get; set; }
        /// <summary>
        /// 短信接口认证码
        /// </summary>
        public string SMS_appcode { get; set; }
        /// <summary>
        /// 短信模板
        /// </summary>
        public string SMS_template { get; set; }
       /// <summary>
       /// 手机端首页设置
       /// </summary>
        public string mobile_FirstPage { get; set; }

    }
}
