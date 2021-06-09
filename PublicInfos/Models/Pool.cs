using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SqlSugar;

namespace PublicInfos
{
    /// <summary>
    /// 描述卡池的模型类
    /// </summary>
    [SugarTable("Pool")]
    public class Pool
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int PoolID { get; set; }
        /// <summary>
        /// 卡池名称
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// 单抽指令
        /// </summary>
        public string SingalGachaOrder { get; set; } = "#单抽指令";
        /// <summary>
        /// 多抽指令
        /// </summary>
        public string MultiOrder { get; set; } = "#多抽指令";
        /// <summary>
        /// 多抽抽取次数
        /// </summary>
        public int MultiGachaNumber { get; set; } = 10;
        /// <summary>
        /// 卡池内容
        /// </summary>
        [SugarColumn(ColumnDataType = "Text", IsJson = true)]
        public List<int> Content { get; set; } = new List<int>();
        /// <summary>
        /// 配置卡池绘制配置
        /// </summary>
        [SugarColumn(ColumnDataType = "Text", IsJson = true)]
        public PoolDrawConfig PoolDrawConfig { get; set; } = new PoolDrawConfig();
        /// <summary>
        /// 保底所需要的次数
        /// </summary>
        public int BaodiCount { get; set; } = 10;
        /// <summary>
        /// 卡池背景图片相对路径
        /// </summary>
        public string BackgroundImagePath { get; set; } = "";
        /// <summary>
        /// 自动设置相对路径
        /// </summary>
        public string RelativePath { get; set; } = "";
        /// <summary>
        /// 插件路径
        /// </summary>
        public string PluginPath { get; set; } = "";
        /// <summary>
        /// New 图片相对路径
        /// </summary>
        public string NewPicPath { get; set; } = "";
        /// <summary>
        /// New 图片绘制宽度
        /// </summary>
        public int NewPicWidth { get; set; } = 0;
        /// <summary>
        /// New 图片绘制高度
        /// </summary>
        public int NewPicHeight { get; set; } = 0;
        /// <summary>
        /// New 图片绘制坐标 X
        /// </summary>
        public int NewPicX { get; set; } = 0;
        /// <summary>
        /// New 图片绘制坐标 Y
        /// </summary>
        public int NewPicY { get; set; } = 0;
        public string SingalGachaText { get; set; } = "";
        public string MultiGachaText { get; set; } = "";
        public string Remark { get; set; } = "";
        /// <summary>
        /// 图片与背景之间的绘制关系
        /// </summary>
        [SugarColumn(ColumnDataType = "Text", IsJson = true)]
        public ItemDrawConfig ImageConfig { get; set; } = new ItemDrawConfig();
        public DateTime CreateDt { get; set; } = DateTime.Now;
        public DateTime UpdateDt { get; set; }
        /// <summary>
        /// 插件_绘制核心图片
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public object DrawMainImage { get; set; }
        /// <summary>
        /// 插件_绘制抽卡子项目
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public object DrawItem { get; set; }

        /// <summary>
        /// 插件_获取绘制坐标
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public object DrawPoints { get; set; }
        /// <summary>
        /// 插件_最终绘制
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public object FinallyDraw { get; set; }
        /// <summary>
        /// 插件_绘制全部项目
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public object DrawAllItems { get; set; }
        /// <summary>
        /// 插件_处理消息
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<IPluginOrderModel> PluginMessageHandler { get; set; } = new List<IPluginOrderModel>();
        public string GUID { get; set; } = "";
        public bool Visable { get; set; } = true;

        private Assembly plugin = null;
        public void PluginInit()
        {
            PluginMessageHandler.Clear();
            if (string.IsNullOrWhiteSpace(PluginPath) is false)
            {
                string filePath = Path.Combine(RelativePath, PluginPath);
                if (File.Exists(filePath))
                {
                    byte[] fsContent;
                    using (FileStream fs = File.OpenRead(filePath))
                    {
                        fsContent = new byte[fs.Length];
                        fs.Read(fsContent, 0, fsContent.Length);
                    }
                    plugin = Assembly.Load(fsContent);
                    try
                    {
                        foreach (var item in plugin.GetTypes())
                        {
                            if (item.GetInterface("IDrawMainImage") != null)
                            {
                                DrawMainImage = plugin.CreateInstance(item.FullName);
                            }

                            if (item.GetInterface("IDrawItem") != null)
                            {
                                DrawItem = plugin.CreateInstance(item.FullName);
                            }

                            if (item.GetInterface("IDrawPoints") != null)
                            {
                                DrawPoints = plugin.CreateInstance(item.FullName);
                            }

                            if (item.GetInterface("IFinallyDraw") != null)
                            {
                                FinallyDraw = plugin.CreateInstance(item.FullName);
                            }

                            if (item.GetInterface("DrawAllItems") != null)
                            {
                                DrawAllItems = plugin.CreateInstance(item.FullName);
                            }                            
                        }
                        foreach (var inter in plugin.GetTypes())
                        {
                            if (inter.IsInterface)
                                continue;
                            foreach (var instance in inter.GetInterfaces())
                            {
                                if (instance == typeof(IPluginOrderModel))
                                {
                                    IPluginOrderModel obj = (IPluginOrderModel)Activator.CreateInstance(inter);
                                    if (obj.ImplementFlag == false)
                                        continue;
                                    PluginMessageHandler.Add(obj);
                                }
                            }
                        }
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        throw new FileLoadException($"{e.LoaderExceptions[0].Message}\n{e.StackTrace}");
                    }
                }
                else
                {
                    //TODO: 补全错误抛出
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
        public Pool Clone()
        {
            return (Pool)this.MemberwiseClone();
        }

    }
}
