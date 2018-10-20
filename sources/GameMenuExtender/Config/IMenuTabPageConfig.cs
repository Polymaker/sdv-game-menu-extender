using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Config
{
    public interface IMenuTabPageConfig
    {
        string TabName { get; }
        string Name { get; }
        bool Visible { get; set; }
        int Index { get; set; }
        string Title { get; set; }
        string DefaultTitle { get; set; }
        bool IsVanilla { get; }
        bool IsNew { get; }
    }
}
