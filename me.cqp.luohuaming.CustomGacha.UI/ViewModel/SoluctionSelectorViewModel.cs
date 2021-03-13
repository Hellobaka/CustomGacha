using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using me.cqp.luohuaming.CustomGacha.UI.Model;
using PublicInfos;
using MahApps.Metro.IconPacks;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    public class SoluctionSelectorViewModel : NotifyicationObject
    {
        #region ---绑定属性---
        private List<RecentSoluction> recentList;
        public List<RecentSoluction> RecentList
        {
            get { return recentList; }
            set
            {
                recentList = value;
                this.RaisePropertyChanged("RecentList");
            }
        }
        private List<ButtonItem> buttonGroup;
        public List<ButtonItem> ButtonGroup
        {
            get { return buttonGroup; }
            set
            {
                buttonGroup = value;
                this.RaisePropertyChanged("ButtonGroup");
            }
        }
        #endregion
        #region ---构造函数---
        public SoluctionSelectorViewModel()
        {
            if (MainSave.PoolInstances == null)
                MainSave.PoolInstances = SQLHelper.GetAllPools();
            RecentList = new List<RecentSoluction>();
            ButtonGroup = new List<ButtonItem> 
            {
                new ButtonItem
                {
                    Remark = "选择具有初始设置基架的项目模板以开始",
                    Title = "创建新项目",
                    ImageKind= PackIconUniconsKind.FilePlusAlt
                },
                new ButtonItem
                {
                    Title = "打开项目",
                    Remark = "打开本地项目",
                    ImageKind= PackIconUniconsKind.Books
                }
            };
            MainSave.PoolInstances.ForEach(x =>
            {
                RecentList.Add(new RecentSoluction
                {
                    Date = x.UpdateDt.ToString("yyyy/MM/dd HH:mm"),
                    Name = x.Name,
                    Path = x.RelativePath,
                    Object = x
                });
            });
        }
        #endregion
        #region ---绑定命令---

        #endregion
    }
}
