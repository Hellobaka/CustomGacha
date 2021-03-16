﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.IconPacks;

namespace me.cqp.luohuaming.CustomGacha.UI.View
{
    /// <summary>
    /// Workbench.xaml 的交互逻辑
    /// </summary>
    public partial class Workbench : Window
    {
        public Workbench()
        {
            PackIconCodicons o = new PackIconCodicons();
            PackIconMaterial oo = new PackIconMaterial();
            PackIconUnicons ooo = new PackIconUnicons();
            InitializeComponent();
        }

        private void Frame_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Page page = e.Content as Page;
            page.DataContext = this.DataContext;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}