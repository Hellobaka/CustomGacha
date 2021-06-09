using System;
using SqlSugar;

namespace PublicInfos
{
    /// <summary>
    /// 全局设置
    /// </summary>
    [SugarTable("Config")]
    public class Config
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int RowID { get; set; }
        /// <summary>
        /// 签到重置时间点
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public DateTime SignResetTime { get; set; }
        /// <summary>
        /// 签到获得的货币上限
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int SignCeil { get; set; }
        /// <summary>
        /// 签到获得的货币下限
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int SignFloor { get; set; }
        /// <summary>
        /// 每抽消耗货币数
        /// </summary>
        public int GachaCost { get; set; }
        public int RegisterMoney { get; set; }
        public Config Clone()
        {
            return (Config)this.MemberwiseClone();
        }
    }
}
