using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace SaveDataExtended
{
	internal class TextureJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is Texture2D texture)
			{
				writer.WriteStartObject(); // Start of the object

				writer.WritePropertyName("name");
				writer.WriteValue(texture.name);

				writer.WritePropertyName("data");
				writer.WriteValue(Convert.ToBase64String(texture.EncodeToPNG())); // Convert byte array to Base64 string

				writer.WriteEndObject(); // End of the object
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var token = JToken.Load(reader);
			var name = token["name"].Value<string>();
			var data = Convert.FromBase64String(token["data"].Value<string>()); // Decode Base64 string to byte array

			var finishedTexture = new Texture2D(1, 1);
			finishedTexture.LoadImage(data);
			finishedTexture.name = name;

			return finishedTexture;
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Texture2D);
		}
	}

	internal class Vector4JsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is Vector4 vector4 == false)
			{
				return;
			}

			writer.WriteStartObject();

			writer.WritePropertyName("x");
			writer.WriteValue(vector4.x);

			writer.WritePropertyName("y");
			writer.WriteValue(vector4.y);

			writer.WritePropertyName("z");
			writer.WriteValue(vector4.z);

			writer.WritePropertyName("w");
			writer.WriteValue(vector4.w);

			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
			JsonSerializer serializer)
		{
			var token = JToken.Load(reader);
			return new Vector4(token["x"].Value<float>(), token["y"].Value<float>(), token["z"].Value<float>(),
				token["w"].Value<float>());
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Vector4);
		}
	}

	internal class ColorJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (!(value is Color col))
			{
				return;
			}

			writer.WriteStartObject();

			writer.WritePropertyName("r");
			writer.WriteValue(col.r);

			writer.WritePropertyName("g");
			writer.WriteValue(col.g);

			writer.WritePropertyName("b");
			writer.WriteValue(col.b);

			writer.WritePropertyName("a");
			writer.WriteValue(col.a);

			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
			JsonSerializer serializer)
		{
			var token = JToken.Load(reader);
			return new Color(token["r"].Value<float>(), token["g"].Value<float>(), token["b"].Value<float>(),
				token["a"].Value<float>());
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Color);
		}
	}
}