using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandyControl.Tools.Extension;
using me.cqp.luohuaming.CustomGacha.UI.Command;
using me.cqp.luohuaming.CustomGacha.UI.View;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    class NewPoolStepViewModel : NotifyicationObject, IDialogResultable<Pool>
    {
        public NewPoolStepViewModel()
        {
            EditPool = new Pool();
            Result = new Pool();
            NextCmd = new DelegateCommand 
            {
                ExecuteAction=new Action<object>(nextCmd)
            };
            PrevCmd = new DelegateCommand 
            {
                ExecuteAction=new Action<object>(prevCmd)
            };
            CloseWithoutPool = new DelegateCommand 
            {
                ExecuteAction=new Action<object>(closeWithoutPool)
            };
        }
        private int stepIndex;
        public int StepIndex
        {
            get { return stepIndex; }
            set
            {
                stepIndex = value;
                this.RaisePropertyChanged("StepIndex");
            }
        }
        private Pool editpool;
        public Pool EditPool
        {
            get { return editpool; }
            set
            {
                editpool = value;
                this.RaisePropertyChanged("EditPool");
            }
        }

        public Pool Result { get; set; }
        public Action CloseAction { get; set; }
        public DelegateCommand CloseCmd => new Lazy<DelegateCommand>(() => new DelegateCommand { ExecuteAction = closeAction }).Value;
        private void closeAction(object permerter)
        {
            CloseAction?.Invoke();
        }

        public DelegateCommand PrevCmd { get; set; }
        private void prevCmd(object permerter)
        {
            NewPoolStep.stepBar_Export.Prev();
        }
        public DelegateCommand CloseWithoutPool { get; set; }
        private void closeWithoutPool(object permerter)
        {
            Result = null;
            CloseCmd.Execute(null);
        }
        public DelegateCommand NextCmd { get; set; }
        private void nextCmd(object permerter)
        {
            NewPoolStep.stepBar_Export.Next();
        }
    }
}
