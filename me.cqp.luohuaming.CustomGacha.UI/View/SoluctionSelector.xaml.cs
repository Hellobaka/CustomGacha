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
    /// SoluctionSelector.xaml 的交互逻辑
    /// </summary>
    public partial class SoluctionSelector : Window
    {
        public SoluctionSelector()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object c = (sender as ListBox).SelectedItem;
            if(c is RecentSoluction)
            {
                //新建窗口
                Workbench fm = new Workbench();
                fm.InitializeComponent();
                fm.Show();
                //传递
                (fm.DataContext as WorkbenchViewModel).EditPool = (c as RecentSoluction).Object;
            }
            this.Hide();
        }
    }
}
