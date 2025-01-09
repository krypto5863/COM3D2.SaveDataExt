using System.Collections.Generic;
using Newtonsoft.Json;

namespace SaveDataExtended
{
	/// <summary>
	/// Represents the data structure used for saving game-related information, 
	/// including versioning and custom data entries.
	/// </summary>
	public class SaveData
	{
		/// <summary>
		/// Gets or sets the version of the save data.
		/// </summary>
		/// <remarks>
		/// Useful if you make changes to your <see cref="Data"/> as it allows you to handle old versions and new.
		/// </remarks>
		[JsonRequired]
		public uint Version { get; set; }

		/// <summary>
		/// A dictionary containing the key-value pairs of saved data items.
		/// </summary>
		/// <remarks>
		/// Since objects are generic, you typically want to explicitly cast to get your information back.
		/// </remarks>
		/// <example>
		/// <code>
		/// if (Data.TryGetValue("SomeFloatISaved", out var newData))
		/// {
		///		var myNewData = (Float)newData;
		///		//Do something...
		/// }
		/// </code>
		/// </example>
		[JsonRequired]
		public Dictionary<string, object> Data = new Dictionary<string, object>();
	}
}