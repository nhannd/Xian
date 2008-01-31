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
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	[MenuAction("synchronize", "global-menus/MenuTools/MenuStandard/MenuAutoSynchronizeStacking", "ToggleAutoSynchronize", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("synchronize", "global-toolbars/ToolbarStandard/ToolbarAutoSynchronizeStacking", "ToggleAutoSynchronize", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("synchronize", "AutoSynchronizeActive", "AutoSynchronizeActiveChanged")]
	[Tooltip("synchronize", "TooltipAutoSynchronizeStacking")]
	[IconSet("synchronize", IconScheme.Colour, "Icons.SynchronizeToolSmall.png", "Icons.SynchronizeToolMedium.png", "Icons.SynchronizeToolLarge.png")]
	[GroupHint("synchronize", "Tools.Image.Manipulation.Stacking.AutoSynchronize")]

	[MenuAction("synchronizeAll", "global-menus/MenuTools/MenuStandard/MenuAutoSynchronizeStackingAll", "ToggleAutoSynchronizeAll", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("synchronizeAll", "global-toolbars/ToolbarStandard/ToolbarAutoSynchronizeStackingAll", "ToggleAutoSynchronizeAll", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("synchronizeAll", "AutoSynchronizeAllActive", "AutoSynchronizeAllActiveChanged")]
	[EnabledStateObserver("synchronizeAll", "AutoSynchronizeAllEnabled", "AutoSynchronizeAllEnabledChanged")]
	[Tooltip("synchronizeAll", "TooltipAutoSynchronizeStackingAll")]
	[IconSet("synchronizeAll", IconScheme.Colour, "Icons.SynchronizeAllToolSmall.png", "Icons.SynchronizeAllToolMedium.png", "Icons.SynchronizeAllToolLarge.png")]
	[GroupHint("synchronizeAll", "Tools.Image.Manipulation.Stacking.AutoSynchronizeAll")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class StackingSynchronizationTool : ImageViewerTool
	{
		#region ImageInfo 

		private class ImageInfo
		{
			public Vector3D Normal;
			public Vector3D Position;
		}

		#endregion

		private class Offset
		{
			public Offset(string frameOfReferenceUid, string studyInstanceUid)
			{
				FrameOfReferenceUid = frameOfReferenceUid;
				StudyInstanceUid = studyInstanceUid;
			}

			public readonly string FrameOfReferenceUid;
			public readonly string StudyInstanceUid;

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				if (obj is Offset)
				{
					Offset other = (Offset) obj;
					return other.FrameOfReferenceUid == FrameOfReferenceUid && other.StudyInstanceUid == StudyInstanceUid;
				}

				return false;
			}

			public override int  GetHashCode()
			{
				return 3*FrameOfReferenceUid.GetHashCode() + 7*StudyInstanceUid.GetHashCode();
			}
		}

		private class OffsetValue
		{
			public Vector3D Offset = Vector3D.GetNullVector();
			public int Count = 0;
		}

		private bool _autoSynchronizeActive;
		private event EventHandler _autoSynchronizeActiveChanged;

		private bool _autoSynchronizeAllActive;
		private event EventHandler _autoSynchronizeAllActiveChanged;

		private bool _autoSynchronizeAllEnabled;
		private event EventHandler _autoSynchronizeAllEnabledChanged;

		private SynchronizationToolCoordinator _coordinator;

		private readonly Dictionary<string, ImageInfo> _sopInfoDictionary;
		private readonly Dictionary<Offset, OffsetValue> _currentOffsets;

		private static readonly float _fiveDegreesInRadians = (float)(5 * Math.PI / 180);

		private bool _deferSynchronizeUntilDisplaySetChanged;

		public StackingSynchronizationTool()
		{
			_deferSynchronizeUntilDisplaySetChanged = false;

			_autoSynchronizeActive = false;
			_autoSynchronizeAllActive = false;
			_autoSynchronizeAllEnabled = false;

			_sopInfoDictionary = new Dictionary<string, ImageInfo>();
			_currentOffsets = new Dictionary<Offset, OffsetValue>();
		}

		public bool AutoSynchronizeActive
		{
			get { return _autoSynchronizeActive; }
			set
			{
				if (_autoSynchronizeActive == value)
					return;

				_autoSynchronizeActive = value;
				EventsHelper.Fire(_autoSynchronizeActiveChanged, this, EventArgs.Empty);

				this.AutoSynchronizeAllEnabled = _autoSynchronizeActive;
			}
		}

		public event EventHandler AutoSynchronizeActiveChanged
		{
			add { _autoSynchronizeActiveChanged += value; }
			remove { _autoSynchronizeActiveChanged -= value; }
		}

		public bool AutoSynchronizeAllActive
		{
			get { return _autoSynchronizeAllActive; }
			set
			{
				if (value && !AutoSynchronizeActive)
					value = false;

				if (_autoSynchronizeAllActive == value)
					return;

				_autoSynchronizeAllActive = value;
				EventsHelper.Fire(_autoSynchronizeAllActiveChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler AutoSynchronizeAllActiveChanged
		{
			add { _autoSynchronizeAllActiveChanged += value; }
			remove { _autoSynchronizeAllActiveChanged -= value; }
		}

		public bool AutoSynchronizeAllEnabled
		{
			get { return _autoSynchronizeAllEnabled; }
			set
			{
				if (value && !AutoSynchronizeActive)
					value = false;

				if (_autoSynchronizeAllEnabled == value)
					return;

				if (!value)
					AutoSynchronizeAllActive = false;

				_autoSynchronizeAllEnabled = value;
				EventsHelper.Fire(_autoSynchronizeAllEnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler AutoSynchronizeAllEnabledChanged
		{
			add { _autoSynchronizeAllEnabledChanged += value; }
			remove { _autoSynchronizeAllEnabledChanged -= value; }
		}

		public override void Initialize()
		{
			base.Initialize();

			base.ImageViewer.EventBroker.DisplaySetChanging += OnDisplaySetChanging;
			base.ImageViewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;
			base.ImageViewer.PhysicalWorkspace.LayoutCompleted += OnLayoutCompleted;

			_coordinator = SynchronizationToolCoordinator.Get(base.ImageViewer);
			_coordinator.StackingSynchronizationTool = this;
		}

		protected override void Dispose(bool disposing)
		{
			base.ImageViewer.EventBroker.DisplaySetChanging -= OnDisplaySetChanging;
			base.ImageViewer.EventBroker.DisplaySetChanged -= OnDisplaySetChanged;
			base.ImageViewer.PhysicalWorkspace.LayoutCompleted -= OnLayoutCompleted;

			_coordinator.Release();

			base.Dispose(disposing);
		}

		private void ToggleAutoSynchronize()
		{
			AutoSynchronizeActive = !AutoSynchronizeActive;
			AutoSynchronizeAllActive = AutoSynchronizeActive;
			
			_coordinator.OnSynchronizedImages(SynchronizeAll());

			_currentOffsets.Clear();
		}

		private void ToggleAutoSynchronizeAll()
		{
			AutoSynchronizeAllActive = !AutoSynchronizeAllActive;

			RecalculateOffsets();
		}

		private void OnLayoutCompleted(object sender, EventArgs e)
		{
			// When something new is dropped in, do a synchronize all ignoring the one dropped in.
			// It will then get automatically synchronized with any others in the same plane.
			_coordinator.OnSynchronizedImages(SynchronizeAllExcept(this.Context.Viewer.SelectedImageBox));
		}

		private void OnDisplaySetChanging(object sender, DisplaySetChangingEventArgs e)
		{
			_deferSynchronizeUntilDisplaySetChanged = true;
		}

		private void OnDisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
		{
			_deferSynchronizeUntilDisplaySetChanged = false;

			// When something new is dropped in, do a synchronize all ignoring the one dropped in.
			// It will then get automatically synchronized with any others in the same plane.
			_coordinator.OnSynchronizedImages(SynchronizeAllExcept(this.Context.Viewer.SelectedImageBox));
		}

		private void RecalculateOffsets()
		{
			if (!AutoSynchronizeAllActive)
				return;

			_currentOffsets.Clear();

			foreach (IImageBox referenceImageBox in this.ImageViewer.PhysicalWorkspace.ImageBoxes)
			{
				foreach (IImageBox imageBox in GetImageBoxesToSynchronize(referenceImageBox))
				{
					if (imageBox.TopLeftPresentationImage is IImageSopProvider)
					{
						ImageSop sop = ((IImageSopProvider) imageBox.TopLeftPresentationImage).ImageSop;
						if (!String.IsNullOrEmpty(sop.FrameOfReferenceUid) && !String.IsNullOrEmpty(sop.StudyInstanceUID))
						{
							ImageSop referenceSop = ((IImageSopProvider) referenceImageBox.TopLeftPresentationImage).ImageSop;
							if (sop.FrameOfReferenceUid != referenceSop.FrameOfReferenceUid && sop.StudyInstanceUID != referenceSop.FrameOfReferenceUid)
							{
								ImageInfo referenceImageInfo = GetImageInformation(referenceSop);
								ImageInfo info = GetImageInformation(sop);

								if (NormalsWithinLimits(referenceImageInfo, info))
								{
									Offset key = new Offset(sop.FrameOfReferenceUid, sop.StudyInstanceUID);
									if (!_currentOffsets.ContainsKey(key))
										_currentOffsets.Add(key, new OffsetValue());

									OffsetValue value = _currentOffsets[key];

									++value.Count;
									value.Offset = value.Offset + (info.Position - referenceImageInfo.Position);
								}
							}
						}
					}
				}
			}

			foreach (OffsetValue value in _currentOffsets.Values)
			{
				value.Offset = value.Offset/(float)value.Count;
				value.Count = 1;
			}
		}

		private Vector3D GetOffset(ImageSop sop)
		{
			Offset key = new Offset(sop.FrameOfReferenceUid, sop.StudyInstanceUID);
			if (_currentOffsets.ContainsKey(key))
				return _currentOffsets[key].Offset;

			return Vector3D.GetNullVector();
		}

		private ImageInfo GetImageInformation(ImageSop sop)
		{
			ImageInfo info;

			//Caching as much of the floating point arithmetic as we can for each image
			//improves the efficiency of finding the closest slice by about 4x.
			if (!_sopInfoDictionary.ContainsKey(sop.SopInstanceUID))
			{
				// Calculation of position of the center of the image in patient coordinates 
				// using the matrix method described in Dicom PS 3.3 C.7.6.2.1.1.
				info = new ImageInfo();
				info.Position = ImagePositionHelper.SourceToPatientCenterOfImage(sop);
				info.Normal = ImagePositionHelper.CalculateNormalVector(sop);

				if (info.Position == null || info.Normal == null)
					return null;

				_sopInfoDictionary[sop.SopInstanceUID] = info;
			}
			else
			{
				info = _sopInfoDictionary[sop.SopInstanceUID];
			}

			return info;
		}

		private static bool NormalsWithinLimits(ImageInfo referenceImageInfo, ImageInfo compareImageInfo)
		{
			float angle = Math.Abs((float)Math.Acos(compareImageInfo.Normal.Dot(referenceImageInfo.Normal)));
			return (angle <= _fiveDegreesInRadians || (Math.PI - angle) <= _fiveDegreesInRadians);
		}

		private int CalculateClosestSlice(IImageBox referenceImageBox, IImageBox imageBox)
		{
			ImageSop referenceSop = ((IImageSopProvider) referenceImageBox.TopLeftPresentationImage).ImageSop;

			ImageInfo referenceImageInfo = GetImageInformation(referenceSop);
			if (referenceImageInfo == null)
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

					bool sameFrameOfReference = sop.FrameOfReferenceUid == referenceSop.FrameOfReferenceUid &&
					                             sop.StudyInstanceUID == referenceSop.StudyInstanceUID;

					if (this.AutoSynchronizeAllActive || sameFrameOfReference)
					{
						ImageInfo info = GetImageInformation(sop);
						if (info != null && NormalsWithinLimits(referenceImageInfo, info))
						{
							Vector3D offset = sameFrameOfReference ? Vector3D.GetNullVector() : GetOffset(sop);
							if (offset != null)
							{
								Vector3D difference = referenceImageInfo.Position + offset - info.Position;
								float distance = difference.Magnitude;

								if (Math.Abs(distance) <= closestDistance)
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

		private IEnumerable<IImageBox> GetImageBoxesToSynchronize(IImageBox referenceImageBox)
		{
			IImageSopProvider provider = referenceImageBox.TopLeftPresentationImage as IImageSopProvider;
			if (provider == null || String.IsNullOrEmpty(provider.ImageSop.FrameOfReferenceUid) || String.IsNullOrEmpty(provider.ImageSop.StudyInstanceUID))
				yield break;

			foreach (IImageBox imageBox in this.Context.Viewer.PhysicalWorkspace.ImageBoxes)
			{
				if (referenceImageBox != imageBox && imageBox != null && imageBox.DisplaySet != null && imageBox.DisplaySet.PresentationImages.Count > 1)
					yield return imageBox;
			}
		}

		private IEnumerable<IImageBox> SynchronizeWithImage(IImageBox referenceImageBox)
		{
			foreach (IImageBox imageBox in GetImageBoxesToSynchronize(referenceImageBox))
			{
				int index = CalculateClosestSlice(referenceImageBox, imageBox);
				if (index >= 0 && index != imageBox.TopLeftPresentationImageIndex)
				{
					imageBox.TopLeftPresentationImageIndex = index;
					yield return imageBox;
				}
			}
		}

		private IEnumerable<IImageBox> SynchronizeAllExcept(IImageBox excludeImageBox)
		{
			if (!AutoSynchronizeActive)
				yield break;

			IImageBox selectedImageBox = this.Context.Viewer.SelectedImageBox;
			if (selectedImageBox != excludeImageBox)
			{
				foreach (IImageBox affectedImageBox in SynchronizeWithImage(selectedImageBox))
					yield return affectedImageBox;
			}

			foreach (IImageBox imageBox in this.Context.Viewer.PhysicalWorkspace.ImageBoxes)
			{
				if (imageBox != excludeImageBox)
				{
					foreach (IImageBox affectedImageBox in SynchronizeWithImage(imageBox))
						yield return affectedImageBox;
				}
			}
		}

		private IEnumerable<IImageBox> SynchronizeAll()
		{
			return SynchronizeAllExcept(null);
		}

		public IEnumerable<IImageBox> SynchronizeSelected()
		{
			if (!AutoSynchronizeActive || _deferSynchronizeUntilDisplaySetChanged)
				yield break;

			foreach (IImageBox imageBox in SynchronizeWithImage(this.Context.Viewer.SelectedImageBox))
				yield return imageBox;
		}
	}
}
