﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Config
{
	public class TabPageUserConfig
	{
        /// <summary>
        /// Mod's UniqueName or StardewValley for vanilla
        /// </summary>
        public string Source { get; set; }

        public string TabName { get; set; }

        public string PageName { get; set; }

		public bool? OverrideVisibillity { get; set; }

		public string OverrideLabel { get; set; }

		public int? DisplayIndex { get; set; }
	}
}
