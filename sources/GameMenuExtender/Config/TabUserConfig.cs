using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Config
{
    public class TabUserConfig
    {
        /// <summary>
        /// Mod's UniqueName
        /// </summary>
        public string Source { get; set; }

        public string TabName { get; set; }

        public bool? OverrideVisibillity { get; set; }

        public string OverrideLabel { get; set; }

        public int? DisplayIndex { get; set; }
    }
}
