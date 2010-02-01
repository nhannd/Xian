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
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public interface IDicomTagColumn
	{
		uint Tag { get; }
		string VR { get; }

		string Name { get; }
		string Key { get; }
		IStudyFilter Owner { get; }
		string GetText(IStudyItem item);
		object GetValue(IStudyItem item);
		Type GetValueType();
	}

	public abstract class DicomTagColumn<T> : StudyFilterColumnBase<T>, IDicomTagColumn
	{
		private readonly string _tagName;
		private readonly uint _tag;
		private readonly string _vr;

		protected DicomTagColumn(DicomTag dicomTag)
		{
			_tag = dicomTag.TagValue;
			_vr = dicomTag.VR.Name;

			uint tagGroup = (_tag >> 16) & 0x0000FFFF;
			uint tagElement = _tag & 0x0000FFFF;

			if (DicomTagDictionary.GetDicomTag(dicomTag.TagValue) == null)
				_tagName = string.Format(SR.FormatUnknownDicomTag, tagGroup, tagElement);
			else
				_tagName = string.Format(SR.FormatDicomTag, tagGroup, tagElement, dicomTag.Name);
		}

		public override string Name
		{
			get { return _tagName; }
		}

		public override string Key
		{
			get { return _tag.ToString("x8"); }
		}

		public uint Tag
		{
			get { return _tag; }
		}

		public string VR
		{
			get { return _vr; }
		}

		protected static int CountValues(DicomAttribute attribute)
		{
			return (int) Math.Min(50, attribute.Count);
		}
	}
}