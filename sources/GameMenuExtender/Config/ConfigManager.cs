using GameMenuExtender.Menus;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Config
{
    internal class ConfigManager
    {
        private IMod Mod;

        public List<GameMenuTabConfig> TabConfigs { get; } = new List<GameMenuTabConfig>();

        public List<CustomTabPageConfig> TabPagesConfigs { get; } = new List<CustomTabPageConfig>();

        public IEnumerable<ConfigBase> AllConfigs => TabConfigs.OfType<ConfigBase>().Concat(TabPagesConfigs);

        public ConfigManager(IMod mod)
        {
            Mod = mod;
        }

        public void LoadConfigs()
        {
            TabConfigs.Clear();
            TabPagesConfigs.Clear();

            var configObj = Mod.Helper.ReadConfig<GameMenuExtenderConfig>();
            configObj.LinkTabsAndPages();

            foreach(var vTabConfig in configObj.VanillaTabs.AsList())
            {
                if(vTabConfig != null && !string.IsNullOrEmpty(vTabConfig.DefaultPage))
                {
                    TabConfigs.Add(new VanillaTabConfig(vTabConfig));
                    if (vTabConfig.TabPages != null)
                    {
                        foreach (var tabPage in vTabConfig.TabPages)
                            TabPagesConfigs.Add(new CustomTabPageConfig(tabPage));
                    }
                }
            }

            if (configObj.CustomTabs != null)
            {
                foreach(var customTabConfig in configObj.CustomTabs)
                {
                    TabConfigs.Add(new CustomTabConfig(customTabConfig));
                    foreach (var tabPage in customTabConfig.TabPages)
                        TabPagesConfigs.Add(new CustomTabPageConfig(tabPage));
                }
            }
        }

        public void SaveConfigs()
        {
            var configObj = new GameMenuExtenderConfig();

            foreach(var vTab in TabConfigs.Where(c => c.IsVanilla))
            {
                configObj.VanillaTabs[vTab.Tab] = (GameMenuExtenderConfig.VanillaTabConfig)vTab.GetConfigObject();
                configObj.VanillaTabs[vTab.Tab].TabPages = TabPagesConfigs.Where(p => p.TabName.ToLower() == vTab.Name.ToLower()).OrderBy(c => c.Index).Select(c => c.GetConfigObject()).ToArray();
                if (configObj.VanillaTabs[vTab.Tab].TabPages.Length == 0)
                    configObj.VanillaTabs[vTab.Tab].TabPages = null;
            }

            var customConfigs = new List<GameMenuExtenderConfig.CustomTabConfig>();

            foreach (var cTab in TabConfigs.Where(c => !c.IsVanilla).OrderBy(c => c.Index))
            {
                var tabConfig = (GameMenuExtenderConfig.CustomTabConfig)cTab.GetConfigObject();
                tabConfig.TabPages = TabPagesConfigs.Where(p => p.TabName.ToLower() == cTab.Name.ToLower()).OrderBy(c => c.Index).Select(c => c.GetConfigObject()).ToArray();
                if (tabConfig.TabPages.Length == 0)
                    tabConfig.TabPages = null;
                customConfigs.Add(tabConfig);
            }

            configObj.CustomTabs = customConfigs.ToArray();

            Mod.Helper.WriteConfig(configObj);

            AllConfigs.ToList().ForEach(c => c.MarkAsSaved());
        }

        public VanillaTabConfig GetVanillaTabConfig(GameMenuTabs tab)
        {
            return TabConfigs.OfType<VanillaTabConfig>().FirstOrDefault(c => c.Tab == tab);
        }

        public List<CustomTabPageConfig> GetTabPagesConfig(GameMenuTab tab)
        {
            return TabPagesConfigs.Where(c => tab.NameEquals(c.TabName)).ToList();
        }

        public List<CustomTabPageConfig> GetTabPagesConfig(GameMenuExtenderConfig.TabConfig tab)
        {
            return TabPagesConfigs.Where(c => c.TabName.ToLower() == tab.Name.ToLower()).ToList();
        }

        public GameMenuTabConfig LoadOrCreateConfig(GameMenuTab tab)
        {
            GameMenuTabConfig tabConfig = null;
            if (tab.IsVanilla)
            {
                tabConfig = TabConfigs.FirstOrDefault(c => tab.NameEquals(c.Name) && c.IsVanilla);
                if(tabConfig == null)
                {
                    tabConfig = new VanillaTabConfig((VanillaTab)tab);
                    TabConfigs.Add(tabConfig);
                }
            }
            else
            {
                tabConfig = TabConfigs.FirstOrDefault(c => tab.NameEquals(c.Name) && !c.IsVanilla);
                if(tabConfig == null)
                {
                    tabConfig = new CustomTabConfig((CustomTab)tab);
                    tabConfig.Index = TabConfigs.Count(t => !t.IsVanilla);
                    TabConfigs.Add(tabConfig);
                }
            }

            tabConfig.DefaultTitle = tab.Label;

            if (tabConfig.IsVanilla)
            {
                var vConf = (VanillaTabConfig)tabConfig;
                var vTab = (VanillaTab)tab;
                vConf.DefaultPageTitle = vTab.VanillaPage.Label;
            }
            
            return tabConfig;
        }

        public IMenuTabPageConfig LoadOrCreateConfig(GameMenuTabPage tabPage)
        {
            CustomTabPageConfig tabPageConfig = TabPagesConfigs.FirstOrDefault(p => tabPage.NameEquals(p.Name) && tabPage.Tab.NameEquals(p.TabName));

            if(tabPageConfig == null)
            {
                var tabConfig = TabConfigs.FirstOrDefault(c => tabPage.Tab.NameEquals(c.Name));

                if (tabPage.IsVanilla)
                    return new VanillaTabPageConfig(tabConfig as VanillaTabConfig);

                tabPageConfig = new CustomTabPageConfig(tabPage)
                {
                    Index = GetTabPagesConfig(tabPage.Tab).Count
                };

                if (tabConfig is VanillaTabConfig vtc && vtc.PageIndex > 0)
                    tabPageConfig.Index += 1;

                TabPagesConfigs.Add(tabPageConfig);
            }

            tabPageConfig.DefaultTitle = tabPage.Label;

            return tabPageConfig;
        }

    }
}
