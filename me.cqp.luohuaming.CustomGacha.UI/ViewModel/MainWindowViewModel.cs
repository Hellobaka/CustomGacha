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
        private ObservableCollection<GachaItem> gachaItems;

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

        public Pool SelectPool
        {
            get { return selectPool; }
            set
            {
                selectPool = value;
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
            MainSave.PoolInstances.ForEach(x => Pools.Add(x));
            AddPool = new DelegateCommand
            {
                ExecuteAction = new Action<object>(addPool)
            };
            DeletePool = new DelegateCommand
            {
                ExecuteAction = new Action<object>(deletePool)
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
    }
}
