namespace PublicInfos
{
    public enum DrawOrder
    {
        /// <summary>
        /// 绘制时，核心图片晚于背景绘制，核心图片非透明部分可能会覆盖部分背景
        /// </summary>
        ImageAboveBackground,
        /// <summary>
        /// 绘制时，背景晚于核心图片绘制，背景非透明部分可能会覆盖部分核心图片
        /// </summary>
        ImageBelowBackground
    }

    public class ItemDrawConfig
    {
        /// <summary>
        /// 核心图片绘制大小
        /// </summary>
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        /// <summary>
        /// 背景图片绘制大小
        /// </summary>
        public int BackgroundImageWidth { get; set; }
        public int BackgroundImageHeight { get; set; }
        /// <summary>
        /// 背景与核心图片绘制顺序
        /// </summary>
        public DrawOrder DrawOrder { get; set; }
        /// <summary>
        /// 核心图片相对于背景的偏移坐标
        /// </summary>
        public int ImagePointX { get; set; }
        public int ImagePointY { get; set; }
    }
}
