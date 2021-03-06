﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;

namespace me.cqp.luohuaming.CustomGacha.UI.UserControls
{
    /// <summary>
    /// ImageSelector.xaml 的交互逻辑
    /// </summary>
    public partial class ImageSelector : UserControl
    {
        public ImageSelector()
        {
            InitializeComponent();
        }
        private string reletivePath;
        public string ReletivePath
        {
            get { reletivePath = (this.DataContext as WorkbenchViewModel).EditPool.RelativePath; return reletivePath; }
            set { reletivePath = value; HandleOnPathChanged(this, null); }
        }


        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set
            {
                SetValue(ImagePathProperty, value);
                if (string.IsNullOrWhiteSpace(value))
                    imageViewer.Source = new BitmapImage();
                pathSelector.FilePath = value;
                string path = Path.Combine(ReletivePath, value);
                if (File.Exists(path) && (path.EndsWith(".png") || path.EndsWith(".jpg")))
                {
                    imageViewer.Source = new BitmapImage(new Uri(path));
                }

                HandleOnPathChanged(this, null);
            }
        }
        // Using a DependencyProperty as the backing store for ImagePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(string), typeof(ImageSelector), new PropertyMetadata(""));

        private void PathSelector_OnPathSelected(object sender, RoutedEventArgs e)
        {
            ImagePath = pathSelector.FilePath;
            if (string.IsNullOrWhiteSpace(ImagePath))
                return;
            if(string.IsNullOrWhiteSpace(ReletivePath) is false)
            {
                string path = Path.Combine(ReletivePath, ImagePath);
                if (File.Exists(path) && (path.EndsWith(".png") || path.EndsWith(".jpg")))
                {
                    imageViewer.Source = new BitmapImage(new Uri(path));
                }
            }

            HandleOnPathChanged(sender, e);
        }
        public static readonly RoutedEvent OnPathChangedEvent = EventManager.RegisterRoutedEvent("OnPathChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ImageSelector));
        public event RoutedEventHandler OnPathChanged
        {
            add { AddHandler(OnPathChangedEvent, value); }
            remove { RemoveHandler(OnPathChangedEvent, value); }
        }
        private void HandleOnPathChanged(object sender,RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(ImageSelector.OnPathChangedEvent, this));
        }

    }
}
