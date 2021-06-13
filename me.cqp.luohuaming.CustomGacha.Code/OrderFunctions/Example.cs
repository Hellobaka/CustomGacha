using System;
using CustomGacha.SDK.Sdk.Cqp.EventArgs;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.Code.OrderFunctions
{
    public class Example : IOrderModel
    {
        public bool ImplementFlag { get; set; } = false;

        public string GetOrderStr() => "这里输入触发指令";

        public bool Judge(string destStr) => destStr.StartsWith(GetOrderStr());//这里判断是否能触发指令

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
            result.SendObject.Add(sendText);

            sendText.MsgToSend.Add("这里输入需要发送的文本");
            return result;
        }

        public FunctionResult Progress(CQPrivateMessageEventArgs e)//私聊处理
        {
            throw new NotImplementedException();
        }
    }
}
