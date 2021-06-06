using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using me.cqp.luohuaming.CustomGacha.UI.Model;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;

namespace me.cqp.luohuaming.CustomGacha.UI.View
{
    /// <summary>
    /// ManagePoolsView.xaml 的交互逻辑
    /// </summary>
    public partial class ManagePoolsView : Window
    {
        public ManagePoolsView()
        {
            InitializeComponent();
            vm = (ManagePoolsModelView)DataContext;
        }
        public ManagePoolsModelView vm;
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            vm.SelectNum = vm.RecentList.Where(x => x.Checked).Count();
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            vm.SelectNum = vm.RecentList.Where(x => x.Checked).Count();
        }
    }
}
