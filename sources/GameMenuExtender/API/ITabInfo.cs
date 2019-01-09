using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.API
{
    public interface ITabInfo
    {
        string Name { get; }
        string Label { get; set; }
        //string Tooltip { get; set; }
        bool Visible { get; set; }
        bool Enabled { get; set; }
        bool Suppressed { get; }
    }
}
