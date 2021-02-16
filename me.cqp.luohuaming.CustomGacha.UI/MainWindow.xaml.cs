using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SQLHelper.CreateDB();
            this.DataContext = new MainWindowViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Content_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as ListBox).SelectedItem != null)
            {
                PropertyEdit.SelectedObject = (sender as ListBox).SelectedItem;
            }
        }
        private void Pools_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as ListBox).SelectedItem != null && e.ClickCount == 2)
            {
                PropertyEdit.SelectedObject = (sender as ListBox).SelectedItem;
            }
        }

        private void Pools_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var c = (sender as ListBox).SelectedItem;
            if (c == null)
            {
                PropertyEdit.SelectedObject = new object();
            }
            else
            {
                PropertyEdit.SelectedObject = c;
            }
            if (e.AddedItems.Count>0)
                (this.DataContext as MainWindowViewModel).SelectPool = (Pool)c;
        }

        private void Content_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (this.DataContext as MainWindowViewModel).SelectGachaItem = (GachaItem)e.AddedItems[0];
        }
    }
}
