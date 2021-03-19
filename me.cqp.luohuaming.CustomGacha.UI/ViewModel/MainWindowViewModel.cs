using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using me.cqp.luohuaming.CustomGacha.UI.Command;
using me.cqp.luohuaming.CustomGacha.UI.View;
using PublicInfos;
using System.Diagnostics;
using System.IO;
using HandyControl.Data;
using System.Windows;
using System.Threading;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    class MainWindowViewModel : NotifyicationObject
    {
        public MainWindowViewModel()
        {
            if (MainSave.PoolInstances == null)
                MainSave.PoolInstances = SQLHelper.GetAllPools();
            Pools = new ObservableCollection<Pool>();
            GachaItems = new ObservableCollection<GachaItem>();
            Categories = new ObservableCollection<Category>();
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
        public static Pool GetSelectPool()
        {
            return selectPool;
        }

        #region ----绑定属性----
        private bool buttonDirection;
        public bool ButtonDirection
        {
            get { return buttonDirection; }
            set
            {
                buttonDirection = value;
                this.RaisePropertyChanged("ButtonDirection");
            }
        }

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
                gachaItems = new ObservableCollection<GachaItem>(value.OrderByDescending(x => x.IsUp)
                                                                                    .ThenBy(x => x.Probablity));
                this.RaisePropertyChanged("GachaItems");
            }
        }
        private ObservableCollection<Category> categories;
        public ObservableCollection<Category> Categories
        {
            get { return categories; }
            set
            {
                categories = value;
                this.RaisePropertyChanged("Categories");
            }
        }


        private static Pool selectPool;
        public Pool SelectPool
        {
            get { return selectPool; }
            set
            {
                GachaItems.Clear();
                Categories.Clear();
                if (value != null)
                {
                    SQLHelper.UpdatePool(selectPool);
                    Categories = Helper.List2ObservableCollection(SQLHelper.GetCategoriesByIDs(value.Content));
                }
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
                if (value != null)
                    SQLHelper.InsertOrUpdateGachaItem(selectGachaItem);
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
                SQLHelper.UpdateOrAddCategory(SelectCategory);
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

        #region ----绑定命令----
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
            SelectPool = c;
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
            if (ButtonDirection is false)
            {
                addCategory(peremeter);
                return;
            }
            var c = new GachaItem
            {
                Name = "示例项目"
            };
            GachaItems.Add(c);
            c.ItemID = SQLHelper.InsertOrUpdateGachaItem(c);
            SelectCategory.Content.Add(c.ItemID);
            SQLHelper.UpdateOrAddCategory(SelectCategory);
            SelectGachaItem = c;
        }
        public DelegateCommand DeleteGachaItem { get; set; }
        private void deleteGachaItem(object peremeter)
        {
            if (ButtonDirection is false)
            {
                removeCategory(peremeter);
                return;
            }
            MessageBoxInfo info = new MessageBoxInfo
            {
                Button = MessageBoxButton.YesNo,
                Caption = "疑问",
                Message = "确认要删除此抽卡项目吗？此操作并不会从数据库中删除此项",
            };
            if (HandyControl.Controls.MessageBox.Show(info) == MessageBoxResult.Yes)
            {
                SelectCategory.Content.Remove(SelectGachaItem.ItemID);
                //SQLHelper.RemoveGachaItem(SelectGachaItem);
                SQLHelper.UpdateOrAddCategory(SelectCategory);
                GachaItems.Remove(SelectGachaItem);
                SelectGachaItem = null;
            }
        }
        public DelegateCommand CopyGachaItem { get; set; }
        private void copyGachaItem(object peremeter)
        {
            if (ButtonDirection is false)
            {
                copyCategory(peremeter);
                return;
            }
            if (SelectGachaItem == null)
                return;
            var c = SelectGachaItem.Clone();
            c.ItemID = 0;
            GachaItems.Add(c);
            c.ItemID = SQLHelper.InsertOrUpdateGachaItem(c);
            SelectCategory.Content.Add(c.ItemID);
            SQLHelper.UpdatePool(SelectPool);
            SelectGachaItem = c;
        }
        public DelegateCommand PoolDrawTest { get; set; }
        private void poolDrawTest(object peremeter)
        {
            Thread thread = new Thread(() =>
            {
                //TODO: 进度条请求
                if (SelectPool == null)
                {
                    Helper.ShowGrowlMsg("请先选中一个池"); return;
                }
                Directory.CreateDirectory("DrawTest");
                long testQQ = 8863450594;
                var c = GachaCore.DoGacha(SelectPool, SelectPool.MultiGachaNumber);
                c = SQLHelper.UpdateGachaItemsNewStatus(c, testQQ);
                SQLHelper.InsertGachaItem2Repo(c, testQQ);
                string filename = Guid.NewGuid().ToString() + ".jpg";
                GachaCore.DrawGachaResult(c, SelectPool).Save("DrawTest\\" + filename);
                Process.Start("DrawTest\\" + filename);
            });
            thread.Start();
        }
        private void addCategory(object peremeter)
        {
            if (SelectPool == null)
                return;
            var c = new Category
            {
                Name = "示例目录"
            };
            Categories.Add(c);
            c.ID = SQLHelper.UpdateOrAddCategory(c, true);
            SelectPool.Content.Add(c.ID);
            SQLHelper.UpdatePool(SelectPool);
            SelectCategory = c;
        }
        private void removeCategory(object peremeter)
        {
            MessageBoxInfo info = new MessageBoxInfo
            {
                Button = MessageBoxButton.YesNo,
                Caption = "疑问",
                Message = "确认要删除此目录吗？此操作不可逆",
            };
            if (HandyControl.Controls.MessageBox.Show(info) == MessageBoxResult.Yes)
            {
                SelectPool.Content.Remove(SelectCategory.ID);
                SQLHelper.RemoveCategory(SelectCategory);
                SQLHelper.UpdatePool(SelectPool);
                Categories.Remove(SelectCategory);
                SelectCategory = null;
            }
        }
        private void copyCategory(object peremeter)
        {
            if (SelectCategory == null)
                return;
            var c = SelectCategory.Clone();
            c.ID = 0;
            c.Content.Clear();
            Categories.Add(c);
            c.ID = SQLHelper.UpdateOrAddCategory(c, true);
            SelectPool.Content.Add(c.ID);
            SQLHelper.UpdatePool(SelectPool);
            SelectCategory = c;
        }
        public DelegateCommand ShowInteractiveDialogCmd { get; set; }
        private void ShowInteractiveDialog(object peremeter)
        {
            if (SelectCategory == null)
            {
                Helper.ShowGrowlMsg("请先选中一个目录");
                return;
            }
            if (ButtonDirection)
            {
                GachaItemQueryDialogViewModel.RelateivePath = SelectPool.RelativePath;
                Dialog.Show<GachaItemQueryDialog>()
                .Initialize<GachaItemQueryDialogViewModel>(vm => { vm.Result = GachaItems.ToList(); vm.OpenMode = "Query"; })
                .GetResultAsync<List<GachaItem>>().ContinueWith(x =>
                {
                    Application.Current.Dispatcher.Invoke(()
                        => x.Result.ForEach(o =>
                        {
                            if (GachaItems.Any(z => z.ItemID == o.ItemID) is false)
                            {
                                GachaItems.Add(o);
                                SelectCategory.Content.Add(o.ItemID);
                                SQLHelper.UpdateOrAddCategory(SelectCategory);
                            }
                        }));
                });
            }
            else
            {
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
                            SelectCategory.UpContent.Clear();
                            x.Result.ForEach(o =>
                            {
                                SelectCategory.UpContent.Add(o.ItemID);
                            });
                            SQLHelper.UpdateOrAddCategory(SelectCategory);
                            GachaItems.Clear();
                            GachaItems = Helper.List2ObservableCollection(SQLHelper.GetGachaItemsByIDs(SelectCategory.Content));
                            GachaItems.Where(o => x.Result.Any(i => i.ItemID == o.ItemID)).Do(o => o.IsUp = true);
                            this.RaisePropertyChanged("GachaItems");
                        });
                 });
            }
        }
        #endregion
    }
}
