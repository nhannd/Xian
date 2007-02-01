using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	public delegate string ResultFormatterDelegate<T>(T input);

	public static class DicomBasicResultFormatter
	{
		#region String Input Formatters

		public static string RawStringFormat(string input)
		{
			return input;
		}

		public static string StringListFormat(string[] input)
		{
			string result = "";
			foreach (string inputString in input)
				result += inputString + ",\n";

			if (!String.IsNullOrEmpty(result))
				result = result.Remove(result.Length - 2);

			return result;
		}

		public static string DateFormat(string input)
		{ 
			if (String.IsNullOrEmpty(input))
				return String.Empty;

			DateTime date;
			if (!DateParser.Parse(input, out date))
				return input;

			return date.ToString(Format.DateFormat);
		}

		public static string TimeFormat(string input)
		{
			if (String.IsNullOrEmpty(input))
				return String.Empty;

			DateTime time;
			if (!TimeParser.Parse(input, out time))
				return input;

			return time.ToLongTimeString();
		}

		public static string DateTimeFormat(string input)
		{
			if (String.IsNullOrEmpty(input))
				return String.Empty;

			DateTime datetime;
			if (!DateTimeParser.Parse(input, out datetime))
				return input;

			return datetime.ToString();
		}

		public static string BooleanFormatter(string input)
		{
			if (String.IsNullOrEmpty(input))
				return String.Empty;

#if MONO
			if (Convert.ToByte(input) == 0)
				return SR.BoolNo;
			else
				return SR.BoolYes;
#else
			byte value;
			if (!byte.TryParse(input, out value))
				return input;

			if (value == 0)
				return SR.BoolNo;
			else
				return SR.BoolYes;
#endif
		}

		#endregion

		#region Person Name Formatters

		public static string PersonNameFormatter(PersonName personName)
		{
			return String.Format("{0} {1}", personName.FirstName, personName.LastName);
		}

		public static string PersonNameListFormatter(IEnumerable<PersonName> personNames)
		{ 
			string result = "";
			foreach (PersonName personName in personNames)
				result += PersonNameFormatter(personName) + ",\n";

			if (!String.IsNullOrEmpty(result))
				result = result.Remove(result.Length - 2);

			return result;
		}

		#endregion
	}
}
