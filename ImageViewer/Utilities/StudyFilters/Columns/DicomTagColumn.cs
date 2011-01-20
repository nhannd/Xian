#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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