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
