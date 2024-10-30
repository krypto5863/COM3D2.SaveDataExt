using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

//These two lines tell your plugin to not give a flying fuck about accessing private variables/classes whatever. It requires a publicized stub of the library with those private objects though.
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace SaveDataExtended
{
	[BepInPlugin(Guid, PluginName, Version)]
	internal class SaveDataExt : BaseUnityPlugin
	{
		/// <summary> Plugin GUID </summary>
		public const string Guid = "krypto5863.saveDataExt.com3d2.com";
		/// <summary> Plugin name </summary>
		public const string PluginName = "Events";
		/// <summary> Plugin version </summary>
		public const string Version = "1.0";
		internal static SaveDataExt Instance { get; private set; }
		internal static ManualLogSource LogSource => Instance.Logger;

		private void Awake()
		{
			//Useful for engaging co-routines or accessing variables non-static variables. Completely optional though.
			Instance = this;
			var harmony = new Harmony(Guid);
			Serialization.RegisterHooks(harmony);

#if DEBUG
			Events.SaveBeingSaved += () =>
			{
				Storage.CreateSaveDataWithId("Events", new SaveData
				{
					Version = 1,
					Data = new Dictionary<string, object>
					{
						{
							"date", DateTime.Now
						}
					}
				});
			};

			Events.SaveLoaded += () =>
			{
				var data = Storage.GetSaveDataById("Events");

				if (data == null)
				{
					Logger.LogWarning("Debug save data was not found in the loaded save!");
					return;
				}

				var dateTime = (DateTime)data.Data["date"];
				Logger.LogDebug(dateTime.ToString(CultureInfo.CurrentCulture));
			};

			Events.PresetBeingSaved += maid =>
			{
				Storage.CreateMaidDataWithId(maid, "Events", new SaveData
				{
					Data = new Dictionary<string, object>
					{
						{"date", DateTime.Now}
					},
					Version = 1
				});
			};


			Events.PresetLoaded += maid =>
			{
				var data = Storage.GetMaidDataById(maid, "Events");

				if (data?.Data == null || data.Data.TryGetValue("date", out var date) == false)
				{
					Logger.LogWarning("Debug save data was not found in the loaded preset!");
					return;
				}

				var dateTime = (DateTime)date;
				Logger.LogDebug(dateTime.ToString(CultureInfo.CurrentCulture));
			};
#endif
		}
	}
}