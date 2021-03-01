using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace me.cqp.luohuaming.CustomGacha.UI.Command
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public Action<object> ExecuteAction { get; set; }
        public Func<object, bool> CanExecuteFunc { get; set; }
        public bool CanExecute(object parameter)
        {
            if (CanExecuteFunc is null) return true;
            return CanExecuteFunc(parameter);
        }

        public void Execute(object parameter)
        {
            if (ExecuteAction is null) return;
            ExecuteAction(parameter);
        }
    }
}
