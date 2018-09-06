using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
    public class CustomTabPage : GameMenuTabPage
    {
        public override bool IsCustom => true;

        public CustomTabPage(CustomTab tab) : base(tab)
        {

        }
    }
}
