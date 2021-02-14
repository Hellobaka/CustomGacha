using System.Text;

namespace me.cqp.luohuaming.CustomGacha.Code.GachaCore.Models
{
    /// <summary>
    /// 描述抽卡子项目的模型类
    /// </summary>
    public class GachaItem
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 项目概率
        /// </summary>
        public double Probablity { get; set; }
        /// <summary>
        /// 图片相对路径
        /// </summary>
        public string ImagePath { get; set; }
        /// <summary>
        /// 背景图片相对路径
        /// </summary>
        public string BackgroundImagePath { get; set; }
        /// <summary>
        /// 是否为保底项目
        /// </summary>
        public bool IsBaodi { get; set; }
        /// <summary>
        /// 表示当前项目的数量
        /// </summary>
        public long Count { get; set; }
        /// <summary>
        /// 最低数量
        /// </summary>
        public int CountFloor { get; set; }
        /// <summary>
        /// 最高数量
        /// </summary>
        public int CountUpper { get; set; }
        /// <summary>
        /// 是否能被折叠
        /// </summary>
        public bool CanBeFolded { get; set; }
        /// <summary>
        /// 卡片价值，影响后续排序
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// 图片与背景之间的绘制关系
        /// </summary>
        public ItemDrawConfig ImageConfig { get; set; }

        public GachaItem Clone()
        {
            return (GachaItem)this.MemberwiseClone();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"名称：{Name}");
            return sb.ToString();
        }
    }
}
