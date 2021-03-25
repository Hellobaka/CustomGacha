using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace me.cqp.luohuaming.CustomGacha.UI.View.ChildView
{
    /// <summary>
    /// TestPage.xaml 的交互逻辑
    /// </summary>
    public partial class TestPage : Page
    {
        public TestPage()
        {
            InitializeComponent();
        }
        WorkbenchViewModel datacontext;
        private void TestDraw_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            Thread thread = new Thread(() =>
            {
                try
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    datacontext.PoolDrawTest.Execute(null);
                    testPage.Dispatcher.Invoke(() =>
                    {
                        sw.Stop();
                        Helper.ShowGrowlMsg($"图片合成成功，耗时 {sw.ElapsedMilliseconds} ms", Helper.NoticeEnum.Success, 3);
                        this.Cursor = Cursors.Arrow;
                    });
                }
                catch(Exception exc)
                {
                    testPage.Dispatcher.Invoke(() =>
                    {
                        Helper.ShowGrowlMsg($"图片合成失败，失败信息: {exc.Message}", Helper.NoticeEnum.Error, 3);
                        this.Cursor = Cursors.Arrow;
                    });
                }
            });
            thread.Start();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            datacontext = this.DataContext as WorkbenchViewModel;
        }
    }
}
