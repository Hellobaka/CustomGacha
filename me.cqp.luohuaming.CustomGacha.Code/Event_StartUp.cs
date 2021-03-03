using System.Drawing;
using System.IO;
using me.cqp.luohuaming.CustomGacha.Code.OrderFunctions;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using PluginInterface;
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
            MainSave.GachaResultRootPath = Path.Combine(MainSave.ImageDirectory, "CustomGacha");
            MainSave.DBPath = Path.Combine(e.CQApi.AppDirectory, "data.db");
            if (File.Exists(MainSave.DBPath) is false)
            {
                SQLHelper.CreateDB();
            }
            SQLHelper.LoadConfig();
            MainSave.PoolInstances = SQLHelper.GetAllPools();
            Directory.CreateDirectory(MainSave.GachaResultRootPath);
            MainSave.Instances.Add(new Register());//这里需要将指令实例化填在这里
            MainSave.Instances.Add(new Sign());
            MainSave.Instances.Add(new MultiGacha());
        }
        private protected class G7TUJSM2 : IDrawItem{ public Bitmap DrawPicItem(GachaItem item, string relativePath, ItemDrawConfig ImageConfig){ return null; }}
    }
}
