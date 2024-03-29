﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data
{
    public class DbContext
    {
        private static string _connectionString = "";
        private static DbType _dbType;
        public SqlSugarClient Db { get; set; }
        public DbContext()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new ArgumentNullException("数据库连接字符串为空");
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = _connectionString,
                DbType = _dbType,
                IsAutoCloseConnection = true,
                IsShardSameThread = false,
                InitKeyType = InitKeyType.Attribute,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    //DataInfoCacheService = new HttpRuntimeCache()
                },
                MoreSettings = new ConnMoreSettings()
                {
                    //IsWithNoLockQuery = true,
                    IsAutoRemoveDataCache = true
                }
            });
            Db.Aop.OnLogExecuting = (sql, pars) => //SQL执行前 可以修改SQL
            {
#if DEBUG
                StringBuilder sbsql = new StringBuilder();
                sbsql.AppendLine("---------------------------------sql start-----------------------------------------");
                sbsql.AppendLine(sql);
                sbsql.AppendLine("Parameters:");
                foreach (var item in pars)
                {
                    sbsql.AppendLine(item.ParameterName+"       "+ item.Value);
                }
                sbsql.AppendLine("----------------------------------sql end----------------------------------------");
                System.Diagnostics.Trace.WriteLine(sbsql.ToString());
#endif
            };

        }
        private static DbType getDbType(string dbTypeString)
        {
            switch (dbTypeString.ToLower())
            {
                case "mysql": return DbType.MySql;
                case "oracle": return DbType.Oracle;
                case "postgresql": return DbType.PostgreSQL;
                case "sqlite": return DbType.Sqlite;
                case "sqlserver": return DbType.SqlServer;
                default:
                    throw new NotSupportedException($"不支持的数据库类型：{dbTypeString}");
            }
        }

        public SimpleClient<T> GetEntityDB<T>() where T : class, new()
        {
            return new SimpleClient<T>(Db);
        }

        /// <summary>
        /// 获取dbContext的新实例
        /// </summary>
        /// <returns></returns>
        public static DbContext Get()
        {
            return new DbContext();
        }
        public static void Init(string strConnectionString, DbType enmDbType = SqlSugar.DbType.MySql)
        {
            _connectionString = strConnectionString;
            _dbType = enmDbType;
        }
        public static void Init(string strConnectionString, string enmDbType)
        {
            Init(strConnectionString, getDbType(enmDbType));            
        }



        public SimpleClient<Setting> setting => new SimpleClient<Setting>(Db);
    }
}
