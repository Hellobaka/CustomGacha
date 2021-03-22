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
        public static void ShowGrowlMsg(string msg, NoticeEnum notice = NoticeEnum.Success, int showTime = 2)
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
        public static ObservableCollection<T> ToPageList<T>(ObservableCollection<T> ls, int pageIndex, int pageCount)
        {
            return List2ObservableCollection(ToPageList<T>(ls.ToList(), pageIndex, pageCount));
        }

        public static List<T> ToPageList<T>(List<T> ls, int pageIndex, int pageCount)
        {
            List<T> result = new List<T>();
            if (ls.Count < pageIndex  * pageCount)
            {
                int index = (pageIndex - 1) * pageCount;
                result = ls.GetRange(index, ls.Count - (index));
            }
            else
            {
                int index = (pageIndex - 1) * pageCount;
                result = ls.GetRange(index, pageCount);
            }
            return result;
        }
    }
}
