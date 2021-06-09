using SqlSugar;

namespace PublicInfos
{
    /// <summary>
    /// 注册与签到 指令文本设置
    /// </summary>
    [SugarTable("OrderConfig")]
    public class OrderConfig
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int RowID { get; set; }
        /// <summary>
        /// 注册指令
        /// </summary>
        public string RegisterOrder { get; set; }
        /// <summary>
        /// 签到指令
        /// </summary>
        public string SignOrder { get; set; }
        public string DuplicateRegisterText { get; set; }
        public string SuccessfulRegisterText { get; set; }
        public string LeakMoneyText { get; set; }
        public string SuccessfulSignText { get; set; }
        public string DuplicateSignText { get; set; }

        public OrderConfig Clone()
        {
            return (OrderConfig)this.MemberwiseClone();
        }
    }
}
