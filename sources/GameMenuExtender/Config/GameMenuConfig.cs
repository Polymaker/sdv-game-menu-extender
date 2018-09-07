using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Config
{
    public class GameMenuConfig
    {
        public TabUserConfig[] TabsConfigs { get; set; }

        public TabPageUserConfig[] TabPagesConfigs { get; set; }

        public GameMenuConfig()
        {
            TabsConfigs = new TabUserConfig[0];
            TabPagesConfigs = new TabPageUserConfig[0];
        }
    }
}
