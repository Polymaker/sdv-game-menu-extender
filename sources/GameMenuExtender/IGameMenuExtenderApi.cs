using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender
{
	public interface IGameMenuExtenderApi
	{
		void RegisterGameMenuExtension(string targetTab, Type customPageType, string label);
	}
}
