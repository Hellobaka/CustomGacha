using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using HandyControl.Controls;
using HandyControl.Data;
using me.cqp.luohuaming.CustomGacha.UI.Command;
using me.cqp.luohuaming.CustomGacha.Code;
using PublicInfos;
using Native.Sdk.Cqp.EventArgs;
using System.Threading;
using System.Diagnostics;
using Native.Sdk.Cqp;
using Native.Sdk.Cqp.Model;
using me.cqp.luohuaming.CustomGacha.UI.UserControls;
using System.IO;
using System.Windows.Media.Imaging;
using me.cqp.luohuaming.CustomGacha.Code.OrderFunctions;
using System.Collections.Generic;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    class ChatBoxViewModel : NotifyicationObject
    {
        public ChatBoxViewModel()
        {
            SendString = new DelegateCommand
            {
                ExecuteAction = new Action<object>(sendString)
            };
            ReadMessage = new DelegateCommand
            {
                ExecuteAction = new Action<object>(readMessage)
            };
            //TODO: 每次加指令都得这样吗？
            if (MainSave.Instances.Count == 0)
            {
                MainSave.Instances.Add(new Register());//这里需要将指令实例化填在这里
                MainSave.Instances.Add(new Sign());
                MainSave.Instances.Add(new MultiGacha());
            }
        }
        private long testQQ = 1145141919;
        public long TestQQ
        {
            get { return testQQ; }
            set
            {
                testQQ = value;
                this.RaisePropertyChanged("TestQQ");
            }
        }
        public List<string> HistoryChatStrings { get; set; } = new List<string>();
        public ObservableCollection<ChatInfoModel> ChatInfos { get; set; } = new ObservableCollection<ChatInfoModel>();

        private readonly string _id = Guid.NewGuid().ToString();

        private string chatString;
        public string ChatString
        {
            get { return chatString; }
            set
            {
                chatString = value;
                this.RaisePropertyChanged("ChatString");
            }
        }
        public int historyIndex = 0;
        public DelegateCommand SendString { get; set; }
        private void sendString(object o)
        {
            KeyEventArgs e = o as KeyEventArgs;
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(ChatString)) return;
                var info = new ChatInfoModel
                {
                    Message = ChatString,
                    SenderId = _id,
                    Type = ChatMessageType.String,
                    Role = ChatRoleType.Sender
                };
                ChatInfos.Add(info);
                HistoryChatStrings.Add(ChatString);
                ChatInfoModel timeMsg = new ChatInfoModel
                {
                    Type = ChatMessageType.String,
                    Role = ChatRoleType.Receiver,
                    Message = $"处理中..."
                };
                ChatInfos.Add(timeMsg);
                Thread thread = new Thread(() =>
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    FunctionResult c = CallPluginFunction(ChatString);
                    sw.Stop();
                    if (c.Result)
                    {
                        RemoveInfoByControlInvoke(timeMsg);
                        timeMsg.Message = $"处理完成, 耗时 {sw.ElapsedMilliseconds} ms";
                        AddInfoByControlInvoke(timeMsg);
                        foreach (var item in c.SendObject)
                        {
                            foreach (var items in item.MsgToSend)
                            {
                                ChatInfoModel result = new ChatInfoModel
                                {
                                    Message = items,
                                    Role = ChatRoleType.Receiver,
                                    Type = ChatMessageType.String
                                };
                                AddInfoByControlInvoke(result);
                                if (items.Contains("[CQ:image,file="))
                                {
                                    var cqcode = CQCode.Parse(items);
                                    foreach(var keys in cqcode)
                                    {
                                        string filePath = Path.Combine(MainSave.ImageDirectory, keys.Items["file"]);
                                        if (File.Exists(filePath))
                                        {
                                            var picModel = new ChatInfoModel
                                            {
                                                Message = BitmapFrame.Create(new Uri(filePath)),
                                                Type = ChatMessageType.Image,
                                                Role = ChatRoleType.Receiver,
                                                Enclosure = filePath
                                            };
                                            AddInfoByControlInvoke(picModel);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        RemoveInfoByControlInvoke(timeMsg);
                        timeMsg.Message = $"插件放行了处理, 耗时 {sw.ElapsedMilliseconds} ms";
                        AddInfoByControlInvoke(timeMsg);
                    }
                    ChatString = string.Empty;
                });
                thread.Start();
            }
        }
        public void AddInfoByControlInvoke(ChatInfoModel vm)
        {
            ChatBox.chatbox_Export.Dispatcher.Invoke(() => ChatInfos.Add(vm));
        }
        public void RemoveInfoByControlInvoke(ChatInfoModel vm)
        {
            ChatBox.chatbox_Export.Dispatcher.Invoke(() => ChatInfos.Remove(vm));
        }

        public DelegateCommand ReadMessage { get; set; }
        private void readMessage(object o)
        {
            RoutedEventArgs e = o as RoutedEventArgs;
            if (e.OriginalSource is FrameworkElement element && element.Tag is ChatInfoModel info)
            {
                if (info.Type == ChatMessageType.Image)
                {
                    new ImageBrowser(new Uri(info.Enclosure.ToString())).Show();
                }
                else if (info.Type == ChatMessageType.Audio)
                { }
            }
        }
        public FunctionResult CallPluginFunction(string str)
        {
            CQApi tmpApi = new CQApi(new AppInfo("", 0, 0, "", "1.0.0", 0, "", "", 0));
            CQLog tmpLog = new CQLog(0);
            CQGroupMessageEventArgs e = new CQGroupMessageEventArgs(tmpApi, tmpLog, 0, 0, "", "", 0, 0, 0, 100000, TestQQ, "",
                                                                    str, false);
            return Event_GroupMessage.GroupMessage(e);
        }
    }
    public class ChatInfoModel
    {
        public object Message { get; set; }

        public string SenderId { get; set; }

        public ChatRoleType Role { get; set; }

        public ChatMessageType Type { get; set; }

        public object Enclosure { get; set; }
    }
}
