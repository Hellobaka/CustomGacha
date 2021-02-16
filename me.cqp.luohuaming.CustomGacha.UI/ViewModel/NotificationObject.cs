using System.ComponentModel;

namespace me.cqp.luohuaming.CustomGacha.UI.ViewModel
{
    class NotifyicationObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
