using System.IO;
using System.Windows.Controls;
using HandyControl.Controls;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;
using System.Linq;
using System.Windows;
using PublicInfos;
using ImageSelector = me.cqp.luohuaming.CustomGacha.UI.UserControls.ImageSelector;
using System.Windows.Input;
using System.Collections.Generic;

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

        private void CategoryListBox_KeyDown(object sender, KeyEventArgs e)
        {
            dataContext = DataContext as WorkbenchViewModel;
            if (e.Key == Key.Delete)
            {
                List<Category> ls = new List<Category>();
                foreach (Category item in CategoryListBox.SelectedItems)
                {
                    ls.Add(item);
                }
                dataContext.DeleteCategoryFromDB.Execute(ls);
            }
        }

        private void ContentListBox_KeyDown(object sender, KeyEventArgs e)
        {
            dataContext = DataContext as WorkbenchViewModel;
            if(e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift))
            {
                if (e.Key == Key.Delete)
                {
                    List<GachaItem> ls = new List<GachaItem>();
                    foreach (GachaItem item in ContentListBox.SelectedItems)
                    {
                        ls.Add(item);
                    }
                    dataContext.DeleteItemFromDB.Execute(ls);
                }
            }
            else if (e.Key == Key.Delete)
            {
                List<GachaItem> ls = new List<GachaItem>();
                foreach (GachaItem item in ContentListBox.SelectedItems)
                {
                    ls.Add(item);
                }
                dataContext.DeleteItem.Execute(ls);
            }
        }
    }
}