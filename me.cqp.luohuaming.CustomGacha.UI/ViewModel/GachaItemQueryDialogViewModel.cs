using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HandyControl.Tools.Extension;
using me.cqp.luohuaming.CustomGacha.UI.Command;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    class GachaItemQueryDialogViewModel : NotifyicationObject, IDialogResultable<List<GachaItem>>
    {
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
        private List<GachaItem> result;

        public List<GachaItem> Result
        {
            get { return result; }
            set
            {
                result = value;
                this.RaisePropertyChanged("Result");
            }
        }

        public GachaItemQueryDialogViewModel()
        {
            GachaItems = new ObservableCollection<GachaItem>();
            var c = SQLHelper.GetAllGachaItem();
            c.ForEach(x => GachaItems.Add(x));
        }
        private void closeAction(object permerter)
        {
            CloseAction?.Invoke();
        }
        public Action CloseAction { get; set; }
        public DelegateCommand CloseCmd => new Lazy<DelegateCommand>(() => new DelegateCommand { ExecuteAction = closeAction }).Value;
    }
}
