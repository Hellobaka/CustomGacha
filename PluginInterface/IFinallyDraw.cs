using System.Drawing;
using PublicInfos;

namespace PluginInterface
{
    /// <summary>
    /// 在所有基础绘制结束后，自定义对图片处理.
    /// </summary>
    public interface IFinallyDraw
    {
        /// <summary>
        /// 自定义对原图片重新绘制
        /// </summary>
        /// <param name="finPic">最终处理的图片</param>
        /// <param name="QQ">调用者QQ</param>
        /// <param name="pool">目标池</param>
        /// <returns>自定义绘制图片</returns>
        Bitmap FinallyDraw(Bitmap finPic, long QQ, Pool pool);
    }
}