using System;
using System.IO;
using System.Linq;
using CustomGacha.SDK.Sdk.Cqp;
using CustomGacha.SDK.Sdk.Cqp.EventArgs;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.Code.OrderFunctions
{
    public class SingalGacha : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => string.Empty;

        public bool Judge(string destStr) => MainSave.PoolInstances.Any(x => x.SingalGachaOrder == destStr);

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
            //检索能够应答指令的卡池
            var destPool = MainSave.PoolInstances.Find(x => x.SingalGachaOrder == e.Message.Text);
            //预构建图片保存目录
            string resultPicPath = Path.Combine(MainSave.GachaResultRootPath, destPool.Name);
            Directory.CreateDirectory(resultPicPath);

            //检索用户
            DB_User user = SQLHelper.GetUser(e.FromQQ);
            long gachaCost = MainSave.ApplicationConfig.GachaCost;
            if (user.Money < gachaCost)//货币不足以抽卡
            {
                sendText.MsgToSend.Add(Helper.HandleModelString(MainSave.OrderConfig.LeakMoneyText, e.FromQQ, user));
                result.SendObject.Add(sendText);
                return result;
            }
            else//减去所需货币并更新相关字段
            {
                user.Money -= gachaCost;
                user.GachaTotalCount ++;
                user.MoneyTotalCount += gachaCost;
            }
            //发送回复文本
            Helper.SendTmpMsg(e.FromGroup, Helper.HandleModelString(destPool.SingalGachaText, e.FromQQ, user));
            //进行抽卡
            int baodiCount = user.GachaCount;
            var c = GachaCore.DoGacha(destPool, 1, ref baodiCount);
            //从仓库检索项目是否为new
            c = SQLHelper.UpdateGachaItemsNewStatus(c, e.FromQQ);
            //更新用户字段
            user.GachaCount = baodiCount;
            SQLHelper.UpdateUser(user);
            //向仓库插入项目
            SQLHelper.InsertGachaItem2Repo(c, e.FromQQ);
            //保存图片
            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            GachaCore.DrawGachaResult(c, destPool, e.FromQQ).Save(Path.Combine(resultPicPath, filename));
            //发送结果
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
