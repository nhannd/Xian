using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
	public static class VMStringConverter
	{
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
