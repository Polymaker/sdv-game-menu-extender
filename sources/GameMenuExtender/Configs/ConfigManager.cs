using GameMenuExtender.Configs.Serialization;
using GameMenuExtender.Menus;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Configs
{
    internal class ConfigManager
    {
        private static IMod Mod => GameMenuExtenderMod.Instance;

        public List<MenuTabConfig> TabConfigs { get; } = new List<MenuTabConfig>();

        public List<CustomTabPageConfig> TabPagesConfigs { get; } = new List<CustomTabPageConfig>();

        public IEnumerable<ConfigBase> AllConfigs => TabConfigs.OfType<ConfigBase>().Concat(TabPagesConfigs);

        public void Reload()
        {
            TabConfigs.Clear();
            TabPagesConfigs.Clear();

            var configObj = Mod.Helper.ReadConfig<GameMenuExtenderCfg>();
            configObj.LinkTabsAndPages();

            foreach(var vTabConfig in configObj.VanillaTabs.AsList())
            {
                if(vTabConfig != null)
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

            foreach (var tab in TabConfigs)
            {
                tab.TabPages.Clear();
                tab.TabPages.AddRange(GetTabPagesConfigs(tab));
            }
        }

        public void Save()
        {
            var configObj = new GameMenuExtenderCfg();

            foreach(var vTab in TabConfigs.Where(c => c.IsVanilla))
            {
                configObj.VanillaTabs[vTab.Tab] = (VanillaTabCfg)vTab.GetJsonObject();
                configObj.VanillaTabs[vTab.Tab].TabPages = TabPagesConfigs.Where(p => p.TabName.ToLower() == vTab.Name.ToLower()).OrderBy(c => c.Index).Select(c => c.GetJsonObject()).ToArray();
                if (configObj.VanillaTabs[vTab.Tab].TabPages.Length == 0)
                    configObj.VanillaTabs[vTab.Tab].TabPages = null;
            }

            var customConfigs = new List<CustomTabCfg>();

            foreach (var cTab in TabConfigs.Where(c => !c.IsVanilla).OrderBy(c => c.Index))
            {
                var tabConfig = (CustomTabCfg)cTab.GetJsonObject();
                tabConfig.TabPages = TabPagesConfigs.Where(p => p.TabName.ToLower() == cTab.Name.ToLower()).OrderBy(c => c.Index).Select(c => c.GetJsonObject()).ToArray();
                if (tabConfig.TabPages.Length == 0)
                    tabConfig.TabPages = null;
                customConfigs.Add(tabConfig);
            }

            configObj.CustomTabs = customConfigs.ToArray();

            Mod.Helper.WriteConfig(configObj);

            AllConfigs.ToList().ForEach(c => c.MarkAsSaved());
        }

        public static ConfigManager Load()
        {
            var config = new ConfigManager();
            config.Reload();
            return config;
        }

        public VanillaTabConfig GetVanillaTabConfig(GameMenuTabs tab)
        {
            return TabConfigs.OfType<VanillaTabConfig>().FirstOrDefault(c => c.Tab == tab);
        }

        public List<CustomTabPageConfig> GetTabPagesConfig(GameMenuTab tab)
        {
            return TabPagesConfigs.Where(c => tab.NameEquals(c.TabName)).ToList();
        }

        public List<CustomTabPageConfig> GetTabPagesConfig(TabCfgBase tab)
        {
            return TabPagesConfigs.Where(c => c.TabName.ToLower() == tab.Name.ToLower()).ToList();
        }

        public List<MenuTabPageConfig> GetTabPagesConfigs(MenuTabConfig tabConfig)
        {
            var allPages = TabPagesConfigs.Where(c => c.TabName == tabConfig.Name).OfType<MenuTabPageConfig>().ToList();
            if (tabConfig is VanillaTabConfig vanillaTab)
                allPages.Add(new VanillaTabPageConfig(vanillaTab));
            
            return allPages.OrderBy(p => p.Index).ToList();
        }

        public MenuTabConfig LoadOrCreateConfig(GameMenuTab tab)
        {
            MenuTabConfig tabConfig = null;
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
                vConf.DefaultVanillaTitle = vTab.VanillaPage.Label;
            }
            
            return tabConfig;
        }

        public MenuTabPageConfig LoadOrCreateConfig(GameMenuTabPage tabPage)
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

                if (tabConfig is VanillaTabConfig vtc && vtc.VanillaPageIndex > 0)
                    tabPageConfig.Index += 1;

                TabPagesConfigs.Add(tabPageConfig);
            }

            tabPageConfig.DefaultTitle = tabPage.Label;

            return tabPageConfig;
        }

        public void PurgeRemovedModsConfigs(bool saveIfNeeded = true)
        {
            for (int i = TabPagesConfigs.Count - 1; i >= 0; i--)
            {
                var pageConfig = TabPagesConfigs[i];
                if (!Mod.Helper.ModRegistry.IsLoaded(pageConfig.ModID))
                {
                    TabPagesConfigs.Remove(pageConfig);
                }
            }
            for (int i = TabConfigs.Count - 1; i >= 0; i--)
            {
                var tabConfig = TabConfigs[i] as CustomTabConfig;
                if (tabConfig != null && !Mod.Helper.ModRegistry.IsLoaded(tabConfig.ModID))
                {
                    TabConfigs.Remove(tabConfig);
                }
            }

            if (saveIfNeeded && AllConfigs.Any(c => c.HasChanged))
                Save();
        }

        public static void ValidateAndAdjustTabsConfigs(ConfigManager configs, bool saveIfNeeded = true)
        {
            //ENSURE THAT THE CONFIGURED VANILLA PAGES OVERRIDES EXISTS
            foreach (var tab in configs.TabConfigs.OfType<VanillaTabConfig>())
            {
                if (!string.IsNullOrEmpty(tab.VanillaPageOverride) &&
                    !tab.TabPages.Any(p => p.NameEquals(tab.VanillaPageOverride)))
                {
                    //page override does not exists
                    tab.VanillaPageOverride = null;
                }
            }

            //UPDATES AND CORRECTS THE TABS ORDER
            int currentTabIndex = 0;

            foreach (var tab in configs.TabConfigs.OfType<CustomTabConfig>().OrderBy(t => t.Index))
            {
                tab.Index = currentTabIndex++;
                int currentPageIndex = 0;
                foreach(var page in configs.GetTabPagesConfigs(tab).OrderBy(p => p.Index))
                    page.Index = currentPageIndex++;
            }

            if (saveIfNeeded && configs.AllConfigs.Any(c => c.HasChanged))
                configs.Save();
        }
    }
}
