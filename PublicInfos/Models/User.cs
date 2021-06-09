using System;
using System.Text;
using SqlSugar;

namespace PublicInfos
{
    /// <summary>
    /// 描述用户信息
    /// </summary>
    [SugarTable("User")]
    public class DB_User
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int RowID { get; set; }
        /// <summary>
        /// QQ号
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long QQID { get; set; }
        /// <summary>
        /// 现有的货币数
        /// </summary>
        public long Money { get; set; } = 0;
        /// <summary>
        /// 抽卡总次数
        /// </summary>
        public int GachaTotalCount { get; set; } = 0;
        /// <summary>
        /// 总消耗代币数
        /// </summary>
        public long MoneyTotalCount { get; set; } = 0;
        /// <summary>
        /// 抽卡次数, 保底后重置
        /// </summary>
        public int GachaCount { get; set; } = 0;
        /// <summary>
        /// 总签到次数
        /// </summary>
        public int SignTotalCount { get; set; } = 0;
        /// <summary>
        /// 上次签到时间
        /// </summary>
        public DateTime LastSignTime { get; set; } = new DateTime(1970, 1, 1, 0, 0, 0);
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"QQ号：{QQID}");
            sb.AppendLine($"货币数：{Money}");
            sb.AppendLine($"总抽卡次数：{GachaTotalCount}");
            sb.AppendLine($"总消耗货币数：{MoneyTotalCount}");
            sb.AppendLine($"总签到次数：{SignTotalCount}");
            sb.AppendLine($"抽卡保底剩余次数：{GachaCount}");
            sb.AppendLine($"上次签到时间：{LastSignTime}");
            return sb.ToString();
        }
    }
}
