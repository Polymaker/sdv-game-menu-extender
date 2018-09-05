﻿using StardewModdingAPI;
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
		static FieldInfo ModMetadataDictionaryField;
		static MethodInfo ContainsKeyMethod;
		static PropertyInfo ItemProperty;

		static ModHelper()
		{
			var modRegistryHelperType = typeof(IModRegistry).Assembly.GetType("StardewModdingAPI.Framework.ModHelpers.ModRegistryHelper");
			RegistryField = modRegistryHelperType.GetField("Registry", BindingFlags.Instance | BindingFlags.NonPublic);
			ModRegistryType = RegistryField.FieldType;
			IModMetadataType = typeof(IModRegistry).Assembly.GetType("StardewModdingAPI.Framework.IModMetadata");
			IModMetadataManifestProperty = IModMetadataType.GetProperty("Manifest");

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
					if(IsModAssemblyExist(modDictionary, assemblyName))
					{
						var modMetadata = ItemProperty.GetValue(modDictionary, new object[] { assemblyName });
						if (modMetadata != null)
						{
							return IModMetadataManifestProperty.GetValue(modMetadata) as IManifest;
						}
					}
				}

			}
			return null;
		}

		public static IManifest GetModByType(this IModRegistry registryHelper, Type type)
		{
			var registry = RegistryField.GetValue(registryHelper);
			var modDictionary = ModMetadataDictionaryField.GetValue(registry);
			if (IsModAssemblyExist(modDictionary, type.Assembly.FullName))
			{
				var modMetadata = ItemProperty.GetValue(modDictionary, new object[] { type.Assembly.FullName });
				if (modMetadata != null)
				{
					return IModMetadataManifestProperty.GetValue(modMetadata) as IManifest;
				}
			}
			return null;
		}

		private static bool IsModAssemblyExist(object dict, string assemblyName)
		{
			return (bool)ContainsKeyMethod.Invoke(dict, new object[] { assemblyName });
		}
	}
}