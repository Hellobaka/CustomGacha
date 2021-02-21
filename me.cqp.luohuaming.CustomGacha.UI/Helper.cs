﻿using System;
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
        public static void ShowGrowlMsg(string msg, int showTime = 1)
        {
            GrowlInfo info = new GrowlInfo
            {
                Message = msg,
                WaitTime = showTime
            };
            Growl.Success(info);
        }
        public static ObservableCollection<T> List2ObservableCollection<T>(List<T> content)
        {
            ObservableCollection<T> tmp = new ObservableCollection<T>();
            content.ForEach(x => tmp.Add(x));
            return tmp;
        }
    }
}
