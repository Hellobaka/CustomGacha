using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandyControl.Controls;
using HandyControl.Data;

namespace me.cqp.luohuaming.CustomGacha.UI
{
    public static class Helper
    {
        public static void ShowGrowlMsg(string msg, int showTime = 1)
        {
            GrowlInfo info = new GrowlInfo
            {
                Message = msg,
                WaitTime = showTime
            };
            Growl.Success(info);
        }
    }
}
