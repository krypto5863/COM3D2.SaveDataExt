using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SaveDataExtended
{
	internal static partial class Serialization
	{
		private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Auto,
			Converters = { new ColorJsonConverter(), new TextureJsonConverter(), new Vector4JsonConverter() }
		};

		internal static void RegisterHooks(Harmony harmony) => harmony.PatchAll(typeof(Hooks));

		private static void HandleSaved(string presetPath)
		{
			Events.OnSaveBeingSaved();

			var saveData = new object[2];

			if (Storage.Data.Count > 0)
			{
				saveData[0] = Storage.Data;
			}

			if (Storage.MaidData.Count > 0)
			{
				saveData[1] = Storage.MaidData;
			}

			if (saveData.All(m => m == null))
			{
				return;
			}

			var serializedData = JsonConvert.SerializeObject(saveData, JsonSerializerSettings);
			File.WriteAllText(presetPath + ".extData", serializedData);

			Storage.CurrentData = Storage.Data;
			Storage.CurrentMaidData = Storage.MaidData;

			Storage.MaidData = null;
			Storage.Data = null;
		}

		private static void HandleLoaded(string presetPath)
		{
			Storage.CurrentData = null;
			Storage.CurrentMaidData = null;

			var extDataPath = presetPath + ".extData";

			if (File.Exists(extDataPath) == false)
			{
				SaveDataExt.LogSource.LogDebug("No data to be loaded was found!");
				goto final;
			}

			var data = File.ReadAllText(extDataPath);

			try
			{
				var allData = JsonConvert.DeserializeObject<object[]>(data, JsonSerializerSettings);

				if (allData == null)
				{
					SaveDataExt.LogSource.LogError("The data loaded from a save was null! It will be discarded!");
				}
				else
				{
					if (allData[0] is Dictionary<string, SaveData> saveData)
					{
						Storage.CurrentData = saveData;
					}

					if (allData[1] is Dictionary<string, Dictionary<string, SaveData>> maidData)
					{
						Storage.CurrentMaidData = maidData;
					}
				}
			}
			catch (Exception exception)
			{
				SaveDataExt.LogSource.LogError($"A error occured while trying to read the extra save data from a save! It will be discarded: {exception.Message}\n{exception.StackTrace}");
			}

			final:
			Events.OnSaveLoaded();
		}

		private static void HandleDeleted(string presetPath)
		{
			var extDataPath = presetPath + ".extData";

			if (File.Exists(extDataPath))
			{
				File.Delete(extDataPath);
			}
		}

		private static partial class Hooks
		{
			[HarmonyPatch(typeof(GameMain), nameof(GameMain.Serialize))]
			[HarmonyPostfix]
			private static void OnSaveLoaded(GameMain __instance, int __0)
			{
				if (__0 < 0)
				{
					return;
				}

				var fileName = __instance.MakeSavePathFileName(__0);
				HandleSaved(fileName);
			}

			[HarmonyPatch(typeof(GameMain), nameof(GameMain.Deserialize))]
			[HarmonyPostfix]
			private static void OnSaveSaved(ref GameMain __instance, int __0)
			{
				var fileName = __instance.MakeSavePathFileName(__0);
				HandleLoaded(fileName);
			}

			[HarmonyPatch(typeof(GameMain), nameof(GameMain.DeleteSerializeData))]
			[HarmonyPostfix]
			private static void OnSavedDeleted(ref GameMain __instance, int __0)
			{
				var fileName = __instance.MakeSavePathFileName(__0);
				HandleDeleted(fileName);
			}
		}
	}
}