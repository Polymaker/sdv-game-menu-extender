using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
    internal class GameMenuManager
    {
        public IModHelper Helper { get; private set; }
        public IMonitor Monitor { get; private set; }

        private bool HasInitialized;

        internal GameMenu Menu;

        internal List<GameMenuTab> MenuTabs { get; private set; }
        private List<GameMenuTabPage> MenuTabPages;
        private List<VanillaTab> VanillaTabs => MenuTabs.OfType<VanillaTab>().ToList();
        private IEnumerable<CustomTabPage> CustomTabPages => MenuTabPages.OfType<CustomTabPage>();

        public GameMenuTab CurrentTab { get; private set; }
        public GameMenuTabPage CurrentTabPage { get; private set; }

        internal GameMenuManager(IMod mod)
        {
            Helper = mod.Helper;
            Monitor = mod.Monitor;
            MenuTabs = new List<GameMenuTab>();
            MenuTabPages = new List<GameMenuTabPage>();
        }

        internal void Initialize()
        {
            MenuTabs.Clear();
            MenuTabPages.Clear();

            var gameMenu = new GameMenu();
            var vanillaTabs = Helper.Reflection.GetField<List<ClickableComponent>>(gameMenu, "tabs").GetValue();
            var vanillaPages = Helper.Reflection.GetField<List<IClickableMenu>>(gameMenu, "pages").GetValue();

            for (int i = 0; i < vanillaTabs.Count; i++)
            {
                if (Enum.TryParse(vanillaTabs[i].name, true, out GameMenuTabs tabName))
                {
                    var menuTab = new VanillaTab(i, vanillaTabs[i]);
                    MenuTabs.Add(menuTab);
                    var tabPage = new VanillaTabPage(menuTab, vanillaPages[i]);
                }
            }

            HasInitialized = true;
        }

        internal void ExtendGameMenu()
        {
            CurrentTab = MenuTabs[Menu.currentTab];

            var currentTabs = Helper.Reflection.GetField<List<ClickableComponent>>(Menu, "tabs").GetValue();
            var currentPages = Helper.Reflection.GetField<List<IClickableMenu>>(Menu, "pages").GetValue();
            
            for (int i = 0; i < VanillaTabs.Count; i++)
            {
                var currentPage = currentPages[i];
                var vanillaPageMenuType = GetDefaultTabPageType(VanillaTabs[i].TabName);

                if (currentPage.GetType() == vanillaPageMenuType)
                {
                    VanillaTabs[i].VanillaPage.PageWindow = currentPage;
                }
                else
                {
                    VanillaTabs[i].VanillaPage.InstanciateNewPage();
                    var customPageMod = Helper.ModRegistry.GetModByType(currentPage.GetType());

                    if (customPageMod != null)
                    {
                        var existingTabPage = CustomTabPages.FirstOrDefault(p => p.Tab.TabIndex == i && p.OwnerMod == customPageMod);
                        if (existingTabPage != null)
                        {
                            existingTabPage.PageWindow = currentPage;
                        }
                        else
                        {

                        }
                    }
                }

                currentPages[i] = MenuTabs[i].PageExtender;
            }

            MenuTabs.ForEach(t =>
            {
                t.CurrentTabPageIndex = 0;
                if (t.TabPages.Any(p => p.Visible))
                    t.CurrentTabPageIndex = t.TabPages.IndexOf(t.TabPages.First(p => p.Visible));
            });
        }

        public void ChangeTab(GameMenuTab newTab)
        {
            if (CurrentTab.IsCustom && !newTab.IsCustom)
                VanillaTabs[Menu.currentTab].TabButton.bounds.Y += 8;

            if (newTab.IsVanilla)
                Menu.changeTab((newTab as VanillaTab).TabIndex);
            else
                VanillaTabs[Menu.currentTab].TabButton.bounds.Y -= 8;
        }

        internal void DrawCustomTabs(SpriteBatch b)
        {

        }

        private static Type GetDefaultTabPageType(GameMenuTabs tab)
        {
            switch (tab)
            {
                case GameMenuTabs.Inventory:
                    return typeof(InventoryPage);
                case GameMenuTabs.Skills:
                    return typeof(SkillsPage);
                case GameMenuTabs.Social:
                    return typeof(SocialPage);
                case GameMenuTabs.Map:
                    return typeof(MapPage);
                case GameMenuTabs.Crafting:
                    return typeof(CraftingPage);
                case GameMenuTabs.Collections:
                    return typeof(CollectionsPage);
                case GameMenuTabs.Options:
                    return typeof(OptionsPage);
                case GameMenuTabs.Exit:
                    return typeof(ExitPage);
            }
            return null;
        }
    }
}
