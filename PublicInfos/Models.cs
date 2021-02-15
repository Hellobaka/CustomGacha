﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Native.Sdk.Cqp.EventArgs;
using SqlSugar;

namespace PublicInfos
{
    public interface IOrderModel
    {
        string GetOrderStr();
        bool Judge(string destStr);
        FunctionResult Progress(CQGroupMessageEventArgs e);
        FunctionResult Progress(CQPrivateMessageEventArgs e);
    }
    public class ItemDrawConfig
    {
        /// <summary>
        /// 核心图片绘制大小
        /// </summary>
        public Size ImageSize { get; set; }
        /// <summary>
        /// 背景图片绘制大小
        /// </summary>
        public Size BackgroundImageSize { get; set; }
        /// <summary>
        /// 背景与核心图片绘制顺序
        /// </summary>
        public DrawOrder DrawOrder { get; set; }
        /// <summary>
        /// 核心图片相对于背景的偏移坐标
        /// </summary>
        public Point ImagePoint { get; set; }
    }
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
    /// <summary>
    /// 池子内全局图片绘制设置，间隔不包括图片大小
    /// </summary>
    public class PoolDrawConfig
    {
        /// <summary>
        /// 绘制的起点坐标
        /// </summary>
        public Point StartPoint { get; set; }
        /// <summary>
        /// 每个项目之间的X坐标间隔
        /// </summary>
        public int DrawXInterval { get; set; }
        /// <summary>
        /// 每个项目之间的Y坐标间隔
        /// </summary>
        public int DrawYInterval { get; set; }
        /// <summary>
        /// 大于等于此值将会换行
        /// </summary>
        public int MaxX { get; set; }
        /// <summary>
        /// 换行之后Y坐标变化值，请不考虑图片大小
        /// </summary>
        public int YChangeValue { get; set; }
        /// <summary>
        /// 换行之后X坐标变化值
        /// </summary>
        public int XChangeValue { get; set; }
        /// <summary>
        /// 抽卡结束之后按价值的排序方式
        /// </summary>
        public OrderOptional OrderOptional { get; set; }
    }
    /// <summary>
    /// 排序方式
    /// </summary>
    public enum OrderOptional
    {
        /// <summary>
        /// 升序，从小到大
        /// </summary>
        Increasing,
        /// <summary>
        /// 降序，从大到小
        /// </summary>
        Descending,
        /// <summary>
        /// 不排序
        /// </summary>
        None
    }

    [SugarTable("User")]
    public class DB_User
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int RowID { get; set; }
        /// <summary>
        /// QQ号
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long QQID { get; set; }
        public long Money { get; set; } = 0;
        /// <summary>
        /// 抽卡总次数
        /// </summary>
        public int GachaTotalCount { get; set; } = 0;
        /// <summary>
        /// 总消耗代币数
        /// </summary>
        public int MoneyTotalCount { get; set; } = 0;
        /// <summary>
        /// 抽卡次数, 保底后重置
        /// </summary>
        public int GachaCount { get; set; } = 0;
        /// <summary>
        /// 总签到次数
        /// </summary>
        public int SignTotalCount { get; set; } = 0;
        /// <summary>
        /// 上次签到时间
        /// </summary>
        public DateTime LastSignTime { get; set; } = new DateTime(1970, 1, 1, 0, 0, 0);
    }
    [SugarTable("Repo")]
    public class DB_Repo
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public int ItemID { get; set; }
        /// <summary>
        /// 物品名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 物品数量
        /// </summary>
        public int ItemCount { get; set; }
        /// <summary>
        /// 物品获取时间
        /// </summary>
        public DateTime ItemGetTime { get; set; }
    }
    /// <summary>
    /// 描述卡池的模型类
    /// </summary>
    [SugarTable("Pool")]
    public class Pool
    {
        public int PoolID { get; set; }
        /// <summary>
        /// 卡池名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 卡池内容
        /// </summary>
        [SugarColumn(ColumnDataType ="blob", IsJson = true)]
        public List<GachaItem> Content { get; set; } = new List<GachaItem>();
        /// <summary>
        /// 配置卡池绘制配置
        /// </summary>
        [SugarColumn(ColumnDataType = "blob", IsJson = true)]
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
    /// <summary>
    /// 描述抽卡子项目的模型类
    /// </summary>
    [SugarTable("Items")]
    public class GachaItem
    {
        public int ItemID { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 项目概率
        /// </summary>
        public double Probablity { get; set; }
        /// <summary>
        /// 图片相对路径
        /// </summary>
        public string ImagePath { get; set; }
        /// <summary>
        /// 背景图片相对路径
        /// </summary>
        public string BackgroundImagePath { get; set; }
        /// <summary>
        /// 是否为保底项目
        /// </summary>
        public bool IsBaodi { get; set; }
        /// <summary>
        /// 表示当前项目的数量
        /// </summary>
        public long Count { get; set; }
        /// <summary>
        /// 最高数量
        /// </summary>
        public int CountCeil { get; set; }
        /// <summary>
        /// 最低数量
        /// </summary>
        public int CountFloor { get; set; }
        /// <summary>
        /// 是否能被折叠
        /// </summary>
        public bool CanBeFolded { get; set; }
        /// <summary>
        /// 卡片价值，影响后续排序
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// 图片与背景之间的绘制关系
        /// </summary>
        [SugarColumn(ColumnDataType = "blob", IsJson = true)]
        public ItemDrawConfig ImageConfig { get; set; }

        public GachaItem Clone()
        {
            return (GachaItem)this.MemberwiseClone();
        }
    }
    [SugarTable("Config")]
    public class Config 
    {
        [SugarColumn(IsNullable =false)]
        public DateTime SignResetTime { get; set; }
        [SugarColumn(IsNullable = false)]
        public int SignCeil { get; set; }
        [SugarColumn(IsNullable = false)]
        public int SignFloor { get; set; }
    }
    [SugarTable("OrderConfig")]
    public class OrderConfig
    {
        public string Register { get; set; }
        public string Sign { get; set; }
    }
}
