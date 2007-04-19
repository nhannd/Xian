using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	public static class StringUtilities
	{
		public delegate string FormatDelegate<T>(T value);

		public static string CombineDouble(IEnumerable<double> values, string separator, string formatSpecifier)
		{
			return Combine<double>(values, separator,
				delegate(double value)
				{
					if (String.IsNullOrEmpty(formatSpecifier))
						return value.ToString();

					return value.ToString(formatSpecifier);
				});
		}

		public static string CombineFloat(IEnumerable<float> values, string separator, string formatSpecifier)
		{
			return Combine<float>(values, separator,
				delegate(float value)
				{
					if (String.IsNullOrEmpty(formatSpecifier))
						return value.ToString();

					return value.ToString(formatSpecifier);
				});
		}

		public static string CombineInt(IEnumerable<int> values, string separator, string formatSpecifier)
		{
			return Combine<int>(values, separator,
				delegate(int value)
				{
					if (String.IsNullOrEmpty(formatSpecifier))
						return value.ToString();

					return value.ToString(formatSpecifier);
				});
		}

		public static string CombineDateTime(IEnumerable<DateTime> values, string separator, string formatSpecifier)
		{
			return Combine<DateTime>(values, separator,
				delegate(DateTime value)
				{
					if (String.IsNullOrEmpty(formatSpecifier))
						return value.ToString();

					return value.ToString(formatSpecifier);
				});
		}

		public static string Combine<T>(IEnumerable<T> values, string separator)
		{
			return Combine<T>(values, separator, true);
		}

		public static string Combine<T>(IEnumerable<T> values, string separator, bool skipEmptyValues)
		{
			return Combine<T>(values, separator, null, skipEmptyValues);
		}

		public static string Combine<T>(IEnumerable<T> values, string separator, FormatDelegate<T> formatDelegate)
		{
			return Combine<T>(values, separator, formatDelegate, true);
		}

		public static string Combine<T>(IEnumerable<T> values, string separator, FormatDelegate<T> formatDelegate, bool skipEmptyValues)
		{
			if (values == null)
				return "";

			if (separator == null)
				separator = "";

			StringBuilder builder = new StringBuilder();
			int count = 0;
			foreach (T value in values)
			{
				string stringValue = null;
				if (formatDelegate == null)
					stringValue = (value == null) ? null : value.ToString();
				else
					stringValue = formatDelegate(value);

				if (String.IsNullOrEmpty(stringValue) && skipEmptyValues)
					continue;

				if (count++ > 0)
					builder.Append(separator);

				builder.Append(stringValue);
			}

			return builder.ToString();
		}
	}
}
