using System.Collections.Generic;
using System.Drawing;
using PublicInfos;

namespace PluginInterface
{
    /// <summary>
    /// 自定义所有项目的绘制方式.
    /// 实现该接口后, 原本的自带默认（坐标运算）绘制将失效
    /// </summary>
    public interface IDrawAllItems
    {
        /// <summary>
        /// 自定义所有子项目的绘制
        /// </summary>
        /// <param name="allItemImages">所有子项目图片</param>
        /// <param name="gachaItems">图片对应的项目</param>
        /// <param name="drawPoints">绘制的坐标</param>
        /// <param name="backgroundImg">背景图片</param>
        /// <param name="pool">目标池</param>
        /// <returns>自定义绘制图片</returns>
        Bitmap DrawAllItems(List<Image> allItemImages, List<GachaItem> gachaItems, Point[] drawPoints, Bitmap backgroundImg, Pool pool);
    }
}