using GameMenuExtender.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Configs.Serialization
{

    public class GameMenuExtenderCfg
    {
        public VanillaTabCfg Inventory { get; set; }
        public VanillaTabCfg Skills { get; set; }
        public VanillaTabCfg Social { get; set; }
        public VanillaTabCfg Map { get; set; }
        public VanillaTabCfg Crafting { get; set; }
        public VanillaTabCfg Collections { get; set; }
        public VanillaTabCfg Options { get; set; }
        public VanillaTabCfg Exit { get; set; }

        public CustomTabCfg[] CustomTabs { get; set; }

        [JsonIgnore]
        public ArrayProperty<GameMenuTabs, VanillaTabCfg> VanillaTabs { get; private set; }

        public GameMenuExtenderCfg()
        {
            CustomTabs = new CustomTabCfg[0];

            VanillaTabs = new ArrayProperty<GameMenuTabs, VanillaTabCfg>(
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

        
    }
}
