#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuSynchronizeStacking", "Toggle", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarSynchronizeStacking", "Toggle", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActiveChanged")]
	[Tooltip("activate", "TooltipSynchronizeStacking")]
	[IconSet("activate", IconScheme.Colour, "Icons.StackingSynchronizationToolSmall.png", "Icons.StackingSynchronizationToolMedium.png", "Icons.StackingSynchronizationToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Stacking.Synchronize")]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class StackingSynchronizationTool : ImageViewerTool
	{
		#region ImageInfo struct 

		private struct ImageInfo
		{
			public Vector3D Normal;
			public Vector3D Position;
		}

		#endregion

		private bool _active;
		private event EventHandler _activeChanged;

		private readonly Dictionary<string, ImageInfo> _sopInfoDictionary;

		private static readonly float _fiveDegreesInRadians = (float)(5 * Math.PI / 180);

		public StackingSynchronizationTool()
		{
			_active = false;
			_sopInfoDictionary = new Dictionary<string, ImageInfo>();
		}

		public bool Active
		{
			get { return _active; }
			set
			{
				if (_active == value)
					return;

				_active = value;
				EventsHelper.Fire(_activeChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler ActiveChanged
		{
			add { _activeChanged += value; }
			remove { _activeChanged -= value; }
		}

		private IEnumerable<IImageBox> GetImageBoxesToSynchronize(IImageBox referenceImageBox)
		{
			IImageSopProvider provider = referenceImageBox.TopLeftPresentationImage as IImageSopProvider;
			if (provider == null || String.IsNullOrEmpty(provider.ImageSop.FrameOfReferenceUid) || String.IsNullOrEmpty(provider.ImageSop.StudyInstanceUID))
				yield break;

			foreach (IImageBox imageBox in this.Context.Viewer.PhysicalWorkspace.ImageBoxes)
			{
				if (referenceImageBox != imageBox && imageBox.DisplaySet != null && imageBox.DisplaySet.PresentationImages.Count > 1)
					yield return imageBox;
			}
		}

		private void CalculateNormalAndPosition(ImageSop sop, out Vector3D normal, out Vector3D positionVector)
		{
			ImageOrientationPatient orientation = sop.ImageOrientationPatient;
			PixelSpacing spacing = sop.PixelSpacing;

			normal = null;
			positionVector = null;

			if (orientation.IsNull || spacing.IsNull)
				return;
			
			ImageInfo info;

			//Caching as much of the floating point arithmetic as we can for each image
			//improves the efficiency of finding the closest slice by about 4x.

			if (!_sopInfoDictionary.ContainsKey(sop.SopInstanceUID))
			{
				// Calculation of position of the center of the image in patient coordinates 
				// using the matrix method described in Dicom PS 3.3 C.7.6.2.1.1.

				ImagePositionPatient position = sop.ImagePositionPatient;

				Matrix mReference = new Matrix(4, 4);

				mReference[0, 0] = (float) (orientation.RowX*spacing.Column);
				mReference[1, 0] = (float) (orientation.RowY*spacing.Column);
				mReference[2, 0] = (float) (orientation.RowZ*spacing.Column);

				mReference[0, 1] = (float) (orientation.ColumnX*spacing.Row);
				mReference[1, 1] = (float) (orientation.ColumnY*spacing.Row);
				mReference[2, 1] = (float) (orientation.ColumnZ*spacing.Row);

				mReference[0, 3] = (float) (position.X);
				mReference[1, 3] = (float) (position.Y);
				mReference[2, 3] = (float) (position.Z);
				mReference[3, 3] = 1F;

				Matrix columnMatrix = new Matrix(4, 1);
				columnMatrix[0, 0] = (sop.Columns - 1)/2F;
				columnMatrix[1, 0] = (sop.Rows - 1)/2F;
				columnMatrix[3, 0] = 1F;

				Matrix result = mReference.Multiply(columnMatrix);

				info = new ImageInfo();
				info.Position = new Vector3D(result[0, 0], result[1, 0], result[2, 0]);
				info.Normal = Vector3D.Cross(new Vector3D((float) orientation.RowX, (float) orientation.RowY, (float) orientation.RowZ),
				                        new Vector3D((float) orientation.ColumnX, (float) orientation.ColumnY,
				                                     (float) orientation.ColumnZ));

				_sopInfoDictionary[sop.SopInstanceUID] = info;
			}
			else
			{
				info = _sopInfoDictionary[sop.SopInstanceUID];
			}


			positionVector = info.Position.Clone();
			normal = info.Normal.Clone();
		}

		private int CalculateClosestSlice(IImageBox referenceImageBox, IImageBox imageBox)
		{
			ImageSop referenceSop = ((IImageSopProvider) referenceImageBox.TopLeftPresentationImage).ImageSop;

			Vector3D referencePosition;
			Vector3D referenceNormal;
			CalculateNormalAndPosition(referenceSop, out referenceNormal, out referencePosition);

			if (referencePosition == null || referenceNormal == null)
				return -1;

			float closestDistance = float.MaxValue;
			int closestIndex = -1;

			//find the closest one, closest to the top of the stack (beginning of display set).
			for (int index = imageBox.DisplaySet.PresentationImages.Count - 1; index >= 0 ; --index)
			{
				IImageSopProvider provider = imageBox.DisplaySet.PresentationImages[index] as IImageSopProvider;
				if (provider != null)
				{
					ImageSop sop = provider.ImageSop;

					if (sop.FrameOfReferenceUid == referenceSop.FrameOfReferenceUid && sop.StudyInstanceUID == referenceSop.StudyInstanceUID)
					{
						Vector3D normal;
						Vector3D position;
						CalculateNormalAndPosition(sop, out normal, out position);
						
						if (normal != null && position != null)
						{
							float angle = (float) Math.Acos(Vector3D.Dot(normal, referenceNormal));
							if (Math.Abs(angle) <= _fiveDegreesInRadians)
							{
								position.Subtract(referencePosition);
								float distance = position.Magnitude;

								if (Math.Abs(distance) < closestDistance)
								{
									closestDistance = distance;
									closestIndex = index;
								}
							}
						}
					}
				}
			}

			return closestIndex;
		}

		private void SynchronizeWithImage(IImageBox referenceImageBox)
		{
			foreach (IImageBox imageBox in GetImageBoxesToSynchronize(referenceImageBox))
			{
				int index = CalculateClosestSlice(referenceImageBox, imageBox);
				if (index >= 0)
				{
					imageBox.TopLeftPresentationImageIndex = index;
					imageBox.Draw();
				}
			}
		}

		private void Synchronize()
		{
			if (Active)
				SynchronizeWithImage(this.Context.Viewer.SelectedImageBox);
		}

		private void Toggle()
		{
			Active = !Active;
			Synchronize();
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			Synchronize();
		}
	}
}
