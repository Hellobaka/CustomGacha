using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;
using PublicInfos;

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
                        item.IsChecked = true;
                    }
                    ReloadItems();
                    break;
                case "NonSelect":
                    foreach (var item in datacontext.QueryItems)
                    {
                        item.IsChecked = false;
                    }
                    ReloadItems();
                    break;
                case "AntiSelect":
                    foreach (var item in datacontext.QueryItems)
                    {
                        item.IsChecked = !item.IsChecked;
                    }
                    ReloadItems();
                    break;
                case "DeleteFromDB":
                    if (HandyControl.Controls.MessageBox.Ask("确认从数据库中删除此项目吗？此操作不可逆！", "提示") == MessageBoxResult.Cancel)
                        return;
                    for (int i = 0; i < DataGrid_Main.SelectedItems.Count; i++)
                    {
                        GachaItem item = DataGrid_Main.SelectedItems[0] as GachaItem;
                        datacontext.GachaItems.Remove(item);
                        datacontext.MaxPageCount = (int)Math.Ceiling(datacontext.GachaItems.Count / (double)datacontext.PageCount);
                        datacontext.QueryItems = Helper.ToPageList(datacontext.GachaItems, datacontext.PageIndex, datacontext.PageCount);
                        SQLHelper.RemoveGachaItem(item);
                    }
                    ReloadItems();
                    break;
                default:
                    break;
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ((GachaItemQueryDialogViewModel)this.DataContext).Result = datacontext.GachaItems.Where(x => x.IsChecked).ToList();
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
                var c = datacontext.QueryItems.Where(x => datacontext.Result.Any(o => x.ItemID == o.ItemID)).ToList();
                foreach (var item in c)
                {
                    item.IsChecked = true;
                }
            }
            else if (datacontext.OpenMode == "SelectUp")
            {
                var c = datacontext.GachaItems.Where(x => datacontext.UpContent.Any(o => x.ItemID == o)).ToList();
                foreach (var item in c)
                {
                    item.IsChecked = true;
                }
            }
        }
        ObservableCollection<GachaItem> arrayBackup = null;
        private void SearchBar_SearchStarted(object sender, HandyControl.Data.FunctionEventArgs<string> e)
        {
            string key = e.Info;
            if (string.IsNullOrWhiteSpace(key))
            {
                if (arrayBackup == null)
                {
                    return;
                }
                arrayBackup = Helper.List2ObservableCollection(arrayBackup.OrderByDescending(x => x.IsChecked).ToList());
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
                    arrayBackup = new ObservableCollection<GachaItem>();
                    foreach (var item in datacontext.GachaItems)
                        arrayBackup.Add(item);
                }
                ObservableCollection<GachaItem> c = new ObservableCollection<GachaItem>();
                foreach (var item in arrayBackup)
                {
                    if (item.Name.Contains(key) ||
                        item.ItemID.ToString().Contains(key) ||
                        item.Probablity.ToString().Contains(key) ||
                        item.UpProbablity.ToString().Contains(key))
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
            var c = new ObservableCollection<GachaItem>();
            foreach (var item in datacontext.QueryItems)
            {
                c.Add(item);
            }
            datacontext.QueryItems = c;
        }

        private void DataGrid_Main_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            datacontext.SelectNum = datacontext.GachaItems.Count(x=>x.IsChecked);
        }
    }
}
