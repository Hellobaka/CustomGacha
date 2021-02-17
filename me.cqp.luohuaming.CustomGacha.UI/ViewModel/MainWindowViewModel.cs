using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using me.cqp.luohuaming.CustomGacha.UI.Command;
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
        private Pool selectPool;

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


        public Pool SelectPool
        {
            get { return selectPool; }
            set
            {
                SQLHelper.UpdatePool(selectPool);
                selectPool = value;
                GachaItems.Clear();
                selectPool.Content.ForEach(x => GachaItems.Add(x));
                this.RaisePropertyChanged("SelectPool");
            }
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
        }
        public DelegateCommand AddPool { get; set; }
        private void addPool(object parameter)
        {
            var c =new Pool
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
                Name="示例项目"
            };
            GachaItems.Add(c);
            SelectPool.Content.Add(c);
        }
    }
}
