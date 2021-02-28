using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace PublicInfos
{
    public static class GachaCore
    {
        public static List<GachaItem> DoGacha(Pool pool, int count, int gachaCount = 1)
        {
            List<GachaItem> results = new List<GachaItem>();
            for (int i = 0; i < count; i++)
            {
                GachaItem gachaItem = GetGachaItem(pool, gachaCount);
                gachaItem = CalcGachaItemCount(gachaItem);
                gachaCount = (gachaCount == pool.BaodiCount) ? 1 : ++gachaCount;
                if (gachaItem == null)
                    break;
                if (gachaItem.CanBeFolded)
                {
                    var tmp = results.Find(x => x.Name == gachaItem.Name);
                    if (tmp != null)
                    {
                        tmp.Count += gachaItem.Count;
                        continue;
                    }
                }
                results.Add(gachaItem);
            }
            switch (pool.PoolDrawConfig.OrderOptional)
            {
                case OrderOptional.Increasing:
                    results = results.OrderBy(x => x.Value).ToList(); break;
                case OrderOptional.Descending:
                    results = results.OrderByDescending(x => x.Value).ToList(); break;
                case OrderOptional.None:
                    break;
            }
            //TODO: 抽卡命令处写入数据库，保持模块功能单一
            //TODO: 根据结果更新用户剩余保底数
            //user.SignTotalCount += count;            
            //SQLHelper.UpdateUser(user);
            //SQLHelper.InsertGachaItem(results, user.QQID);
            return results;
        }
        /// <summary>
        /// 进行一次抽卡，全局保底次数
        /// </summary>
        /// <param name="pool">池</param>
        /// <returns>抽卡结果</returns>
        private static GachaItem GetGachaItem(Pool pool, int gachaCount = 1)
        {
            var categraies = SQLHelper.GetCategoriesByIDs(pool.Content);
            Category destCategory = RandomGetItem(categraies);
            List<GachaItem> content = SQLHelper.GetGachaItemsByIDs(destCategory.Content);
            content.ForEach(x =>
                    x.Probablity = destCategory.UpContent.Any(o => o == x.ItemID) ? x.UpProbablity : x.Probablity);
            if (gachaCount == pool.BaodiCount)
                return GetBaodiItem(categraies);
            else
                return RandomGetItem(content);
        }
        private static T RandomGetItem<T>(List<T> ls)
        {
            //原理: 先将所有概率加合, 之后以这个概率为上限, 取个随机数. 之后遍历所有项目, 将项目概率累计
            //       若累计概率总和小于先前取的随机数则未命中项目, 继续遍历. 直至累计概率总和大于等于目标随机数

            //totalProp => 物品概率总和
            //destProp => 当前已未命中的概率累计
            //randomNum => 目标随机数

            double totalProp = 0, destProp = 0;
            foreach (dynamic item in ls)
            {
                totalProp += item.Probablity;
            }
            double randomNum = new Random(GetRandomSeed()).NextDouble() / 100 * totalProp;
            foreach (dynamic item in ls)
            {
                destProp += item.Probablity / 100;
                if (randomNum <= destProp)
                {
                    return item;
                }
            }
            return default;
        }
        private static GachaItem GetBaodiItem(List<Category> ls)
        {
            var c = RandomGetItem(ls.Where(x => x.IsBaodi).ToList());
            if (c == null)//不存在保底项目，则随机返回一个
            {
                return RandomGetItem(SQLHelper.GetGachaItemsByIDs(RandomGetItem(ls).Content));
            }
            else
            {
                return RandomGetItem(SQLHelper.GetGachaItemsByIDs(c.Content));
            }
        }
        /// <summary>
        /// 进行随机数量的处理
        /// </summary>
        private static GachaItem CalcGachaItemCount(GachaItem targetItem)
        {
            GachaItem gachaItem = targetItem.Clone();
            gachaItem.Count = new Random(GetRandomSeed()).Next(gachaItem.CountFloor, gachaItem.CountCeil + 1);
            return gachaItem;
        }
        /// <summary>
        /// 使用RNGCryptoServiceProvider生成种子
        /// </summary>
        /// <returns>按此格式 new Random(GetRandomSeed()) 使用随机数种子</returns>
        private static int GetRandomSeed()
        {
            byte[] bytes = new byte[new Random().Next(0, 10000000)];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
        /// <summary>
        /// 绘制抽卡结果图片
        /// </summary>
        /// <param name="gachaItems">抽卡结果，请先排好序</param>
        /// <param name="pool">抽卡的池</param>
        public static Image DrawGachaResult(List<GachaItem> gachaItems, Pool pool)
        {
            //TODO: 坐标预先计算, 便于后期开放接口
            string backgroundImagePath = Path.Combine(pool.RelativePath, pool.BackgroundImagePath);
            if (!File.Exists(backgroundImagePath))
                throw new FileNotFoundException($"池子的背景图片文件不存在，在池{pool.Name}中 路径{backgroundImagePath}");
            Image background = Image.FromFile(backgroundImagePath);

            Point[] DrawPoints = new Point[pool.MultiGachaNumber];
            if (pool.DrawPoints != null)
            {
                var o = pool.DrawPoints;
                var method = o.GetType().GetMethod("GetDrawPoints");
                DrawPoints = (Point[])method.Invoke(o, new object[] { DrawPoints.Length });
            }
            else
            {
                int x = pool.PoolDrawConfig.StartPointX, y = pool.PoolDrawConfig.StartPointY;
                for (int i = 0; i < DrawPoints.Length; i++)
                {
                    DrawPoints[i] = new Point(x, y);
                    if (x >= pool.PoolDrawConfig.MaxX)
                    {
                        x = pool.PoolDrawConfig.XChangeValue + pool.PoolDrawConfig.StartPointX;
                        y = pool.PoolDrawConfig.YChangeValue + pool.PoolDrawConfig.StartPointY;
                    }
                    else
                    {
                        x += pool.PoolDrawConfig.DrawXInterval + pool.ImageConfig.BackgroundImageWidth;
                        y += pool.PoolDrawConfig.DrawYInterval;
                    }
                }
            }
            using (Graphics g = Graphics.FromImage(background))
            using (Bitmap newImage = (Bitmap)Image.FromFile(Path.Combine(pool.RelativePath, pool.NewPicPath)))
            {
                int index = 0;
                foreach (var item in gachaItems)
                {
                    Image itemImage = GetGachaItemImage(item, pool.RelativePath, pool.ImageConfig, pool.DrawMainImage, pool.DrawItem);
                    g.DrawImage(itemImage, new Rectangle(DrawPoints[index], itemImage.Size));
                    if(item.IsNew)
                    {
                        Point newPoint = new Point(DrawPoints[index].X + pool.NewPicX, DrawPoints[index].Y + pool.NewPicY);
                        g.DrawImage(newImage, new Rectangle(newPoint, new Size(pool.NewPicWidth, pool.NewPicHeight)));
                    }
                    index++;
                }
            }
            return background;
        }
        /// <summary>
        /// 生成GachaItem的图片
        /// </summary>
        /// <param name="item">需要生成的GachaItem</param>
        /// <param name="relativePath">池子的相对路径</param>
        private static Image GetGachaItemImage(GachaItem item, string relativePath, ItemDrawConfig ImageConfig, object P_DMI, object P_DI)
        {
            //不需要合成时请填写图片路径，忽略背景路径
            string bkImagePath = Path.Combine(relativePath, item.BackgroundImagePath);
            string ImagePath = Path.Combine(relativePath, item.ImagePath);
            bool nobackgroundFlag = false;
            if (string.IsNullOrWhiteSpace(bkImagePath))
                nobackgroundFlag = true;

            if (!File.Exists(ImagePath))
                throw new FileNotFoundException($"卡片的图片文件不存在，在卡 {item.Name} 中 路径{ImagePath}");
            if (!File.Exists(bkImagePath) && nobackgroundFlag is false)
                throw new FileNotFoundException($"卡片的背景图片文件不存在，在卡 {item.Name} 中 路径{ImagePath}");
            Image DestImage = Image.FromFile(ImagePath);
            if (P_DMI != null)
            {
                var method = P_DMI.GetType().GetMethod("RedrawMainImage");
                DestImage = (Bitmap)method.Invoke(P_DMI, new object[] { DestImage });
            }
            if (P_DI != null)
            {
                var method = P_DMI.GetType().GetMethod("DrawPicItem");
                return (Bitmap)method.Invoke(P_DMI, new object[] { item, relativePath, ImageConfig });
            }
            else
            {
                Point DrawPoint = new Point(ImageConfig.ImagePointX, ImageConfig.ImagePointY);
                Size destSize = new Size(ImageConfig.ImageWidth, ImageConfig.ImageHeight);
                Size backGroundReSizeSize = new Size(ImageConfig.BackgroundImageWidth, ImageConfig.BackgroundImageHeight);

                if (!string.IsNullOrWhiteSpace(item.BackgroundImagePath))
                {
                    Image background = Image.FromFile(bkImagePath);
                    Bitmap backgroundResize = new Bitmap(background, backGroundReSizeSize);
                    switch (ImageConfig.DrawOrder)
                    {
                        case DrawOrder.ImageAboveBackground:
                            using (Graphics g = Graphics.FromImage(backgroundResize))
                            {
                                g.DrawImage(DestImage, new Rectangle(DrawPoint, destSize));
                            }
                            break;
                        case DrawOrder.ImageBelowBackground:
                            Bitmap emptyBitmap = new Bitmap(ImageConfig.BackgroundImageWidth, ImageConfig.BackgroundImageHeight);
                            using (Graphics g = Graphics.FromImage(emptyBitmap))
                            {
                                g.DrawImage(DestImage, new Rectangle(DrawPoint, destSize));
                                g.DrawImage(backgroundResize, new Point(0, 0));
                                backgroundResize = emptyBitmap;
                            }
                            break;
                    }
                    return backgroundResize;
                }
                else
                {
                    Bitmap destBitmap = new Bitmap(DestImage, new Size(ImageConfig.ImageWidth, ImageConfig.ImageHeight));
                    DestImage.Dispose();
                    return destBitmap;
                }
            }
        }
    }
}
