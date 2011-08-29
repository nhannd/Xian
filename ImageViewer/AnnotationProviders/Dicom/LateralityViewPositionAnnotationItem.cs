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
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal class LateralityViewPositionAnnotationItem : AnnotationItem
	{
		private readonly bool _showLaterality;
		private readonly bool _showViewPosition;

		public LateralityViewPositionAnnotationItem(string identifier, bool showLaterality, bool showViewPosition)
			: base(identifier, new AnnotationResourceResolver(typeof(LateralityViewPositionAnnotationItem).Assembly))
		{
			Platform.CheckTrue(showViewPosition || showLaterality, "At least one of showLaterality and showViewPosition must be true.");

			_showLaterality = showLaterality;
			_showViewPosition = showViewPosition;
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			const string nullString = "-";

			IImageSopProvider provider = presentationImage as IImageSopProvider;
			if (provider == null)
				return "";

			string laterality = null;
			if (_showLaterality)
			{
				laterality = provider.ImageSop.ImageLaterality;
				if (string.IsNullOrEmpty(laterality))
					laterality = provider.ImageSop.Laterality;
			}

			string viewPosition = null;
			if (_showViewPosition)
			{
				viewPosition = provider.ImageSop.ViewPosition;
				if (string.IsNullOrEmpty(viewPosition))
				{
					//TODO: later, we could translate to ACR MCQM equivalent, at least for mammo.
					DicomAttributeSQ codeSequence = provider.ImageSop[DicomTags.ViewCodeSequence] as DicomAttributeSQ;
					if (codeSequence != null && !codeSequence.IsNull && codeSequence.Count > 0)
						viewPosition = codeSequence[0][DicomTags.CodeMeaning].GetString(0, null);
				}
			}

			string str = "";
			if (_showLaterality && _showViewPosition)
			{
				if (string.IsNullOrEmpty(laterality))
					laterality = nullString;
				if (string.IsNullOrEmpty(viewPosition))
					viewPosition = nullString;

				if (laterality == nullString && viewPosition == nullString)
					str = ""; // if both parts are null then just show one hyphen (rather than -/-)
				else
					str = String.Format(SR.FormatLateralityViewPosition, laterality, viewPosition);
			}
			else if (_showLaterality)
			{
				str = laterality;
			}
			else if (_showViewPosition)
			{
				str = viewPosition;
			}

			return str;
		}
	}
}
