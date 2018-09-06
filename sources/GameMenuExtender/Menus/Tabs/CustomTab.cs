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

		public int DisplayIndex { get; set; }

        public CustomTab(string name)
        {
            Name = name;
        }
    }
}
