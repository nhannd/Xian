using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.DataStore
{
	internal static class Convert
	{
		internal static IEnumerable<T> Cast<T>(IEnumerable original)
		{
			foreach (T item in original)
				yield return item;
		}

		private static string ToString(object value, TypeConverter converter)
		{
			if (value is string)
				return (string)value;
			else
				return converter.ConvertToInvariantString(value);
		}

		public static string ToDicomStringArray(object value, TypeConverter converter)
		{
			if (value == null)
				return null;

			if (value.GetType().IsArray)
			{
				Array array = value as Array;
				if (array == null)
					return null;

				return DicomStringHelper.GetDicomStringArray(ToStringArray(array, converter));
			}
			else
			{
				return ToString(value, converter);
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
					stringArray[i++] = ToString(arrayValue, converter);

				return stringArray;
			}
			else
			{
				return new string[] { ToString(value, converter) };
			}
		}
	}
}
