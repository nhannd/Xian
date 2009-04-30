#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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