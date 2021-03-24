using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using MahApps.Metro.IconPacks;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;

namespace me.cqp.luohuaming.CustomGacha.UI.View
{
    /// <summary>
    /// Workbench.xaml 的交互逻辑
    /// </summary>
    public partial class Workbench : System.Windows.Window
    {
        public Workbench()
        {
            Application.ResourceAssembly = Assembly.Load("me.cqp.luohuaming.CustomGacha.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            PackIconCodicons o = new PackIconCodicons();
            PackIconMaterial oo = new PackIconMaterial();
            PackIconUnicons ooo = new PackIconUnicons();
            InitializeComponent();
        }
        WorkbenchViewModel datacontext;
        public static TabControl contentTab_Export;
        private void Frame_LoadCompleted(object sender, NavigationEventArgs e)
        {
            Page page = e.Content as Page;
            page.DataContext = this.DataContext;
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
            {
                switch (e.Key)//Ctrl + S
                {
                    case Key.S:
                        datacontext.SaveAction.Execute(null);
                        break;
                    default:
                        break;
                }
            }
        }

        private void ExitApplication(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
