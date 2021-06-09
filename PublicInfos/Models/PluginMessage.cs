namespace PublicInfos
{
    public enum MsgOrigin
    {
        Private,
        Group
    }
    public class PluginMessage
    {
        public MsgOrigin MsgOrigin { get; set; }
        public long OriginID { get; set; }
        public string Message { get; set; }
        public bool HandleFlag { get; set; }
    }
}
