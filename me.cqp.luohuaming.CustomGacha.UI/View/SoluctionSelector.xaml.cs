using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using me.cqp.luohuaming.CustomGacha.UI.Model;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.View
{
    /// <summary>
    /// SoluctionSelector.xaml 的交互逻辑
    /// </summary>
    public partial class SoluctionSelector : Window
    {
        public static bool ExitFlag = false;
        public SoluctionSelector()
        {
            InitializeComponent();
            SQLHelper.LoadConfig();
            SoluctionSelector_Export = this;
        }
        public static SoluctionSelector SoluctionSelector_Export;
        private void OrderButtonPressed(object sender, SelectionChangedEventArgs e)
        {
            ButtonItem item = (ButtonItem)(sender as ListBox).SelectedItem;
            if (item is ButtonItem && item.Action != null)
                item.Action.Execute(null);
            (sender as ListBox).SelectedItem = null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ExitFlag is false)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ((SoluctionSelectorViewModel)(DataContext)).ReloadList();
            this.Show();
        }

        private void HideMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                Thread.Sleep(50);
                poolSelector.Dispatcher.Invoke(() =>
                {
                    object c = poolSelector.SelectedItem;
                    if (c is RecentSoluction)
                    {
                        //新建窗口
                        Workbench fm = new Workbench();
                        WorkbenchViewModel vm = (fm.DataContext as WorkbenchViewModel);
                        vm.EditPool = (c as RecentSoluction).Object;
                        vm.Config = MainSave.ApplicationConfig.Clone();
                        vm.OrderConfig = MainSave.OrderConfig.Clone();
                        fm.InitializeComponent();
                        fm.Show();
                        this.Hide();
                    }
                    poolSelector.SelectedItem = null;
                });
            });
            thread.Start();
        }
    }
}
