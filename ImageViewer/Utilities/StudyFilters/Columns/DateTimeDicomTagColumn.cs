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
using System.Globalization;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class DateTimeDicomTagColumn : DicomTagColumn<DicomArray<DateTime>>, ITemporalSortableColumn
	{
		private readonly string _dateTimeFormat;

		public DateTimeDicomTagColumn(DicomTag dicomTag) : base(dicomTag)
		{
			_dateTimeFormat = "f";
			if (base.VR == "DA")
				_dateTimeFormat = "d";
			else if (base.VR == "TM")
				_dateTimeFormat = "t";
		}

		private bool DateTimeTryParseExact(string s, out DateTime result)
		{
			return DateTime.TryParseExact(s, _dateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out result);
		}

		public override DicomArray<DateTime> GetTypedValue(StudyItem item)
		{
			DicomAttribute attribute = item[base.Tag];

			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return new DicomArray<DateTime>();

			DateTime?[] result;
			try
			{
				result = new DateTime?[CountValues(attribute)];
				for (int n = 0; n < result.Length; n++)
				{
					DateTime value;
					if (attribute.TryGetDateTime(n, out value))
						result[n] = value;
				}
			}
			catch (DicomException)
			{
				return null;
			}

			return new DicomArray<DateTime>(result, FormatArray(result, _dateTimeFormat));
		}

		public override bool Parse(string input, out DicomArray<DateTime> output)
		{
			if (DicomArray<DateTime>.TryParse(input, DateTimeTryParseExact, out output))
				return true;

			if (DicomArray<DateTime>.TryParse(input, DateTime.TryParse, out output))
				return true;

			switch (base.VR)
			{
				case "DA":
					if (DicomArray<DateTime>.TryParse(input, DateParser.Parse, out output))
						return true;
					break;
				case "TM":
					if (DicomArray<DateTime>.TryParse(input, TimeParser.Parse, out output))
						return true;
					break;
				case "DT":
				default:
					if (DicomArray<DateTime>.TryParse(input, DateTimeParser.Parse, out output))
						return true;
					break;
			}
			return false;
		}

		public override int Compare(StudyItem x, StudyItem y)
		{
			return this.CompareTemporally(x, y);
		}

		public int CompareTemporally(StudyItem x, StudyItem y)
		{
			return DicomArray<DateTime>.Compare(this.GetTypedValue(x), this.GetTypedValue(y));
		}

		private static string FormatArray(IEnumerable<DateTime?> input, string elementFormat)
		{
			StringBuilder sb = new StringBuilder();
			foreach (DateTime? element in input)
			{
				if (element.HasValue)
					sb.Append(element.Value.ToString(elementFormat));
				sb.Append('\\');
			}
			return sb.ToString(0, Math.Max(0, sb.Length - 1));
		}
	}
}