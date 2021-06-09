using System;
using SqlSugar;

namespace PublicInfos
{
    /// <summary>
    /// 描述仓库信息
    /// </summary>
    [SugarTable("Repo")]
    public class DB_Repo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int RowID { get; set; }
        public long QQID { get; set; }
        /// <summary>
        /// 物品ID
        /// </summary>
        public int ItemID { get; set; }
        /// <summary>
        /// 物品名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 物品数量
        /// </summary>
        public long ItemCount { get; set; }
        /// <summary>
        /// 物品获取时间
        /// </summary>
        public DateTime ItemGetTime { get; set; }
    }
}
