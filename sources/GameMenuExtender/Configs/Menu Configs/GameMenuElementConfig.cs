using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Configs
{
    public abstract class GameMenuElementConfig : ConfigBase
    {
        public string UniqueID { get; }
        public string ModID { get; private set; }
        public abstract bool IsVanilla { get; }
        public bool IsCustom => !IsVanilla;

        public GameMenuElementConfig(string uniqueID)
        {
            UniqueID = uniqueID;
            ModID = uniqueID.Contains(":") ? uniqueID.Split(':')[0] : "StardewValley";
        }

        public bool NameEquals(string name)
        {
            return UniqueID.Trim().Equals(name?.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
