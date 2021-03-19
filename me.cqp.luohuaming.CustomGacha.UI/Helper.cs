using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandyControl.Controls;
using HandyControl.Data;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI
{
    public static class Helper
    {
        public enum NoticeEnum
        {
            Error,
            Info,
            Success,
        }
        public static void ShowGrowlMsg(string msg, NoticeEnum notice = NoticeEnum.Success, int showTime = 1)
        {
            GrowlInfo info = new GrowlInfo
            {
                Message = msg,
                WaitTime = showTime
            };
            switch (notice)
            {
                case NoticeEnum.Error:
                    Growl.Error(info);
                    break;
                case NoticeEnum.Info:
                    Growl.Info(info);
                    break;
                case NoticeEnum.Success:
                    Growl.Success(info);
                    break;
                default:
                    break;
            }
        }
        public static ObservableCollection<T> List2ObservableCollection<T>(List<T> content)
        {
            ObservableCollection<T> tmp = new ObservableCollection<T>();
            content.ForEach(x => tmp.Add(x));
            return tmp;
        }
    }
}
