using System;

namespace SaveDataExtended
{
	/// <summary>
	/// Events that occur when data is saved or loaded are here.
	/// </summary>
	/// <remarks>Typically, you'll want to keep track of your data changing by subscribing to the events here.</remarks>
	public static class Events
	{
		/// <summary>
		/// Represents a method that handles events when a preset is being saved for a specified <see cref="Maid"/>.
		/// </summary>
		public delegate void PresetSaveEventHandler(Maid maid, CharacterMgr.PresetType presetType);

		/// <summary>
		/// Event triggered when a preset is being saved.
		/// </summary>
		/// <remarks>
		/// This is usually called right before your <see cref="SaveData"/> is saved.
		/// Usually, you want to use this to set your <see cref="SaveData"/> using <see cref="CreateMaidDataWithId"/> for saving.
		/// </remarks>
		public static event PresetSaveEventHandler PresetBeingSaved;

		/// <summary>
		/// Represents a method that handles events when a preset is being loaded for a specified <see cref="Maid"/>.
		/// </summary>
		public delegate void PresetLoadEventHandler(Maid maid, CharacterMgr.PresetType presetType);

		/// <summary>
		/// Event triggered when a preset is being loaded.
		/// </summary>
		/// <remarks>
		/// This is usually called right after your <see cref="SaveData"/> has been loaded.
		/// You should subscribe to this to know when you should retrieve new data using <see cref="GetMaidDataById"/>.
		/// </remarks>
		public static event PresetLoadEventHandler PresetLoaded;

		/// <summary>
		/// Represents a method that handles events when the game is being saved.
		/// </summary>
		public delegate void SaveEventHandler();

		/// <summary>
		/// Event triggered when the game is being saved.
		/// </summary>
		/// <remarks>
		/// This is usually called right before your <see cref="SaveData"/> for both your maids and saves has been saved.
		/// This means that <see cref="PresetBeingSaved"/> is not called despite <see cref="Maid"/> <see cref="SaveData"/> also being saved.
		/// Usually, you want to use this to set your <see cref="SaveData"/> using <see cref="CreateSaveDataWithId"/> and/or <see cref="CreateMaidDataWithId"/> for saving.
		/// </remarks>
		public static event SaveEventHandler SaveBeingSaved;

		/// <summary>
		/// Represents a method that handles events when a saved game is loaded.
		/// </summary>
		public delegate void LoadEventHandler();

		/// <summary>
		/// Event triggered when a saved game is loaded.
		/// </summary>
		/// <remarks>
		/// This is usually called right after your <see cref="SaveData"/> has been loaded for both maids and saves.
		/// This means that <see cref="PresetLoaded"/> is not called despite <see cref="Maid"/> <see cref="SaveData"/> also being loaded.
		/// You should subscribe to this to know when you should retrieve new data using <see cref="GetSaveDataById"/> and/or <see cref="GetMaidDataById"/>.
		/// </remarks>
		public static event LoadEventHandler SaveLoaded;

		internal static void OnPresetBeingSaved(Maid maid, CharacterMgr.PresetType presetType)
		{
			if (PresetBeingSaved == null)
				return;

			foreach (var entry in PresetBeingSaved.GetInvocationList())
			{
				var handler = (PresetSaveEventHandler)entry;
				try
				{
					handler.Invoke(maid, presetType);
				}
				catch (Exception ex)
				{
					SaveDataExt.LogSource.LogError($"Subscriber crash in {nameof(Events)}.{nameof(PresetBeingSaved)} - {ex}");
				}
			}
		}
		internal static void OnPresetLoaded(Maid maid, CharacterMgr.PresetType presetType)
		{
			if (PresetLoaded == null)
				return;

			foreach (var entry in PresetLoaded.GetInvocationList())
			{
				var handler = (PresetLoadEventHandler)entry;
				try
				{
					handler.Invoke(maid, presetType);
				}
				catch (Exception ex)
				{
					SaveDataExt.LogSource.LogError($"Subscriber crash in {nameof(Events)}.{nameof(PresetLoaded)} - {ex}");
				}
			}
		}

		internal static void OnSaveLoaded()
		{
			if (SaveLoaded == null)
				return;

			foreach (var entry in SaveLoaded.GetInvocationList())
			{
				var handler = (LoadEventHandler)entry;
				try
				{
					handler.Invoke();
				}
				catch (Exception ex)
				{
					SaveDataExt.LogSource.LogError($"Subscriber crash in {nameof(Events)}.{nameof(SaveLoaded)} - {ex}");
				}
			}
		}

		internal static void OnSaveBeingSaved()
		{
			if (SaveBeingSaved == null)
				return;

			foreach (var entry in SaveBeingSaved.GetInvocationList())
			{
				var handler = (SaveEventHandler)entry;
				try
				{
					handler.Invoke();
				}
				catch (Exception ex)
				{
					SaveDataExt.LogSource.LogError($"Subscriber crash in {nameof(Events)}.{nameof(SaveBeingSaved)} - {ex}");
				}
			}
		}
	}
}