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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using UserControl = System.Windows.Controls.UserControl;

namespace me.cqp.luohuaming.CustomGacha.UI.UserControls
{
    /// <summary>
    /// PathSelector.xaml 的交互逻辑
    /// </summary>
    public partial class PathSelector : UserControl
    {
        public PathSelector()
        {
            InitializeComponent();
        }
        #region ---依赖属性---
        public OpenTypeEnum OpenType
        {
            get { return (OpenTypeEnum)GetValue(OpenTypeProperty); }
            set { SetValue(OpenTypeProperty, value); }
        }
        public static readonly DependencyProperty OpenTypeProperty =
            DependencyProperty.Register("OpenType", typeof(OpenTypeEnum), typeof(PathSelector), new PropertyMetadata(OpenTypeEnum.File));

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(PathSelector), new PropertyMetadata(""));

        public string ReletivePath
        {
            get { return (string)GetValue(ReletivePathProperty); }
            set { SetValue(ReletivePathProperty, value); }
        }
        public static readonly DependencyProperty ReletivePathProperty =
            DependencyProperty.Register("ReletivePath", typeof(string), typeof(PathSelector), new PropertyMetadata(""));
        #endregion
        public enum OpenTypeEnum
        {
            File,
            Folder
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowDialog();
        }
        private void ShowDialog()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Multiselect = false
            };
            switch (OpenType)
            {
                case OpenTypeEnum.File:
                    dialog.InitialDirectory = ReletivePath;
                    dialog.Filters.Add(new CommonFileDialogFilter("图像文件", "*.png;*.jpg"));
                    dialog.Filters.Add(new CommonFileDialogFilter("插件文件", "*.dll"));
                    break;
                case OpenTypeEnum.Folder:
                    dialog.IsFolderPicker = true;
                    dialog.InitialDirectory = ReletivePath;
                    break;
            }
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                switch (OpenType)
                {
                    case OpenTypeEnum.File:
                        FilePath = dialog.FileName.Replace(dialog.InitialDirectory + "\\", "");
                        break;
                    case OpenTypeEnum.Folder:
                        FilePath = dialog.FileName;
                        break;
                }
            }
        }
        private void PathSelector_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(Keyboard.FocusedElement is TextBox))
                return;
            if (e.Key == Key.F4)
            {
                ShowDialog();
            }
        }
    }
}
