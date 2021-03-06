﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using CustomGacha.SDK.Tool.Http;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using me.cqp.luohuaming.CustomGacha.UI.Command;
using me.cqp.luohuaming.CustomGacha.UI.View;
using me.cqp.luohuaming.CustomGacha.UI.View.ChildView;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    //TODO: 优化MVVM的样子, 看起来很累赘
    //TODO: 可视化插件编辑器
    //TODO: 增加更多的指令操作(氪金 黑名单等
    //TODO: 云数据库构建希望
    //TODO: 云卡池构建希望
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
                ExecuteAction = new Action<object>(o => { ShowInteractiveDialog(Model.WorkBenchModel.DialogAction.NewCategory); })
            };
            SetUpContentDialog = new DelegateCommand
            {
                ExecuteAction = new Action<object>(o => { ShowInteractiveDialog(Model.WorkBenchModel.DialogAction.SetUpContent); })
            };
            EditCategory = new DelegateCommand
            {
                ExecuteAction = new Action<object>(o => { ShowInteractiveDialog(Model.WorkBenchModel.DialogAction.EditCategory); })
            };
            CopyCategory = new DelegateCommand
            {
                ExecuteAction = new Action<object>(copyCategory)
            };
            DeleteItem = new DelegateCommand
            {
                ExecuteAction = new Action<object>(deleteItem)
            };
            DeleteItemFromDB = new DelegateCommand
            {
                ExecuteAction = new Action<object>(deleteItemFromDB)
            };
            DeleteCategoryFromDB = new DelegateCommand
            {
                ExecuteAction = new Action<object>(deleteCategoryFromDB)
            };
            CopyItem = new DelegateCommand
            {
                ExecuteAction = new Action<object>(copyItem)
            };
            MultiCopyItem = new DelegateCommand
            {
                ExecuteAction = new Action<object>(multicopyItem)
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
                ExecuteAction = new Action<object>(o => { ShowInteractiveDialog(Model.WorkBenchModel.DialogAction.QueryGachaItem); })
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
            ExportPool = new DelegateCommand
            {
                ExecuteAction = new Action<object>(exportPool)
            };
            ImportPool = new DelegateCommand
            {
                ExecuteAction = new Action<object>(importPool)
            };
            ForeConfigDialog = new DelegateCommand
            {
                ExecuteAction = new Action<object>(o => { ShowInteractiveDialog(Model.WorkBenchModel.DialogAction.ForeConfig); })
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
                    ShowInteractiveDialog(Model.WorkBenchModel.DialogAction.NewPool);
                })
            };
            TemplateCopyItem = new DelegateCommand
            {
                ExecuteAction = new Action<object>(templateCopyItem)
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
                    LoadCategories();
                }
                return categories;
            }
            set
            {
                categories = value;
                this.RaisePropertyChanged("Categories");
            }
        }

        private void LoadCategories()
        {
            Categories = Helper.List2ObservableCollection(SQLHelper.GetCategoriesByIDs(EditPool.Content));
            Thread thread = new Thread(() =>
            {
                GachaitemsInCategory = new Dictionary<Category, List<GachaItem>>();
                int index = 0;
                foreach (var item in Categories)
                {
                    if (item == null)
                    {
                        EditPool.Content.RemoveAt(index);
                        SQLHelper.UpdatePool(EditPool);
                        Helper.ShowGrowlMsg("遇到一个无效目录，重启编辑器以除错.");
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(item.GUID))
                    {
                        item.GUID = Guid.NewGuid().ToString();
                        SQLHelper.UpdateOrAddCategory(item);
                    }
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
                    c.ForEach(x =>
                    {
                        if (x != null && string.IsNullOrWhiteSpace(x.GUID))
                        {
                            x.GUID = Guid.NewGuid().ToString();
                            SQLHelper.UpdateOrAddGachaItem(x);
                        }
                    });
                    c.Where(x => item.UpContent.Any(o => o == x.ItemID)).Do(x => x.IsUp = true);
                    c.Do(x => x.Editted = false);
                    GachaitemsInCategory.Add(item, c);
                }
            });
            thread.Start();
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
            var c = SelectCategory.ID;
            var o = Categories.First(x => x.ID == c);
            o.IsBaodi = true;
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
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (var item in Categories)
            {
                if (item.ID == -1)
                {
                    item.GUID = Guid.NewGuid().ToString();
                    item.ID = SQLHelper.UpdateOrAddCategory(item, true);
                    count++;
                }
                else
                    SQLHelper.UpdateOrAddCategory(item);
            }
            sw.Stop();
            Console.WriteLine($"检索目录耗时：{sw.ElapsedMilliseconds} ms");
            if (count > 0)
            {
                Helper.ShowGrowlMsg($"在数据库新建了 {count} 个目录");
                count = 0;
            }
            sw.Restart();
            foreach (var item in GachaitemsInCategory)
            {
                int index = 0;
                foreach (var items in item.Value)
                {
                    if (items.ItemID == -1)
                    {
                        items.GUID = Guid.NewGuid().ToString();
                        items.ItemID = SQLHelper.UpdateOrAddGachaItem(items);
                        item.Key.Content[index] = items.ItemID;
                        count++;
                    }
                    else if (items.Editted)
                    {
                        SQLHelper.UpdateOrAddGachaItem(items);
                        items.Editted = false;
                    }
                    index++;
                }
                SQLHelper.UpdateOrAddCategory(item.Key);
            }
            sw.Stop();
            Console.WriteLine($"更新已更改项目耗时：{sw.ElapsedMilliseconds} ms");
            if (count > 0)
            {
                Helper.ShowGrowlMsg($"在数据库新建了 {count} 个子项目");
            }
            sw.Restart();
            EditPool.Content = Categories.Select(x => x.ID).ToList();
            SQLHelper.UpdatePool(EditPool);
            sw.Stop();
            Console.WriteLine($"更新卡池耗时：{sw.ElapsedMilliseconds} ms");
            sw.Restart();
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
            sw.Stop();
            Console.WriteLine($"启用插件耗时：{sw.ElapsedMilliseconds} ms");
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
            GachaItems.Add(c);
            SelectGachaItem = c;
            SelectCategory.Content.Add(c.ItemID);
            GachaitemsInCategory[SelectCategory].Add(c);
            ReloadItems();
            RaisePropertyChanged("GachaItems");
            Helper.ShowGrowlMsg($"成功新建了一个模板项目");
        }
        public DelegateCommand DeleteItem { get; set; }

        private void deleteItem(object parameter)
        {
            if (parameter is List<GachaItem> ls)
            {
                if (HandyControl.Controls.MessageBox.Ask("确认删除选中的项目吗？此操作只会将项目从目录中剔除，不影响数据库", "提示") == MessageBoxResult.Cancel)
                    return;
                ls.Do(x =>
                { 
                    SelectCategory.Content.Remove(x.ItemID);
                    SelectCategory.UpContent.Remove(x.ItemID);
                    GachaitemsInCategory[SelectCategory].Remove(x);
                });
                Helper.ShowGrowlMsg($"从 {SelectCategory.Name} 目录中成功删除了 {ls.Count} 个子项目");
            }
            else
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
                Helper.ShowGrowlMsg($"从 {SelectCategory.Name} 目录中成功删除了子项目 {name}");
            }
            ReloadItems();
            SelectGachaItem = new GachaItem();
        }
        public DelegateCommand DeleteItemFromDB { get; set; }
        private void deleteItemFromDB(object parameter)
        {
            if (parameter is List<GachaItem> ls)
            {
                if (HandyControl.Controls.MessageBox.Ask("确认从数据库中删除选中的项目吗？此操作不可逆！", "提示") == MessageBoxResult.Cancel)
                    return;
                SQLHelper.RemoveGachaItems(ls);
                ls.Do(x => GachaItems.Remove(x));
                SQLHelper.UpdatePool(EditPool);
                Helper.ShowGrowlMsg($"从数据库中成功删除了 {ls.Count} 个目录");
            }
            else
            {
                if (HandyControl.Controls.MessageBox.Ask("确认从数据库中删除此项目吗？此操作不可逆！", "提示") == MessageBoxResult.Cancel)
                    return;
                string name = SelectGachaItem.Name;
                SQLHelper.RemoveGachaItem(SelectGachaItem);
                Helper.ShowGrowlMsg($"从数据库中成功删除了项目 {name}");
            }
            ReloadItems();
        }

        public DelegateCommand DeleteCategoryFromDB { get; set; }
        private void deleteCategoryFromDB(object parameter)
        {
            if (parameter is List<Category> ls)
            {
                if (HandyControl.Controls.MessageBox.Ask("确认从数据库中删除选中的目录吗？此操作不可逆！删除不会影响目录的内容", "提示") == MessageBoxResult.Cancel)
                    return;
                SQLHelper.RemoveCategoryByIDs(ls.Select(x => x.ID).ToList());
                ls.Do(x =>
                {
                    EditPool.Content.Remove(x.ID);
                    GachaitemsInCategory.Remove(x);
                    Categories.Remove(x);
                });
                SelectCategory = new Category();
                GachaItems.Clear();
                Helper.ShowGrowlMsg($"从数据库中成功删除了 {ls.Count} 个目录");
            }
            else
            {
                if (HandyControl.Controls.MessageBox.Ask("确认从数据库中删除此目录吗？此操作不可逆！删除不会影响目录的内容", "提示") == MessageBoxResult.Cancel)
                    return;
                string name = selectCategory.Name;
                SQLHelper.RemoveCategory(SelectCategory);
                GachaitemsInCategory.Remove(SelectCategory);
                Categories.Remove(SelectCategory);
                SelectCategory = new Category();
                GachaItems.Clear();
                EditPool.Content.Remove(SelectCategory.ID);
                Helper.ShowGrowlMsg($"从数据库中成功删除了目录 {name}");
            }
            SQLHelper.UpdatePool(EditPool);
            ReloadCategroies();
        }
        public DelegateCommand CopyItem { get; set; }
        private void copyItem(object parameter)
        {
            var c = SelectGachaItem.Clone();
            int index = GachaItems.IndexOf(SelectGachaItem);
            GachaItems.Insert(index, c);
            c.ItemID = -1;
            c.GUID = Guid.NewGuid().ToString();
            SelectCategory.Content.Add(c.ItemID);
            GachaitemsInCategory[SelectCategory].Add(c);
            SelectGachaItem = c;
            if (c.IsUp)
            {
                setItemUp(null);
            }
            RaisePropertyChanged("GachaItems");
            ReloadItems();

            if (parameter == null)
            {
                Helper.ShowGrowlMsg($"成功复制 {c.Name} 子项目");
            }
        }
        public DelegateCommand TemplateCopyItem { get; set; }
        private void templateCopyItem(object parameter)
        {
            string baseDir = Path.Combine(EditPool.RelativePath, SelectGachaItem.ImagePath);
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Multiselect = true,
                InitialDirectory = new FileInfo(baseDir).DirectoryName,
                IsFolderPicker = false,
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Cancel)
                return;
            foreach (var item in dialog.FileNames)
            {
                FileInfo info = new FileInfo(item);
                var c = SelectGachaItem.Clone();
                int index = GachaItems.IndexOf(SelectGachaItem);
                GachaItems.Insert(index, c);
                c.ItemID = -1;
                c.GUID = Guid.NewGuid().ToString();
                c.Name = info.Name.Replace(info.Extension, "");
                c.ImagePath = info.FullName.Replace(EditPool.RelativePath + "\\", "");
                SelectCategory.Content.Add(c.ItemID);
                GachaitemsInCategory[SelectCategory].Add(c);
                SelectGachaItem = c;
                if (c.IsUp)
                {
                    setItemUp(null);
                }
            }
        }
        public DelegateCommand MultiCopyItem { get; set; }
        private void multicopyItem(object parameter)
        {
            int copyNum = 20;
            for (int i = 0; i < copyNum; i++)
            {
                var c = SelectGachaItem.Clone();
                int index = GachaItems.IndexOf(SelectGachaItem);
                GachaItems.Insert(index, c);
                c.ItemID = -1;
                c.GUID = Guid.NewGuid().ToString();
                SelectCategory.Content.Add(c.ItemID);
                GachaitemsInCategory[SelectCategory].Add(c);
                SelectGachaItem = c;
                if (c.IsUp)
                {
                    setItemUp(null);
                }
            }
            RaisePropertyChanged("GachaItems");
            ReloadItems();
            Helper.ShowGrowlMsg($"成功复制 {SelectGachaItem.Name} 子项目 {copyNum} 次");
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
            ReloadItems();
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

            Helper.ShowGrowlMsg($"已将 {SelectCategory} 目录的列表清空", Helper.NoticeEnum.Info);
        }

        public DelegateCommand ForeConfigDialog { get; set; }
        public DelegateCommand NewPoolDialog { get; set; }
        public DelegateCommand QueryItemDialog { get; set; }
        public DelegateCommand OpenNewCategoryDialog { get; set; }
        public DelegateCommand SetUpContentDialog { get; set; }
        private void ShowInteractiveDialog(object peremeter)
        {
            switch ((Model.WorkBenchModel.DialogAction)peremeter)
            {
                case Model.WorkBenchModel.DialogAction.NewCategory:
                    Dialog.Show<NewCategoryPage>()
                    .Initialize<NewPoolViewModel>(vm =>
                    {
                        vm.NowCategory = new Category
                        {
                            Name = "新目录",
                            Probablity = 0.5,
                            IsBaodi = false,
                            GUID = Guid.NewGuid().ToString(),
                            ID = -1
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
                        });
                    });
                    break;
                case Model.WorkBenchModel.DialogAction.SetUpContent:
                    Dialog.Show<GachaItemQueryDialog>()
                    .Initialize<GachaItemQueryDialogViewModel>(vm =>
                    {
                        GachaItems.ToList().ForEach(x => vm.GachaItems.Add(x));
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
                            GachaitemsInCategory[SelectCategory].ForEach(c => c.IsUp = false);
                            int count = 0;
                            x.Result.ForEach(o =>
                            {
                                if (SelectCategory.UpContent.Any(c => c == o.ItemID) is false)
                                {
                                    SelectCategory.UpContent.Add(o.ItemID);
                                    GachaitemsInCategory[SelectCategory].First(c => c.ItemID == o.ItemID).IsUp = true;
                                    count++;
                                }
                            });
                            ReloadItems();
                            this.RaisePropertyChanged("GachaItems");
                            Helper.ShowGrowlMsg($"共设置了 {count} 个Up项");
                        });
                    });
                    break;
                case Model.WorkBenchModel.DialogAction.EditCategory:
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
                                var c = Categories.First(o => o.GUID == x.Result.GUID);
                                Categories.Insert(Categories.IndexOf(c), x.Result);
                                Categories.Remove(c);
                                GachaitemsInCategory.Remove(c);
                                GachaitemsInCategory.Add(x.Result, SQLHelper.GetGachaItemsByIDs(x.Result.Content));
                                SelectCategory = x.Result;
                                ReloadCategroies();
                            }
                        });
                    });
                    break;
                case Model.WorkBenchModel.DialogAction.QueryGachaItem:
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
                                        SelectCategory.Content.Add(o.ItemID);
                                        ReloadItems();
                                        count++;
                                    }
                                });
                                Helper.ShowGrowlMsg($"共添加了 {count} 个项");
                            }
                        });
                    });
                    break;
                case Model.WorkBenchModel.DialogAction.ForeConfig:
                    Dialog.Show<ForeConfig>().Initialize<WorkbenchViewModel>(c =>
                    {
                        c.Config = Config.Clone();
                        c.OrderConfig = OrderConfig.Clone();
                    });
                    break;
                case Model.WorkBenchModel.DialogAction.WorkBenchConfig:
                    break;
                case Model.WorkBenchModel.DialogAction.NewPool:
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
            var c = GachaCore.DoGacha(EditPool, EditPool.MultiGachaNumber, ref baodiCount);
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
        public DelegateCommand ExportPool { get; set; }
        private void exportPool(object peremeter)
        {
            if (HandyControl.Controls.MessageBox.Ask("确认导出此池吗？请确认所有的编辑工作已经保存", "提示") == MessageBoxResult.Cancel)
                return;
            JObject json = new JObject
            {
                new JProperty("Editor_Version", Helper.GetVersionOrDefault()),
                new JProperty("Pool_Info",JsonConvert.SerializeObject(EditPool)),
                new JProperty("Categories_Info",new JArray()),
                new JProperty("Items_Info",new JArray())
            };
            foreach (var item in Categories)
            {
                JObject category = new JObject
                {
                    new JProperty("ID",item.ID),
                    new JProperty("Content",JsonConvert.SerializeObject(item))
                };
                (json["Categories_Info"] as JArray).Add(category);
                foreach (var gachaitem in GachaitemsInCategory[item])
                {
                    JObject tmp = new JObject
                    {
                        new JProperty("ID",gachaitem.ItemID),
                        new JProperty("Content", JsonConvert.SerializeObject(gachaitem))
                    };
                    (json["Items_Info"] as JArray).Add(tmp);
                }
            }
            string dirPath = Path.Combine(MainSave.AppDirectory, "Export", $"{EditPool.Name}_{EditPool.PoolID}");
            string jsonPath = Path.Combine(dirPath, $"{EditPool.Name}.json");
            Directory.CreateDirectory(dirPath);
            File.WriteAllText(jsonPath, json.ToString());
            Process.Start(dirPath);
            HandyControl.Controls.MessageBox.Show("保存完成");
        }
        public DelegateCommand ImportPool { get; set; }
        private void importPool(object peremeter)
        {
            if (HandyControl.Controls.MessageBox.Ask("确认导入吗？请确认所有的编辑工作已经保存", "提示") == MessageBoxResult.Cancel)
                return;
            EditPool = new Pool { Name = "未选择项目", PoolID = -1 };
            categories.Clear();
            GachaitemsInCategory.Clear();
            string filePath = ShowSelectJsonDialog(MainSave.AppDirectory);
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Helper.ShowGrowlMsg("操作已取消", Helper.NoticeEnum.Error);
                return;
            }
            try
            {
                JObject json = JObject.Parse(File.ReadAllText(filePath));
                if (json.ContainsKey("Editor_Version") && json["Editor_Version"].ToObject<int>() > Helper.GetVersionOrDefault())
                {
                    if (HandyControl.Controls.MessageBox.Ask("此卡池的版本似乎高于当前编辑器版本, 可能导致导入失败, 建议升级插件版本. 确实继续导入吗?", "提示") == MessageBoxResult.Cancel)
                        return;
                }
                string relativePath = ShowSelectDirDialog();
                if (string.IsNullOrWhiteSpace(relativePath))
                {
                    Helper.ShowGrowlMsg("操作已取消", Helper.NoticeEnum.Error);
                    return;
                }
                Helper.ShowGrowlMsg("相对目录读取成功");
                if (VeifyJson(relativePath, json) is false)//选择的相对路径与json内填写的相对路径不匹配
                {
                    Helper.ShowGrowlMsg("路径验证失败，请检查相对目录是否设置正确", Helper.NoticeEnum.Error);
                    return;
                }
                Pool pool_dest = JsonConvert.DeserializeObject<Pool>(json["Pool_Info"].ToString());
                bool updateFlag = false;
                if (MainSave.PoolInstances.Any(x => x.Name == pool_dest.Name))//判断同名卡池, 同名可选择覆盖或不冲突命名
                {
                    if (HandyControl.Controls.MessageBox.Ask("似乎现在已经有一个相同名称的卡池了，点是来覆盖卡池, 点否来重命名卡池", "提示") == MessageBoxResult.OK)
                    {
                        updateFlag = true;
                    }
                    else
                    {
                        while (MainSave.PoolInstances.Any(x => x.Name == pool_dest.Name))
                            pool_dest.Name += "_2";
                    }
                }
                pool_dest.PoolID = updateFlag ? MainSave.PoolInstances.First(x => x.Name == pool_dest.Name).PoolID : -1;
                //导入目录, 先取出所有目录
                Dictionary<int, Category> categories = new Dictionary<int, Category>();
                foreach (var item in json["Categories_Info"] as JArray)
                {
                    Category category = JsonConvert.DeserializeObject<Category>(item["Content"].ToString());
                    category.ID = -1;
                    categories.Add(Convert.ToInt32(item["ID"]), category);
                }
                //导入所有项目, 先取出所有项目
                Dictionary<int, GachaItem> gachaItems = new Dictionary<int, GachaItem>();
                foreach (var item in json["Items_Info"] as JArray)
                {
                    GachaItem gachaItem = JsonConvert.DeserializeObject<GachaItem>(item["Content"].ToString());
                    gachaItem.ItemID = -1;
                    gachaItems.Add(Convert.ToInt32(item["ID"].ToString()), gachaItem);
                }
                Helper.ShowGrowlMsg("Json读取成功");
                //按GUID更新或添加所有的项目
                gachaItems.Do(x=> { if (string.IsNullOrWhiteSpace(x.Value.GUID)) { x.Value.GUID = Guid.NewGuid().ToString(); } });
                gachaItems = SQLHelper.UpdateIDByGUID(gachaItems);
                //更新目录内容, 主要是项目的ID
                foreach (var item in pool_dest.Content)//需要导入卡池的目录
                {
                    //更新需要导入卡池目录中的项目ID, 因为项目已经导入了数据库并有了编号
                    List<int> itemID = new List<int>();
                    foreach (var id in categories[item].Content)
                    {
                        itemID.Add(gachaItems[id].ItemID);//按旧卡池的键, 读入项目
                    }
                    //同理, 更新UP内容
                    List<int> upID = new List<int>();
                    foreach (var id in categories[item].UpContent)
                    {
                        try
                        {
                            upID.Add(gachaItems[id].ItemID);
                        }
                        catch (KeyNotFoundException) { }
                    }
                    if (string.IsNullOrWhiteSpace(categories[item].GUID))
                        categories[item].GUID = Guid.NewGuid().ToString(); 
                    categories[item].Content = itemID;
                    categories[item].UpContent = upID;
                    categories[item].ID = SQLHelper.UpdateOrAddCategory(categories[item]);
                }
                List<int> contentID = new List<int>();
                foreach (var item in categories)
                {
                    contentID.Add(item.Value.ID);
                }
                //将卡池的目录也进行ID的更新
                if (string.IsNullOrWhiteSpace(pool_dest.GUID))
                    pool_dest.GUID = Guid.NewGuid().ToString();
                pool_dest.Content = contentID;
                pool_dest.RelativePath = relativePath;
                if (updateFlag)
                {
                    pool_dest.PoolID = SQLHelper.GetAllPools().First(x => x.GUID == pool_dest.GUID).PoolID;
                    SQLHelper.UpdatePool(pool_dest); 
                }
                else
                    pool_dest.PoolID = SQLHelper.AddPool(pool_dest);
                EditPool = pool_dest;
                LoadCategories();
                Helper.ShowGrowlMsg($"导入卡池 {EditPool.Name} 成功, 重启编辑器以测试卡池");
            }
            catch (Exception e)
            {
                //Helper.ShowGrowlMsg("Json读取失败，请验证Json格式", Helper.NoticeEnum.Error);
                Helper.ShowGrowlMsg($"错误信息: {e.Message}", Helper.NoticeEnum.Error);
                return;
            }
        }
        public DelegateCommand CheckUpdate => new Lazy<DelegateCommand>(() =>
                    new DelegateCommand(checkUpdate)).Value;
        public void checkUpdate(object peremeter)
        {
            Helper.ShowGrowlMsg("开始拉取更新信息");
            Thread thread = new Thread(()=>
            {
                HttpWebClient http = new HttpWebClient
                {
                    Encoding = Encoding.UTF8
                };
                string json = http.DownloadString(MainSave.UpdateURL);
                UpdateInfo info = JsonConvert.DeserializeObject<UpdateInfo>(json);
                int versionID = Helper.GetVersionOrDefault();
                if (info.Plugin.Versions[info.Plugin.Versions.Length - 1].VersionID == versionID || info.Plugin.Versions.Any(x => x.VersionID == versionID) is false)
                {
                    Helper.ShowGrowlMsg("当前版本已经为最新版本，感谢你的使用 :)");
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("检测到新版本, 信息如下");
                    foreach (var item in info.Plugin.Versions.Where(x => x.VersionID > versionID))
                    {
                        sb.AppendLine($"版本: {Helper.ParseVerID2String(item.VersionID)}");
                        sb.AppendLine($"更新时间: {item.UpdateTime:G}");
                        sb.AppendLine($"更新内容: {item.Info}");
                        sb.AppendLine("--------------------");
                    }
                    sb.AppendLine("要前往下载页面更新吗？");
                    if (HandyControl.Controls.MessageBox.Ask(sb.ToString(), "发现新版本") == MessageBoxResult.OK)
                    {
                        Process.Start(info.Plugin.URL);
                    }
                }
            });
            thread.Start();
        }
        #endregion
        private bool VeifyJson(string relativePath, JObject json)
        {
            var array = (json["Items_Info"] as JArray);
            if (array.Count < 1)
                return true;
            var c = JObject.Parse(array[0]["Content"].ToString());
            string path = Path.Combine(relativePath, c["ImagePath"].ToString());
            return File.Exists(path);
        }
        private string ShowSelectJsonDialog(string openDir = "")
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Multiselect = false,
                Title = "请选择需要导入的Json"
            };
            dialog.InitialDirectory = openDir;
            dialog.Filters.Add(new CommonFileDialogFilter("Json", "*.json"));
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            else
            {
                return string.Empty;
            }
        }
        private string ShowSelectDirDialog()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Multiselect = false,
                Title = "请选择仓库迁移后 图片所在的相对根目录",
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            else
            {
                return string.Empty;
            }
        }
        private void ReloadCategroies()
        {
            var c = new ObservableCollection<Category>();
            foreach (var item in Categories)
            {
                c.Add(item);
            }
            Categories = c;
            RaisePropertyChanged("Categories");
        }
        private void ReloadItems()
        {
            var c = new ObservableCollection<GachaItem>();
            foreach (var item in GachaitemsInCategory[SelectCategory])
            {
                c.Add(item);
            }
            GachaItems = c;
            RaisePropertyChanged("GachaItems");
        }
        public enum OpenType
        {
            Normal,
            NoPool,
            NewPool
        }
    }
}
