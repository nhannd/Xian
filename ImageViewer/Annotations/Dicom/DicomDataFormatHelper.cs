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

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.Annotations.Dicom
{
	/// <summary>
	/// A delegate used to format data of type <typeparamref name="T"/> as a string.
	/// </summary>
	public delegate string ResultFormatterDelegate<T>(T input);

	/// <summary>
	/// A Helper class providing standard methods (<see cref="ResultFormatterDelegate{T}"/>) 
	/// for formatting various types of Dicom tag data for display on the screen.
	/// </summary>
	public static class DicomDataFormatHelper
	{
		#region String Input Formatters

		/// <summary>
		/// Returns the input value, unchanged.
		/// </summary>
		public static string RawStringFormat(string input)
		{
			return input;
		}

		/// <summary>
		/// Returns the input values combined into a single string, separated by ",\n".
		/// </summary>
		public static string StringListFormat(string[] input)
		{
			return StringUtilities.Combine<string>(input, ",\n");
		}

		/// <summary>
		/// Returns the input value formatted according to <see cref="Format.DateFormat"/>.
		/// </summary>
		/// <param name="input">A date value taken from a Dicom Header.</param>
		public static string DateFormat(string input)
		{ 
			if (String.IsNullOrEmpty(input))
				return String.Empty;

			DateTime date;
			if (!DateParser.Parse(input, out date))
				return input;

			return date.ToString(Format.DateFormat);
		}

		/// <summary>
		/// Returns the input value formatted according to <see cref="Format.TimeFormat"/>.
		/// </summary>
		/// <param name="input">A time value taken from a Dicom Header.</param>
		public static string TimeFormat(string input)
		{
			if (String.IsNullOrEmpty(input))
				return String.Empty;

			DateTime time;
			if (!TimeParser.Parse(input, out time))
				return input;

			return time.ToString(Format.TimeFormat);
		}

		/// <summary>
		/// Returns the input value formatted according to <see cref="Format.DateTimeFormat"/>.
		/// </summary>
		/// <param name="input">A date/time value taken from a Dicom Header.</param>
		public static string DateTimeFormat(string input)
		{
			if (String.IsNullOrEmpty(input))
				return String.Empty;

			DateTime datetime;
			if (!DateTimeParser.Parse(input, out datetime))
				return input;

			return datetime.ToString(Format.DateTimeFormat);
		}

		/// <summary>
		/// Returns <see cref="SR.BoolNo"/> if the input value converts to a byte value of zero, otherwise <see cref="SR.BoolYes"/>.
		/// </summary>
		public static string BooleanFormatter(string input)
		{
			if (String.IsNullOrEmpty(input))
				return String.Empty;

			byte value;
			if (!byte.TryParse(input, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out value))
				return input;

			if (value == 0)
				return SR.BoolNo;
			else
				return SR.BoolYes;
		}

		#endregion

		#region Person Name Formatters

		/// <summary>
		/// Returns the <see cref="PersonName.FormattedName"/> property of the input <see cref="PersonName"/>.
		/// </summary>
		public static string PersonNameFormatter(PersonName personName)
		{
			return personName.FormattedName;
		}

		/// <summary>
		/// Returns the <see cref="PersonName.FormattedName"/> property of each input value, separated by ",\n".
		/// </summary>
		public static string PersonNameListFormatter(IEnumerable<PersonName> personNames)
		{
			return StringUtilities.Combine<PersonName>(personNames, ",\n", PersonNameFormatter);
		}

		#endregion
	}
}
