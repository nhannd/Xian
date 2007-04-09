using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom
{
	public static class VMStringConverter
	{
		static public string ToDicomStringArray<T>(IEnumerable<T> values)
		{
			return StringUtilities.Combine<T>(values, "\\");
		}

		static public string ToDicomStringArray<T>(IEnumerable<T> values, StringUtilities.FormatDelegate<T> formatDelegate)
		{
			return StringUtilities.Combine<T>(values, "\\", formatDelegate);
		}

		static public string ToDicomDoubleArray(IEnumerable<double> values, string formatSpecifier)
		{
			return StringUtilities.CombineDouble(values, "\\", formatSpecifier);
		}

		static public string ToDicomFloatArray(IEnumerable<float> values, string formatSpecifier)
		{
			return StringUtilities.CombineFloat(values, "\\", formatSpecifier);
		}

		static public string ToDicomIntArray(IEnumerable<int> values, string formatSpecifier)
		{
			return StringUtilities.CombineInt(values, "\\", formatSpecifier);
		}
		
		static public string ToDicomPersonNameArray(IEnumerable<PersonName> values)
		{
			return StringUtilities.Combine(values, "\\");
		}

		static public string[] ToStringArray(string arrayString)
		{
			if (arrayString == null)
				arrayString = ""; //return an empty array.

			return arrayString.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
		}

		static public double[] ToDoubleArray(string arrayString)
		{
			string[] stringValues = ToStringArray(arrayString);

			List<double> doubleValues = new List<double>();
			foreach(string value in stringValues)
				doubleValues.Add(System.Convert.ToDouble(value));

			return doubleValues.ToArray();
		}

		static public float[] ToFloatArray(string arrayString)
		{
			string[] stringValues = ToStringArray(arrayString);

			List<float> floatValues = new List<float>();
			foreach (string value in stringValues)
				floatValues.Add((float)System.Convert.ToDouble(value));

			return floatValues.ToArray();
		}

		static public int[] ToIntArray(string arrayString)
		{
			string[] stringValues = ToStringArray(arrayString);

			List<int> intValues = new List<int>();
			foreach (string value in stringValues)
				intValues.Add(System.Convert.ToInt32(value));

			return intValues.ToArray();
		}

		static public PersonName[] ToPersonNameArray(string arrayString)
		{
			string[] stringValues = ToStringArray(arrayString);

			List<PersonName> personNames = new List<PersonName>();
			foreach (string value in stringValues)
				personNames.Add(new PersonName(value));

			return personNames.ToArray();
		}
	}
}
