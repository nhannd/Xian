using System.Text;

namespace ClearCanvas.Ris.Client.Formatting
{
	public static class StringMask
	{
		/// <summary>
		/// Fills the mask with the supplied text.
		/// </summary>
		/// <remarks>
		/// If the supplied text is longer than the mask, the mask is applied to the right-most characters of the text.
		/// </remarks>
		/// <param name="text"></param>
		/// <param name="mask"></param>
		/// <returns></returns>
		public static string Apply(string text, string mask)
		{
			if (string.IsNullOrEmpty(text)) return string.Empty;

			StringBuilder maskedText = new StringBuilder();

			// Fill the mask from right to left
			string reverseMask = Reverse(mask);
			string reverseText = Reverse(text);
			int reverseTextIndex = 0;

			foreach (char c in reverseMask)
			{
				if (reverseTextIndex >= reverseText.Length)
				{
					break;
				}

				// Are there additional mask characters that need to be specified?
				if (c == '0')
				{
					maskedText.Append(reverseText[reverseTextIndex++]);
				}
				else
				{
					maskedText.Append(c);
				}
			}

			if (reverseTextIndex < reverseText.Length)
			{
				maskedText.Append(reverseText.Substring(reverseTextIndex));
			}

			return Reverse(maskedText.ToString());
		}

		// Why doesn't .Net include this ??
		private static string Reverse(string forward)
		{
			char[] reverse = new char[forward.Length];
			for (int i = 0; i < forward.Length; i++)
			{
				reverse[i] = forward[forward.Length - 1 - i];
			}
			return new string(reverse);
		}
	}
}