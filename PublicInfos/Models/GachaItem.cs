using System;
using SqlSugar;

namespace PublicInfos
{
    /// <summary>
    /// 描述抽卡子项目的模型类
    /// </summary>
    [SugarTable("Items")]
    public class GachaItem
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ItemID { get; set; } = 0;
        private string name = "";
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get { return name; } set { name = value; Editted = true; } }
        private double probablity = 0;
        /// <summary>
        /// 项目概率
        /// </summary>
        public double Probablity { get { return probablity; } set { probablity = value; Editted = true; } }

        private double upprobablity = 0;
        /// <summary>
        /// Up后概率
        /// </summary>
        public double UpProbablity { get { return upprobablity; } set { upprobablity = value; Editted = true; } }
        private string imagePath = "";
        /// <summary>
        /// 图片相对路径
        /// </summary>
        public string ImagePath { get { return imagePath; } set { imagePath = value; Editted = true; } }
        private string backgroundimagePath = "";
        /// <summary>
        /// 背景图片相对路径
        /// </summary>
        public string BackgroundImagePath { get { return backgroundimagePath; } set { backgroundimagePath = value; Editted = true; } }
        private long count = 0;
        /// <summary>
        /// 表示当前项目的数量
        /// </summary>
        public long Count { get { return count; } set { count = value; Editted = true; } }

        private int countCeil = 1;
        /// <summary>
        /// 最高数量
        /// </summary>
        public int CountCeil { get { return countCeil; } set { countCeil = value; Editted = true; } }
        private int countFloor = 1;
        /// <summary>
        /// 最低数量
        /// </summary>
        public int CountFloor { get { return countFloor; } set { countFloor = value; Editted = true; } }

        private bool isUp = false;
        [SugarColumn(IsIgnore = true)]
        public bool IsUp { get { return isUp; } set { isUp = value; Editted = true; } }
        private bool canBeFolded = false;
        /// <summary>
        /// 是否能被折叠
        /// </summary>
        public bool CanBeFolded { get { return canBeFolded; } set { canBeFolded = value; Editted = true; } }
        private int _value = 0;
        /// <summary>
        /// 卡片价值，影响后续排序
        /// </summary>
        public int Value { get { return _value; } set { _value = value; Editted = true; } }
        private string remark = "";
        /// <summary>
        /// 自定义备注
        /// </summary>
        public string Remark { get { return remark; } set { remark = value; Editted = true; } }
        private DateTime createDt = DateTime.Now;
        public DateTime CreateDt { get { return createDt; } set { createDt = value; Editted = true; } }

        private DateTime updateDt = DateTime.Now;
        public DateTime UpdateDt { get { return updateDt; } set { updateDt = value; Editted = true; } }

        private bool isNew = true;
        [SugarColumn(IsIgnore = true)]
        public bool IsNew { get { return isNew; } set { isNew = value; Editted = true; } }
        [SugarColumn(IsIgnore = true)]
        public bool Editted { get; set; } = false;
        [SugarColumn(IsIgnore = true)]
        public bool IsChecked { get; set; } = false;
        public string GUID { get; set; } = "";
        public GachaItem Clone()
        {
            return (GachaItem)this.MemberwiseClone();
        }
    }
}
