using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
    public class CustomTab : GameMenuTab
    {
        public override bool IsCustom => true;

        public CustomTab(int index, string name) : base(index)
        {
            Name = name;
        }
    }
}
