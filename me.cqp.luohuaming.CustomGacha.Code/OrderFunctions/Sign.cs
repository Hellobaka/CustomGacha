using System;
using Native.Sdk.Cqp.EventArgs;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.Code.OrderFunctions
{
    public class Sign : IOrderModel
    {
        public string GetOrderStr() => MainSave.OrderConfig.Sign;

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
            int signMoney = SQLHelper.Sign(e.FromQQ);
            if (signMoney > 0)
            {
                sendText.MsgToSend.Add($"{e.FromQQ.CQCode_At()} 签到成功, 获得 {signMoney} 通用货币");
            }
            else
            {
                sendText.MsgToSend.Add($"{e.FromQQ.CQCode_At()} 重复签到");
            }
            result.SendObject.Add(sendText);
            return result;
        }

        public FunctionResult Progress(CQPrivateMessageEventArgs e)//私聊处理
        {
            throw new NotImplementedException();
        }
    }
}
