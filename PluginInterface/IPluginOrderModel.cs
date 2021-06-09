using PublicInfos;

namespace PluginInterface
{
    public interface IPluginOrderModel
    {
        bool ImplementFlag { get; set; }
        string GetOrderStr();
        bool Judge(string destStr);
        FunctionResult Progress(PluginMessage e);
    }
}
