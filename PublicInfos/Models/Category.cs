using System;
using System.Collections.Generic;
using SqlSugar;

namespace PublicInfos
{
    /// <summary>
    /// 描述抽卡项目类别的类
    /// 概率统一, 项目自行设置Up与否
    /// </summary>
    [SugarTable("Category")]
    public class Category
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 是否为保底项目
        /// </summary>
        public bool IsBaodi { get; set; } = false;

        [SugarColumn(ColumnDataType = "Text", IsJson = true)]
        public List<int> Content { get; set; } = new List<int>();
        public double Probablity { get; set; } = 0;

        [SugarColumn(ColumnDataType = "Text", IsJson = true)]
        public List<int> UpContent { get; set; } = new List<int>();
        public DateTime CreateDt { get; set; } = DateTime.Now;
        public DateTime UpdateDt { get; set; }
        public string GUID { get; set; } = "";
        public Category Clone()
        {
            var o = (Category)this.MemberwiseClone();
            o.Content = new List<int>();
            this.Content.ForEach(x => o.Content.Add(x));
            return o;
        }
    }
}
