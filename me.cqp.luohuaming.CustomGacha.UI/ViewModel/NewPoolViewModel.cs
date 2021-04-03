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
            Cancel = new DelegateCommand { ExecuteAction = cancel };
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
        public DelegateCommand Cancel { get; set; }
        public void cancel(object peremeter)
        {
            Result = null;
            closeAction(peremeter);
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
