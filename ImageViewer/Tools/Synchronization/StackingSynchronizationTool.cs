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
	[MenuAction("synchronize", "global-menus/MenuTools/MenuStandard/MenuSynchronizeStacking", "ToggleSynchronize", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("synchronize", "global-toolbars/ToolbarStandard/ToolbarSynchronizeStacking", "ToggleSynchronize", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("synchronize", "SynchronizeActive", "SynchronizeActiveChanged")]
	[Tooltip("synchronize", "TooltipSynchronizeStacking")]
	[IconSet("synchronize", IconScheme.Colour, "Icons.SynchronizeToolSmall.png", "Icons.SynchronizeToolMedium.png", "Icons.SynchronizeToolLarge.png")]
	[GroupHint("synchronize", "Tools.Image.Manipulation.Stacking.Synchronize")]

	[MenuAction("synchronizeAll", "global-menus/MenuTools/MenuStandard/MenuSynchronizeStackingAll", "ToggleSynchronizeAll", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("synchronizeAll", "global-toolbars/ToolbarStandard/ToolbarSynchronizeStackingAll", "ToggleSynchronizeAll", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("synchronizeAll", "SynchronizeAllActive", "SynchronizeAllActiveChanged")]
	[EnabledStateObserver("synchronizeAll", "SynchronizeAllEnabled", "SynchronizeAllEnabledChanged")]
	[Tooltip("synchronizeAll", "TooltipSynchronizeStackingAll")]
	[IconSet("synchronizeAll", IconScheme.Colour, "Icons.SynchronizeAllToolSmall.png", "Icons.SynchronizeAllToolMedium.png", "Icons.SynchronizeAllToolLarge.png")]
	[GroupHint("synchronizeAll", "Tools.Image.Manipulation.Stacking.SynchronizeAll")]

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

		#region OffsetKey

		private class OffsetKey
		{
			public OffsetKey(string studyInstanceUid, string frameOfReferenceUid, Vector3D normal)
			{
				FrameOfReferenceUid = frameOfReferenceUid;
				StudyInstanceUid = studyInstanceUid;
				Normal = normal;
			}

			public readonly string FrameOfReferenceUid;
			public readonly string StudyInstanceUid;
			public readonly Vector3D Normal;

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				if (obj is OffsetKey)
				{
					OffsetKey other = (OffsetKey) obj;
					return other.FrameOfReferenceUid == FrameOfReferenceUid && other.StudyInstanceUid == StudyInstanceUid && other.Normal.Equals(Normal);
				}

				return false;
			}


			public override int  GetHashCode()
			{
				return 3*FrameOfReferenceUid.GetHashCode() + 5*StudyInstanceUid.GetHashCode() + 7 *Normal.GetHashCode();
			}
		}

		#endregion

		#region Private Fields

		private bool _synchronizeActive;
		private event EventHandler _synchronizeActiveChanged;

		private bool _synchronizeAllActive;
		private event EventHandler _synchronizeAllActiveChanged;

		private bool _synchronizeAllEnabled;
		private event EventHandler _synchronizeAllEnabledChanged;

		private SynchronizationToolCoordinator _coordinator;

		private readonly Dictionary<string, ImageInfo> _sopInfoDictionary;
		private readonly Dictionary<OffsetKey, Dictionary<OffsetKey, Vector3D>> _offsets;

		private static readonly float _fiveDegreesInRadians = (float)(5 * Math.PI / 180);

		private bool _deferSynchronizeUntilDisplaySetChanged;

		#endregion

		public StackingSynchronizationTool()
		{
			_deferSynchronizeUntilDisplaySetChanged = false;

			_synchronizeActive = false;
			_synchronizeAllActive = false;
			_synchronizeAllEnabled = false;

			_sopInfoDictionary = new Dictionary<string, ImageInfo>();
			_offsets = new Dictionary<OffsetKey, Dictionary<OffsetKey, Vector3D>>();
		}

		public bool SynchronizeActive
		{
			get { return _synchronizeActive; }
			set
			{
				if (_synchronizeActive == value)
					return;

				_synchronizeActive = value;
				EventsHelper.Fire(_synchronizeActiveChanged, this, EventArgs.Empty);

				this.SynchronizeAllEnabled = _synchronizeActive;
			}
		}

		public event EventHandler SynchronizeActiveChanged
		{
			add { _synchronizeActiveChanged += value; }
			remove { _synchronizeActiveChanged -= value; }
		}

		public bool SynchronizeAllActive
		{
			get { return _synchronizeAllActive; }
			set
			{
				if (value && !SynchronizeActive)
					value = false;

				if (_synchronizeAllActive == value)
					return;

				_synchronizeAllActive = value;
				EventsHelper.Fire(_synchronizeAllActiveChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler SynchronizeAllActiveChanged
		{
			add { _synchronizeAllActiveChanged += value; }
			remove { _synchronizeAllActiveChanged -= value; }
		}

		public bool SynchronizeAllEnabled
		{
			get { return _synchronizeAllEnabled; }
			set
			{
				if (value && !SynchronizeActive)
					value = false;

				if (_synchronizeAllEnabled == value)
					return;

				if (!value)
					SynchronizeAllActive = false;

				_synchronizeAllEnabled = value;
				EventsHelper.Fire(_synchronizeAllEnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler SynchronizeAllEnabledChanged
		{
			add { _synchronizeAllEnabledChanged += value; }
			remove { _synchronizeAllEnabledChanged -= value; }
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

		private void ToggleSynchronize()
		{
			SynchronizeActive = !SynchronizeActive;
			SynchronizeAllActive = SynchronizeActive;

			////Clear all offsets
			if (!SynchronizeActive)
				_offsets.Clear();
			else 
				_coordinator.OnSynchronizedImages(SynchronizeAll());
		}

		private void ToggleSynchronizeAll()
		{
			SynchronizeAllActive = !SynchronizeAllActive;

			if (SynchronizeAllActive)
			{
				RecalculateOffsetForVisibleDisplaySets();
				_coordinator.OnSynchronizedImages(SynchronizeAll());
			}
		}

		private void OnLayoutCompleted(object sender, EventArgs e)
		{
			//this is the best we can do in this situation.
			_coordinator.OnSynchronizedImages(SynchronizeAll());
		}

		private void OnDisplaySetChanging(object sender, DisplaySetChangingEventArgs e)
		{
			_deferSynchronizeUntilDisplaySetChanged = true;
		}

		private void OnDisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
		{
			_deferSynchronizeUntilDisplaySetChanged = false;

			_coordinator.OnSynchronizedImages(SynchronizeNewDisplaySet(e.NewDisplaySet));
		}

		private void RecalculateOffsetForVisibleDisplaySets()
		{
			foreach (IImageBox referenceImageBox in this.ImageViewer.PhysicalWorkspace.ImageBoxes)
			{
				OffsetKey referenceOffsetKey = null;
				Dictionary<OffsetKey, Vector3D> referenceOffsetDictionary = null;

				foreach (IImageBox imageBox in GetImageBoxesToSynchronize(referenceImageBox))
				{
					if (imageBox.TopLeftPresentationImage is IImageSopProvider)
					{
						ImageSop sop = ((IImageSopProvider) imageBox.TopLeftPresentationImage).ImageSop;

						if (!String.IsNullOrEmpty(sop.FrameOfReferenceUid) && !String.IsNullOrEmpty(sop.StudyInstanceUID))
						{
							ImageSop referenceSop = ((IImageSopProvider) referenceImageBox.TopLeftPresentationImage).ImageSop;

							if (sop.FrameOfReferenceUid != referenceSop.FrameOfReferenceUid && sop.StudyInstanceUID != referenceSop.StudyInstanceUID)
							{
								ImageInfo referenceImageInfo = GetImageInformation(referenceSop);
								ImageInfo info = GetImageInformation(sop);

								if (info != null && NormalsWithinLimits(referenceImageInfo, info))
								{
									OffsetKey key = new OffsetKey(sop.StudyInstanceUID, sop.FrameOfReferenceUid, info.Normal);

									if (referenceOffsetKey == null)
									{
										//reset the related offsets
										referenceOffsetKey = new OffsetKey(referenceSop.StudyInstanceUID, referenceSop.FrameOfReferenceUid, referenceImageInfo.Normal);
										if (_offsets.ContainsKey(referenceOffsetKey))
										{
											referenceOffsetDictionary = _offsets[referenceOffsetKey];
											referenceOffsetDictionary.Remove(key);
										}
										else
										{
											referenceOffsetDictionary = new Dictionary<OffsetKey, Vector3D>();
											_offsets[referenceOffsetKey] = referenceOffsetDictionary;
										}
									}

									Vector3D currentOffset = null;
									if (referenceOffsetDictionary.ContainsKey(key))
										currentOffset = referenceOffsetDictionary[key];

									Vector3D offset = info.Position - referenceImageInfo.Position;
									if (currentOffset == null || offset.Magnitude < currentOffset.Magnitude)
										referenceOffsetDictionary[key] = offset;
								}
							}
						}
					}
				}
			}
		}

		private Vector3D GetOffset(ImageSop referenceSop, ImageInfo referenceImageInfo, ImageSop sop, ImageInfo info)
		{
			// looking for offset between A and C, say.

			OffsetKey referenceOffsetKey = new OffsetKey(referenceSop.StudyInstanceUID, referenceSop.FrameOfReferenceUid, referenceImageInfo.Normal);
			if (_offsets.ContainsKey(referenceOffsetKey))
			{
				// A is there.
				OffsetKey key = new OffsetKey(sop.StudyInstanceUID, sop.FrameOfReferenceUid, info.Normal);
				if (!key.Equals(referenceOffsetKey))
				{
					// A and C are not in the same frame of reference.

					if (_offsets[referenceOffsetKey].ContainsKey(key))
					{
						// there is an offset between A and C.
						return _offsets[referenceOffsetKey][key];
					}
					else if (_offsets.ContainsKey(key))
					{
						// No direct offset, but we have other offsets for C.
						Vector3D inferredOffset = null;
						foreach (KeyValuePair<OffsetKey, Vector3D> referenceRelatedOffset in _offsets[referenceOffsetKey])
						{
							// Iterate through A's offsets and see if C has a similar offset.
							if (_offsets[key].ContainsKey(referenceRelatedOffset.Key))
							{
								// A has an offset for B, as does C
								// We now have offset of B relative to A (A --> B) = B - A
								// and offset of C relative to B (C --> B) = B - C
								// Offset of A relative to C is equal to (A --> B + B --> C) = C - A = (C - B) + (B - A)

								// this is (A --> B) - (C --> B) = (A --> B) + (B --> C)
								Vector3D offset = referenceRelatedOffset.Value - _offsets[key][referenceRelatedOffset.Key];

								// again, take the smallest of all the possible offsets.
								if (inferredOffset == null || offset.Magnitude < inferredOffset.Magnitude)
									inferredOffset = offset;
							}
						}

						if (inferredOffset != null)
							return inferredOffset;
					}
				}
			}

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

					// When 'synchronize all' is false, we only synchronize within the same frame of reference.
					if (this.SynchronizeAllActive || sameFrameOfReference)
					{
						ImageInfo info = GetImageInformation(sop);
						if (info != null && NormalsWithinLimits(referenceImageInfo, info))
						{
							//Don't bother getting an offset for something in the same frame of reference.
							Vector3D offset = sameFrameOfReference ? Vector3D.GetNullVector() : GetOffset(referenceSop, referenceImageInfo, sop, info);

							Vector3D difference = referenceImageInfo.Position + offset - info.Position;
							float distance = difference.Magnitude;

							//Add trace statements to make sure what you expected is actually what's happening!
							if (Math.Abs(distance) <= closestDistance)
							{
								closestDistance = distance;
								closestIndex = index;
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
			return SynchronizeWithImage(referenceImageBox, null);
		}

		private IEnumerable<IImageBox> SynchronizeWithImage(IImageBox referenceImageBox, IImageBox ignoreImageBox)
		{
			foreach (IImageBox imageBox in GetImageBoxesToSynchronize(referenceImageBox))
			{
				if (imageBox == ignoreImageBox)
					continue;

				int index = CalculateClosestSlice(referenceImageBox, imageBox);
				if (index >= 0 && index != imageBox.TopLeftPresentationImageIndex)
				{
					imageBox.TopLeftPresentationImageIndex = index;
					yield return imageBox;
				}
			}
		}

		private IEnumerable<IImageBox> SynchronizeNewDisplaySet(IDisplaySet newDisplaySet)
		{
			if (!SynchronizeActive || newDisplaySet == null)
				yield break;

			IImageBox changedImageBox = newDisplaySet.ImageBox;
			IImageBox selectedImageBox = this.Context.Viewer.SelectedImageBox;

			foreach (IImageBox imageBox in this.Context.Viewer.PhysicalWorkspace.ImageBoxes)
			{
				if (imageBox != changedImageBox && imageBox != selectedImageBox)
				{
					foreach (IImageBox affectedImageBox in SynchronizeWithImage(imageBox, changedImageBox == selectedImageBox ? null : selectedImageBox))
						yield return affectedImageBox;
				}
			}

			if (selectedImageBox == null)
				yield break;

			foreach (IImageBox affectedImageBox in SynchronizeWithImage(selectedImageBox))
				yield return affectedImageBox;
		}

		private IEnumerable<IImageBox> SynchronizeAll()
		{
			if (!SynchronizeActive)
				yield break;

			IImageBox selectedImageBox = this.Context.Viewer.SelectedImageBox;

			foreach (IImageBox imageBox in this.Context.Viewer.PhysicalWorkspace.ImageBoxes)
			{
				if (imageBox != selectedImageBox)
				{
					foreach (IImageBox affectedImageBox in SynchronizeWithImage(imageBox, selectedImageBox))
						yield return affectedImageBox;
				}
			}

			if (selectedImageBox == null)
				yield break;

			foreach (IImageBox affectedImageBox in SynchronizeWithImage(selectedImageBox))
				yield return affectedImageBox;
		}

		public IEnumerable<IImageBox> SynchronizeSelected()
		{
			if (!SynchronizeActive || _deferSynchronizeUntilDisplaySetChanged)
				yield break;

			if (this.Context.Viewer.SelectedImageBox == null)
				yield break;

			foreach (IImageBox imageBox in SynchronizeWithImage(this.Context.Viewer.SelectedImageBox))
				yield return imageBox;
		}
	}
}
