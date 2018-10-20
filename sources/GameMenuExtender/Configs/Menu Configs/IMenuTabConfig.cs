using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Configs
{
    public interface IMenuTabConfig
    {
        string Name { get; }
        string Title { get; set; }
        string DefaultTitle { get; set; }
        string DefaultPage { get; set; }
        string PageTitle { get; set; }
        string DefaultPageTitle { get; set; }
        bool PageVisible { get; set; }
        int PageIndex { get; set; }
        bool Visible { get; set; }
        int Index { get; set; }
        bool IsVanilla { get; }
    }
}
