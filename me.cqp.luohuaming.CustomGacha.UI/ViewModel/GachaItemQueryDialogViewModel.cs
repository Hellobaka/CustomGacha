using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HandyControl.Tools.Extension;
using me.cqp.luohuaming.CustomGacha.UI.Command;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    public class GachaItemQueryDialogViewModel : NotifyicationObject, IDialogResultable<List<GachaItem>>
    {
        public static string RelateivePath { get; set; }

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
        private List<int> upContent;
        public List<int> UpContent
        {
            get { return upContent; }
            set
            {
                upContent = value;
                this.RaisePropertyChanged("UpContent");
            }
        }
        private int pageIndex = 1;
        public int PageIndex
        {
            get { return pageIndex; }
            set
            {
                pageIndex = value;
                this.RaisePropertyChanged("PageIndex");
            }
        }
        public DelegateCommand PageUpdatedCmd { get; set; }
        private void pageUpdatedCmd(object peremeter)
        {
            GachaItems = Helper.List2ObservableCollection(SQLHelper.GetPageGachaItem(PageIndex));
        }
        private string openMode;
        public string OpenMode
        {
            get { return openMode; }
            set
            {
                openMode = value;
                this.RaisePropertyChanged("OpenMode");
                switch (openMode)
                {
                    case "Query":
                        //var c = SQLHelper.GetPageGachaItem(1);
                        GachaItems = Helper.List2ObservableCollection(SQLHelper.GetAllGachaItem());
                        break;
                }
            }
        }
        private int maxPageCount;
        public int MaxPageCount
        {
            get { return maxPageCount; }
            set
            {
                maxPageCount = value;
                this.RaisePropertyChanged("MaxPageCount");
            }
        }

        public GachaItemQueryDialogViewModel()
        {
            GachaItems = new ObservableCollection<GachaItem>();
            //MaxPageCount = (int)Math.Ceiling(SQLHelper.GetAllGachaItem().Count / (double)50);
            PageUpdatedCmd = new DelegateCommand
            {
                ExecuteAction = new Action<object>(pageUpdatedCmd)
            };
        }
        private void closeAction(object permerter)
        {
            CloseAction?.Invoke();
        }
        public Action CloseAction { get; set; }
        public DelegateCommand CloseCmd => new Lazy<DelegateCommand>(() => new DelegateCommand { ExecuteAction = closeAction }).Value;
    }
}
