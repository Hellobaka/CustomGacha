using System;
using System.Linq;
using CustomGacha.SDK.Sdk.Cqp.EventArgs;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.Code
{
    public class Event_GroupMessage
    {
        public static FunctionResult GroupMessage(CQGroupMessageEventArgs e)
        {
            FunctionResult result = new FunctionResult()
            {
                SendFlag = false
            };
            try
            {
                //if (SQLHelper.IDExists(e.FromQQ) is false
                //    && e.Message.Text.Replace("＃", "#").Equals(MainSave.OrderConfig.RegisterOrder) is false)
                //{
                //    return result;
                //}

                foreach (var item in MainSave.PoolInstances)
                {
                    foreach (var x in item.PluginMessageHandler)
                    {
                        if (x.Judge(e.Message.Text))
                        {
                            PluginMessage message = new PluginMessage
                            {
                                Message = e.Message.Text,
                                MsgOrigin = MsgOrigin.Group,
                                OriginID = e.FromGroup
                            };
                            return x.Progress(message);
                        }
                    }
                }
                foreach (var item in MainSave.Instances.Where(item => item.Judge(e.Message.Text)))
                {
                    return item.Progress(e);
                }
                return result;
            }
            catch (Exception exc)
            {
                MainSave.CQLog.Info("异常抛出", exc.Message + exc.StackTrace);
                return result;
            }
        }
    }
}
