using System.Collections.Generic;
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
        //TODO: 分页控件请求
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Tag)
            {
                case "AllSelect":
                    for (int i = 0; i < DataGrid_Main.Items.Count; i++)
                    {
                        var c = ((DataGridRow)DataGrid_Main.ItemContainerGenerator.ContainerFromIndex(i));
                        if (c.IsVisible)
                            c.IsSelected = true;
                    }
                    break;
                case "NonSelect":
                    for (int i = 0; i < DataGrid_Main.Items.Count; i++)
                    {
                        var c = ((DataGridRow)DataGrid_Main.ItemContainerGenerator.ContainerFromIndex(i));
                        if (c.IsVisible)
                            c.IsSelected = false;
                    }
                    break;
                case "AntiSelect":
                    for (int i = 0; i < DataGrid_Main.Items.Count; i++)
                    {
                        var c = ((DataGridRow)DataGrid_Main.ItemContainerGenerator.ContainerFromIndex(i));
                        if (c.IsVisible)
                            c.IsSelected = !c.IsSelected;
                    }
                    break;
                case "DeleteFromDB":
                    if (HandyControl.Controls.MessageBox.Ask("确认从数据库中删除此项目吗？此操作不可逆！", "提示") == MessageBoxResult.Cancel)
                        return;

                    break;
                default:
                    break;
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            List<GachaItem> c = new List<GachaItem>();
            for (int i = 0; i < DataGrid_Main.Items.Count; i++)
            {
                if (((DataGridRow)DataGrid_Main.ItemContainerGenerator.ContainerFromIndex(i)).IsSelected)
                    c.Add(DataGrid_Main.Items[i] as GachaItem);
            }
            ((GachaItemQueryDialogViewModel)this.DataContext).Result = c;
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
            if (datacontext.OpenMode == "Query")
            {
                List<int> c = datacontext.GachaItems.Where(x => datacontext.Result.Any(o => x.ItemID == o.ItemID))
                                               .Select(x => datacontext.GachaItems.IndexOf(x)).ToList();
                SelectItemsByIDs(c);
            }
            else if (datacontext.OpenMode == "SelectUp")
            {
                List<int> c = datacontext.GachaItems.Where(x => datacontext.UpContent.Any(o => x.ItemID == o))
                                                            .Select(x => datacontext.GachaItems.IndexOf(x)).ToList();
                SelectItemsByIDs(c);
            }
        }
        public static void SelectItemsByIDs(List<int> ids)
        {
            foreach (var item in ids)
            {
                //((DataGridRow)DataGrid_Export.ItemContainerGenerator.ContainerFromIndex(item)).IsSelected = true;
            }
        }
        private void SearchBar_SearchStarted(object sender, HandyControl.Data.FunctionEventArgs<string> e)
        {
            string key = e.Info;
            for (int i = 0; i < DataGrid_Main.Items.Count; i++)
            {
                ((DataGridRow)DataGrid_Main.ItemContainerGenerator.ContainerFromIndex(i)).Visibility = Visibility.Collapsed;
            }
            for (int i = 0; i < DataGrid_Main.Items.Count; i++)
            {
                GachaItem item = (GachaItem)DataGrid_Main.Items[i];
                if (item.Name.Contains(key) ||
                    item.ItemID.ToString().Contains(key) ||
                    item.Probablity.ToString().Contains(key) ||
                    item.UpProbablity.ToString().Contains(key))
                {
                    ((DataGridRow)DataGrid_Main.ItemContainerGenerator.ContainerFromIndex(i)).Visibility = Visibility.Visible;
                }
            }
        }
    }
}
