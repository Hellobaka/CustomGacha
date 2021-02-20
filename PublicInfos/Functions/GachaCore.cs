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
                if (gachaItem == null)
                    break;
                if (gachaItem.CanBeFolded)
                {
                    var tmp = results.Find(x => x.Name == gachaItem.Name);
                    if (tmp != null)
                    {
                        tmp.Count += gachaItem.Count;
                        gachaItem = null;
                        GC.Collect();
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
            double totalProp = 0, destProp = 0;
            pool.Content.ForEach(x => totalProp += x.Probablity);
            double randomNum = new Random(GetRandomSeed()).NextDouble() / 100 * totalProp;
            foreach (var item in pool.Content)
            {
                if (gachaCount == pool.BaodiCount)
                {
                    List<GachaItem> BaodiList = pool.Content.Where(x => x.IsBaodi).ToList();
                    totalProp = 0;
                    destProp = 0;
                    BaodiList.ForEach(x => totalProp += x.Probablity);
                    randomNum = new Random(GetRandomSeed()).NextDouble() / 100 * totalProp;
                    foreach (var baodiitem in BaodiList)
                    {
                        destProp += baodiitem.Probablity / 100;
                        if (randomNum < destProp)
                        {
                            gachaCount = 1;
                            return CalcGachaItemCount(baodiitem);
                        }
                    }
                }
                destProp += item.Probablity / 100;
                if (randomNum < destProp)
                {
                    gachaCount = item.IsBaodi ? 1 : ++gachaCount;
                    return CalcGachaItemCount(item);
                }
            }
            return null;
        }
        /// <summary>
        /// 进行随机数量的处理
        /// </summary>
        private static GachaItem CalcGachaItemCount(GachaItem baodiitem)
        {
            GachaItem gachaItem = baodiitem.Clone();
            gachaItem.Count = new Random(GetRandomSeed()).Next(gachaItem.CountFloor, gachaItem.CountCeil + 1);
            return gachaItem;
        }
        /// <summary>
        /// 使用RNGCryptoServiceProvider生成种子
        /// </summary>
        /// <returns>按此格式new Random(GetRandomSeed())</returns>
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
            string backgroundImagePath = Path.Combine(pool.RelativePath, pool.BackgroundImagePath);
            if (!File.Exists(backgroundImagePath))
                throw new FileNotFoundException($"池子的背景图片文件不存在，在池{pool.Name}中 路径{backgroundImagePath}");
            Image background = Image.FromFile(backgroundImagePath);
            Point DrawPoint = new Point(pool.PoolDrawConfig.StartPointX, pool.PoolDrawConfig.StartPointY);
            using (Graphics g = Graphics.FromImage(background))
            {
                foreach (var item in gachaItems)
                {
                    Image itemImage = GetGachaItemImage(item, pool.RelativePath);
                    g.DrawImage(itemImage, new Rectangle(DrawPoint, itemImage.Size));
                    if (DrawPoint.X >= pool.PoolDrawConfig.MaxX)
                    {
                        DrawPoint.X = pool.PoolDrawConfig.XChangeValue + pool.PoolDrawConfig.StartPointX;
                        DrawPoint.Y = pool.PoolDrawConfig.YChangeValue + pool.PoolDrawConfig.StartPointY;
                    }
                    else
                    {
                        DrawPoint.X += pool.PoolDrawConfig.DrawXInterval + itemImage.Width;
                        DrawPoint.Y += pool.PoolDrawConfig.DrawYInterval;
                    }
                }
            }
            return background;
        }
        /// <summary>
        /// 生成GachaItem的图片
        /// </summary>
        /// <param name="item">需要生成的GachaItem</param>
        /// <param name="relativePath">池子的相对路径</param>
        private static Image GetGachaItemImage(GachaItem item, string relativePath)
        {
            string bkImagePath = Path.Combine(relativePath, item.BackgroundImagePath);
            string ImagePath = Path.Combine(relativePath, item.ImagePath);

            if (!File.Exists(ImagePath))
                throw new FileNotFoundException($"卡片的图片文件不存在，在卡 {item.Name} 中 路径{ImagePath}");
            if (!File.Exists(bkImagePath))
                throw new FileNotFoundException($"卡片的背景图片文件不存在，在卡 {item.Name} 中 路径{ImagePath}");
            Image DestImage = Image.FromFile(ImagePath);
            Point DrawPoint = new Point(item.ImageConfig.ImagePointX, item.ImageConfig.ImagePointY);
            Size destSize = new Size(item.ImageConfig.ImageWidth, item.ImageConfig.ImageHeight);
            Size backGroundReSizeSize = new Size(item.ImageConfig.BackgroundImageWidth, item.ImageConfig.BackgroundImageHeight);

            if (!string.IsNullOrEmpty(item.BackgroundImagePath))
            {
                Image background = Image.FromFile(bkImagePath);
                Bitmap backgroundResize = new Bitmap(background, backGroundReSizeSize);
                switch (item.ImageConfig.DrawOrder)
                {
                    case DrawOrder.ImageAboveBackground:
                        using (Graphics g = Graphics.FromImage(backgroundResize))
                        {
                            g.DrawImage(DestImage, new Rectangle(DrawPoint, destSize));
                        }
                        break;
                    case DrawOrder.ImageBelowBackground:
                        Bitmap emptyBitmap = new Bitmap(item.ImageConfig.BackgroundImageWidth, item.ImageConfig.BackgroundImageHeight);
                        using (Graphics g = Graphics.FromImage(emptyBitmap))
                        {
                            emptyBitmap.Save("tmp.png");
                            g.DrawImage(DestImage, new Rectangle(DrawPoint, destSize));
                            emptyBitmap.Save("tmp.png");
                            g.DrawImage(backgroundResize, new Point(0, 0));
                            emptyBitmap.Save("tmp.png");
                            backgroundResize = emptyBitmap;
                        }
                        break;
                }
                return backgroundResize;
            }
            else
            {
                Bitmap destBitmap = new Bitmap(DestImage, new Size(item.ImageConfig.ImageWidth, item.ImageConfig.ImageHeight));
                DestImage.Dispose();
                return destBitmap;
            }
        }
    }
}
