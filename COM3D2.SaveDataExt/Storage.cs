﻿using JetBrains.Annotations;
using System.Collections.Generic;

namespace SaveDataExtended
{
	/// <summary>
	/// Contains all primary functions for manipulating, saving and retrieving <see cref="SaveData"/>.
	/// </summary>
	public static class Storage
	{
		/// <summary>
		/// The last loaded maid data, it is replaced by newly loaded data. This is NOT saved.
		/// </summary>
		private static Dictionary<string, Dictionary<string, SaveData>> _currentMaidData;

		internal static Dictionary<string, Dictionary<string, SaveData>> CurrentMaidData
		{
			get => _currentMaidData ??= new Dictionary<string, Dictionary<string, SaveData>>();
			set => _currentMaidData = value;
		}

		private static Dictionary<string, Dictionary<string, SaveData>> _maidData;

		internal static Dictionary<string, Dictionary<string, SaveData>> MaidData
		{
			get => _maidData ??= new Dictionary<string, Dictionary<string, SaveData>>();
			set => _maidData = value;
		}

		/// <summary>
		/// The last loaded data, it is replaced by newly loaded data. This is NOT saved.
		/// </summary>
		private static Dictionary<string, SaveData> _currentData;

		internal static Dictionary<string, SaveData> CurrentData
		{
			get => _currentData ??= new Dictionary<string, SaveData>();
			set => _currentData = value;
		}

		private static Dictionary<string, SaveData> _data;

		internal static Dictionary<string, SaveData> Data
		{
			get => _data ??= new Dictionary<string, SaveData>();
			set => _data = value;
		}

		/// <summary>
		/// Associates the specified <see cref="SaveData"/> with a unique identifier for a given maid,
		/// allowing the data to be stored for saving and later retrieval.
		/// </summary>
		/// <param name="maid">The <see cref="Maid"/> object for which data is being saved.</param>
		/// <param name="id">A unique identifier for the owner of the data. This ID distinguishes
		/// the data's origin and must be unique for your application.</param>
		/// <param name="data">The <see cref="SaveData"/> object containing the information to be saved.</param>
		/// <remarks>
		/// This method initializes a new data dictionary for the maid if none exists and
		/// assigns the provided <paramref name="data"/> to the specified <paramref name="id"/>.
		/// If <paramref name="id"/> already exists, its associated data is overwritten.
		/// </remarks>
		public static void CreateMaidDataWithId([NotNull] Maid maid, [NotNull] string id, SaveData data)
		{
			if (MaidData.TryGetValue(maid.status.guid, out var maidDictionary) == false)
			{
				MaidData[maid.status.guid] = new Dictionary<string, SaveData>();
				maidDictionary = MaidData[maid.status.guid];
			}

			maidDictionary[id] = data;
			MaidData[maid.status.guid] = maidDictionary;
		}

		/// <summary>
		/// Retrieves the <see cref="SaveData"/> associated with a specific maid and identifier.
		/// </summary>
		/// <param name="maid">The <see cref="Maid"/> object whose data is being retrieved.</param>
		/// <param name="id">A unique identifier for the data owner. This ID distinguishes
		/// the data's origin and must be unique for your application.</param>
		/// <returns>The <see cref="SaveData"/> for the specified maid and ID, or <c>null</c> if no data exists.</returns>
		[CanBeNull]
		public static SaveData GetMaidDataById([NotNull] Maid maid, [NotNull] string id)
		{
			if (CurrentMaidData.TryGetValue(maid.status.guid, out var maidDataDictionary) == false)
			{
				return null;
			}

			maidDataDictionary.TryGetValue(id, out var saveData);
			return saveData;
		}

		/// <summary>
		/// Stores the specified <see cref="SaveData"/> with a unique identifier for saving and later retrieval.
		/// </summary>
		/// <param name="id">A unique identifier for the data being saved.</param>
		/// <param name="data">The <see cref="SaveData"/> to be saved.</param>
		public static void CreateSaveDataWithId([NotNull] string id, SaveData data)
		{
			Data[id] = data;
		}

		/// <summary>
		/// Retrieves the <see cref="SaveData"/> associated with the specified identifier.
		/// </summary>
		/// <param name="id">A unique identifier for the data owner.</param>
		/// <returns>The <see cref="SaveData"/> associated with the ID, or <c>null</c> if no data exists.</returns>
		[CanBeNull]
		public static SaveData GetSaveDataById([NotNull] string id)
		{
			CurrentData.TryGetValue(id, out var data);
			return data;
		}
	}
}