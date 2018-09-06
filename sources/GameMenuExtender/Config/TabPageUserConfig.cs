using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Config
{
	public class TabPageUserConfig
	{
		public string TabPageUniqueID { get; set; }

		public bool? OverrideVisibillity { get; set; }

		public string OverrideLabel { get; set; }

		public int DisplayIndex { get; set; }
	}
}
