#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal class CodeSequenceAnnotationItem : DicomAnnotationItem<string>
	{
		public CodeSequenceAnnotationItem(string identifier, IAnnotationResourceResolver resolver, uint codeSequenceTag)
			: this(identifier, resolver, codeSequenceTag, null) {}

		public CodeSequenceAnnotationItem(string identifier, IAnnotationResourceResolver resolver, uint codeSequenceTag, uint? descriptorTag)
			: base(identifier, resolver, new SopDataRetriever(codeSequenceTag, descriptorTag).RetrieveData, FormatResult) {}

		public CodeSequenceAnnotationItem(string identifier, string displayName, string label, uint codeSequenceTag)
			: this(identifier, displayName, label, codeSequenceTag, null) {}

		public CodeSequenceAnnotationItem(string identifier, string displayName, string label, uint codeSequenceTag, uint? descriptorTag)
			: base(identifier, displayName, label, new SopDataRetriever(codeSequenceTag, descriptorTag).RetrieveData, FormatResult) {}

		private static string FormatResult(string s)
		{
			return s;
		}

		private class SopDataRetriever
		{
			private readonly uint _codeSequenceTag;
			private readonly uint? _descriptorTag;

			public SopDataRetriever(uint codeSequenceTag, uint? descriptorTag)
			{
				_codeSequenceTag = codeSequenceTag;
				_descriptorTag = descriptorTag;
			}

			public string RetrieveData(Frame f)
			{
				try
				{
					var codeSequence = f.ParentImageSop[_codeSequenceTag] as DicomAttributeSQ;
					var codeSequenceItem = codeSequence != null && !codeSequence.IsEmpty && !codeSequence.IsNull && codeSequence.Count > 0 ? new CodeSequenceMacro(codeSequence[0]) : null;
					var descriptor = _descriptorTag.HasValue ? f.ParentImageSop[_descriptorTag.Value].ToString() : null;

					if (codeSequenceItem != null && !string.IsNullOrEmpty(codeSequenceItem.CodeMeaning))
						return codeSequenceItem.CodeMeaning;
					if (!string.IsNullOrEmpty(descriptor))
						return descriptor;
					if (codeSequenceItem != null && !string.IsNullOrEmpty(codeSequenceItem.CodeValue))
						return string.Format(SR.FormatCodeSequence, codeSequenceItem.CodeValue, codeSequenceItem.CodingSchemeDesignator);
					return string.Empty;
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Debug, ex, "Failed to parse code sequence attribute at tag ({0:X4},{1:X4})", (_codeSequenceTag >> 16) & 0x00FFFF, _codeSequenceTag & 0x00FFFF);
					return string.Empty;
				}
			}
		}
	}
}