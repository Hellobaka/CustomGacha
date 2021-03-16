using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandyControl.Tools.Extension;
using me.cqp.luohuaming.CustomGacha.UI.Model;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    public class WorkbenchViewModel : NotifyicationObject
    {
        public WorkbenchViewModel()
        {
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
                if (categories == null && EditPool!=null)
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
            }
        }

        #endregion

        #region ---绑定命令---

        #endregion
    }
}
