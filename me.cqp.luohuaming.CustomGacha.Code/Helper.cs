using System;
using CustomGacha.SDK.Sdk.Cqp;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.Code
{
    public static class Helper
    {
        public static string HandleModelString(string str, long QQID, DB_User user, params object[] args)
        {
            str = str.Replace("<@>", CQApi.CQCode_At(QQID).ToSendString() + " ");
            str = str.Replace("<current_money>", user.Money.ToString());
            str = str.Replace("<sign_count>",user.SignTotalCount.ToString());
            str = str.Replace("<gacha_count>",user.GachaTotalCount.ToString());
            str = str.Replace("<baodi_count>",user.GachaCount.ToString());
            str = str.Replace("<gacha_totalcost>",user.MoneyTotalCount.ToString());
            str = str.Replace("<last_sign>",user.LastSignTime.ToString("yyyy-MM-dd HH:mm:ss"));
            for (int i = 0; i < args.Length; i++)
            {
                str = str.Replace($"<${i}>", args[i].ToString());
            }
            return str;
        }
        public static void SendTmpMsg(long GroupID,string str)
        {
            try
            {
                MainSave.CQApi.SendGroupMessage(GroupID, str);
            }
            catch (Exception e)
            {

            }
        }
    }
}
