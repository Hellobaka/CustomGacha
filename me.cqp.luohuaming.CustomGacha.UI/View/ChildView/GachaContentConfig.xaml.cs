using System.Windows.Controls;
using HandyControl.Controls;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;
using System.Linq;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.View.ChildView
{
    /// <summary>
    /// GachaContentConfig.xaml 的交互逻辑
    /// </summary>
    public partial class GachaContentConfig : Page
    {
        public GachaContentConfig()
        {
            dataContext = DataContext as WorkbenchViewModel;
            InitializeComponent();
        }
        private WorkbenchViewModel dataContext;
        private void Transfer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataContext.UpContents.Clear();
            var c = (sender as Transfer).SelectedItems;
            foreach (var item in c)
                dataContext.UpContents.Add((GachaItem)item);
        }
    }
}
