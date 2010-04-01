using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core
{
	public static class ExtendedPropertyUtils
	{
		public static void Update(IDictionary<string, ExtendedPropertyValue> target, IDictionary<string, ExtendedPropertyValue> source)
		{
			if (source == null)
				return;

			foreach (var pair in source)
			{
				target[pair.Key] = (ExtendedPropertyValue)pair.Value.Clone();
			}
		}

		public static void Update(IDictionary<string, ExtendedPropertyValue> target, IDictionary<string, string> source)
		{
			if (source == null)
				return;

			foreach (var pair in source)
			{
				target[pair.Key] = new ExtendedPropertyValue(pair.Value);
			}
		}

		public static Dictionary<string, string> GetStrings(IDictionary<string, ExtendedPropertyValue> source)
		{
			return CollectionUtils.Map(source, kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value.GetString()));
		}
	}
}
