using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Config
{
    public class VanillaTabPageConfig : IMenuTabPageConfig
    {
        private VanillaTabConfig _Tab;

        public string TabName => _Tab.Name;

        public string Name => _Tab.Name;

        public bool Visible { get => _Tab.PageVisible; set => _Tab.PageVisible = value; }

        public int Index { get => _Tab.PageIndex; set => _Tab.PageIndex = value; }

        public string Title { get => _Tab.PageTitle; set => _Tab.PageTitle = value; }

        public string DefaultTitle { get => _Tab.DefaultPageTitle; set => _Tab.DefaultPageTitle = value; }

        public bool IsVanilla => true;

        public bool IsNew => false;

        public VanillaTabPageConfig(VanillaTabConfig tab)
        {
            _Tab = tab;
        }
    }
}
