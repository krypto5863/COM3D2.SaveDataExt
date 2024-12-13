using System.Collections.Generic;
using HarmonyLib;
using System.IO;
using Newtonsoft.Json;

namespace SaveDataExtended
{
	internal static partial class Serialization
	{
		private static void HandlePresetSaved(Maid maid, string presetPath, CharacterMgr.PresetType presetType)
		{
			Events.OnPresetBeingSaved(maid, presetType);

			if (Storage.MaidData.TryGetValue(maid.status.guid, out var saveDatas) == false || saveDatas.Count == 0)
			{
				return;
			}
			
			var serializedData = JsonConvert.SerializeObject(saveDatas, JsonSerializerSettings);

			SaveDataExt.LogSource.LogDebug(serializedData);
			File.WriteAllText(presetPath + ".extData",serializedData);
		}

		private static void HandlePresetLoaded(Maid maid, string presetPath, CharacterMgr.PresetType presetType)
		{
			var extDataPath = presetPath + ".extData";

			if (File.Exists(extDataPath) == false)
			{
				Storage.CurrentMaidData.Remove(maid.status.guid);
				goto final;
			}

			var data = File.ReadAllText(extDataPath);
			SaveDataExt.LogSource.LogDebug(data);
			try
			{
				var maidData = JsonConvert.DeserializeObject<Dictionary<string, SaveData>>(data, JsonSerializerSettings);

				if (maidData == null)
				{
					SaveDataExt.LogSource.LogError("The data loaded from a preset was null! It will be discarded!");
					Storage.CurrentMaidData.Remove(maid.status.guid);
				}
				else
				{
					Storage.CurrentMaidData[maid.status.guid] = maidData;
				}
			}
			catch
			{
				SaveDataExt.LogSource.LogError("A error occured while trying to read the extra save data from a preset! It will be discarded!");
				Storage.CurrentMaidData.Remove(maid.status.guid);
			}

			final:
			Events.OnPresetLoaded(maid, presetType);
		}

		private static void HandlePresetDeleted(string presetPath)
		{
			var extDataPath = presetPath + ".extData";

			if (File.Exists(extDataPath) == false)
			{
				File.Delete(extDataPath);
			}
		}

		private static partial class Hooks
		{
			[HarmonyPatch(typeof(CharacterMgr), nameof(CharacterMgr.PresetSave))]
			[HarmonyPostfix]
			private static void OnPresetSaved(CharacterMgr __instance, Maid __0, CharacterMgr.PresetType __1, CharacterMgr.Preset __result)
			{
				var presetPath = Path.Combine(__instance.PresetDirectory, __result.strFileName);
				HandlePresetSaved(__0, presetPath, __1);
			}

			[HarmonyPatch(typeof(CharacterMgr), nameof(CharacterMgr.PresetSet), typeof(Maid), typeof(CharacterMgr.Preset))]
			[HarmonyPostfix]
			private static void OnPresetLoaded(CharacterMgr __instance, Maid __0, CharacterMgr.Preset __1)
			{
				var presetPath = Path.Combine(__instance.PresetDirectory, __1.strFileName);
				HandlePresetLoaded(__0, presetPath, __1.ePreType);
			}

			[HarmonyPatch(typeof(CharacterMgr), nameof(CharacterMgr.PresetDelete))]
			[HarmonyPostfix]
			private static void OnPresetDeleted(CharacterMgr __instance, CharacterMgr.Preset __0)
			{
				var presetPath = Path.Combine(__instance.PresetDirectory, __0.strFileName);
				HandlePresetDeleted(presetPath);
			}
		}
	}
}