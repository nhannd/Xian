using ClearCanvas.Ris.Client;
using System.Text;

namespace ClearCanvas.Ris.Client.Formatting
{
	public static class AccessionFormat
	{
		/// <summary>
		/// Formats the Accession number according to the default format as specified in <see cref="FormatSettings"/>
		/// </summary>
		/// <param name="accession"></param>
		/// <returns></returns>
		public static string Format(string accession)
		{
			return Format(accession, FormatSettings.Default.AccessionNumberDefaultFormat);
		}

		/// <summary>
		/// Formats the Accession number according to the specified format string
		/// </summary>
		/// <remarks>
		/// Valid format specifiers are as follows:
		///		%L - accession number label as specified in <see cref="FormatSettings"/>
		///		%N - number masked as specified in <see cref="FormatSettings"/>
		///		%n - number without mask
		/// </remarks>
		/// <param name="accession"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string Format(string accession, string format)
		{
			string result = format;

			result = result.Replace("%L", FormatSettings.Default.AccessionNumberLabel ?? "");
			result = result.Replace("%N", StringMask.Apply(accession, FormatSettings.Default.AccessionNumberMask) ?? "");
			result = result.Replace("%n", accession ?? "");

			return result.Trim();
		}

	}
}
