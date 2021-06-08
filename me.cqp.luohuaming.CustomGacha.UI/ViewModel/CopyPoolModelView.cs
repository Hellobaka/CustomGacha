using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HandyControl.Controls;
using me.cqp.luohuaming.CustomGacha.UI.Command;
using me.cqp.luohuaming.CustomGacha.UI.Model;
using me.cqp.luohuaming.CustomGacha.UI.View;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    public class CopyPoolModelView : NotifyicationObject
    {
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

        private RecentSoluction selectSoluction;
        public RecentSoluction SelectSoluction
        {
            get { return selectSoluction; }
            set
            {
                selectSoluction = value;
                CopyName = value.Name + "_2";
                this.RaisePropertyChanged("SelectSoluction");
            }
        }
        private bool useBaseDraw = true;
        public bool UseBaseDraw
        {
            get { return useBaseDraw; }
            set { useBaseDraw = value; this.RaisePropertyChanged("UseBaseDraw"); }
        }
        private bool useCategories = true;
        public bool UseCategories
        {
            get { return useCategories; }
            set { useCategories = value; this.RaisePropertyChanged("UseCategories"); }
        }
        private bool useGachaItems = false;
        public bool UseGachaItems
        {
            get { return useGachaItems; }
            set { useGachaItems = value; this.RaisePropertyChanged("UseGachaItems"); }
        }
        private string copyName;
        public string CopyName
        {
            get { return copyName; }
            set { copyName = value; this.RaisePropertyChanged("CopyName"); }
        }

        public DelegateCommand DoCopy { get; set; }
        private void doCopy(object o)
        {
            try
            {
                Pool selectPool = SelectSoluction.Object.Clone();
                selectPool.PoolID = -1;
                selectPool.Name = CopyName;
                selectPool.CreateDt = DateTime.Now;
                selectPool.UpdateDt = DateTime.Now;
                selectPool.GUID = Guid.NewGuid().ToString();
                if (useBaseDraw is false)
                {
                    selectPool.PoolDrawConfig = new PoolDrawConfig();
                    selectPool.BackgroundImagePath = "";
                    selectPool.RelativePath = "";
                    selectPool.PluginPath = "";
                    selectPool.NewPicHeight = 0;
                    selectPool.NewPicPath = "";
                    selectPool.NewPicWidth = 0;
                    selectPool.NewPicX = 0;
                    selectPool.NewPicY = 0;
                    selectPool.ImageConfig = new ItemDrawConfig();
                }
                if (useCategories is false)
                {
                    selectPool.Content = new List<int>();
                }
                else
                {
                    var c = SQLHelper.GetCategoriesByIDs(selectPool.Content);
                    if (useGachaItems is false)
                    {
                        c.ForEach(x => { x.Content = new List<int>(); x.UpContent = new List<int>(); });
                    }
                    c.ForEach(x =>
                    {
                        x.ID = -1;
                        x.CreateDt = DateTime.Now;
                        x.UpdateDt = DateTime.Now;
                        x.ID = SQLHelper.UpdateOrAddCategory(x, true);
                        x.GUID = Guid.NewGuid().ToString();
                    });
                    selectPool.Content = c.Select(x => x.ID).ToList();
                }
                selectPool.PoolID = SQLHelper.AddPool(selectPool);
                RecentList.Add(new RecentSoluction
                {
                    Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                    Name = selectPool.Name,
                    Object = selectPool,
                    Path = selectPool.RelativePath
                });
                (SoluctionSelector.SoluctionSelector_Export.DataContext as SoluctionSelectorViewModel).RecentList = RecentList;
                HandyControl.Controls.MessageBox.Show($"卡池已复制成功，请返回选择器以编辑项目", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                CopyPoolView.fm.Close();
            }
            catch (Exception e)
            {
                HandyControl.Controls.MessageBox.Show($"出现异常: {e.Message}\n{e.StackTrace}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public CopyPoolModelView()
        {
            DoCopy = new DelegateCommand
            {
                ExecuteAction = new Action<object>(doCopy)
            };
        }
    }
}
