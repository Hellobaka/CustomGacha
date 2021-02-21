﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Native.Sdk.Cqp.EventArgs;
using Newtonsoft.Json;
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
        [Category("核心图片绘制尺寸")]
        public int ImageWidth { get; set; }
        [Category("核心图片绘制尺寸")]
        public int ImageHeight { get; set; }
        /// <summary>
        /// 背景图片绘制大小
        /// </summary>
        [Category("背景图片绘制尺寸")]
        public int BackgroundImageWidth { get; set; }
        [Category("背景图片绘制尺寸")]
        public int BackgroundImageHeight { get; set; }
        /// <summary>
        /// 背景与核心图片绘制顺序
        /// </summary>
        [Category("背景与核心图片绘制顺序")]
        public DrawOrder DrawOrder { get; set; }
        /// <summary>
        /// 核心图片相对于背景的偏移坐标
        /// </summary>
        [Category("核心图片相对于背景的偏移")]
        public int ImagePointX { get; set; }
        [Category("核心图片相对于背景的偏移")]
        public int ImagePointY { get; set; }
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
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        /// <summary>
        /// 绘制的起点坐标
        /// </summary>
        [Category("起始坐标")]
        public int StartPointX { get; set; }
        [Category("起始坐标")]
        public int StartPointY { get; set; }
        /// <summary>
        /// 每个项目之间的X坐标间隔
        /// </summary>
        [Category("绘制间隔")]
        public int DrawXInterval { get; set; }
        /// <summary>
        /// 每个项目之间的Y坐标间隔
        /// </summary>
        [Category("绘制间隔")]
        public int DrawYInterval { get; set; }
        /// <summary>
        /// 大于等于此值将会换行
        /// </summary>
        [Category("换行临界值")]
        public int MaxX { get; set; }
        /// <summary>
        /// 换行之后Y坐标变化值，请不考虑图片大小
        /// </summary>
        [Category("换行偏移")]
        public int YChangeValue { get; set; }
        /// <summary>
        /// 换行之后X坐标变化值
        /// </summary>
        [Category("换行偏移")]
        public int XChangeValue { get; set; }
        /// <summary>
        /// 抽卡结束之后按价值的排序方式
        /// </summary>
        [Category("排序方式")]
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

    /// <summary>
    /// 描述用户信息
    /// </summary>
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
        /// <summary>
        /// 现有的货币数
        /// </summary>
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"QQ号：{QQID}");
            sb.AppendLine($"货币数：{Money}");
            sb.AppendLine($"总抽卡次数：{GachaTotalCount}");
            sb.AppendLine($"总消耗货币数：{MoneyTotalCount}");
            sb.AppendLine($"总签到次数：{SignTotalCount}");
            sb.AppendLine($"抽卡保底剩余次数：{GachaCount}");
            sb.AppendLine($"上次签到时间：{LastSignTime}");
            return sb.ToString();
        }
    }
    /// <summary>
    /// 描述仓库信息
    /// </summary>
    [SugarTable("Repo")]
    public class DB_Repo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int RowID { get; set; }
        public long QQID { get; set; }
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
        public long ItemCount { get; set; }
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
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        [Browsable(false)]
        public int PoolID { get; set; }
        /// <summary>
        /// 卡池名称
        /// </summary>
        [Category("0.基本属性")]
        public string Name { get; set; } = "";
        /// <summary>
        /// 单抽指令
        /// </summary>
        [Category("1.指令")]
        public string SingalGachaOrder { get; set; } = "#单抽指令";
        /// <summary>
        /// 多抽指令
        /// </summary>
        [Category("1.指令")]
        public string MultiOrder { get; set; } = "#多抽指令";
        /// <summary>
        /// 多抽抽取次数
        /// </summary>
        [Category("2.数值")]
        public int MultiGachaNumber { get; set; } = 10;
        /// <summary>
        /// 卡池内容
        /// </summary>
        [SugarColumn(ColumnDataType = "Text", IsJson = true)]
        [Browsable(false)]
        public List<int> Content { get; set; } = new List<int>();
        /// <summary>
        /// 配置卡池绘制配置
        /// </summary>
        [SugarColumn(ColumnDataType = "Text", IsJson = true)]
        [Category("5.绘制配置")]
        public PoolDrawConfig PoolDrawConfig { get; set; } = new PoolDrawConfig();
        /// <summary>
        /// 保底所需要的次数
        /// </summary>
        [Category("2.数值")]
        public int BaodiCount { get; set; } = 10;
        /// <summary>
        /// 卡池背景图片相对路径
        /// </summary>
        [Category("3.路径")]
        public string BackgroundImagePath { get; set; } = "";
        /// <summary>
        /// 自动设置相对路径
        /// </summary>
        [Category("3.路径")]
        public string RelativePath { get; set; } = "";
        /// <summary>
        /// New 图片相对路径
        /// </summary>
        [Category("4.New图片选项")]
        public string NewPicPath { get; set; } = "";
        /// <summary>
        /// New 图片绘制宽度
        /// </summary>
        [Category("4.New图片选项")]
        public int NewPicWidth { get; set; } = 0;
        /// <summary>
        /// New 图片绘制高度
        /// </summary>
        [Category("4.New图片选项")]
        public int NewPicHeight { get; set; } = 0;
        /// <summary>
        /// New 图片绘制坐标 X
        /// </summary>
        [Category("4.New图片选项")]
        public int NewPicX { get; set; } = 0;
        /// <summary>
        /// New 图片绘制坐标 Y
        /// </summary>
        [Category("4.New图片选项")]
        public int NewPicY { get; set; } = 0;
        /// <summary>
        /// 图片与背景之间的绘制关系
        /// </summary>
        [SugarColumn(ColumnDataType = "Text", IsJson = true)]
        [Category("6.卡片绘制设置")] 
        public ItemDrawConfig ImageConfig { get; set; } = new ItemDrawConfig();

        public override string ToString()
        {
            return Name;
        }
    }
    /// <summary>
    /// 描述抽卡子项目的模型类
    /// </summary>
    [SugarTable("Items")]
    public class GachaItem
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        [Browsable(false)]
        public int ItemID { get; set; } = 0;
        /// <summary>
        /// 项目名称
        /// </summary>
        [Category("基本属性")]
        public string Name { get; set; }
        /// <summary>
        /// 项目概率
        /// </summary>
        [Category("数值")]
        public double Probablity { get; set; } = 0;
        /// <summary>
        /// 图片相对路径
        /// </summary>
        [Category("路径")]
        public string ImagePath { get; set; } = "";
        /// <summary>
        /// 背景图片相对路径
        /// </summary>
        [Category("路径")]
        public string BackgroundImagePath { get; set; } = "";
        /// <summary>
        /// 是否为保底项目
        /// </summary>
        [Category("数值")]
        public bool IsBaodi { get; set; } = false;
        /// <summary>
        /// 表示当前项目的数量
        /// </summary>
        [Category("数值")]
        [Browsable(false)]
        public long Count { get; set; } = 0;
        /// <summary>
        /// 最高数量
        /// </summary>
        [Category("数值")]
        public int CountCeil { get; set; } = 1;
        /// <summary>
        /// 最低数量
        /// </summary>
        [Category("数值")]
        public int CountFloor { get; set; } = 1;
        /// <summary>
        /// 是否能被折叠
        /// </summary>
        [Category("数值")]
        public bool CanBeFolded { get; set; } = false;
        /// <summary>
        /// 卡片价值，影响后续排序
        /// </summary>
        [Category("数值")]
        public int Value { get; set; } = 0;

        public GachaItem Clone()
        {
            return (GachaItem)this.MemberwiseClone();
        }
    }
    /// <summary>
    /// 全局设置
    /// </summary>
    [SugarTable("Config")]
    public class Config
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int RowID { get; set; }
        /// <summary>
        /// 签到重置时间点
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public DateTime SignResetTime { get; set; }
        /// <summary>
        /// 签到获得的货币上限
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int SignCeil { get; set; }
        /// <summary>
        /// 签到获得的货币下限
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int SignFloor { get; set; }
    }
    /// <summary>
    /// 描述抽卡项目类别的类
    /// 概率统一, 项目自行设置Up与否
    /// </summary>
    [SugarTable("Category")]
    public class Category
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        [Browsable(false)]
        public int ID { get; set; }
        public string Name { get; set; }
        public double NormalProbablity { get; set; }
        public double UpProbablity { get; set; }
        public List<int> Content { get; set; }
        public Category Clone()
        {
            return (Category)this.MemberwiseClone();
        }
    }

    /// <summary>
    /// 注册与签到 指令文本设置
    /// </summary>
    [SugarTable("OrderConfig")]
    public class OrderConfig
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int RowID { get; set; }
        /// <summary>
        /// 注册指令
        /// </summary>
        public string Register { get; set; }
        /// <summary>
        /// 签到指令
        /// </summary>
        public string Sign { get; set; }
    }
}
