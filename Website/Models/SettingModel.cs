using App.Data;
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
                                    else
                                    mbr.SetValue(instance, item.value);
                                }
                            }
                        }
                    }
            return instance;
        }
        public bool reg_open { get; set; }
        public string reg_agreement { get; set; }
        
    }
}
