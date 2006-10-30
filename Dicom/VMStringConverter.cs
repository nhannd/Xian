using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
	public static class VMStringConverter
	{
		static public string[] ToStringArray(string arrayString)
		{
			if (string.IsNullOrEmpty(arrayString))
				return null;

			return arrayString.Split('\\');
		}

		static public double[] ToDoubleArray(string arrayString)
		{
			string[] stringValues = ToStringArray(arrayString);
			if (stringValues == null)
				return null;

			if (stringValues.Length == 0)
				return null;

			double[] doubleValues = new double[stringValues.Length];
			for (int i = 0; i < stringValues.Length; ++i)
			{
				doubleValues[i] = System.Convert.ToDouble(stringValues[i]);
			}

			return doubleValues;
		}

		static public int[] ToIntArray(string arrayString)
		{
			string[] stringValues = ToStringArray(arrayString);
			if (stringValues == null)
				return null;

			if (stringValues.Length == 0)
				return null;

			int[] intValues = new int[stringValues.Length];
			for (int i = 0; i < stringValues.Length; ++i)
			{
				intValues[i] = System.Convert.ToInt32(stringValues[i]);
			}

			return intValues;
		}

		static public PersonName[] ToPersonNameArray(string arrayString)
		{
			string[] stringValues = ToStringArray(arrayString);
			if (stringValues == null)
				return null;

			if (stringValues.Length == 0)
				return null;

			PersonName[] personNames = new PersonName[stringValues.Length];
			for (int i = 0; i < stringValues.Length; ++i)
			{
				personNames[i] = new PersonName(stringValues[i]);
			}

			return personNames;
		}
	}
}
