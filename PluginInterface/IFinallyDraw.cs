using System.Collections.Generic;
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
        /// <param name="drawPoints">绘制的坐标</param>
        /// <param name="gachaItems">图片对应的项目</param>
        /// <param name="user">调用者QQ</param>
        /// <param name="pool">目标池</param>
        /// <returns>自定义绘制图片</returns>
        Bitmap FinallyDraw(Bitmap finPic, Point[] drawPoints, List<GachaItem> gachaItems, DB_User user, Pool pool);
    }
}