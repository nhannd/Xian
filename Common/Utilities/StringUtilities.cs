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
			return Combine<T>(values, separator, null);
		}

		public static string Combine<T>(IEnumerable<T> values, string separator, FormatDelegate<T> formatDelegate)
		{
			if (values == null)
				return "";

			if (separator == null)
				separator = "";

			StringBuilder builder = new StringBuilder();
			int count = 0;
			foreach (T value in values)
			{
				if (count++ > 0)
					builder.Append(separator);

				if (formatDelegate == null)
					builder.Append(value.ToString());
				else
					builder.Append(formatDelegate(value));
			}

			return builder.ToString();
		}
	}
}
