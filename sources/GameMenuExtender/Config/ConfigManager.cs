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

        public List<TabUserConfig> TabsConfigs { get; } = new List<TabUserConfig>();

        public List<TabPageUserConfig> TabPagesConfigs { get; } = new List<TabPageUserConfig>();

        public ConfigManager(IMod mod)
        {
            Mod = mod;
        }

        public void LoadConfigs()
        {
            TabsConfigs.Clear();
            TabPagesConfigs.Clear();

            var configObj = Mod.Helper.ReadConfig<GameMenuConfig>();

            TabsConfigs.AddRange(configObj.TabsConfigs);
            TabPagesConfigs.AddRange(configObj.TabPagesConfigs);

        }

        public void SaveConfigs()
        {
            var configObj = new GameMenuConfig
            {
                TabsConfigs = TabsConfigs.ToArray(),
                TabPagesConfigs = TabPagesConfigs.ToArray()
            };
            Mod.Helper.WriteConfig(configObj);
        }
    }
}
