using System.Drawing;
using PublicInfos;

namespace PluginInterface
{
    /// <summary>
    /// 自定义获取绘制坐标.
    /// 实现该接口后, 原本的自带默认坐标设置将失效
    /// </summary>
    public interface IDrawPoints
    {
        /// <summary>
        /// 获取绘制的坐标
        /// </summary>
        /// <returns>坐标数组</returns>
        Point[] GetDrawPoints(Pool pool,int count = 10);
    }
}
