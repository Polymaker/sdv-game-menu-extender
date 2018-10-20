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

        //public Dictionary<GameMenuTabs, VanillaTabConfig> VanillaTabConfigs { get; } = new Dictionary<GameMenuTabs, VanillaTabConfig>();

        public List<GameMenuTabConfig> TabConfigs { get; } = new List<GameMenuTabConfig>();

        public List<GameMenuTabPageConfig> TabPagesConfigs { get; } = new List<GameMenuTabPageConfig>();

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

            foreach(var vTabConfig in configObj.VanillaTabs.AsDictionary())
            {
                if(vTabConfig.Value != null && !string.IsNullOrEmpty(vTabConfig.Value.DefaultPage))
                {
                    TabConfigs.Add(new GameMenuTabConfig(vTabConfig.Value, vTabConfig.Key.ToString()));
                    if (vTabConfig.Value.TabPages != null)
                    {
                        foreach(var tabPage in vTabConfig.Value.TabPages)
                            TabPagesConfigs.Add(new GameMenuTabPageConfig(vTabConfig.Key.ToString(), tabPage));
                    }
                    
                }
            }

            if (configObj.CustomTabs != null)
            {
                foreach(var customTabConfig in configObj.CustomTabs)
                {
                    TabConfigs.Add(new GameMenuTabConfig(customTabConfig));
                    foreach (var tabPage in customTabConfig.TabPages)
                        TabPagesConfigs.Add(new GameMenuTabPageConfig(customTabConfig.Name, tabPage));
                }
            }
        }

        public void SaveConfigs()
        {
            var configObj = new GameMenuExtenderConfig();

            foreach(var vTab in TabConfigs.Where(c => c.IsVanilla))
            {
                configObj.VanillaTabs[vTab.Tab] = (GameMenuExtenderConfig.VanillaTabConfig)vTab.GetConfigObject();
                configObj.VanillaTabs[vTab.Tab].TabPages = TabPagesConfigs.Where(p => p.TabName.ToLower() == vTab.Name.ToLower()).Select(c => c.GetConfigObject()).ToArray();
                if (configObj.VanillaTabs[vTab.Tab].TabPages.Length == 0)
                    configObj.VanillaTabs[vTab.Tab].TabPages = null;
            }

            var customConfigs = new List<GameMenuExtenderConfig.CustomTabConfig>();

            foreach (var cTab in TabConfigs.Where(c => !c.IsVanilla))
            {
                var tabConfig = (GameMenuExtenderConfig.CustomTabConfig)cTab.GetConfigObject();
                tabConfig.TabPages = TabPagesConfigs.Where(p => p.TabName.ToLower() == cTab.Name.ToLower()).Select(c => c.GetConfigObject()).ToArray();
                if (tabConfig.TabPages.Length == 0)
                    tabConfig.TabPages = null;
                customConfigs.Add(tabConfig);
            }

            configObj.CustomTabs = customConfigs.ToArray();

            Mod.Helper.WriteConfig(configObj);

            AllConfigs.ToList().ForEach(c => c.MarkAsSaved());
        }

        public GameMenuTabConfig GetVanillaTabConfig(GameMenuTabs tab)
        {
            return TabConfigs.FirstOrDefault(c => c.Tab == tab);
        }

        public List<GameMenuTabPageConfig> GetTabPagesConfig(GameMenuTab tab)
        {
            return TabPagesConfigs.Where(c => tab.NameEquals(c.TabName)).ToList();
        }

        public GameMenuTabConfig LoadOrCreateConfig(GameMenuTab tab)
        {
            GameMenuTabConfig tabConfig = null;
            if (tab.IsVanilla)
            {
                tabConfig = TabConfigs.FirstOrDefault(c => tab.NameEquals(c.Name) && c.IsVanilla);
                if(tabConfig == null)
                {
                    tabConfig = new GameMenuTabConfig((VanillaTab)tab);
                    TabConfigs.Add(tabConfig);
                }
            }
            else
            {
                tabConfig = TabConfigs.FirstOrDefault(c => tab.NameEquals(c.Name) && !c.IsVanilla);
                if(tabConfig == null)
                {
                    tabConfig = new GameMenuTabConfig((CustomTab)tab);
                    tabConfig.Index = TabConfigs.Count(t => !t.IsVanilla);
                    TabConfigs.Add(tabConfig);
                }
            }

            if (string.IsNullOrEmpty(tabConfig.Title))
                tabConfig.Title = tab.Label;

            if (string.IsNullOrEmpty(tabConfig.VanillaPageTitle) && tab is VanillaTab vt)
                tabConfig.VanillaPageTitle = vt.VanillaPage.Label;

            return tabConfig;
        }

        public GameMenuTabPageConfig LoadOrCreateConfig(GameMenuTabPage tabPage)
        {
            GameMenuTabPageConfig tabPageConfig = TabPagesConfigs.FirstOrDefault(p => tabPage.NameEquals(p.Name) && tabPage.Tab.NameEquals(p.TabName));

            if(tabPageConfig == null)
            {
                var tabConfig = TabConfigs.FirstOrDefault(c => tabPage.Tab.NameEquals(c.Name));

                tabPageConfig = new GameMenuTabPageConfig(tabPage);

                tabPageConfig.Index = GetTabPagesConfig(tabPage.Tab).Count;
                if (tabConfig.VanillaPageIndex > 0)
                    tabPageConfig.Index += 1;

                TabPagesConfigs.Add(tabPageConfig);
                

            }

            if (string.IsNullOrEmpty(tabPageConfig.Title))
                tabPageConfig.Title = tabPage.Label;

            return tabPageConfig;
        }

    }
}
