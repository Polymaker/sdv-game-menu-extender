using Newtonsoft.Json;
using System.ComponentModel;

namespace GameMenuExtender.Configs.Serialization
{
    public class CustomPageCfg
    {
        [JsonIgnore]
        public string TabName { get; set; }

        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonIgnore]
        public int Index { get; set; }

        [DefaultValue(true), JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Visible { get; set; } = true;

        [DefaultValue(false), JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool IsNonAPI { get; set; } = false;

        [DefaultValue(false), JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool VanillaReplacement { get; set; } = false;
    }
}
