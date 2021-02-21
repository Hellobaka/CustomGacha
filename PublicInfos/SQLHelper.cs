using System;
using System.Collections.Generic;
using System.IO;
using SqlSugar;

namespace PublicInfos
{
    public class SQLHelper
    {
        private static SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = $"data source={Path.Combine(Environment.CurrentDirectory, "data.db")}",
                //ConnectionString = $"data source={MainSave.DBPath}",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
            });
            return db;
        }

        /// <summary>
        /// 数据库不存在时将会创建
        /// </summary>
        public static void CreateDB()
        {
            using (var db = GetInstance())
            {
                //db.DbMaintenance.CreateDatabase(MainSave.DBPath);
                db.DbMaintenance.CreateDatabase(Path.Combine(Environment.CurrentDirectory, "data.db"));
                db.CodeFirst.InitTables(typeof(DB_Repo));
                db.CodeFirst.InitTables(typeof(DB_User));
                db.CodeFirst.InitTables(typeof(Pool));
                db.CodeFirst.InitTables(typeof(GachaItem));
                db.CodeFirst.InitTables(typeof(Config));
                db.CodeFirst.InitTables(typeof(OrderConfig));
                db.CodeFirst.InitTables(typeof(Category));
            }
        }
        /// <summary>
        /// 进行签到
        /// </summary>
        /// <param name="QQID">QQ号</param>
        /// <returns>签到获得的货币数</returns>
        public static int Sign(long QQID)
        {
            using (var db = GetInstance())
            {
                DB_User user = db.Queryable<DB_User>().First(x => x.QQID == QQID);
                var t = DateTime.Now;
                var dt = new DateTime(t.Year, t.Month, t.Day,
                    MainSave.SignResetTime.Hour,
                    MainSave.SignResetTime.Minute,
                    MainSave.SignResetTime.Second);
                if (user.LastSignTime < dt)
                {
                    int signMoney = new Random().Next(MainSave.SignFloor, MainSave.SignCeil + 1);
                    user.Money += signMoney;
                    user.SignTotalCount++;
                    user.LastSignTime = DateTime.Now;
                    db.Updateable(user).ExecuteCommand();
                    return signMoney;
                }
                return -1;
            }
        }
        public static bool IDExists(long QQID)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<DB_User>().Any(x => x.QQID == QQID);
            }
        }
        public static DB_User Register(long QQID)
        {
            using (var db = GetInstance())
            {
                DB_User user = new DB_User
                {
                    QQID = QQID,
                    Money = 3200
                };
                db.Insertable(user).ExecuteCommand();
                return user;
            }
        }
        public static void InsertGachaItem2Repo(List<GachaItem> ls, long QQID)
        {
            using (var db = GetInstance())
            {
                List<DB_Repo> repoitems = new List<DB_Repo>();
                foreach (var item in ls)
                {
                    repoitems.Add(new DB_Repo
                    {
                        QQID = QQID,
                        ItemCount = item.Count,
                        ItemGetTime = DateTime.Now,
                        ItemID = item.ItemID,
                        ItemName = item.Name
                    });
                }
                db.Insertable(repoitems).ExecuteCommandAsync();
            }
        }
        public static void UpdateUser(DB_User user)
        {
            using (var db = GetInstance())
            {
                db.Updateable(user).ExecuteCommand();
            }
        }
        public static DB_User GetUser(long QQID)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<DB_User>().First(x => x.QQID == QQID);
            }
        }
        public static long GetMoney(long QQID)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<DB_User>().First(x => x.QQID == QQID).Money;
            }
        }
        public static List<Pool> GetAllPools()
        {
            using (var db = GetInstance())
            {
                return db.Queryable<Pool>().ToList();
            }
        }
        public static void AddPool(Pool pool)
        {
            using (var db = GetInstance())
            {
                if (db.Queryable<Pool>().Any(x => x.PoolID == pool.PoolID) is false)
                    db.Insertable(pool).ExecuteCommand();
            }
        }
        public static void UpdatePool(Pool pool)
        {
            if (pool == null)
                return;
            using (var db = GetInstance())
            {
                db.Updateable(pool).ExecuteCommand();
            }
        }
        public static void RemovePool(Pool pool)
        {
            using (var db = GetInstance())
            {
                db.Deleteable(pool).ExecuteCommand();
            }
        }
        public static void LoadConfig()
        {
            using (var db = GetInstance())
            {
                var c = db.Queryable<Config>().First(x => true);
                if (c == null)
                {
                    c = new Config
                    {
                        SignFloor = 1600,
                        SignCeil = 3200,
                        SignResetTime = new DateTime(1970, 1, 1, 0, 0, 0)
                    };
                    db.Insertable(c).ExecuteCommand();
                }
                MainSave.SignResetTime = c.SignResetTime;
                MainSave.SignCeil = c.SignCeil;
                MainSave.SignFloor = c.SignFloor;

                var o = db.Queryable<OrderConfig>().First(x => true);
                MainSave.OrderConfig = o;
                if (o == null)
                {
                    o = new OrderConfig
                    {
                        Register = "#抽卡注册",
                        Sign = "#抽卡签到"
                    };
                    db.Insertable(o).ExecuteCommand();
                }
            }
        }
        #region ---Category---
        public static List<Category> GetCategoriesByIDs(List<int> id)
        {
            using(var db = GetInstance())
            {
                List<Category> c = new List<Category>();
                foreach(var item in id)
                {
                    c.Add(db.Queryable<Category>().First(x => x.ID == item));
                }
                return c;
            }
        }
        public static int UpdateOrAddCategory(Category item)
        {
            if (item == null)
                return -1;
            using (var db = GetInstance())
            {
                if (db.Queryable<Category>().Any(x => x.ID == item.ID))
                {
                    db.Updateable(item).ExecuteCommand();
                    return item.ID;
                }
                else
                {
                    return db.Insertable(item).ExecuteReturnIdentity();
                }
            }
        }
        public static void RemoveCategory(Category category) 
        {
            using (var db = GetInstance())
            {
                db.Deleteable(category).ExecuteCommand();
            }
        }
        #endregion

        #region ---GachaItem---
        public static List<GachaItem> GetAllGachaItem()
        {
            using(var db = GetInstance())
            {
                return db.Queryable<GachaItem>().ToList();
            }
        }
        public static int InsertOrUpdateGachaItem(GachaItem item)
        {
            if (item == null)
                return -1;
            using (var db = GetInstance())
            {
                if (db.Queryable<GachaItem>().Any(x => x.ItemID == item.ItemID))
                {
                    db.Updateable(item).ExecuteCommand();
                    return item.ItemID;
                }
                else
                {
                    return db.Insertable(item).ExecuteReturnIdentity();
                }
            }
        }
        public static void RemoveGachaItem(GachaItem item)
        {
            using (var db = GetInstance())
            {
                db.Deleteable(item).ExecuteCommand();
            }
        }
        public static List<GachaItem> GetContentByIDs(List<int> id)
        {
            List<GachaItem> c = new List<GachaItem>();
            using (var db = GetInstance())
            {
               foreach(var item in id)
                {
                    c.Add(db.Queryable<GachaItem>().First(x => x.ItemID == item));
                }
            }
            return c;
        }
        #endregion
    }
}
