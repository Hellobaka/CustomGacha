using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandyControl.Tools.Extension;
using me.cqp.luohuaming.CustomGacha.UI.Command;
using me.cqp.luohuaming.CustomGacha.UI.Model;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    public class WorkbenchViewModel : NotifyicationObject
    {
        public WorkbenchViewModel()
        {
            AddUpItem = new DelegateCommand
            {
                ExecuteAction = new Action<object>(addUpItem)
            };
            RemoveUpItem = new DelegateCommand
            {
                ExecuteAction = new Action<object>(removeUpItem)
            };
            NonUpContents = new List<GachaItem>();
        }

        #region ---绑定属性---
        private Pool editPool;
        public Pool EditPool
        {
            get { return editPool; }
            set
            {
                editPool = value;
                this.RaisePropertyChanged("EditPool");
            }
        }
        private ObservableCollection<GachaItem> gachaItems;
        public ObservableCollection<GachaItem> GachaItems
        {
            get { return gachaItems; }
            set
            {
                gachaItems = new ObservableCollection<GachaItem>(value.OrderByDescending(x => x.IsUp)
                                                                                    .ThenBy(x => x.Probablity));
                this.RaisePropertyChanged("GachaItems");
            }
        }
        private ObservableCollection<Category> categories;
        public ObservableCollection<Category> Categories
        {
            get
            {
                if (categories == null && EditPool != null)
                {
                    categories = Helper.List2ObservableCollection(SQLHelper.GetCategoriesByIDs(EditPool.Content));
                }
                return categories;
            }
            set
            {
                categories = value;
                this.RaisePropertyChanged("Categories");
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
        private Category selectCategory;
        public Category SelectCategory
        {
            get { return selectCategory; }
            set
            {
                selectCategory = value;
                this.RaisePropertyChanged("SelectCategory");
                if (value == null)
                    return;
                var c = SQLHelper.GetGachaItemsByIDs(value.Content);
                c.Where(x => value.UpContent.Any(o => o == x.ItemID)).Do(x => x.IsUp = true);
                GachaItems = Helper.List2ObservableCollection(c);
                UpContents = SQLHelper.GetGachaItemsByIDs(value.UpContent);
                NonUpContents = new List<GachaItem>();
                c.Where(x => !value.UpContent.Any(o => o == x.ItemID)).Do(x => NonUpContents.Add(x));
            }
        }
        private List<GachaItem> upContents;
        public List<GachaItem> UpContents
        {
            get { return upContents; }
            set
            {
                upContents = value;
                this.RaisePropertyChanged("UpContents");
            }
        }
        private List<GachaItem> nonUpContents;
        public List<GachaItem> NonUpContents
        {
            get { return nonUpContents; }
            set
            {
                nonUpContents = value;
                this.RaisePropertyChanged("NonUpContents");
            }
        }
        private GachaItem selectItemLeft;
        public GachaItem SelectItemLeft
        {
            get { return selectItemLeft; }
            set
            {
                selectItemLeft = value;
                this.RaisePropertyChanged("SelectItemLeft");
            }
        }
        private GachaItem selectItemRight;
        public GachaItem SelectItemRight
        {
            get { return selectItemRight; }
            set
            {
                selectItemRight = value;
                this.RaisePropertyChanged("SelectItemRight");
            }
        }

        #endregion

        #region ---绑定命令---
        public DelegateCommand AddUpItem { get; set; }
        private void addUpItem(object parameter)
        {
            UpContents.Add(SelectItemRight);
            NonUpContents.Remove(SelectItemRight);
        }
        public DelegateCommand RemoveUpItem { get; set; }
        private void removeUpItem(object parameter)
        {
            UpContents.Remove(SelectItemLeft);
            NonUpContents.Add(SelectItemLeft);
        }
        #endregion
    }
}
