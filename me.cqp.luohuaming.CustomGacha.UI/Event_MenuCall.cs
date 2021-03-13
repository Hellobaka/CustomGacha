using System;
using System.Threading;
using System.Windows;
using me.cqp.luohuaming.CustomGacha.UI.View;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI
{
    public class Event_MenuCall : IMenuCall
    {
        private App window = null;
        public void MenuCall(object sender, CQMenuCallEventArgs e)
        {
            try
            {
                if (window == null)
                {
                    Thread thread = new Thread(() =>
                    {
                        App app = new App();
                        window = app;
                        app.Exit += (A,B) => { window = null; };
                        app.InitializeComponent();
                        app.Run();
                    });
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();                    
                }
                else
                {
                    MessageBox.Show("已经打开了一个控制台");
                }
            }
            catch (Exception exc)
            {
                MainSave.CQLog.Info("Error", exc.Message, exc.StackTrace);
            }
        }
    }
}
