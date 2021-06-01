using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;
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
        private string reletivePath;
        public string ReletivePath
        {
            get { reletivePath = (this.DataContext as WorkbenchViewModel).EditPool.RelativePath; return reletivePath; }
            set { reletivePath = value; }
        }

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

        public static readonly RoutedEvent OnPathSelectedEvent = EventManager.RegisterRoutedEvent("OnPathSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PathSelector));
        public event RoutedEventHandler OnPathSelected
        {
            add { AddHandler(OnPathSelectedEvent, value); }
            remove { RemoveHandler(OnPathSelectedEvent, value); }
        }
        #endregion
        private void HandleOnPathSelected(object sender,RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(PathSelector.OnPathSelectedEvent, this));
        }
        public enum OpenTypeEnum
        {
            File,
            Folder
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowDialog();
            HandleOnPathSelected(sender, e);
        }
        private void ShowDialog()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Multiselect = false,
            };
            string baseDir = Path.Combine(ReletivePath, FilePath);
            if (string.IsNullOrWhiteSpace(baseDir) is false && File.Exists(baseDir))
            {
                var flag = File.GetAttributes(baseDir);
                if (flag.HasFlag(FileAttributes.Directory) is false)
                {
                    FileInfo info = new FileInfo(baseDir);
                    baseDir = info.DirectoryName;
                }
            }
            switch (OpenType)
            {
                case OpenTypeEnum.File:
                    dialog.InitialDirectory = baseDir;
                    dialog.Filters.Add(new CommonFileDialogFilter("图像文件", "*.png;*.jpg"));
                    dialog.Filters.Add(new CommonFileDialogFilter("插件文件", "*.dll"));
                    if (string.IsNullOrWhiteSpace(FilePath) is false)
                    {
                        dialog.InitialDirectory = baseDir;
                        if (FilePath.EndsWith(".dll"))
                        {
                            dialog.Filters.Clear();
                            dialog.Filters.Add(new CommonFileDialogFilter("插件文件", "*.dll"));
                            dialog.Filters.Add(new CommonFileDialogFilter("图像文件", "*.png;*.jpg"));
                        }
                    }
                    break;
                case OpenTypeEnum.Folder:
                    dialog.IsFolderPicker = true;
                    dialog.InitialDirectory = baseDir;
                    break;
            }
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                switch (OpenType)
                {
                    case OpenTypeEnum.File:
                        FilePath = dialog.FileName.Replace(ReletivePath + "\\", "");
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
                HandleOnPathSelected(sender, e);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilePath = (sender as TextBox).Text;
            HandleOnPathSelected(sender, e);
        }
    }
}
