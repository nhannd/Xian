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

using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class StringDicomTagColumn : DicomTagColumn<DicomObjectArray<string>>, ILexicalSortableColumn
	{
		public StringDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomObjectArray<string> GetTypedValue(StudyItem item)
		{
			DicomAttribute attribute = item[base.Tag];

			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return new DicomObjectArray<string>();
			if (attribute is DicomAttributeSingleValueText)
				return new DicomObjectArray<string>(attribute.ToString());
			if (!(attribute is DicomAttributeMultiValueText))
				return new DicomObjectArray<string>(string.Format(SR.LabelVRIncorrect, attribute.Tag.VR.Name, base.VR));

			string[] result;
			result = new string[CountValues(attribute)];
			for (int n = 0; n < result.Length; n++)
				result[n] = attribute.GetString(n, string.Empty);
			return new DicomObjectArray<string>(result);
		}

		public override bool Parse(string input, out DicomObjectArray<string> output)
		{
			return DicomObjectArray<string>.TryParse(
				input,
				delegate(string s, out string result)
					{
						result = s;
						return true;
					},
				out output);
		}

		public override int Compare(StudyItem x, StudyItem y)
		{
			return this.CompareLexically(x, y);
		}

		public int CompareLexically(StudyItem x, StudyItem y)
		{
			return DicomObjectArray<string>.Compare(this.GetTypedValue(x), this.GetTypedValue(y));
		}
	}
}