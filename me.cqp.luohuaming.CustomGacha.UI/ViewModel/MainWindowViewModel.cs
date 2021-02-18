using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using me.cqp.luohuaming.CustomGacha.UI.Command;
using me.cqp.luohuaming.CustomGacha.UI.View;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    class MainWindowViewModel : NotifyicationObject
    {
        private ObservableCollection<Pool> pools;

        public ObservableCollection<Pool> Pools
        {
            get { return pools; }
            set
            {
                pools = value;
                this.RaisePropertyChanged("Pools");
            }
        }

        private ObservableCollection<GachaItem> gachaItems;
        public ObservableCollection<GachaItem> GachaItems
        {
            get { return gachaItems; }
            set
            {
                gachaItems = value;
                this.RaisePropertyChanged("GachaItems");
            }
        }

        private static Pool selectPool;

        public Pool SelectPool
        {
            get { return selectPool; }
            set
            {
                SQLHelper.UpdatePool(selectPool);
                selectPool = value;
                GachaItems.Clear();
                this.RaisePropertyChanged("SelectPool");
                if (value != null)
                    selectPool.Content.ForEach(x => GachaItems.Add(x));
            }
        }
        public static Pool GetSelectPool()
        {
            return selectPool;
        }
        private GachaItem selectGachaItem;

        public GachaItem SelectGachaItem
        {
            get { return selectGachaItem; }
            set
            {
                selectGachaItem = value;
                this.RaisePropertyChanged("SelectGachaItem");
            }
        }

        public MainWindowViewModel()
        {
            if (MainSave.PoolInstances == null)
                MainSave.PoolInstances = SQLHelper.GetAllPools();
            Pools = new ObservableCollection<Pool>();
            GachaItems = new ObservableCollection<GachaItem>();
            MainSave.PoolInstances.ForEach(x => Pools.Add(x));
            AddPool = new DelegateCommand
            {
                ExecuteAction = new Action<object>(addPool)
            };
            DeletePool = new DelegateCommand
            {
                ExecuteAction = new Action<object>(deletePool)
            };
            AddGachaItem = new DelegateCommand
            {
                ExecuteAction = new Action<object>(addGachaItem)
            };
            DeleteGachaItem = new DelegateCommand
            {
                ExecuteAction = new Action<object>(deleteGachaItem)
            };
            CopyGachaItem = new DelegateCommand
            {
                ExecuteAction = new Action<object>(copyGachaItem)
            };
            PoolDrawTest = new DelegateCommand
            {
                ExecuteAction = new Action<object>(poolDrawTest)
            };
            ShowInteractiveDialogCmd = new DelegateCommand
            {
                ExecuteAction = new Action<object>(ShowInteractiveDialog)
            };
        }
        public DelegateCommand AddPool { get; set; }
        private void addPool(object parameter)
        {
            var c = new Pool
            {
                Name = "新池子"
            };
            MainSave.PoolInstances.Add(c);
            Pools.Add(c);
            SQLHelper.AddPool(c);
        }
        public DelegateCommand DeletePool { get; set; }
        private void deletePool(object peremeter)
        {
            MainSave.PoolInstances.Remove(SelectPool);
            Pools.Remove(SelectPool);
            SQLHelper.RemovePool(SelectPool);
            SelectPool = null;
        }
        public DelegateCommand AddGachaItem { get; set; }
        private void addGachaItem(object peremeter)
        {
            if (SelectPool == null)
                return;
            var c = new GachaItem
            {
                Name = "示例项目"
            };
            GachaItems.Add(c);
            SelectPool.Content.Add(c);
            c.ItemID = SQLHelper.InsertOrUpdateGachaItem(c);
        }
        public DelegateCommand DeleteGachaItem { get; set; }
        private void deleteGachaItem(object peremeter)
        {
            SelectPool.Content.Remove(SelectGachaItem);
            GachaItems.Remove(SelectGachaItem);
            SQLHelper.RemoveGachaItem(SelectGachaItem);
            SelectGachaItem = null;
        }
        public DelegateCommand CopyGachaItem { get; set; }
        private void copyGachaItem(object peremeter)
        {
            if (SelectGachaItem == null)
                return;
            var c = SelectGachaItem.Clone();
            c.ItemID = 0;
            GachaItems.Add(c);
            SelectPool.Content.Add(c);
            c.ItemID = SQLHelper.InsertOrUpdateGachaItem(c);
        }
        public DelegateCommand PoolDrawTest { get; set; }
        private void poolDrawTest(object peremeter)
        {
            var c = GachaCore.DoGacha(SelectPool, SelectPool.MultiGachaNumber);
            string filename = Guid.NewGuid().ToString() + ".jpg";
            GachaCore.DrawGachaResult(c, SelectPool).Save(filename);
            new ImageBrowser(filename).Show();
        }
        public DelegateCommand ShowInteractiveDialogCmd { get; set; }
        private void ShowInteractiveDialog(object peremeter)
        {
            if (SelectPool == null)
            {
                Helper.ShowGrowlMsg("请先选中一个池");
                return;
            }
            Dialog.Show<GachaItemQueryDialog>()
                .Initialize<GachaItemQueryDialogViewModel>(vm => vm.Result = new List<GachaItem>())
                .GetResultAsync<List<GachaItem>>().ContinueWith(x =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() 
                        => x.Result.ForEach(o =>
                    {
                        if (GachaItems.Any(z => z.ItemID == o.ItemID) is false)
                            GachaItems.Add(o);
                    }));
                });
        }
    }
}
