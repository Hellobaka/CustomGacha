using System.Collections.Generic;

namespace me.cqp.luohuaming.CustomGacha.Code.GachaCore.Models
{
    /// <summary>
    /// 描述卡池的模型类
    /// </summary>
    public class Pool
    {
        /// <summary>
        /// 卡池名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 卡池内容
        /// </summary>
        public List<GachaItem> Content { get; set; }
        /// <summary>
        /// 配置卡池绘制配置
        /// </summary>
        public PoolDrawConfig PoolDrawConfig { get; set; }
        /// <summary>
        /// 保底所需要的次数
        /// </summary>
        public int BaodiCount { get; set; }
        /// <summary>
        /// 卡池背景图片相对路径
        /// </summary>
        public string BackgroundImagePath { get; set; }
        /// <summary>
        /// 自动设置相对路径
        /// </summary>
        public string RelativePath { get; set; }
    }
}
