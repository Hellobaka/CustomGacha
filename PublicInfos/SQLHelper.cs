using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SqlSugar;

namespace PublicInfos
{
    public class SQLHelper
    {
        private static SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                //TODO: 插件发布时替换此处
                //ConnectionString = $"data source={Path.Combine(Environment.CurrentDirectory, "data.db")}",
                ConnectionString = $"data source={MainSave.DBPath}",
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
                //TODO: 插件发布时替换此处
                db.DbMaintenance.CreateDatabase(MainSave.DBPath);
                //db.DbMaintenance.CreateDatabase(Path.Combine(Environment.CurrentDirectory, "data.db"));
                db.CodeFirst.InitTables(typeof(DB_Repo));
                db.CodeFirst.InitTables(typeof(DB_User));
                db.CodeFirst.InitTables(typeof(Pool));
                db.CodeFirst.InitTables(typeof(GachaItem));
                db.CodeFirst.InitTables(typeof(Config));
                db.CodeFirst.InitTables(typeof(OrderConfig));
                db.CodeFirst.InitTables(typeof(Category));
            }
        }
        #region ---Functions---
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
                    MainSave.ApplicationConfig.SignResetTime.Hour,
                    MainSave.ApplicationConfig.SignResetTime.Minute,
                    MainSave.ApplicationConfig.SignResetTime.Second);
                if (user.LastSignTime < dt)
                {
                    int signMoney = new Random().Next(MainSave.ApplicationConfig.SignFloor, MainSave.ApplicationConfig.SignCeil + 1);
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
                    Money = MainSave.ApplicationConfig.RegisterMoney
                };
                db.Insertable(user).ExecuteCommand();
                return user;
            }
        }
        #endregion

        #region ---Users---
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
        #endregion

        #region ---Pools---
        public static List<Pool> GetAllPools()
        {
            using (var db = GetInstance())
            {
                var c = db.Queryable<Pool>().ToList();
                c.ForEach(x => x.PluginInit());
                return c;
            }
        }
        public static Pool GetPoolByID(int id)
        {
            using (var db = GetInstance())
            {
                var c = db.Queryable<Pool>().First(x=>x.PoolID==id);
                c.PluginInit();
                return c;
            }
        }
        public static int AddPool(Pool pool)
        {
            using (var db = GetInstance())
            {
                if (db.Queryable<Pool>().Any(x => x.GUID == pool.GUID) is false)
                {
                    return db.Insertable(pool).ExecuteReturnIdentity();
                }
                return -1;
            }
        }
        public static void UpdatePool(Pool pool)
        {
            if (pool == null)
                return;
            using (var db = GetInstance())
            {
                pool.UpdateDt = DateTime.Now;
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
        #endregion

        #region ---Configs---
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
                        SignResetTime = new DateTime(1970, 1, 1, 0, 0, 0),
                        RegisterMoney = 6400,
                        GachaCost = 160
                    };
                    c.RowID = db.Insertable(c).ExecuteReturnIdentity();
                }
                MainSave.ApplicationConfig = c;

                var o = db.Queryable<OrderConfig>().First(x => true);
                MainSave.OrderConfig = o;
                if (o == null)
                {
                    o = new OrderConfig
                    {
                        RegisterOrder = "#抽卡注册",
                        SignOrder = "#抽卡签到",
                        SuccessfulSignText = "<@>签到成功，获得通用货币<$0>",
                        DuplicateSignText = "<@>你今天签过到了",
                        SuccessfulRegisterText = "<@>注册成功，获得通用货币<current_money>",
                        DuplicateRegisterText = "<@>重复注册是打咩的",
                        LeakMoneyText = "<@>剩余货币不足以抽卡了呢~\n你目前还有<current_money>通用货币",
                    };
                    db.Insertable(o).ExecuteCommand();
                    MainSave.OrderConfig = o;
                }
            }
        }
        public static void UpdateConfig(Config config)
        {
            MainSave.ApplicationConfig = config.Clone();
            using (var db = GetInstance())
            {
                db.Updateable(config).ExecuteCommand();
            }
        }
        public static void UpdateOrderConfig(OrderConfig config)
        {
            MainSave.OrderConfig = config.Clone();
            using (var db = GetInstance())
            {
                db.Updateable(config).ExecuteCommand();
            }
        }
        #endregion

        #region ---Category---
        public static List<Category> GetCategoriesByIDs(List<int> id)
        {
            using (var db = GetInstance())
            {
                List<Category> c = new List<Category>();
                foreach (var item in id)
                {
                    c.Add(db.Queryable<Category>().First(x => x.ID == item));
                }
                return c;
            }
        }
        public static int UpdateOrAddCategory(Category item, bool addFlag = false)
        {
            if (item == null)
                return -1;
            using (var db = GetInstance())
            {
                if (addFlag)
                {
                    return db.Insertable(item).ExecuteReturnIdentity();
                }
                else
                {
                    if (db.Queryable<Category>().Any(x => x.GUID == item.GUID))
                    {
                        item.ID = db.Queryable<Category>().First(x => x.GUID == item.GUID).ID;
                        item.UpdateDt = DateTime.Now;
                        db.Updateable(item).ExecuteCommand();
                        return item.ID;
                    }
                    else
                    {
                        return db.Insertable(item).ExecuteReturnIdentity();
                    }
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
            using (var db = GetInstance())
            {
                return db.Queryable<GachaItem>().ToList();
            }
        }
        public static List<GachaItem> GetPageGachaItem(int pageIndex, int pageCount = 50)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<GachaItem>().ToPageList(pageIndex, pageCount);
            }
        }
        public static int UpdateOrAddGachaItem(GachaItem item)
        {
            if (item == null)
                return -1;
            using (var db = GetInstance())
            {
                if (db.Queryable<GachaItem>().Any(x => x.GUID == item.GUID))
                {
                    item.UpdateDt = DateTime.Now;
                    db.Updateable(item).ExecuteCommand();
                    return db.Queryable<GachaItem>().First(x => x.GUID == item.GUID).ItemID;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(item.Remark))
                        item.Remark = string.Empty;
                    return db.Insertable(item).ExecuteReturnIdentity();
                }
            }
        }
        public static Dictionary<int, GachaItem> UpdateIDByGUID(Dictionary<int, GachaItem> ls)
        {
            if (ls == null || ls.Count == 0)
                return ls;
            using (var db = GetInstance())
            {
                foreach(var pair in ls)
                {
                    var item = pair.Value;
                    if (db.Queryable<GachaItem>().Any(x => x.GUID == item.GUID))
                    {
                        item.UpdateDt = DateTime.Now;
                        db.Updateable(item).ExecuteCommand();
                        item.ItemID = db.Queryable<GachaItem>().First(x => x.GUID == item.GUID).ItemID;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(item.Remark))
                            item.Remark = string.Empty;
                        item.ItemID = db.Insertable(item).ExecuteReturnIdentity();
                    }
                }
            }
            return ls;
        }
        public static void RemoveGachaItem(GachaItem item)
        {
            using (var db = GetInstance())
            {
                db.Deleteable(item).ExecuteCommand();
            }
        }
        public static GachaItem GetGachaItemByID(int id)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<GachaItem>().First(x => x.ItemID == id);
            }
        }
        public static List<GachaItem> GetGachaItemsByIDs(List<int> id)
        {
            List<GachaItem> c = new List<GachaItem>();
            using (var db = GetInstance())
            {
                foreach (var item in id)
                {
                    c.Add(db.Queryable<GachaItem>().First(x => x.ItemID == item));
                }
            }
            return c;
        }
        public static List<GachaItem> UpdateGachaItemsNewStatus(List<GachaItem> ls, long QQ)
        {
            using (var db = GetInstance())
            {
                foreach (var item in ls)
                {
                    if (ls.Any(x => x.ItemID == item.ItemID && x.IsNew))
                        continue;
                    item.IsNew = !db.Queryable<DB_Repo>().Any(x => x.QQID == QQ && x.ItemID == item.ItemID);
                }
                return ls;
            }
        }
        #endregion
    }
}
