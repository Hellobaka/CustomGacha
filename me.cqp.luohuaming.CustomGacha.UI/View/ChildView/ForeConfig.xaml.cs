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
                SignResetTime = new DateTime(1970, 1, 1, 0, 0, 0),
                RegisterMoney = 6400,
                GachaCost = 160
            };
            datacontext.OrderConfig= new OrderConfig
            {
                RegisterOrder = "#抽卡注册",
                SignOrder = "#抽卡签到",
                SuccessfulSignText = "<@>签到成功，获得通用货币<$0>",
                DuplicateSignText = "<@>你今天签过到了",
                SuccessfulRegisterText = "<@>注册成功，获得通用货币<current_money>",
                DuplicateRegisterText = "<@>重复注册是打咩的",
                LeakMoneyText = "<@>剩余货币不足以抽卡了呢~\n你目前还有<current_money>通用货币",
            };
        }
    }
}
