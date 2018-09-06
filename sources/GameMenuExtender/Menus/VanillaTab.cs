using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
    public class VanillaTab : GameMenuTab
    {
        public override bool IsCustom => false;

        public override bool Enabled { get => true; set => throw new NotSupportedException(); }

        public override bool Visible { get => true; set => throw new NotSupportedException(); }

        public GameMenuTabs TabName => (GameMenuTabs)TabIndex;

        public VanillaTabPage VanillaPage => TabPages.OfType<VanillaTabPage>().FirstOrDefault();

        public VanillaTab(int index, ClickableComponent tab) : base(index)
        {
            TabButton = tab;
            Name = tab.name;
        }
    }
}
