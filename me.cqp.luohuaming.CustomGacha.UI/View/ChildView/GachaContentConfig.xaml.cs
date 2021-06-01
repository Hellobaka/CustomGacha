using System.IO;
using System.Windows.Controls;
using HandyControl.Controls;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;
using System.Linq;
using System.Windows;
using PublicInfos;
using ImageSelector = me.cqp.luohuaming.CustomGacha.UI.UserControls.ImageSelector;

namespace me.cqp.luohuaming.CustomGacha.UI.View.ChildView
{
    /// <summary>
    /// GachaContentConfig.xaml 的交互逻辑
    /// </summary>
    public partial class GachaContentConfig : Page
    {
        public GachaContentConfig()
        {
            InitializeComponent();
            dataContext = DataContext as WorkbenchViewModel;
        }

        private WorkbenchViewModel dataContext;

        private void ImageSelector_OnOnPathChanged(object sender, RoutedEventArgs e)
        {            
            dataContext = DataContext as WorkbenchViewModel;
            string path = Path.Combine(dataContext.EditPool.RelativePath, (sender as ImageSelector)?.ImagePath);
            if (File.Exists(path) && dataContext.SelectGachaItem.ItemID == -1)
            {
                FileInfo info = new FileInfo(path);
                dataContext.SelectGachaItem.Name = info.Name.Replace(info.Extension,"");
                dataContext.RaisePropertyChanged("SelectGachaItem.Name");
                Name_TextBox.Text = dataContext.SelectGachaItem.Name;
            }
        }
    }
}