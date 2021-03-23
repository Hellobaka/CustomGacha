using System;
using System.Windows;
using HandyControl.Controls;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.View
{
    /// <summary>
    /// GachaItemQueryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NewPoolStep
    {
        public NewPoolStep()
        {
            InitializeComponent();
        }
        NewPoolStepViewModel datacontext;
        public static StepBar stepBar_Export;
        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            datacontext = DataContext as NewPoolStepViewModel;
            stepBar_Export = stepBar;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            datacontext.Result = datacontext.EditPool;
            datacontext.CloseCmd.Execute(null);
        }
    }
}
