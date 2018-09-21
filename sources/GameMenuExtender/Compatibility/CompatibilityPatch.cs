using GameMenuExtender.Menus;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Compatibility
{
    internal abstract class CompatibilityPatch : IDisposable
    {
        private GameMenuTabPage _PreviousPage;

        protected GameMenuManager Manager { get; private set; }

        protected IModHelper Helper => Manager.Helper;

        protected IMonitor Monitor => Manager.Monitor;

        public CompatibilityPatch(GameMenuManager manager)
        {
            Manager = manager;
        }

        public abstract bool IsAppliable();

        public abstract bool InitializePatch();

        internal void SuscribeToManagerEvents()
        {
            Manager.CurrentTabPageChanged += Manager_CurrentTabPageChanged;
            Manager.GameMenuOpened += Manager_GameMenuOpened;
            Manager.GameMenuClosed += Manager_GameMenuClosed;
        }

        internal void UnsuscribeToManagerEvents()
        {
            Manager.CurrentTabPageChanged -= Manager_CurrentTabPageChanged;
            Manager.GameMenuOpened -= Manager_GameMenuOpened;
            Manager.GameMenuClosed -= Manager_GameMenuClosed;
        }

        private void Manager_GameMenuOpened(object sender, EventArgs e)
        {
            _PreviousPage = Manager.CurrentTabPage;
            OnGameMenuOpened();
        }

        private void Manager_GameMenuClosed(object sender, EventArgs e)
        {
            _PreviousPage = null;
            OnGameMenuClosed();
        }

        private void Manager_CurrentTabPageChanged(object sender, EventArgs e)
        {
            OnCurrentTabPageChanged(_PreviousPage, Manager.CurrentTabPage);
            _PreviousPage = Manager.CurrentTabPage;
        }

        protected virtual void OnCurrentTabPageChanged(GameMenuTabPage previous, GameMenuTabPage current)
        {

        }

        protected virtual void OnGameMenuOpened()
        {

        }

        protected virtual void OnGameMenuClosed()
        {

        }

        public void Dispose()
        {
            UnsuscribeToManagerEvents();
        }
    }
}
