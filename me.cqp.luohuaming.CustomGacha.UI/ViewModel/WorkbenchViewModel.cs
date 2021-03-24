using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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
            SaveAction = new DelegateCommand
            {
                ExecuteAction = new Action<object>(saveAction)
            };
            PoolDrawTest = new DelegateCommand
            {
                ExecuteAction = new Action<object>(poolDrawTest)
            };
            OpenGitHub = new DelegateCommand
            {
                ExecuteAction = new Action<object>(openGitHub)
            };
            ForeConfigDialog = new DelegateCommand
            {
                ExecuteAction = new Action<object>(o => { ShowInteractiveDialog(DialogAction.ForeConfig); })
            };
            NewPoolDialog = new DelegateCommand
            {
                ExecuteAction = new Action<object>(o =>
                {
                    if (bool.Parse(o.ToString()))
                    {
                        if (HandyControl.Controls.MessageBox.Ask("如果数据未保存将会失去所有未保存数据！", "提示") == MessageBoxResult.Cancel)
                        {
                            return;
                        }
                    }
                    ShowInteractiveDialog(DialogAction.NewPool);
                })
            };
        }

        #region ---绑定属性---
        public Dictionary<Category, List<GachaItem>> GachaitemsInCategory { get; set; }
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
                    Thread thread = new Thread(() =>
                    {
                        GachaitemsInCategory = new Dictionary<Category, List<GachaItem>>();
                        foreach (var item in categories)
                        {
                            var c = SQLHelper.GetGachaItemsByIDs(item.Content);
                            if (c.Any(x => x == null))
                            {
                                int count = 0;
                                for (int i = 0; i < c.Count; i++)
                                {
                                    if (c[i] == null)
                                    {
                                        c.RemoveAt(i);
                                        item.Content.RemoveAt(i);
                                        SQLHelper.UpdateOrAddCategory(item);
                                        count++;
                                    }
                                }
                                Helper.ShowGrowlMsg($"检测到目录 {item.Name} 存在无效项目，共清理了 {count} 个无效项目", Helper.NoticeEnum.Info, 3);
                            }
                            c.Where(x => item.UpContent.Any(o => o == x.ItemID)).Do(x => x.IsUp = true);
                            GachaitemsInCategory.Add(item, c);
                        }
                    });
                    thread.Start();
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
                if (GachaitemsInCategory.ContainsKey(value) is false)
                {
                    GachaItems = new ObservableCollection<GachaItem>();
                    return;
                }
                GachaItems = Helper.List2ObservableCollection(GachaitemsInCategory[value]);
            }
        }
        private Config config;
        public Config Config
        {
            get { return config; }
            set
            {
                config = value;
                this.RaisePropertyChanged("Config");
            }
        }
        private OrderConfig orderConfig;
        public OrderConfig OrderConfig
        {
            get { return orderConfig; }
            set
            {
                orderConfig = value;
                this.RaisePropertyChanged("OrderConfig");
            }
        }
        private OpenType dialogType;
        public OpenType DialogType
        {
            get { return dialogType; }
            set
            {
                dialogType = value;
                this.RaisePropertyChanged("DialogType");
            }
        }
        #endregion

        #region ---绑定命令---
        public DelegateCommand SetCategoryBaodi { get; set; }
        private void setCategoryBaodi(object parameter)
        {
            //TODO: 数据库操作
            var c = SelectCategory.ID;
            var o = Categories.First(x => x.ID == c);
            o.IsBaodi = true;
            //Categories2Change.Add(new ChangedCategory{Object = o,Action = ObjectAction.Update});
            RaisePropertyChanged("Categories");
            ReloadCategroies();
            Helper.ShowGrowlMsg($"设置子项目 {o.Name} 的保底属性为 True");
        }
        public DelegateCommand EditCategory { get; set; }
        public DelegateCommand CopyCategory { get; set; }
        private void copyCategory(object parameter)
        {
            var c = SelectCategory.Clone();
            c.ID = -1;
            int index = Categories.IndexOf(SelectCategory);
            Categories.Insert(index, c);
            GachaitemsInCategory.Add(c, SQLHelper.GetGachaItemsByIDs(c.Content));
            SelectCategory = c;
            RaisePropertyChanged("Categories");
            ReloadCategroies();
            //TODO: 数据库操作
            Helper.ShowGrowlMsg($"成功复制目录 {c.Name}");
        }
        public DelegateCommand SaveAction { get; set; }
        private void saveAction(object parameter)
        {
            if (EditPool.PoolID == -1)
            {
                Helper.ShowGrowlMsg("保存之前请先新建或者打开一个项目", Helper.NoticeEnum.Error);
                return;
            }
            int count = 0;
            foreach (var item in Categories)
            {
                if (item.ID == -1)
                {
                    item.ID = SQLHelper.UpdateOrAddCategory(item, true);
                    count++;
                }
            }
            if (count > 0)
            {
                Helper.ShowGrowlMsg($"在数据库新建了 {count} 个目录");
                count = 0;
            }
            EditPool.Content = Categories.Select(x => x.ID).ToList();
            foreach (var item in GachaitemsInCategory)
            {
                int index = 0;
                foreach (var items in item.Value)
                {
                    if (items.ItemID == -1)
                    {
                        items.ItemID = SQLHelper.InsertOrUpdateGachaItem(items);
                        item.Key.Content[index] = items.ItemID;
                        count++;
                    }
                    index++;
                }
                SQLHelper.UpdateOrAddCategory(item.Key);
            }
            if (count > 0)
            {
                Helper.ShowGrowlMsg($"在数据库新建了 {count} 个子项目");
            }
            SQLHelper.UpdatePool(EditPool);
            try
            {
                EditPool.PluginInit();
            }
            catch (Exception e)
            {
                Helper.ShowGrowlMsg($"插件初始化失败，错误信息: {e.Message}", Helper.NoticeEnum.Error, 2);
                EditPool.DrawAllItems = null;
                EditPool.DrawItem = null;
                EditPool.DrawMainImage = null;
                EditPool.DrawPoints = null;
                EditPool.FinallyDraw = null;
            }

            Helper.ShowGrowlMsg("保存完成");
        }
        public DelegateCommand ClearCategories { get; set; }
        private void clearCategories(object parameter)
        {
            if (HandyControl.Controls.MessageBox.Ask("确认清空吗？此操作将会从数据库中删除这些目录，但不会影响内容", "提示") == MessageBoxResult.Cancel)
                return;
            Categories.Clear();
            GachaitemsInCategory.Clear();
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
            GachaitemsInCategory.Remove(SelectCategory);
            Categories.Remove(Categories.First(x => x.ID == c));
            RaisePropertyChanged("Categories");
            SelectCategory = new Category();
            GachaItems.Clear();
            ReloadCategroies();
            Helper.ShowGrowlMsg($"已删除目录 {name}", Helper.NoticeEnum.Info);

            //TODO: 数据库操作
        }
        public DelegateCommand UnSetCategoryBaodi { get; set; }
        private void unSetCategoryBaodi(object parameter)
        {
            var c = SelectCategory.ID;
            var o = Categories.First(x => x.ID == c);
            o.IsBaodi = false;
            SelectCategory = o;
            RaisePropertyChanged("Categories");
            ReloadCategroies();
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
                ItemID = -1
            };
            //c.ItemID = SQLHelper.InsertOrUpdateGachaItem(c);
            GachaItems.Add(c);
            SelectGachaItem = c;
            SelectCategory.Content.Add(c.ItemID);
            GachaitemsInCategory[SelectCategory].Add(c);
            ReloadItems();
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
            SelectCategory.Content.Remove(c);
            if (SelectCategory.UpContent.Any(x => c == x))
            {
                SelectCategory.UpContent.Remove(c);
            }
            GachaitemsInCategory[SelectCategory].Remove(SelectGachaItem);
            RaisePropertyChanged("GachaItems");
            ReloadItems();
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
            GachaItems.Insert(index, c);
            c.ItemID = -1;
            //c.ItemID = SQLHelper.InsertOrUpdateGachaItem(c);
            SelectCategory.Content.Add(c.ItemID);
            GachaitemsInCategory[SelectCategory].Add(c);
            SelectGachaItem = c;
            if (c.IsUp)
            {
                setItemUp(null);
            }
            RaisePropertyChanged("GachaItems");
            ReloadItems();
            //TODO: 数据库操作
            Helper.ShowGrowlMsg($"成功复制 {c.Name} 子项目");
        }
        public DelegateCommand UnsetItemUp { get; set; }
        private void unsetItemUp(object parameter)
        {
            var c = SelectGachaItem.ItemID;
            var o = GachaItems.First(x => x.ItemID == c);
            o.IsUp = false;
            SelectCategory.UpContent.Remove(o.ItemID);
            SelectGachaItem = o;
            GachaitemsInCategory[SelectCategory].First(x => x.ItemID == o.ItemID).IsUp = false;
            RaisePropertyChanged("GachaItems");
            ReloadItems();
            //TODO: 数据库操作
            Helper.ShowGrowlMsg($"设置子项目 {o.Name} 的Up属性为 False");
        }
        public DelegateCommand SetItemUp { get; set; }
        private void setItemUp(object parameter)
        {
            var c = SelectGachaItem.ItemID;
            var o = GachaItems.First(x => x.ItemID == c);
            o.IsUp = true;
            SelectCategory.UpContent.Add(o.ItemID);
            SelectGachaItem = o;
            GachaitemsInCategory[SelectCategory].First(x => x.ItemID == o.ItemID).IsUp = true;
            RaisePropertyChanged("GachaItems");
            ReloadItems();
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
            GachaitemsInCategory[SelectCategory].Clear();
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
            QueryGachaItem,
            ForeConfig,
            WorkBenchConfig,
            NewPool
        }
        public DelegateCommand ForeConfigDialog { get; set; }
        public DelegateCommand NewPoolDialog { get; set; }
        public DelegateCommand QueryItemDialog { get; set; }
        public DelegateCommand OpenNewCategoryDialog { get; set; }
        public DelegateCommand SetUpContentDialog { get; set; }
        private void ShowInteractiveDialog(object peremeter)
        {
            switch ((DialogAction)peremeter)
            {
                case DialogAction.NewCategory:
                    Dialog.Show<NewCategoryPage>()
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
                            {
                                Categories.Add(x.Result);
                                GachaitemsInCategory.Add(x.Result, new List<GachaItem>());
                            }
                            //TODO: 数据库操作
                        });
                    });
                    break;
                case DialogAction.SetUpContent:
                    Dialog.Show<GachaItemQueryDialog>()
                    .Initialize<GachaItemQueryDialogViewModel>(vm =>
                    {
                        GachaItems.ToList().ForEach(x => vm.GachaItems.Add(new GachaItemQueryDialogViewModel.VMArray { Object = x, IsSelected = selectCategory.UpContent.Any(o => o == x.ItemID) }));
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
                                if (SelectCategory.UpContent.Any(c => c == o.ItemID) is false)
                                {
                                    SelectCategory.UpContent.Add(o.ItemID);
                                    GachaitemsInCategory[SelectCategory].First(c => c.ItemID == o.ItemID).IsUp = true;
                                    ReloadItems();
                                    count++;
                                }
                            });
                            //SQLHelper.UpdateOrAddCategory(SelectCategory);
                            ReloadItems();
                            this.RaisePropertyChanged("GachaItems");
                            Helper.ShowGrowlMsg($"共设置了 {count} 个Up项");
                        });
                    });
                    break;
                case DialogAction.EditCategory:
                    Dialog.Show<NewCategoryPage>()
                    .Initialize<NewPoolViewModel>(vm =>
                    {
                        vm.NowCategory = SelectCategory.Clone();
                    }).GetResultAsync<Category>().ContinueWith(x =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (x.Result != null)
                            {
                                var c = Categories.First(o => o.ID == x.Result.ID);
                                Categories.Insert(Categories.IndexOf(c), x.Result);
                                Categories.Remove(c);
                                GachaitemsInCategory.Remove(c);
                                GachaitemsInCategory.Add(c, SQLHelper.GetGachaItemsByIDs(x.Result.Content));
                                SelectCategory = c;
                                ReloadCategroies();
                                //TODO: 数据库操作
                                RaisePropertyChanged("Categories");
                            }
                        });
                    });
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
                                        GachaitemsInCategory[SelectCategory].Add(o);
                                        ReloadItems();
                                        count++;
                                    }
                                });
                                Helper.ShowGrowlMsg($"共添加了 {count} 个项");
                            }
                        });
                    });
                    break;
                case DialogAction.ForeConfig:
                    Dialog.Show<ForeConfig>().Initialize<WorkbenchViewModel>(c =>
                    {
                        c.Config = Config.Clone();
                        c.OrderConfig = OrderConfig.Clone();
                    });
                    break;
                case DialogAction.WorkBenchConfig:
                    break;
                case DialogAction.NewPool:
                    Dialog.Show<NewPoolStep>().GetResultAsync<Pool>().ContinueWith(x =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (x.Result != null)
                            {
                                EditPool = x.Result;
                                EditPool.PoolID = SQLHelper.AddPool(x.Result);
                                EditPool.CreateDt = DateTime.Now;
                                EditPool.UpdateDt = DateTime.Now;
                                Helper.ShowGrowlMsg($"成功新建了卡池 {x.Result.Name}");
                                Workbench.contentTab_Export.IsEnabled = true;
                            }
                        });
                    });
                    break;
                default:
                    break;
            }
        }
        public DelegateCommand PoolDrawTest { get; set; }
        private void poolDrawTest(object peremeter)
        {
            Directory.CreateDirectory("DrawTest");
            long testQQ = 1145141919;
            int baodiCount = 1;
            var c = GachaCore.DoGacha(EditPool, EditPool.MultiGachaNumber,ref baodiCount);
            c = SQLHelper.UpdateGachaItemsNewStatus(c, testQQ);
            SQLHelper.InsertGachaItem2Repo(c, testQQ);
            string filename = Guid.NewGuid().ToString() + ".jpg";
            GachaCore.DrawGachaResult(c, EditPool).Save("DrawTest\\" + filename);
            Process.Start("DrawTest\\" + filename);
        }
        public DelegateCommand OpenGitHub { get; set; }
        private void openGitHub(object peremeter)
        {
            Process.Start("https://github.com/Hellobaka/CustomGacha");
        }
        #endregion
        private void ReloadCategroies()
        {
            var c = new ObservableCollection<Category>();
            foreach (var item in Categories)
            {
                c.Add(item);
            }
            Categories = c;
        }
        private void ReloadItems()
        {
            var c = new ObservableCollection<GachaItem>();
            foreach (var item in GachaitemsInCategory[SelectCategory])
            {
                c.Add(item);
            }
            GachaItems = c;
        }
        public enum OpenType
        {
            Normal,
            NoPool,
            NewPool
        }
    }
}
