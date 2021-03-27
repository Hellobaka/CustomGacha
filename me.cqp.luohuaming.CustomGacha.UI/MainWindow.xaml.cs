using System.IO;
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
            //TODO: 搜索框
            InitializeComponent();
            SQLHelper.CreateDB();
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
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(Keyboard.FocusedElement is TextBox textBox))
                return;
            if (e.Key == Key.F4)
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    InitialDirectory = MainWindowViewModel.GetSelectPool().RelativePath,
                    Filter = "图像文件|*.jpg;*.png|插件文件|*.dll"
                };
                dialog.ShowDialog();
                if (string.IsNullOrWhiteSpace(dialog.FileName))
                    return;
                textBox.Text = dialog.FileName.Replace(dialog.InitialDirectory + "\\", "");
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
            {
                switch (e.Key)//Ctrl + S
                {
                    case Key.S:
                        var c = PropertyEdit.SelectedObject;
                        if (c is Pool)
                        {
                            SQLHelper.UpdatePool(c as Pool);
                        }
                        else if (c is GachaItem)
                        {
                            SQLHelper.UpdatePool(MainSave.PoolInstances[Pools.SelectedIndex]);
                            SQLHelper.UpdateOrAddGachaItem(c as GachaItem);
                        }
                        else if(c is Category)
                        {
                            SQLHelper.UpdateOrAddCategory(c as Category);
                        }
                        Helper.ShowGrowlMsg("项目已保存");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
