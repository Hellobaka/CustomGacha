using System;
using HandyControl.Tools.Extension;
using me.cqp.luohuaming.CustomGacha.UI.Command;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    class NewPoolViewModel : NotifyicationObject, IDialogResultable<Category>
    {
        public NewPoolViewModel()
        {

        }
        private Category nowCategory;

        public Category NowCategory
        {
            get
            {
                return nowCategory; 
            }
            set
            {
                nowCategory = value;
                this.RaisePropertyChanged("NowCategory");
            }
        }

        public Category Result { get { return NowCategory; } set { NowCategory = value; } }
        public DelegateCommand CloseCmd => new Lazy<DelegateCommand>(() => new DelegateCommand { ExecuteAction = closeAction }).Value;
        private void closeAction(object permerter)
        {
            CloseAction?.Invoke();
        }
        public Action CloseAction { get; set; }
    }
}
