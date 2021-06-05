using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;
using PublicInfos;
using static me.cqp.luohuaming.CustomGacha.UI.ViewModel.GachaItemQueryDialogViewModel;

namespace me.cqp.luohuaming.CustomGacha.UI.View
{
    /// <summary>
    /// GachaItemQueryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class GachaItemQueryDialog
    {
        public GachaItemQueryDialog()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            InitializeComponent();
            sw.Stop();
            Debug.WriteLine(sw.ElapsedMilliseconds);
        }
        GachaItemQueryDialogViewModel datacontext;
        public static DataGrid DataGrid_Export { get; set; }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Tag)
            {
                case "AllSelect":
                    foreach (var item in datacontext.QueryItems)
                    {
                        item.IsSelected = true;
                    }
                    ReloadItems();
                    break;
                case "NonSelect":
                    foreach (var item in datacontext.QueryItems)
                    {
                        item.IsSelected = false;
                    }
                    ReloadItems();
                    break;
                case "AntiSelect":
                    foreach (var item in datacontext.QueryItems)
                    {
                        item.IsSelected = !item.IsSelected;
                    }
                    ReloadItems();
                    break;
                case "DeleteFromDB":
                    if (HandyControl.Controls.MessageBox.Ask("确认从数据库中删除此项目吗？此操作不可逆！", "提示") == MessageBoxResult.Cancel)
                        return;
                    for (int i = 0; i < DataGrid_Main.SelectedItems.Count; i++)
                    {
                        VMArray item = DataGrid_Main.SelectedItems[0] as VMArray;
                        datacontext.GachaItems.Remove(item);
                        datacontext.MaxPageCount = (int)Math.Ceiling(datacontext.GachaItems.Count / (double)datacontext.PageCount);
                        datacontext.QueryItems = Helper.ToPageList(datacontext.GachaItems, datacontext.PageIndex, datacontext.PageCount);
                        SQLHelper.RemoveGachaItem(item.Object);
                    }
                    ReloadItems();
                    break;
                default:
                    break;
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ((GachaItemQueryDialogViewModel)this.DataContext).Result = datacontext.GachaItems.Where(x => x.IsSelected).Select(x => x.Object).ToList();
            ((GachaItemQueryDialogViewModel)this.DataContext).CloseCmd.Execute(null);
        }
        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            ((GachaItemQueryDialogViewModel)this.DataContext).Result = null;
            ((GachaItemQueryDialogViewModel)this.DataContext).CloseCmd.Execute(null);
        }
        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid_Export = DataGrid_Main;
            datacontext = (GachaItemQueryDialogViewModel)this.DataContext;
            datacontext.QueryItems = Helper.ToPageList(datacontext.GachaItems, datacontext.PageIndex, datacontext.PageCount);
            datacontext.MaxPageCount = (int)Math.Ceiling(datacontext.GachaItems.Count / (double)datacontext.PageCount);
            if (datacontext.OpenMode == "Query")
            {
                var c = datacontext.QueryItems.Where(x => datacontext.Result.Any(o => x.Object.ItemID == o.ItemID)).ToList();
                foreach (var item in c)
                {
                    item.IsSelected = true;
                }
            }
            else if (datacontext.OpenMode == "SelectUp")
            {
                var c = datacontext.GachaItems.Where(x => datacontext.UpContent.Any(o => x.Object.ItemID == o)).ToList();
                foreach (var item in c)
                {
                    item.IsSelected = true;
                }
            }
        }
        ObservableCollection<VMArray> arrayBackup = null;
        private void SearchBar_SearchStarted(object sender, HandyControl.Data.FunctionEventArgs<string> e)
        {
            string key = e.Info;
            if (string.IsNullOrWhiteSpace(key))
            {
                datacontext.GachaItems = arrayBackup;
                arrayBackup = null;
                datacontext.PageIndex = 1;
                datacontext.MaxPageCount = (int)Math.Ceiling(datacontext.GachaItems.Count / (double)datacontext.PageCount);
                datacontext.QueryItems = Helper.ToPageList(datacontext.GachaItems, datacontext.PageIndex, datacontext.PageCount);
            }
            else
            {
                if (arrayBackup == null)
                {
                    arrayBackup = new ObservableCollection<VMArray>();
                    foreach (var item in datacontext.GachaItems)
                        arrayBackup.Add(item);
                }
                ObservableCollection<VMArray> c = new ObservableCollection<VMArray>();
                foreach (var item in arrayBackup)
                {
                    if (item.Object.Name.Contains(key) ||
                        item.Object.ItemID.ToString().Contains(key) ||
                        item.Object.Probablity.ToString().Contains(key) ||
                        item.Object.UpProbablity.ToString().Contains(key))
                    {
                        c.Add(item);
                    }
                }
                datacontext.GachaItems = c;
                datacontext.PageIndex = 1;
                datacontext.MaxPageCount = (int)Math.Ceiling(datacontext.GachaItems.Count / (double)datacontext.PageCount);
                datacontext.QueryItems = Helper.ToPageList(datacontext.GachaItems, datacontext.PageIndex, datacontext.PageCount);
            }
        }
        public void ReloadItems()
        {
            var c = new ObservableCollection<VMArray>();
            foreach (var item in datacontext.QueryItems)
            {
                c.Add(item);
            }
            datacontext.QueryItems = c;
        }
    }
}
