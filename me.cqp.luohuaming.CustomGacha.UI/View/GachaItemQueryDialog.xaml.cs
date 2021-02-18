using System.Collections.Generic;
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
            InitializeComponent();
            this.DataContext = new GachaItemQueryDialogViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Tag)
            {
                case "AllSelect":
                    for (int i = 0; i < DataGrid_Main.Items.Count; i++)
                        ((DataGridRow)DataGrid_Main.ItemContainerGenerator.ContainerFromIndex(i)).IsSelected = true;
                    break;
                case "NonSelect":
                    for (int i = 0; i < DataGrid_Main.Items.Count; i++)
                        ((DataGridRow)DataGrid_Main.ItemContainerGenerator.ContainerFromIndex(i)).IsSelected = false;
                    break;
                case "AntiSelect":
                    for (int i = 0; i < DataGrid_Main.Items.Count; i++)
                    {
                        var c = ((DataGridRow)DataGrid_Main.ItemContainerGenerator.ContainerFromIndex(i));
                        c.IsSelected = !c.IsSelected;
                    }
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

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            var c = ((GachaItemQueryDialogViewModel)this.DataContext).Result;
            foreach (var item in c)
            {
                for (int i = 0; i < DataGrid_Main.Items.Count; i++)
                {
                    if ((DataGrid_Main.Items[i] as GachaItem).ItemID == item.ItemID)
                        ((DataGridRow)DataGrid_Main.ItemContainerGenerator.ContainerFromIndex(i)).IsSelected = true;
                }
            }
        }
    }
}
