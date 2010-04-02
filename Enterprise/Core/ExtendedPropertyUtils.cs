using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core
{
	public static class ExtendedPropertyUtils
	{
		public static void Update(IDictionary<string, string> target, IDictionary<string, string> source)
		{
			if (source == null)
				return;

			foreach (var pair in source)
			{
				target[pair.Key] = pair.Value;
			}
		}

		public static Dictionary<string, string> Copy(IDictionary<string, string> source)
		{
			return source == null ? new Dictionary<string, string>() :
				new Dictionary<string, string>(source);
		}
	}
}
