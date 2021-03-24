using System;
using Native.Sdk.Cqp.EventArgs;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.Code.OrderFunctions
{
    public class Register : IOrderModel
    {
        public string GetOrderStr() => MainSave.OrderConfig.RegisterOrder;

        public bool Judge(string destStr) => destStr.Replace("＃", "#").Equals(GetOrderStr());//这里判断是否能触发指令

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
            if (SQLHelper.IDExists(e.FromQQ))
            {
                DB_User user = SQLHelper.GetUser(e.FromQQ);
                sendText.MsgToSend.Add(Helper.HandleModelString(MainSave.OrderConfig.DuplicateRegisterText, e.FromQQ, user));
                result.SendObject.Add(sendText);
                return result;
            }
            var c = SQLHelper.Register(e.FromQQ);
            sendText.MsgToSend.Add(Helper.HandleModelString(MainSave.OrderConfig.SuccessfulRegisterText, e.FromQQ, c));
            result.SendObject.Add(sendText);
            return result;
        }

        public FunctionResult Progress(CQPrivateMessageEventArgs e)//私聊处理
        {
            throw new NotImplementedException();
        }
    }
}
