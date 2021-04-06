using System;
using System.Threading;
using System.Windows;
using me.cqp.luohuaming.CustomGacha.UI.View;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;
using CustomGacha.SDK.Sdk.Cqp.EventArgs;
using CustomGacha.SDK.Sdk.Cqp.Interface;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI
{
    public class Event_MenuCall : IMenuCall
    {
        private bool isOpen = false;
        private App app = null;
        public void MenuCall(object sender, CQMenuCallEventArgs e)
        {
            try
            {
                if (isOpen is false)
                {
                    isOpen = true; 
                    Thread thread = new Thread(() =>
                    {
                        AppDomain.CurrentDomain.DomainUnload += (a,b)=> { app.Dispatcher.Invoke(() => { SoluctionSelector.ExitFlag = true; SoluctionSelector.SoluctionSelector_Export.Close(); }); };
                        app = new App();
                        app.ShutdownMode = ShutdownMode.OnMainWindowClose;
                        app.InitializeComponent();
                        app.Run();
                    });
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }
                else
                {
                    var c = SoluctionSelector.SoluctionSelector_Export;
                    c.Dispatcher.InvokeAsync(()=>
                    {
                        c.DataContext = new SoluctionSelectorViewModel();
                        c.Visibility = Visibility.Visible;
                    });
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message + exc.StackTrace);
                MainSave.CQLog.Info("Error", exc.Message, exc.StackTrace);
            }
        }
    }
}