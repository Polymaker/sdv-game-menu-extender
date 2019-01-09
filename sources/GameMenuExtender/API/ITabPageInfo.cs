using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.API
{
    public interface ITabPageInfo
    {
        string Name { get; }
        string TabName { get; }
        ITabInfo Tab { get; }
        string Label { get; set; }
        string Tooltip { get; set; }
        bool Visible { get; set; }
        bool Enabled { get; set; }
        bool Suppressed { get; }
    }
}
