using Newtonsoft.Json;

namespace GameMenuExtender.Configs.Serialization
{
    public abstract class TabCfgBase
    {
        public virtual string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)/*, JsonConverter(typeof(TabPageConfigArrayConverter))*/]
        public CustomPageCfg[] TabPages { get; set; }
    }
}
