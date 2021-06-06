using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using HandyControl.Tools.Extension;
using me.cqp.luohuaming.CustomGacha.UI.Command;
using me.cqp.luohuaming.CustomGacha.UI.Model;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    public class ManagePoolsModelView : NotifyicationObject
    {
        private ObservableCollection<RecentSoluction> recentList=new ObservableCollection<RecentSoluction>();
        public ObservableCollection<RecentSoluction> RecentList
        {
            get { return recentList; }
            set
            {
                recentList = value;
                this.RaisePropertyChanged("RecentList");
            }
        }
        private RecentSoluction selectSoluction;
        public RecentSoluction SelectSoluction
        {
            get { return selectSoluction; }
            set
            {
                selectSoluction = value;
                this.RaisePropertyChanged("SelectSoluction");
            }
        }
        private int selectNum;
        public int SelectNum
        {
            get { return selectNum; }
            set
            {
                selectNum = value;
                this.RaisePropertyChanged("SelectNum");
            }
        }

        public void selectAll(object o)
        {
            RecentList.Do(x => x.Checked = true);
            ReloadList();
        }
        public void selectNon(object o)
        {
            RecentList.Do(x => x.Checked = false);
            ReloadList();
        }
        public void selectAnit(object o)
        {
            RecentList.Do(x => x.Checked = !x.Checked);
            ReloadList();
        }
        public void setUnVisable(object o)
        {
            RecentList.Where(x => x.Checked).Do(i => { i.Object.Visable = false; SQLHelper.UpdatePool(i.Object); });
            ReloadList();
            Helper.ShowGrowlMsg("操作完成");
        }
        public void setVisable(object o)
        {
            RecentList.Where(x => x.Checked).Do(i => { i.Object.Visable = true; SQLHelper.UpdatePool(i.Object); });
            ReloadList();
            Helper.ShowGrowlMsg("操作完成");
        }
        public void delectSelected(object o)
        {
            if (HandyControl.Controls.MessageBox.Ask("确认删除所选的项目吗？此操作将会从数据库中删除他们的目录，但不会影响卡池内容", "提示") == MessageBoxResult.Cancel)
                return;
            RecentList.Where(x => x.Checked).Do(i =>
            {
                SQLHelper.RemoveCategoryByIDs(i.Object.Content);
                SQLHelper.RemovePool(i.Object);
                RecentList.Remove(i);
            });
            ReloadList();
            Helper.ShowGrowlMsg("操作完成");
        }
        public void ReloadList()
        {
            var c = new ObservableCollection<RecentSoluction>();
            RecentList.Do(x => c.Add(x));
            RecentList = c;
            SelectNum = RecentList.Where(x => x.Checked).Count();
            RaisePropertyChanged("RecentList");
        }
        public DelegateCommand SelectAll => new Lazy<DelegateCommand>(() =>
                  new DelegateCommand(selectAll)).Value;
        public DelegateCommand SelectNon => new Lazy<DelegateCommand>(() =>
                  new DelegateCommand(selectNon)).Value;
        public DelegateCommand SelectAnti => new Lazy<DelegateCommand>(() =>
                  new DelegateCommand(selectAnit)).Value;
        public DelegateCommand SetUnVisable => new Lazy<DelegateCommand>(() =>
                  new DelegateCommand(setUnVisable)).Value;
        public DelegateCommand SetVisable => new Lazy<DelegateCommand>(() =>
                  new DelegateCommand(setVisable)).Value;
        public DelegateCommand DeleteSelected => new Lazy<DelegateCommand>(() =>
                  new DelegateCommand(delectSelected)).Value;
        public ManagePoolsModelView()
        {
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
    }
}
