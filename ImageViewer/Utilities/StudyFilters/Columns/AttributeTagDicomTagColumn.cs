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
using System.Text;
using System.Text.RegularExpressions;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class AttributeTagDicomTagColumn : UnsignedDicomTagColumn
	{
		private static readonly Regex _pattern = new Regex(@"^(?<Open>[(])?\s*([0-9a-fA-F]{4})\s*[,]?\s*([0-9a-fA-F]{4})\s*(?(Open)[)]|)$", RegexOptions.Compiled);

		public AttributeTagDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomArray<uint> GetTypedValue(StudyItem item)
		{
			DicomAttribute attribute = item[base.Tag];

			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return new DicomArray<uint>();

			uint?[] result;
			try
			{
				result = new uint?[CountValues(attribute)];
				for (int n = 0; n < result.Length; n++)
				{
					uint value;
					if (attribute.TryGetUInt32(n, out value))
						result[n] = value;
				}
			}
			catch (DicomException)
			{
				return null;
			}
			return new DicomArray<uint>(result, FormatArray(result));
		}

		public override bool Parse(string input, out DicomArray<uint> output)
		{
			return DicomArray<uint>.TryParse(input, TagTryParse, out output);
		}

		private static bool TagTryParse(string s, out uint result)
		{
			result = 0;

			Match m = _pattern.Match(s);
			if (m.Success)
				result = uint.Parse(m.Groups[1].Value + ushort.Parse(m.Groups[2].Value));
			return m.Success;
		}

		private static string FormatArray(IEnumerable<uint?> input)
		{
			StringBuilder sb = new StringBuilder();
			foreach (uint? element in input)
			{
				if (element.HasValue)
					sb.AppendFormat("({0:x4},{1:x4})", (element.Value >> 16) & 0x0000ffff, element.Value & 0x0000ffff);
				sb.Append('\\');
			}
			return sb.ToString(0, Math.Max(0, sb.Length - 1));
		}
	}
}