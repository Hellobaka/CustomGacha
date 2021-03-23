using System;
using System.Collections.Generic;
using System.Linq;
using me.cqp.luohuaming.CustomGacha.UI.Model;
using PublicInfos;
using MahApps.Metro.IconPacks;
using me.cqp.luohuaming.CustomGacha.UI.Command;
using me.cqp.luohuaming.CustomGacha.UI.View;

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
            SQLHelper.CreateDB();
            if (MainSave.PoolInstances == null)
                MainSave.PoolInstances = SQLHelper.GetAllPools();
            RecentList = new List<RecentSoluction>();
            ButtonGroup = new List<ButtonItem>
            {
                new ButtonItem
                {
                    Remark = "选择具有初始设置基架的项目模板以开始",
                    Title = "创建新项目",
                    ImageKind= PackIconUniconsKind.FilePlusAlt,
                    Action = new DelegateCommand
                    {
                         ExecuteAction = new Action<object> (openWithNewPool)
                    }
                },
                new ButtonItem
                {
                    Title = "敬请期待...",
                    Remark = "未来可期未来可期",
                    ImageKind= PackIconUniconsKind.EllipsisH
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
            OpenWithNoPool = new DelegateCommand
            {
                ExecuteAction = new Action<object>(openWithNoPool)
            };
        }
        #endregion
        #region ---绑定命令---
        public DelegateCommand OpenWithNoPool { get; set; }
        public void openWithNoPool(object o)
        {
            Workbench fm = new Workbench();
            WorkbenchViewModel vm = (fm.DataContext as WorkbenchViewModel);
            vm.DialogType = WorkbenchViewModel.OpenType.NoPool;
            vm.EditPool = new Pool { Name = "未选择项目" ,PoolID = -1 };
            vm.Config = MainSave.ApplicationConfig.Clone();
            vm.OrderConfig = MainSave.OrderConfig.Clone();
            fm.InitializeComponent();
            fm.Show(); 
            SoluctionSelector.SoluctionSelector_Export.Hide();
        }
        public void openWithNewPool(object o)
        {
            Workbench fm = new Workbench();
            WorkbenchViewModel vm = (fm.DataContext as WorkbenchViewModel);
            vm.DialogType = WorkbenchViewModel.OpenType.NewPool;
            vm.EditPool = new Pool { Name = "未选择项目" , PoolID=-1};
            vm.Config = MainSave.ApplicationConfig.Clone();
            vm.OrderConfig = MainSave.OrderConfig.Clone();
            fm.InitializeComponent();
            fm.Show();
            SoluctionSelector.SoluctionSelector_Export.Hide();
        }
        #endregion
    }
}