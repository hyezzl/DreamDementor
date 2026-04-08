using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute()]
	public class ES3UserType_NPC : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_NPC() : base(typeof(NPC)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (NPC)obj;
			
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (NPC)obj;
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


	public class ES3UserType_NPCArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_NPCArray() : base(typeof(NPC[]), ES3UserType_NPC.Instance)
		{
			Instance = this;
		}
	}
}