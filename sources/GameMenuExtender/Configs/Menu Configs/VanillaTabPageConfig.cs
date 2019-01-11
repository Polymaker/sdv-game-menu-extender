using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Configs
{
    public class VanillaTabPageConfig : MenuTabPageConfig
    {
        private VanillaTabConfig _Tab;

        public override bool Visible { get => !_Tab.HideVanillaPage; set { } }

        public override int Index { get => _Tab.VanillaPageIndex; set => _Tab.VanillaPageIndex = value; }

        public override string Title { get => string.IsNullOrEmpty(_Tab.VanillaPageTitle) ? DefaultTitle : _Tab.VanillaPageTitle; set => _Tab.VanillaPageTitle = value; }

        //public override string DefaultTitle { get => _Tab.DefaultVanillaTitle; set => _Tab.DefaultVanillaTitle = value; }

        public override bool IsVanilla => true;

        public VanillaTabPageConfig(VanillaTabConfig tab)
            : base(tab.Name, tab.Name)
        {
            _Tab = tab;
        }
    }
}
