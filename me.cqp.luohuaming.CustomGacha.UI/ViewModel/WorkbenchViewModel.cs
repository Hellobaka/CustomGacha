using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using me.cqp.luohuaming.CustomGacha.UI.Model;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    public class WorkbenchViewModel : NotifyicationObject
    {
        public WorkbenchViewModel()
        {
        }

        #region ---绑定属性---
        private Pool editPool;
        public Pool EditPool
        {
            get { return editPool; }
            set
            {
                editPool = value;
                this.RaisePropertyChanged("EditPool");
            }
        }

        #endregion

        #region ---绑定命令---

        #endregion
    }
}
