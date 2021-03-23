using System;
using System.Windows;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.View
{
    /// <summary>
    /// GachaItemQueryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ForeConfig
    {
        public ForeConfig()
        {
            InitializeComponent();
        }
        WorkbenchViewModel datacontext;
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SQLHelper.UpdateConfig(datacontext.Config);
            SQLHelper.UpdateOrderConfig(datacontext.OrderConfig);
            Helper.ShowGrowlMsg("设置已保存");
        }
        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            datacontext = DataContext as WorkbenchViewModel;
        }
        private void DefaultValue_Click(object sender, RoutedEventArgs e)
        {
            datacontext.Config = new Config
            {
                RowID = datacontext.Config.RowID,
                SignFloor = 1600,
                SignCeil = 3200,
                SignResetTime = new DateTime(1970, 1, 1, 0, 0, 0)
            };
            datacontext.OrderConfig= new OrderConfig
            {
                Register = "#抽卡注册",
                Sign = "#抽卡签到"
            };
        }
    }
}
