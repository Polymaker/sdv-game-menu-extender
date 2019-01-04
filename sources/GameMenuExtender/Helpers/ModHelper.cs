using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender
{
	static class ModHelper
	{
		static FieldInfo RegistryField;
		static Type ModRegistryType;
		static Type IModMetadataType;
		static PropertyInfo IModMetadataManifestProperty;
        static PropertyInfo IModMetadataModProperty;
        static FieldInfo ModMetadataDictionaryField;
		static MethodInfo ContainsKeyMethod;
		static PropertyInfo ItemProperty;
		static MethodInfo GetModMethod;

		static ModHelper()
		{
			var modRegistryHelperType = typeof(IModRegistry).Assembly.GetType("StardewModdingAPI.Framework.ModHelpers.ModRegistryHelper");
			RegistryField = modRegistryHelperType.GetField("Registry", BindingFlags.Instance | BindingFlags.NonPublic);
			ModRegistryType = RegistryField.FieldType;
			GetModMethod = ModRegistryType.GetMethod("Get");
			IModMetadataType = typeof(IModRegistry).Assembly.GetType("StardewModdingAPI.Framework.IModMetadata");
			IModMetadataManifestProperty = IModMetadataType.GetProperty("Manifest");
            IModMetadataModProperty = IModMetadataType.GetProperty("Mod");
            var modMetadataDictionaryType = typeof(IDictionary<,>).MakeGenericType(typeof(string), IModMetadataType);
			ContainsKeyMethod = modMetadataDictionaryType.GetMethod("ContainsKey");
			ItemProperty = modMetadataDictionaryType.GetProperty("Item");
			ModMetadataDictionaryField = ModRegistryType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
				.FirstOrDefault(f => f.FieldType == modMetadataDictionaryType);
		}

		public static IManifest GetCallingMod(this IModRegistry registryHelper)
		{
			var registry = RegistryField.GetValue(registryHelper);
			var modDictionary = ModMetadataDictionaryField.GetValue(registry);


			StackTrace stack = new StackTrace(2);
			StackFrame[] frames = stack.GetFrames();
			if (frames == null)
			{
				return null;
			}
			StackFrame[] array = frames;
			foreach (StackFrame frame in array)
			{
				MethodBase method = frame.GetMethod();
				if(method.ReflectedType != null)
				{
					var assemblyName = method.ReflectedType.Assembly.FullName;
					if(ModAssemblyExists(modDictionary, assemblyName))
					{
						var modMetadata = ItemProperty.GetValue(modDictionary, new object[] { assemblyName });
						if (modMetadata != null)
						{
                            var modInfo = IModMetadataModProperty.GetValue(modMetadata) as IMod;
                            return modInfo.ModManifest;
						}
					}
				}

			}
			return null;
		}

		public static IManifest GetModByType(this IModRegistry registryHelper, Type type)
		{
            if (type == null)
                return null;

            var registry = RegistryField.GetValue(registryHelper);
			var modDictionary = ModMetadataDictionaryField.GetValue(registry);
			if (ModAssemblyExists(modDictionary, type.Assembly.FullName))
			{
				var modMetadata = ItemProperty.GetValue(modDictionary, new object[] { type.Assembly.FullName });
				if (modMetadata != null)
				{
                    var modInfo = IModMetadataModProperty.GetValue(modMetadata) as IMod;
                    return modInfo.ModManifest;
				}
			}
			return null;
		}

        public static IMod GetMod(this IModRegistry registryHelper, string uniqueID)
        {
			var registry = RegistryField.GetValue(registryHelper);
			var modInfo = GetModMethod.Invoke(registry, new object[] { uniqueID });
			if(modInfo != null)
			{
				return IModMetadataModProperty.GetValue(modInfo) as IMod;
			}
			return null;
        }

        private static bool ModAssemblyExists(object dict, string assemblyName)
		{
			return (bool)ContainsKeyMethod.Invoke(dict, new object[] { assemblyName });
		}
	}
}
