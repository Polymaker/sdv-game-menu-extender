using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GameMenuExtender.Menus;
using StardewModdingAPI;

namespace GameMenuExtender.Compatibility
{
    internal class GiftTasteHelperPatch : CompatibilityPatch
    {
        private IMod ModInstance;
        private MethodInfo SubscribeEventsMethod;
        private MethodInfo UnsubscribeEventsMethod;
        private FieldInfo CurrentGiftHelperField;

        public GiftTasteHelperPatch(GameMenuManager manager) : base(manager)
        {
            
        }

        public override bool IsAppliable()
        {
            return Helper.ModRegistry.IsLoaded("tstaples.GiftTasteHelper");
        }

        public override bool InitializePatch()
        {
            Manager.Monitor.Log("Initializing Compatibility Patch for staples.GiftTasteHelper");

            ModInstance = Manager.Helper.ModRegistry.GetMod("tstaples.GiftTasteHelper");
            if (ModInstance != null)
            {
                SubscribeEventsMethod = ModInstance.GetType().GetMethod("SubscribeEvents", BindingFlags.NonPublic | BindingFlags.Instance);
                UnsubscribeEventsMethod = ModInstance.GetType().GetMethod("UnsubscribeEvents", BindingFlags.NonPublic | BindingFlags.Instance);
                CurrentGiftHelperField = ModInstance.GetType().GetField("CurrentGiftHelper", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            return SubscribeEventsMethod != null && UnsubscribeEventsMethod != null && CurrentGiftHelperField != null;
        }

        protected override void OnGameMenuOpened()
        {
            base.OnGameMenuOpened();
            UnsubscribeEvents();
        }

        protected override void OnCurrentTabPageChanged(GameMenuTabPage previous, GameMenuTabPage current)
        {
            if(SubscribeEventsMethod != null && UnsubscribeEventsMethod != null)
            {
                if (current.IsVanilla && current.PageType == typeof(StardewValley.Menus.SocialPage))
                {
                    SubscribeEvents();
                }
                else if (previous != null && previous.IsVanilla && previous.PageType == typeof(StardewValley.Menus.SocialPage))
                {
                    UnsubscribeEvents();
                }
            }
        }

        private void SubscribeEvents()
        {
            var currentHelper = CurrentGiftHelperField.GetValue(ModInstance);
            if (currentHelper != null)
            {
                Manager.Monitor.Log("Calling GiftTasteHelper.SubscribeEvents");
                SubscribeEventsMethod.Invoke(ModInstance, null);
            }
        }

        private void UnsubscribeEvents()
        {
            var currentHelper = CurrentGiftHelperField.GetValue(ModInstance);
            if (currentHelper != null)
            {
                Manager.Monitor.Log("Calling GiftTasteHelper.UnsubscribeEvents");
                UnsubscribeEventsMethod.Invoke(ModInstance, null);
            }
        }
    }
}
