using Newtonsoft.Json;
using System.ComponentModel;

namespace GameMenuExtender.Configs.Serialization
{
    public class CustomTabCfg : TabCfgBase
    {
        [JsonIgnore]
        public int Index { get; set; }

        [DefaultValue(true), JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Visible { get; set; } = true;
    }
}
