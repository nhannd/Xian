#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Drawing;

namespace ClearCanvas.ImageViewer.Annotations.Dicom
{
	/// <summary>
	/// This class is not necessarily intended to be used.  It is slightly modified from
	/// the original (temporary) code that was included in 0.95 for the text overlay.  Now 
	/// it simply serves as an example of how a hard-coded overlay can still be used 
	/// in the new model.
	/// </summary>
	internal sealed class DicomFixedAnnotationLayoutProvider : AnnotationLayoutProvider
	{
		private string[] _AssignmentsTopLeft = 
		{
			"Dicom.Patient.PatientId",
			"Dicom.Patient.PatientsName",
			"Dicom.Patient.PatientsBirthDate",
			"Dicom.Patient.PatientsSex",
			"Dicom.PatientStudy.PatientsAge",
			"", "", "", "", "",
			"", "", "", "", ""
		};

		private string[] _AssignmentsTopRight =
		{
			"Dicom.GeneralStudy.AccessionNumber",
			"Dicom.GeneralStudy.StudyDescription",
			"Dicom.GeneralStudy.StudyId",
			"Dicom.GeneralStudy.StudyDate",
			"Dicom.GeneralStudy.StudyTime",
			"", "", "", "", "",
			"", "", "", "", ""
		};

		private string[] _AssignmentsBottomLeft = 
		{
			"", "", "", "", 
			"", "", "", "", 
			"Dicom.GeneralSeries.Laterality",
			"Dicom.GeneralSeries.SeriesNumber",
			"Dicom.GeneralImage.InstanceNumber",
			"Presentation.Zoom",
			"Presentation.AppliedLut",
			"Dicom.GeneralSeries.ProtocolName",
			"Dicom.GeneralSeries.SeriesDescription"
		};

		private string[] _AssignmentsBottomRight = 
		{
			"", "", "", "",
			"", "", "", "", "",
			"", "",
			"Dicom.GeneralSeries.OperatorsName",
			"Dicom.GeneralEquipment.StationName",
			"Dicom.GeneralStudy.ReferringPhysiciansName",
			"Dicom.PatientStudy.AdditionalPatientsHistory"
		};

		private const string _layoutIdentifier = "Dicom.General";

		public DicomFixedAnnotationLayoutProvider()
		{
		}

		private StoredAnnotationLayout GetLayout()
		{
			StoredAnnotationLayout layout = new StoredAnnotationLayout(_layoutIdentifier);

			int numberOfBoxesPerQuadrant = 15;
			float boxheight = 1 / 32.0F;

			float x = 0F, y = 0F, dx = 0.5F, dy = boxheight;

			StoredAnnotationBoxGroup group = new StoredAnnotationBoxGroup("TopLeftQuadrant");
			layout.AnnotationBoxGroups.Add(group);
			group.DefaultBoxSettings.Bold = true;
			group.DefaultBoxSettings.Font = "Century Gothic";
			//TL
			for (int i = 0; i < numberOfBoxesPerQuadrant; ++i)
			{
				dx = (i > 0) ? 0.5F : 0.4F; //make room for directional markers.

				RectangleF normalizedRectangle = new RectangleF(x, y, dx, dy);
				AnnotationBox newBox = group.GetNewBox();
				newBox.NormalizedRectangle = normalizedRectangle;
				newBox.AnnotationItem = this.GetAnnotationItem(_AssignmentsTopLeft[i]);

				group.AnnotationBoxes.Add(newBox);
				y += boxheight;
			}

			group = new StoredAnnotationBoxGroup("TopRightQuadrant");
			layout.AnnotationBoxGroups.Add(group);
			group.DefaultBoxSettings.Color = "OrangeRed";
			group.DefaultBoxSettings.Font = "Century Gothic";
			group.DefaultBoxSettings.Justification = AnnotationBox.JustificationBehaviour.Right;
			y = 0.0F;
			//TR
			for (int i = 0; i < numberOfBoxesPerQuadrant; ++i)
			{
				x = (i > 0) ? 0.5F : 0.6F; //make room for directional markers.
				dx = (i > 0) ? 0.5F : 0.4F; //make room for directional markers.

				RectangleF normalizedRectangle = new RectangleF(x, y, dx, dy);
				AnnotationBox newBox = group.GetNewBox();
				newBox.NormalizedRectangle = normalizedRectangle;
				newBox.AnnotationItem = this.GetAnnotationItem(_AssignmentsTopRight[i]);

				group.AnnotationBoxes.Add(newBox);
				y += boxheight;
			}

			group = new StoredAnnotationBoxGroup("BottomLeftQuadrant");
			layout.AnnotationBoxGroups.Add(group);
			group.DefaultBoxSettings.Color = "Cyan";
			group.DefaultBoxSettings.Font = "Century Gothic";
			x = 0F;
			y = 1.0F - boxheight;
			//BL
			for (int i = numberOfBoxesPerQuadrant - 1; i >= 0; --i)
			{
				dx = (i < (numberOfBoxesPerQuadrant - 1)) ? 0.5F : 0.4F; //make room for directional markers.

				RectangleF normalizedRectangle = new RectangleF(x, y, dx, dy);
				AnnotationBox newBox = group.GetNewBox();
				newBox.NormalizedRectangle = normalizedRectangle;
				newBox.AnnotationItem = this.GetAnnotationItem(_AssignmentsBottomLeft[i]);

				if (i < numberOfBoxesPerQuadrant - 4 && !String.IsNullOrEmpty(_AssignmentsBottomLeft[i]))
				{
					newBox.ConfigurationOptions = new AnnotationItemConfigurationOptions();
					newBox.ConfigurationOptions.ShowLabel = true;
				}

				group.AnnotationBoxes.Add(newBox);
				y -= boxheight;
			}

			group = new StoredAnnotationBoxGroup("BottomRightQuadrant");
			layout.AnnotationBoxGroups.Add(group);
			group.DefaultBoxSettings.Color = "Yellow";
			group.DefaultBoxSettings.Font = "Century Gothic";
			//group.DefaultBoxSettings.NumberOfLines = 2;
			group.DefaultBoxSettings.Justification = AnnotationBox.JustificationBehaviour.Right;

			y = 1.0F - boxheight;
			//BR
			for (int i = numberOfBoxesPerQuadrant - 1; i >= 0; --i)
			{
				x = (i < (numberOfBoxesPerQuadrant - 1)) ? 0.5F : 0.6F; //make room for directional markers.
				dx = (i < (numberOfBoxesPerQuadrant - 1)) ? 0.5F : 0.4F; //make room for directional markers.

				RectangleF normalizedRectangle = new RectangleF(x, y, dx, dy);
				AnnotationBox newBox = group.GetNewBox();
				newBox.NormalizedRectangle = normalizedRectangle;
				newBox.AnnotationItem = this.GetAnnotationItem(_AssignmentsBottomRight[i]);
				if (!String.IsNullOrEmpty(_AssignmentsBottomRight[i]))
					newBox.NumberOfLines = 2;

				group.AnnotationBoxes.Add(newBox);
				y -= boxheight;
			}

			group = new StoredAnnotationBoxGroup("DirectionalMarkers");
			layout.AnnotationBoxGroups.Add(group);
			group.DefaultBoxSettings.Color = "White";
			group.DefaultBoxSettings.Bold = true;
			group.DefaultBoxSettings.Font = "Century Gothic";
			group.DefaultBoxSettings.NumberOfLines = 1;

			CreateDirectionalMarkerBox(0.00F, (1F - boxheight) / 2F, 0.1F, boxheight, AnnotationBox.JustificationBehaviour.Left, "Presentation.DirectionalMarkers.Left", group);
			CreateDirectionalMarkerBox(0.90F, (1F - boxheight) / 2F, 0.1F, boxheight, AnnotationBox.JustificationBehaviour.Right, "Presentation.DirectionalMarkers.Right", group);
			CreateDirectionalMarkerBox(0.45F, 0F, 0.1F, boxheight, AnnotationBox.JustificationBehaviour.Center, "Presentation.DirectionalMarkers.Top", group);
			CreateDirectionalMarkerBox(0.45F, 1F - boxheight, 0.1F, boxheight, AnnotationBox.JustificationBehaviour.Center, "Presentation.DirectionalMarkers.Bottom", group);

			return layout;
		}

		private void CreateDirectionalMarkerBox(float x, float y, float dx, float dy, AnnotationBox.JustificationBehaviour justification, string identifier, StoredAnnotationBoxGroup group)
		{
			RectangleF normalizedRectangle = new RectangleF(x, y, dx, dy);
			AnnotationBox newBox = group.GetNewBox();
			newBox.NormalizedRectangle = normalizedRectangle;
			newBox.AnnotationItem = this.GetAnnotationItem(identifier);
			newBox.Justification = justification;
			group.AnnotationBoxes.Add(newBox);
		}

		public override IAnnotationLayout  AnnotationLayout
		{
			get
			{
				return this.GetLayout();
			}
		}
	}
}
