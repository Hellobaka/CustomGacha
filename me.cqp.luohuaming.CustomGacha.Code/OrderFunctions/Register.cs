using System;
using Native.Sdk.Cqp;
using Native.Sdk.Cqp.EventArgs;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.Code.OrderFunctions
{
    public class Register : IOrderModel
    {
        public string GetOrderStr() => MainSave.OrderConfig.Register;

        public bool Judge(string destStr) => destStr.Replace("＃","#").Equals(GetOrderStr());//这里判断是否能触发指令

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
            if(SQLHelper.IDExists(e.FromQQ))
            {
                sendText.MsgToSend.Add("注册重复");
                return result;
            }
            var c = SQLHelper.Register(e.FromQQ);
            sendText.MsgToSend.Add($"{e.FromQQ.CQCode_At()} 注册成功，初始货币{c.Money}");
            result.SendObject.Add(sendText);
            return result;
        }

        public FunctionResult Progress(CQPrivateMessageEventArgs e)//私聊处理
        {
            throw new NotImplementedException();
        }
    }
}
