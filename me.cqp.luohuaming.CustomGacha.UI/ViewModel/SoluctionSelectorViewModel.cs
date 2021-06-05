using System;
using System.Collections.Generic;
using System.Linq;
using me.cqp.luohuaming.CustomGacha.UI.Model;
using PublicInfos;
using MahApps.Metro.IconPacks;
using me.cqp.luohuaming.CustomGacha.UI.Command;
using me.cqp.luohuaming.CustomGacha.UI.View;
using System.Collections.ObjectModel;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    public class SoluctionSelectorViewModel : NotifyicationObject
    {
        #region ---绑定属性---
        private ObservableCollection<RecentSoluction> recentList;
        public ObservableCollection<RecentSoluction> RecentList
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
        public DelegateCommand HidePool { get; set; }
        public void hidePool(object o)
        {

        }
        public DelegateCommand DeletePool { get; set; }
        public void deletePool(object o)
        {

        }

        #endregion
        #region ---构造函数---
        public SoluctionSelectorViewModel()
        {
            SQLHelper.CreateDB();
            MainSave.PoolInstances = SQLHelper.GetAllPools();
            MainSave.PoolInstances = MainSave.PoolInstances.OrderBy(x => x.UpdateDt).ToList();
            RecentList = new ObservableCollection<RecentSoluction>();
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
                    Title = "复制模板",
                    Remark = "从现有的卡池继承所有的设置，便于快速部署",
                    ImageKind= PackIconUniconsKind.Copy,
                    Action = new DelegateCommand
                    {
                        ExecuteAction = new Action<object>(copyPool)
                    }
                },
                new ButtonItem
                {
                    Title = "敬请期待...",
                    Remark = "未来可期未来可期",
                    ImageKind= PackIconUniconsKind.EllipsisH
                },
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
            HidePool = new DelegateCommand
            {
                ExecuteAction = new Action<object>(hidePool)
            };
            DeletePool = new DelegateCommand
            {
                ExecuteAction = new Action<object>(deletePool)
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
        public void copyPool(object o)
        {
            CopyPoolView fm = new CopyPoolView();
            ((CopyPoolModelView)fm.DataContext).RecentList = RecentList;
            fm.InitializeComponent();
            fm.Show();
        }
        public void ReloadList()
        {
            var c = new ObservableCollection<RecentSoluction>();
            foreach (var item in RecentList)
                c.Add(item);
            RecentList = c;
        }
        #endregion
    }
}