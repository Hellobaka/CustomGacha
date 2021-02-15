using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            }
        }
        /// <summary>
        /// 进行签到
        /// </summary>
        /// <param name="QQID">QQ号</param>
        /// <returns>签到是否成功</returns>
        public static int Sign(long QQID)
        {
            using (var db = GetInstance())
            {
                DB_User user = db.Queryable<DB_User>().First(x => x.QQID == QQID);
                var dt = MainSave.SignResetTime;
                if (user.LastSignTime.Year != DateTime.Now.Year //上次签到与今天不是同一天
                    && user.LastSignTime.Month != DateTime.Now.Month
                    && user.LastSignTime.Day != DateTime.Now.Day
                    && user.LastSignTime.Hour >= dt.Hour //上次签到时间大于设定的签到重置
                    && user.LastSignTime.Minute >= dt.Minute
                    && user.LastSignTime.Second > dt.Second)
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
        public static void InsertGachaItem(List<GachaItem> ls)
        {
            using (var db = GetInstance())
            {
                db.Insertable(ls).ExecuteCommandAsync();
            }
        }
        public static void InsertGachaItem(GachaItem item)
        {
            InsertGachaItem(new List<GachaItem> { item });
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
    }
}
