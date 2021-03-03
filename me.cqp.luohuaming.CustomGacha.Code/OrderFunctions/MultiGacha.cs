using System;
using System.IO;
using System.Linq;
using Native.Sdk.Cqp;
using Native.Sdk.Cqp.EventArgs;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.Code.OrderFunctions
{
    public class MultiGacha : IOrderModel
    {
        public string GetOrderStr() => string.Empty;

        public bool Judge(string destStr) => MainSave.PoolInstances.Any(x => x.MultiOrder == destStr);

        public FunctionResult Progress(CQGroupMessageEventArgs e)//群聊处理
        {
            FunctionResult result = new FunctionResult
            {
                Result = true,
                SendFlag = true,
            };
            SendText sendText = new SendText
            {
                SendID = e.FromGroup,
            };
            var destPool = MainSave.PoolInstances.Find(x => x.MultiOrder == e.Message.Text);
            destPool.PluginInit();
            string resultPicPath = Path.Combine(MainSave.GachaResultRootPath, destPool.Name);
            Directory.CreateDirectory(resultPicPath);

            var c = GachaCore.DoGacha(destPool, destPool.MultiGachaNumber);
            c = SQLHelper.UpdateGachaItemsNewStatus(c, e.FromQQ);
            SQLHelper.InsertGachaItem2Repo(c, e.FromQQ);
            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            GachaCore.DrawGachaResult(c, destPool).Save(Path.Combine(resultPicPath, filename));
            //TODO: 回复文本设置
            sendText.MsgToSend.Add(CQApi.CQCode_Image($@"CustomGacha\{destPool.Name}\{filename}").ToSendString());
            result.SendObject.Add(sendText);
            return result;
        }

        public FunctionResult Progress(CQPrivateMessageEventArgs e)//私聊处理
        {
            throw new NotImplementedException();
        }
    }
}
