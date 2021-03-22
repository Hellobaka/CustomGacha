using System.Drawing;
using PublicInfos;

namespace PluginInterface
{
    /// <summary>
    /// 自定义绘制子图片.
    /// 实现该接口后, 原本的自带默认绘制将失效
    /// </summary>
    public interface IDrawItem
    {
        /// <summary>
        /// 自定义重新绘制结果子图片
        /// </summary>
        /// <param name="item">描述需要绘制图片的类</param>
        /// <param name="pool">目标池</param>
        /// <returns>自定义绘制图片</returns>
        Bitmap DrawPicItem(GachaItem item, Pool pool);
    }
}
