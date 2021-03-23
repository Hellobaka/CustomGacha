using System;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using MahApps.Metro.IconPacks;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.View
{
    /// <summary>
    /// Workbench.xaml 的交互逻辑
    /// </summary>
    public partial class Workbench : System.Windows.Window
    {
        public Workbench()
        {
            PackIconCodicons o = new PackIconCodicons();
            PackIconMaterial oo = new PackIconMaterial();
            PackIconUnicons ooo = new PackIconUnicons();
            InitializeComponent();
        }
        WorkbenchViewModel datacontext;
        public static System.Windows.Controls.TabControl contentTab_Export;
        private void Frame_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Page page = e.Content as Page;
            page.DataContext = this.DataContext;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            contentTab_Export = contentTab;
            datacontext = DataContext as WorkbenchViewModel;
            switch (datacontext.DialogType)
            {
                case WorkbenchViewModel.OpenType.NoPool:
                    contentTab.IsEnabled = false;
                    break;
                case WorkbenchViewModel.OpenType.NewPool:
                    contentTab.IsEnabled = false;
                    datacontext.NewPoolDialog.Execute(false);
                    break;
                default:
                    break;
            }
        }
    }
}
