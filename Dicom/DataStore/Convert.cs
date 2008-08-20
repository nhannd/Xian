using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.DataStore
{
	internal static class Convert
	{
		private static string ConvertToString(object value, TypeConverter converter)
		{
			if (value is string)
				return (string)value;
			else
				return converter.ConvertToInvariantString(value);
		}

		private static IEnumerable<string> Cast(IEnumerable source, TypeConverter converter)
		{
			foreach (object value in source)
				yield return ConvertToString(value, converter);
		}

		public static string ToString(object value, TypeConverter converter)
		{
			if (value == null)
				return null;

			if (value.GetType().IsArray)
			{
				Array array = value as Array;
				if (array == null)
					return null;

				return DicomStringHelper.GetDicomStringArray(Cast(array, converter));
			}
			else
			{
				return ConvertToString(value, converter);
			}
		}

		public static string[] ToStringArray(object value, TypeConverter converter)
		{
			if (value == null)
				return new string[]{ };

			if (value.GetType().IsArray)
			{
				Array array = value as Array;
				if (array == null)
					return new string[]{ };

				string[] stringArray = new string[array.Length];
				int i = 0;
				foreach (object arrayValue in array)
					stringArray[i++] = ConvertToString(arrayValue, converter);

				return stringArray;
			}
			else
			{
				return new string[] { ConvertToString(value, converter) };
			}
		}
	}
}
