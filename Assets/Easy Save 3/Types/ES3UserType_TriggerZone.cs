using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute()]
	public class ES3UserType_TriggerZone : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_TriggerZone() : base(typeof(TriggerZone)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (TriggerZone)obj;
			
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (TriggerZone)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_TriggerZoneArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_TriggerZoneArray() : base(typeof(TriggerZone[]), ES3UserType_TriggerZone.Instance)
		{
			Instance = this;
		}
	}
}