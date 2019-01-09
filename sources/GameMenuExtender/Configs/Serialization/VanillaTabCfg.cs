using Newtonsoft.Json;
using System.ComponentModel;

namespace GameMenuExtender.Configs.Serialization
{
    public class VanillaTabCfg : TabCfgBase
    {
        [JsonIgnore]
        public GameMenuTabs MenuTab { get; set; }

        [JsonIgnore]
        public override string Name { get => MenuTab.ToString(); set { } }

        [DefaultValue(null), JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string VanillaPageOverride { get; set; }

        [DefaultValue(0), JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int VanillaPageIndex { get; set; }

        [DefaultValue(null), JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string VanillaPageTitle { get; set; }
    }
}
