using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("isFriend", "isContacted")]
	public class ES3UserType_Flower : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Flower() : base(typeof(Flower)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Flower)obj;
			
			writer.WriteProperty("isFriend", instance.isFriend, ES3Type_bool.Instance);
			writer.WriteProperty("isContacted", instance.isContacted, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Flower)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "isFriend":
						instance.isFriend = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "isContacted":
						instance.isContacted = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_FlowerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_FlowerArray() : base(typeof(Flower[]), ES3UserType_Flower.Instance)
		{
			Instance = this;
		}
	}
}