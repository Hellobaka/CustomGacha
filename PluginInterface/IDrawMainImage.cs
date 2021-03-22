using System.Drawing;
using PublicInfos;

namespace PluginInterface
{
    /// <summary>
    /// 自定义绘制子元素中心图片.
    /// 实现该接口后, 原本的自带默认绘制将失效
    /// </summary>
    public interface IDrawMainImage
    {
        /// <summary>
        /// 重新绘制抽卡子元素中的中心图片
        /// </summary>
        /// <param name="mainImage">未经处理的中心图片</param>
        /// <param name="pool">目标池</param>
        /// <returns>处理后的图片</returns>
        Bitmap RedrawMainImage(Bitmap mainImage, Pool pool);
    }
}
