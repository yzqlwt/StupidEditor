
namespace StupidEditor
{
    using System;
    using System.Collections.Generic;

    public class ConfigItem
    {
        public string Name;
        public DateTime Time;
        public string Tag;
    }

    public class ConfigTemplate
    {
        public List<string> plist;
        public Dictionary<string, ConfigItem> resource;
    }
}
