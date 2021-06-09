using System;
using Newtonsoft.Json;

namespace PublicInfos
{
    public class UpdateInfo
    {
        public Plugin Plugin { get; set; }
        public Update_Pool[] Pools { get; set; }
    }

    public class Plugin
    {
        public string URL { get; set; }
        public Version[] Versions { get; set; }
    }

    public class Version
    {
        public int VersionID { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Info { get; set; }
    }
    [JsonObject("Pool")]
    public class Update_Pool
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Info { get; set; }
        public Font[] Fonts { get; set; }
    }

    public class Font
    {
        public string Name { get; set; }
        public string URL { get; set; }
    }
}
