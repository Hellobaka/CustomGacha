using me.cqp.luohuaming.CustomGacha.Code.OrderFunctions;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.Code
{
    public class Event_StartUp : ICQStartup
    {
        public void CQStartup(object sender, CQStartupEventArgs e)
        {
            MainSave.AppDirectory = e.CQApi.AppDirectory;
            MainSave.CQApi = e.CQApi;
            MainSave.CQLog = e.CQLog;
            MainSave.ImageDirectory = CommonHelper.GetAppImageDirectory();
            //这里写处理逻辑
            MainSave.Instances.Add(new ExampleFunction());//这里需要将指令实例化填在这里
        }
    }
}
