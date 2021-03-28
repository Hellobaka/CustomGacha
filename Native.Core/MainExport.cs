using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using me.cqp.luohuaming.CustomGacha.Code;
using CustomGacha.SDK.Sdk.Cqp.EventArgs;
using CustomGacha.SDK.Sdk.Cqp.Interface;
using PublicInfos;
using System.Windows;


namespace CustomGacha.SDK.Core
{
    public class MainExport : IGroupMessage, IPrivateMessage
    {
        public void GroupMessage(object sender, CQGroupMessageEventArgs e)
        {
            try
            {

                FunctionResult result = Event_GroupMessage.GroupMessage(e);
                if (result.SendFlag)
                {
                    if (result.SendObject == null || result.SendObject.Count == 0)
                    {
                        e.Handler = false;
                    }
                    foreach (var item in result.SendObject)
                    {
                        foreach (var sendMsg in item.MsgToSend)
                        {
                            e.CQApi.SendGroupMessage(item.SendID, sendMsg);
                        }
                    }
                }
                e.Handler = result.Result;
            }
            catch (Exception exc)
            {
                e.CQLog.Error($"{exc.Message}\n {exc.StackTrace}");
            }
        }
        public void PrivateMessage(object sender, CQPrivateMessageEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
