using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute()]
	public class ES3UserType_BoxCollider : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_BoxCollider() : base(typeof(UnityEngine.BoxCollider)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.BoxCollider)obj;
			
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.BoxCollider)obj;
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


	public class ES3UserType_BoxColliderArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_BoxColliderArray() : base(typeof(UnityEngine.BoxCollider[]), ES3UserType_BoxCollider.Instance)
		{
			Instance = this;
		}
	}
}