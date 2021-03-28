using System;
using System.Collections.Generic;
using System.IO;
using CustomGacha.SDK.Sdk.Cqp;

namespace PublicInfos
{
    public static class MainSave
    {
        /// <summary>
        /// 保存各种事件的数组
        /// </summary>
        public static List<IOrderModel> Instances { get; set; } = new List<IOrderModel>();
        public static CQLog CQLog { get; set; }
        public static CQApi CQApi { get; set; }
        //TODO: 发布时抹去此处
        public static string AppDirectory { get; set; } = @"E:\酷Q机器人插件开发\学习\OPQBot-Native\Launcher\bin\x86\Debug\data\app\me.cqp.luohuaming.CustomGacha";
        public static string ImageDirectory { get; set; } = @"E:\酷Q机器人插件开发\学习\OPQBot-Native\Launcher\bin\x86\Debug\data\image";
        public static string DBPath { get; set; } = @"E:\酷Q机器人插件开发\学习\OPQBot-Native\Launcher\bin\x86\Debug\data\app\me.cqp.luohuaming.CustomGacha\data.db";
        public static string GachaResultRootPath { get; set; } = @"E:\酷Q机器人插件开发\学习\OPQBot-Native\Launcher\bin\x86\Debug\data\image\CustomGacha";
        public static OrderConfig OrderConfig { get; set; }
        public static Config ApplicationConfig { get; set; }
        public static List<Pool> PoolInstances { get; set; }
    }
}
