using GameMenuExtender.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
    public interface IGameMenuTab
    {
        GameMenuTabConfig Configuration { get; }
    }
}
