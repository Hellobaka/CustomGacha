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
            ReadInterfaceInfo();
            ReadOrders();
        }
        private void ReadInterfaceInfo()
        {
            PluginInterfaceInfo.Items.Clear();
            if (datacontext.EditPool.DrawAllItems != null)
                PluginInterfaceInfo.Items.Add("DrawAllItems");
            if (datacontext.EditPool.DrawItem != null)
                PluginInterfaceInfo.Items.Add("DrawItem");
            if (datacontext.EditPool.DrawMainImage != null)
                PluginInterfaceInfo.Items.Add("DrawMainImage");
            if (datacontext.EditPool.DrawPoints != null)
                PluginInterfaceInfo.Items.Add("DrawPoints");
            if (datacontext.EditPool.FinallyDraw != null)
                PluginInterfaceInfo.Items.Add("FinallyDraw");
        }
        private void ReadOrders()
        {
            PluginOrderInfo.Items.Clear();
            datacontext.EditPool.PluginMessageHandler.ForEach(x =>PluginOrderInfo.Items.Add(x.GetOrderStr()));
        }
        private void ReloadPlugin_Click(object sender, RoutedEventArgs e)
        {
            PluginInterfaceInfo.Items.Clear();
            try
            {
                datacontext.EditPool.PluginInit();
                ReadInterfaceInfo();
                ReadOrders();
                Helper.ShowGrowlMsg($"插件重载成功", Helper.NoticeEnum.Success, 2);
            }
            catch (Exception exc)
            {
                Helper.ShowGrowlMsg($"插件初始化失败，错误信息: {exc.Message}", Helper.NoticeEnum.Error, 2);
                datacontext.EditPool.DrawAllItems = null;
                datacontext.EditPool.DrawItem = null;
                datacontext.EditPool.DrawMainImage = null;
                datacontext.EditPool.DrawPoints = null;
                datacontext.EditPool.FinallyDraw = null;
            }
        }
    }
}
