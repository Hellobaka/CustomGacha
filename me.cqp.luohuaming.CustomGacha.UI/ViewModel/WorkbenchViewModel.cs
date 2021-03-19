using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using me.cqp.luohuaming.CustomGacha.UI.Command;
using me.cqp.luohuaming.CustomGacha.UI.View;
using me.cqp.luohuaming.CustomGacha.UI.View.ChildView;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    public class WorkbenchViewModel : NotifyicationObject
    {
        public WorkbenchViewModel()
        {
            SetCategoryBaodi = new DelegateCommand
            {
                ExecuteAction = new Action<object>(setCategoryBaodi)
            };
            UnSetCategoryBaodi = new DelegateCommand
            {
                ExecuteAction = new Action<object>(unSetCategoryBaodi)
            };
            OpenNewCategoryDialog = new DelegateCommand
            {
                ExecuteAction = new Action<object>(o => { ShowInteractiveDialog(DialogAction.NewCategory); })
            };
            SetUpContentDialog = new DelegateCommand
            {
                ExecuteAction = new Action<object>(o => { ShowInteractiveDialog(DialogAction.SetUpContent); })
            };
            EditCategory = new DelegateCommand
            {
                ExecuteAction = new Action<object>(o => { ShowInteractiveDialog(DialogAction.EditCategory); })
            };
            CopyCategory = new DelegateCommand
            {
                ExecuteAction = new Action<object>(copyCategory)
            };
            DeleteCategory = new DelegateCommand
            {
                ExecuteAction = new Action<object>(deleteCategory)
            };
            DeleteItem = new DelegateCommand
            {
                ExecuteAction = new Action<object>(deleteItem)
            };
            DeleteCategoryFromDB = new DelegateCommand
            {
                ExecuteAction = new Action<object>(deleteCategoryFromDB)
            };
            CopyItem = new DelegateCommand
            {
                ExecuteAction = new Action<object>(copyItem)
            };
            SetItemUp = new DelegateCommand
            {
                ExecuteAction = new Action<object>(setItemUp)
            };
            UnsetItemUp = new DelegateCommand
            {
                ExecuteAction = new Action<object>(unsetItemUp)
            };
            NewItem = new DelegateCommand
            {
                ExecuteAction = new Action<object>(newItem)
            };
            QueryItemDialog = new DelegateCommand
            {
                ExecuteAction = new Action<object>(o => { ShowInteractiveDialog(DialogAction.QueryGachaItem); })
            };
            ClearItems = new DelegateCommand
            {
                ExecuteAction = new Action<object>(clearItems)
            };
            ClearCategories = new DelegateCommand
            {
                ExecuteAction = new Action<object>(clearCategories)
            };
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
            }
        }
        #endregion

        #region ---绑定命令---
        public DelegateCommand SetCategoryBaodi { get; set; }
        private void setCategoryBaodi(object parameter)
        {
            //TODO: 数据库操作
            var c = SelectCategory.ID;
            Categories.Clear();
            Categories = Helper.List2ObservableCollection(SQLHelper.GetCategoriesByIDs(EditPool.Content));
            var o = Categories.First(x => x.ID == c);
            o.IsBaodi = true;
            SelectCategory = o;
            RaisePropertyChanged("Categories");
            Helper.ShowGrowlMsg($"设置子项目 {o.Name} 的保底属性为 True");
        }
        public DelegateCommand EditCategory { get; set; }
        public DelegateCommand CopyCategory { get; set; }
        private void copyCategory(object parameter)
        {
            var c = SelectCategory.Clone();
            int index = Categories.IndexOf(SelectCategory);
            ReloadCategroies();
            Categories.Insert(index, c);
            SelectCategory = c;
            RaisePropertyChanged("Categories");
            //TODO: 数据库操作
            Helper.ShowGrowlMsg($"成功复制目录 {c.Name}");
        }
        public DelegateCommand ClearCategories { get; set; }
        private void clearCategories(object parameter)
        {
            if (HandyControl.Controls.MessageBox.Ask("确认清空吗？此操作将会从数据库中删除这些目录，但不会影响内容", "提示") == MessageBoxResult.Cancel)
                return;
            Categories.Clear();
            SelectCategory = new Category();
            RaisePropertyChanged("Categories");
            Helper.ShowGrowlMsg($"已将目录列表清空", Helper.NoticeEnum.Info);
            //TODO: 数据库操作
        }
        public DelegateCommand DeleteCategory { get; set; }
        private void deleteCategory(object parameter)
        {
            if (HandyControl.Controls.MessageBox.Ask("确认删除此目录吗？此操作将会从数据库中删除此目录，但不会影响内容", "提示") == MessageBoxResult.Cancel)
                return;
            string name = SelectCategory.Name;
            var c = SelectCategory.ID;
            ReloadCategroies();
            Categories.Remove(Categories.First(x => x.ID == c));
            RaisePropertyChanged("Categories");
            SelectCategory = new Category();
            GachaItems.Clear();
            Helper.ShowGrowlMsg($"已删除目录 {name}", Helper.NoticeEnum.Info);

            //TODO: 数据库操作
        }
        public DelegateCommand UnSetCategoryBaodi { get; set; }
        private void unSetCategoryBaodi(object parameter)
        {
            var c = SelectCategory.ID;
            ReloadCategroies();
            var o = Categories.First(x => x.ID == c);
            o.IsBaodi = false;
            SelectCategory = o;
            RaisePropertyChanged("Categories"); 
            Helper.ShowGrowlMsg($"设置目录 {o.Name} 的保底属性为 False");
        }
        public DelegateCommand NewItem { get; set; }
        private void newItem(object parameter)
        {
            if (SelectCategory == null)
            {
                return;
            }
            var c = new GachaItem
            {
                Name = "新项目",
            };
            ReloadItems();
            //c.ItemID = SQLHelper.InsertOrUpdateGachaItem(c);
            GachaItems.Add(c);
            SelectGachaItem = c;
            SelectCategory.Content.Add(c.ItemID);
            RaisePropertyChanged("GachaItems");
            Helper.ShowGrowlMsg($"成功新建了一个模板项目");
            //TODO: DataBase Action
        }
        public DelegateCommand DeleteItem { get; set; }
        private void deleteItem(object parameter)
        {
            if (HandyControl.Controls.MessageBox.Ask("确认删除此项目吗？此操作只会将项目从目录中剔除，不影响数据", "提示") == MessageBoxResult.Cancel)
                return;
            string name = SelectGachaItem.Name;
            var c = SelectGachaItem.ItemID;
            ReloadItems();
            GachaItems.Remove(GachaItems.First(x => x.ItemID == c));
            RaisePropertyChanged("GachaItems");
            SelectGachaItem = new GachaItem();
            Helper.ShowGrowlMsg($"从 {SelectCategory.Name} 目录中成功删除了子项目 {name}");
            //TODO: 数据库操作
        }
        public DelegateCommand DeleteCategoryFromDB { get; set; }
        private void deleteCategoryFromDB(object parameter)
        {
            if (HandyControl.Controls.MessageBox.Ask("确认从数据库中删除此项目吗？此操作不可逆！", "提示") == MessageBoxResult.Cancel)
                return; 
            string name = SelectGachaItem.Name;
            var c = SelectGachaItem.ItemID;
            deleteItem(null);
            //TODO: 数据库操作
            Helper.ShowGrowlMsg($"从数据库中成功删除了目录 {name}");
        }
        public DelegateCommand CopyItem { get; set; }
        private void copyItem(object parameter)
        {
            var c = SelectGachaItem.Clone();
            int index = GachaItems.IndexOf(SelectGachaItem);
            ReloadItems();
            GachaItems.Insert(index, c);
            c.ItemID = -1;
            //c.ItemID = SQLHelper.InsertOrUpdateGachaItem(c);
            SelectCategory.Content.Add(c.ItemID);
            SelectGachaItem = c;
            RaisePropertyChanged("GachaItems");
            if (c.IsUp)
            {
                setItemUp(null);
            }
            //TODO: 数据库操作
            Helper.ShowGrowlMsg($"成功复制 {c.Name} 子项目");
        }
        public DelegateCommand UnsetItemUp { get; set; }
        private void unsetItemUp(object parameter)
        {
            var c = SelectGachaItem.ItemID;
            ReloadItems();
            var o = GachaItems.First(x => x.ItemID == c);
            o.IsUp = false;
            SelectCategory.UpContent.Remove(o.ItemID);
            SelectGachaItem = o;
            RaisePropertyChanged("GachaItems");
            //TODO: 数据库操作
            Helper.ShowGrowlMsg($"设置子项目 {o.Name} 的Up属性为 False");
        }
        public DelegateCommand SetItemUp { get; set; }
        private void setItemUp(object parameter)
        {
            var c = SelectGachaItem.ItemID;
            ReloadItems();
            var o = GachaItems.First(x => x.ItemID == c);
            o.IsUp = true;
            SelectCategory.UpContent.Add(o.ItemID);
            SelectGachaItem = o;
            RaisePropertyChanged("GachaItems");
            //TODO: 数据库操作
            Helper.ShowGrowlMsg($"设置子项目 {o.Name} 的Up属性为 True");
        }
        public DelegateCommand ClearItems { get; set; }
        private void clearItems(object parameter)
        {
            if (SelectCategory == null)
            {
                return;
            }
            if (HandyControl.Controls.MessageBox.Ask("确认清空此目录吗？此操作只会将项目从目录中剔除，不影响数据", "提示") == MessageBoxResult.Cancel)
                return;
            SelectCategory.Content.Clear();
            SelectCategory.UpContent.Clear();
            GachaItems.Clear();
            RaisePropertyChanged("GachaItems");
            //TODO: 数据库操作
            Helper.ShowGrowlMsg($"已将 {SelectCategory} 目录的列表清空", Helper.NoticeEnum.Info);
        }

        enum DialogAction
        {
            NewCategory,
            SetUpContent,
            EditCategory,
            NewGachaItem,
            QueryGachaItem
        }
        public DelegateCommand QueryItemDialog { get; set; }
        public DelegateCommand OpenNewCategoryDialog { get; set; }
        public DelegateCommand SetUpContentDialog { get; set; }
        private void ShowInteractiveDialog(object peremeter)
        {
            switch ((DialogAction)peremeter)
            {
                case DialogAction.NewCategory:
                    Dialog.Show<NewPoolPage>()
                    .Initialize<NewPoolViewModel>(vm =>
                    {
                        vm.NowCategory = new Category
                        {
                            Name = "新目录",
                            Probablity = 0.5,
                            IsBaodi = false
                        };
                    }).GetResultAsync<Category>().ContinueWith(x =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (x.Result != null)
                                Categories.Add(x.Result);
                            //TODO: 数据库操作
                        });
                    });
                    break;
                case DialogAction.SetUpContent:
                    Dialog.Show<GachaItemQueryDialog>()
                    .Initialize<GachaItemQueryDialogViewModel>(vm =>
                    {
                        vm.GachaItems = GachaItems;
                        vm.UpContent = SelectCategory.UpContent;
                        vm.OpenMode = "SelectUp";
                    })
                    .GetResultAsync<List<GachaItem>>().ContinueWith(x =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (x.Result == null)
                                return;
                            SelectCategory.UpContent.Clear();
                            int count = 0;
                            x.Result.ForEach(o =>
                            {
                                if (SelectCategory.UpContent.Any(c=>c == o.ItemID) is false)
                                {
                                    SelectCategory.UpContent.Add(o.ItemID);
                                    count++;
                                }
                            });
                            //SQLHelper.UpdateOrAddCategory(SelectCategory);
                            GachaItems.Clear();
                            GachaItems = Helper.List2ObservableCollection(SQLHelper.GetGachaItemsByIDs(SelectCategory.Content));
                            GachaItems.Where(o => x.Result.Any(i => i.ItemID == o.ItemID)).Do(o => o.IsUp = true);
                            this.RaisePropertyChanged("GachaItems");
                            Helper.ShowGrowlMsg($"共设置了 {count} 个Up项");
                        });
                    });
                    break;
                case DialogAction.EditCategory:
                    Dialog.Show<NewPoolPage>()
                    .Initialize<NewPoolViewModel>(vm =>
                    {
                        vm.NowCategory = SelectCategory.Clone();
                    }).GetResultAsync<Category>().ContinueWith(x =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (x.Result != null)
                            {
                                ReloadCategroies();
                                var c = Categories.First(o => o.ID == x.Result.ID);
                                Categories.Insert(Categories.IndexOf(c), x.Result);
                                Categories.Remove(c);
                                SelectCategory = c;
                                //TODO: 数据库操作
                                RaisePropertyChanged("Categories");
                            }
                        });
                    });
                    break;
                case DialogAction.NewGachaItem:
                    break;
                case DialogAction.QueryGachaItem:
                    if (SelectCategory == null)
                    {
                        Helper.ShowGrowlMsg($"请至少选中一个目录", Helper.NoticeEnum.Error);
                        return;
                    }
                    Dialog.Show<GachaItemQueryDialog>()
                        .Initialize<GachaItemQueryDialogViewModel>(vm =>
                      {
                          vm.Result = GachaItems.ToList(); vm.OpenMode = "Query";
                      }).GetResultAsync<List<GachaItem>>().ContinueWith(x =>
                    {
                        Application.Current.Dispatcher.Invoke(() => 
                        {
                            if (x.Result != null)
                            {
                                int count = 0;
                                x.Result.ForEach(o =>
                                {
                                    if (GachaItems.Any(z => z.ItemID == o.ItemID) is false)
                                    {
                                        GachaItems.Add(o);
                                        SelectCategory.Content.Add(o.ItemID);
                                        count++;
                                        //SQLHelper.UpdateOrAddCategory(SelectCategory);
                                    }
                                });
                                Helper.ShowGrowlMsg($"共设置了 {count} 个Up项");
                            }
                        }); 
                    });
                    break;
                default:
                    break;
            }
        }
        #endregion
        private void ReloadCategroies()
        {
            Categories.Clear();
            Categories = Helper.List2ObservableCollection(SQLHelper.GetCategoriesByIDs(EditPool.Content));
        }
        private void ReloadItems()
        {
            GachaItems.Clear();
            GachaItems = Helper.List2ObservableCollection(SQLHelper.GetGachaItemsByIDs(SelectCategory.Content));
        }
    }
}
