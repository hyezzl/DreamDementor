using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("isContacted")]
	public class ES3UserType_SorrowDoor : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_SorrowDoor() : base(typeof(SorrowDoor)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (SorrowDoor)obj;
			
			writer.WriteProperty("isContacted", instance.isContacted, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (SorrowDoor)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
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


	public class ES3UserType_SorrowDoorArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SorrowDoorArray() : base(typeof(SorrowDoor[]), ES3UserType_SorrowDoor.Instance)
		{
			Instance = this;
		}
	}
}