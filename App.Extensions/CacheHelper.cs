using App.Data;
using System;
using System.Collections.Generic;

namespace App.Extensions
{
    public class CacheHelper
    {
        private delegate object Dlg_getdata();
        private static List<T> AllTable<T>(string cacheKey, double expirationMinute = 5) where T : class, new()
        {
            return Obj<List<T>>(cacheKey, expirationMinute, ()=> DbContext.Get().GetEntityDB<T>().GetList()); 
        }
        private static List<T> List<T>(string cacheKey, double expirationMinute = 5, Dlg_getdata g = null) where T : class, new()
        {        
            return Obj<List<T>>(cacheKey, expirationMinute, g);
        }
        private static T Obj<T>(string cacheKey, double expirationMinute = 5, Dlg_getdata g = null) where T : class, new()
        {
            T tmpData = MemoryCacheService.Default.GetCache<T>(cacheKey);
            if (tmpData == null)
            {
                lock (MemoryCacheService.Default)
                {
                    tmpData = MemoryCacheService.Default.GetCache<T>(cacheKey);
                    if (tmpData == null && g != null)
                    {
                        tmpData = (T)g.Invoke();
                        if (tmpData != null)
                        {
                            MemoryCacheService.Default.SetCache(cacheKey, tmpData, expirationMinute);
                        }
                    }
                }
            }
            return tmpData;
        }
        public static List<App.Data.Setting> GetSettings()
        {            
            return AllTable<App.Data.Setting>("db_setting");
        }
    }
}
