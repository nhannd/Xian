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
	public class IntegerDicomTagColumn : DicomTagColumn<DicomArray<int>>, INumericSortableColumn
	{
		public IntegerDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomArray<int> GetTypedValue(StudyItem item)
		{
			DicomAttribute attribute = item[base.Tag];

			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return new DicomArray<int>();

			int?[] result;
			try
			{
				result = new int?[CountValues(attribute)];
				for (int n = 0; n < result.Length; n++)
				{
					int value;
					if (attribute.TryGetInt32(n, out value))
						result[n] = value;
				}
			}
			catch (DicomException)
			{
				return null;
			}
			return new DicomArray<int>(result);
		}

		public override bool Parse(string input, out DicomArray<int> output)
		{
			return DicomArray<int>.TryParse(input, int.TryParse, out output);
		}

		public override int Compare(StudyItem x, StudyItem y)
		{
			return this.CompareNumerically(x, y);
		}

		public int CompareNumerically(StudyItem x, StudyItem y)
		{
			return DicomArray<int>.Compare(this.GetTypedValue(x), this.GetTypedValue(y));
		}
	}
}