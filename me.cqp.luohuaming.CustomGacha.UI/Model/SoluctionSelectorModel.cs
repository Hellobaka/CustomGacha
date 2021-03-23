using MahApps.Metro.IconPacks;
using me.cqp.luohuaming.CustomGacha.UI.Command;
using PublicInfos;

namespace me.cqp.luohuaming.CustomGacha.UI.Model
{
    public class RecentSoluction
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public string Path { get; set; }
        public Pool Object { get; set; }
    }
    public class ButtonItem
    {
        public string Title { get; set; }
        public string Remark { get; set; }
        public PackIconUniconsKind ImageKind { get; set; }
        public DelegateCommand Action { get; set; }
    }
}
