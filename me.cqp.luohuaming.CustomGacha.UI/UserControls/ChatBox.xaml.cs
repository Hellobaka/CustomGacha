using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HandyControl.Tools;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;

namespace me.cqp.luohuaming.CustomGacha.UI.UserControls
{
    /// <summary>
    /// ChatBox.xaml 的交互逻辑
    /// </summary>
    public partial class ChatBox : UserControl
    {
        public ChatBox()
        {
            InitializeComponent();
            ListBoxChat.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
        }
        private ScrollViewer _scrollViewer;
        private void ItemContainerGenerator_ItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        {
            if (_scrollViewer == null)
            {
                _scrollViewer = VisualHelper.GetChild<ScrollViewer>(ListBoxChat);
            }
            _scrollViewer?.ScrollToBottom();
        }

        public static ChatBox chatbox_Export;
        ChatBoxViewModel datacontext;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            chatbox_Export = this;
            datacontext = DataContext as ChatBoxViewModel;
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                var c = datacontext.HistoryChatStrings.Reverse<string>().ToList();
                if (c.Count == 0)
                    return;
                datacontext.ChatString = c[datacontext.historyIndex];
                datacontext.historyIndex++;
                if (datacontext.historyIndex >= datacontext.HistoryChatStrings.Count - 1)
                    datacontext.historyIndex = datacontext.HistoryChatStrings.Count - 1;
            }
            else if (e.Key == Key.Down)
            {
                datacontext.historyIndex--;
                if (datacontext.historyIndex == -1)
                {
                    datacontext.ChatString = string.Empty;
                    datacontext.historyIndex = 0;
                    return;
                }
                var c = datacontext.HistoryChatStrings.Reverse<string>().ToList();
                if (c.Count == 0)
                    return;
                datacontext.ChatString = c[datacontext.historyIndex];
            }
            else
            {
                datacontext.historyIndex = 0;
            }
        }
    }
}
