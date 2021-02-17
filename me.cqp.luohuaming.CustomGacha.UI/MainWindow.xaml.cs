using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;
using Microsoft.Win32;
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
            //SQLHelper.CreateDB();
            this.DataContext = new MainWindowViewModel();
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
            if (e.AddedItems.Count > 0)
            {
                if (c as Pool == null)
                    (this.DataContext as MainWindowViewModel).SelectGachaItem = (GachaItem)c;
                else
                    (this.DataContext as MainWindowViewModel).SelectPool = (Pool)c;
            }
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var c = (sender as ListBox).SelectedItem;
            if (c == null)
                return;
            else
            {
                PropertyEdit.SelectedObject = c;
            }
        }
        private string filePath = "";
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(Keyboard.FocusedElement is TextBox textBox))
                return;
            if (e.Key == Key.F4)
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    InitialDirectory = filePath,
                    Filter = "图像文件|*.jpg;*.png"
                };
                dialog.ShowDialog();
                filePath = dialog.FileName;
                textBox.Text = dialog.FileName;
            }
        }
    }
}
