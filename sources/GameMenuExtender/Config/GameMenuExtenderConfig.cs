using GameMenuExtender.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Config
{
    public class GameMenuExtenderConfig
    {
        public VanillaTabConfig Inventory { get; set; }
        public VanillaTabConfig Skills { get; set; }
        public VanillaTabConfig Social { get; set; }
        public VanillaTabConfig Map { get; set; }
        public VanillaTabConfig Crafting { get; set; }
        public VanillaTabConfig Collections { get; set; }
        public VanillaTabConfig Options { get; set; }
        public VanillaTabConfig Exit { get; set; }

        public CustomTabConfig[] CustomTabs { get; set; }

        [JsonIgnore]
        public ArrayProperty<GameMenuTabs, VanillaTabConfig> VanillaTabs { get; private set; }

        public GameMenuExtenderConfig()
        {
            CustomTabs = new CustomTabConfig[0];

            VanillaTabs = new ArrayProperty<GameMenuTabs, VanillaTabConfig>(
                (tab) =>
                {
                    switch (tab)
                    {
                        case GameMenuTabs.Inventory:
                            return Inventory;
                        case GameMenuTabs.Skills:
                            return Skills;
                        case GameMenuTabs.Social:
                            return Social;
                        case GameMenuTabs.Map:
                            return Map;
                        case GameMenuTabs.Crafting:
                            return Crafting;
                        case GameMenuTabs.Collections:
                            return Collections;
                        case GameMenuTabs.Options:
                            return Options;
                        case GameMenuTabs.Exit:
                            return Exit;
                        default:
                            return null;
                    }
                },
                (tab, config) =>
                {
                    switch (tab)
                    {
                        case GameMenuTabs.Inventory:
                            Inventory = config; break;
                        case GameMenuTabs.Skills:
                            Skills = config; break;
                        case GameMenuTabs.Social:
                            Social = config; break;
                        case GameMenuTabs.Map:
                            Map = config; break;
                        case GameMenuTabs.Crafting:
                            Crafting = config; break;
                        case GameMenuTabs.Collections:
                            Collections = config; break;
                        case GameMenuTabs.Options:
                            Options = config; break;
                        case GameMenuTabs.Exit:
                            Exit = config; break;
                    }
                }
            );
        }

        public void LinkTabsAndPages()
        {
            foreach(var kv in VanillaTabs.AsDictionary())
            {
                if (kv.Value == null)
                    continue;

                kv.Value.MenuTab = kv.Key;

                if(kv.Value.TabPages != null)
                {
                    int index = 0;
                    foreach (var page in kv.Value.TabPages)
                    {
                        page.TabName = kv.Key.ToString();
                        page.Index = index++;
                    }
                }
            }

            if(CustomTabs != null)
            {
                int tabIndex = 0;
                foreach (var tab in CustomTabs)
                {
                    tab.Index = tabIndex++;
                    if (tab.TabPages != null)
                    {
                        int pageIndex = 0;
                        foreach (var page in tab.TabPages)
                        {
                            page.TabName = tab.Name;
                            page.Index = pageIndex++;
                        }
                    }
                }
            }
        }

        public class CustomTabPageConfig
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
        }

        public abstract class TabConfig
        {
            public virtual string Name { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string Title { get; set; }
            public string DefaultPage { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public CustomTabPageConfig[] TabPages { get; set; }// = new CustomTabPageConfig[0];
        }

        public class VanillaTabConfig : TabConfig
        {
            [JsonIgnore]
            public GameMenuTabs MenuTab { get; set; }

            [JsonIgnore]
            public override string Name { get => MenuTab.ToString(); set { } }

            [DefaultValue(true), JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public bool VanillaPageVisible { get; set; } = true;

            [DefaultValue(0), JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public int VanillaPageIndex { get; set; }

            [DefaultValue(null), JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public string VanillaPageTitle { get; set; }
        }

        public class CustomTabConfig : TabConfig
        {
            [JsonIgnore]
            public int Index { get; set; }

            [DefaultValue(true), JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public bool Visible { get; set; } = true;
        }
    }
}
