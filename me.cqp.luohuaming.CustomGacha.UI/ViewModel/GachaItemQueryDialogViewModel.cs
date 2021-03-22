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
        public class VMArray : NotifyicationObject
        {
            public GachaItem Object { get; set; }
            public bool IsSelected { get; set; }
        }
        private ObservableCollection<VMArray> gachaItems;
        public ObservableCollection<VMArray> GachaItems
        {
            get { return gachaItems; }
            set
            {
                gachaItems = value;
                this.RaisePropertyChanged("GachaItems");
            }
        }
        private ObservableCollection<VMArray> queryItems;
        public ObservableCollection<VMArray> QueryItems
        {
            get { return queryItems; }
            set
            {
                queryItems = value;
                this.RaisePropertyChanged("QueryItems");
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
            QueryItems = Helper.ToPageList(GachaItems, PageIndex, PageCount);
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
                        var c = SQLHelper.GetAllGachaItem();
                        GachaItems = new ObservableCollection<VMArray>();
                        c.ForEach(x => GachaItems.Add(new VMArray { Object = x, IsSelected = false }));
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
        public int PageCount { get; set; } = 30;
        public GachaItemQueryDialogViewModel()
        {
            GachaItems = new ObservableCollection<VMArray>();
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
