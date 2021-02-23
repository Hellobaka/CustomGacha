﻿using System.Collections.Generic;
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
            var c = (GachaItemQueryDialogViewModel)this.DataContext;
            if (c.OpenMode == "Query")
            {
                foreach (var item in c.Result)
                {
                    for (int i = 0; i < DataGrid_Main.Items.Count; i++)
                    {
                        if ((DataGrid_Main.Items[i] as GachaItem).ItemID == item.ItemID)
                            ((DataGridRow)DataGrid_Main.ItemContainerGenerator.ContainerFromIndex(i)).IsSelected = true;
                    }
                }
            }
            else if (c.OpenMode == "SelectUp")
            {
                foreach (var item in c.UpContent)
                {
                    for (int i = 0; i < DataGrid_Main.Items.Count; i++)
                    {
                        if ((DataGrid_Main.Items[i] as GachaItem).ItemID == item)
                            ((DataGridRow)DataGrid_Main.ItemContainerGenerator.ContainerFromIndex(i)).IsSelected = true;
                    }
                }
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
