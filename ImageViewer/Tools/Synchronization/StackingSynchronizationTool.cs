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
	[MenuAction("synchronize", "global-menus/MenuTools/MenuSynchronization/MenuSynchronizeStacking", "ToggleSynchronize", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("synchronize", "global-toolbars/ToolbarSynchronization/ToolbarSynchronizeStacking", "ToggleSynchronize", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("synchronize", "SynchronizeActive", "SynchronizeActiveChanged")]
	[Tooltip("synchronize", "TooltipSynchronizeStacking")]
	[IconSet("synchronize", IconScheme.Colour, "Icons.SynchronizeToolSmall.png", "Icons.SynchronizeToolMedium.png", "Icons.SynchronizeToolLarge.png")]
	[GroupHint("synchronize", "Tools.Image.Manipulation.Stacking.Synchronize")]

	[MenuAction("linkStudies", "global-menus/MenuTools/MenuSynchronization/MenuSynchronizeStackingLinkStudies", "ToggleLinkStudies")]
	[ButtonAction("linkStudies", "global-toolbars/ToolbarSynchronization/ToolbarSynchronizeStackingLinkStudies", "ToggleLinkStudies")]
	[EnabledStateObserver("linkStudies", "LinkStudiesEnabled", "LinkStudiesEnabledChanged")]
	[LabelValueObserver("linkStudies", "LinkStudiesLabel", "StudiesLinkedChanged")]
	[IconSetObserver("linkStudies", "LinkStudiesIconSet", "StudiesLinkedChanged")]
	[TooltipValueObserver("linkStudies", "LinkStudiesTooltip", "StudiesLinkedChanged")]
	[GroupHint("linkStudies", "Tools.Image.Manipulation.Stacking.LinkStudies")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class StackingSynchronizationTool : ImageViewerTool
	{
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
				return 3*FrameOfReferenceUid.GetHashCode() + 5*StudyInstanceUid.GetHashCode() + 7*Normal.GetHashCode();
			}
		}

		#endregion

		#region Private Fields

		private bool _synchronizeActive;
		private event EventHandler _synchronizeActiveChanged;

		private bool _linkStudiesEnabled;
		private event EventHandler _linkStudiesEnabledChanged;

		private bool _studiesLinked;
		private event EventHandler _studiesLinkedChanged;

		private readonly IconSet _linkStudiesIconSet;
		private readonly IconSet _unlinkStudiesIconSet;
		
		private SynchronizationToolCoordinator _coordinator;

		private SopInfoCache _cache;
		private readonly Dictionary<OffsetKey, Dictionary<OffsetKey, Vector3D>> _offsets;

		private static readonly float _fiveDegreesInRadians = (float)(5 * Math.PI / 180);

		private bool _deferSynchronizeUntilDisplaySetChanged;

		#endregion

		public StackingSynchronizationTool()
		{
			_deferSynchronizeUntilDisplaySetChanged = false;

			_synchronizeActive = false;
			_linkStudiesEnabled = false;

			_studiesLinked = true;
			_linkStudiesIconSet = new IconSet(IconScheme.Colour, "Icons.LinkStudiesToolSmall.png", "Icons.LinkStudiesToolMedium.png", "Icons.LinkStudiesToolLarge.png");
			_unlinkStudiesIconSet = new IconSet(IconScheme.Colour, "Icons.UnlinkStudiesToolSmall.png", "Icons.UnlinkStudiesToolMedium.png", "Icons.UnlinkStudiesToolLarge.png");

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
				
				LinkStudiesEnabled = _synchronizeActive;
			}
		}

		public event EventHandler SynchronizeActiveChanged
		{
			add { _synchronizeActiveChanged += value; }
			remove { _synchronizeActiveChanged -= value; }
		}

		public bool LinkStudiesEnabled
		{
			get { return _linkStudiesEnabled; }
			set
			{
				if (value && !SynchronizeActive)
					value = false;

				if (_linkStudiesEnabled == value)
					return;

				if (!value)
				{
					_offsets.Clear();
					StudiesLinked = true;
				}

				_linkStudiesEnabled = value;
				EventsHelper.Fire(_linkStudiesEnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler LinkStudiesEnabledChanged
		{
			add { _linkStudiesEnabledChanged += value; }
			remove { _linkStudiesEnabledChanged -= value; }
		}

		public bool StudiesLinked
		{
			get { return _studiesLinked; }
			set
			{
				if (value && !SynchronizeActive)
					value = true;

				if (_studiesLinked == value)
					return;

				_studiesLinked = value;
				EventsHelper.Fire(_studiesLinkedChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler StudiesLinkedChanged
		{
			add { _studiesLinkedChanged += value; }
			remove { _studiesLinkedChanged -= value; }
		}

		public IconSet LinkStudiesIconSet
		{
			get
			{
				return _studiesLinked ? _unlinkStudiesIconSet : _linkStudiesIconSet;
			}	
		}

		public string LinkStudiesLabel
		{
			get
			{
				return _studiesLinked ? SR.LabelUnlinkStudies : SR.LabelLinkStudies;
			}
		}

		public string LinkStudiesTooltip
		{
			get
			{
				return _studiesLinked ? SR.LabelUnlinkStudies : SR.LabelLinkStudies;
			}
		}

		public override void Initialize()
		{
			base.Initialize();

			base.ImageViewer.EventBroker.DisplaySetChanging += OnDisplaySetChanging;
			base.ImageViewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;
			base.ImageViewer.PhysicalWorkspace.LayoutCompleted += OnLayoutCompleted;

			_cache = SopInfoCache.Get();

			_coordinator = SynchronizationToolCoordinator.Get(base.ImageViewer);
			_coordinator.StackingSynchronizationTool = this;
		}

		protected override void Dispose(bool disposing)
		{
			base.ImageViewer.EventBroker.DisplaySetChanging -= OnDisplaySetChanging;
			base.ImageViewer.EventBroker.DisplaySetChanged -= OnDisplaySetChanged;
			base.ImageViewer.PhysicalWorkspace.LayoutCompleted -= OnLayoutCompleted;

			_cache.Release();
			_coordinator.Release();

			base.Dispose(disposing);
		}

		private void ToggleSynchronize()
		{
			SynchronizeActive = !SynchronizeActive;

			if (SynchronizeActive)
				_coordinator.OnSynchronizedImages(SynchronizeAll());
		}

		private void ToggleLinkStudies()
		{
			StudiesLinked = !StudiesLinked;

			if (StudiesLinked)
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
				List<OffsetKey> foundRelatedKeys = new List<OffsetKey>();

				foreach (IImageBox imageBox in GetImageBoxesToSynchronize(referenceImageBox))
				{
					if (imageBox.TopLeftPresentationImage is IImageSopProvider)
					{
						Frame frame = ((IImageSopProvider) imageBox.TopLeftPresentationImage).Frame;

						if (!String.IsNullOrEmpty(frame.FrameOfReferenceUid) && !String.IsNullOrEmpty(frame.ParentImageSop.StudyInstanceUID))
						{
							Frame referenceFrame = ((IImageSopProvider) referenceImageBox.TopLeftPresentationImage).Frame;

							if (frame.FrameOfReferenceUid != referenceFrame.FrameOfReferenceUid && frame.ParentImageSop.StudyInstanceUID != referenceFrame.ParentImageSop.StudyInstanceUID)
							{
								ImageInfo referenceImageInfo = _cache.GetImageInformation(referenceFrame);
								ImageInfo info = _cache.GetImageInformation(frame);

								if (info != null && NormalsWithinLimits(referenceImageInfo, info))
								{
									OffsetKey key = new OffsetKey(frame.ParentImageSop.StudyInstanceUID, frame.FrameOfReferenceUid, info.Normal);

									if (referenceOffsetKey == null)
									{
										referenceOffsetKey = new OffsetKey(referenceFrame.ParentImageSop.StudyInstanceUID, referenceFrame.FrameOfReferenceUid, referenceImageInfo.Normal);
										if (_offsets.ContainsKey(referenceOffsetKey))
										{
											referenceOffsetDictionary = _offsets[referenceOffsetKey];
										}
										else
										{
											referenceOffsetDictionary = new Dictionary<OffsetKey, Vector3D>();
											_offsets[referenceOffsetKey] = referenceOffsetDictionary;
										}
									}

									//as each new key is found, clear it from the dictionary so it can be recalculated
									//based on the currently visible display sets.
									if (!foundRelatedKeys.Contains(key))
									{
										foundRelatedKeys.Add(key);
										referenceOffsetDictionary.Remove(key);
									}

									Vector3D currentOffset = null;
									if (referenceOffsetDictionary.ContainsKey(key))
										currentOffset = referenceOffsetDictionary[key];

									Vector3D offset = info.PositionPatientCenterOfImage - referenceImageInfo.PositionPatientCenterOfImage;
									if (currentOffset == null || offset.Magnitude < currentOffset.Magnitude)
										referenceOffsetDictionary[key] = offset;
								}
							}
						}
					}
				}
			}
		}

		private Vector3D GetOffset(Frame referenceFrame, ImageInfo referenceImageInfo, Frame frame, ImageInfo info)
		{
			OffsetKey referenceOffsetKey = new OffsetKey(referenceFrame.ParentImageSop.StudyInstanceUID, referenceFrame.FrameOfReferenceUid, referenceImageInfo.Normal);
			if (_offsets.ContainsKey(referenceOffsetKey))
			{
				OffsetKey key = new OffsetKey(frame.ParentImageSop.StudyInstanceUID, frame.FrameOfReferenceUid, info.Normal);
				if (!key.Equals(referenceOffsetKey))
				{
					Vector3D offset = GetOffset(referenceOffsetKey, key, new List<OffsetKey>());
					if (offset != null)
						return offset;
				}
			}

			return Vector3D.Empty;
		}

		private Vector3D GetOffset(OffsetKey referenceOffsetKey, OffsetKey key, List<OffsetKey> eliminated)
		{
			if (referenceOffsetKey.Equals(key))
				return null;

			// This 'reference key' has now been checked against 'key', so whether it 
			// has a direct dependency or not, it should not be considered again,
			// otherwise, we could end up in infinite recursion.
			eliminated.Add(referenceOffsetKey);

			if (_offsets[referenceOffsetKey].ContainsKey(key))
			{
				return _offsets[referenceOffsetKey][key];
			}
			else
			{
				Vector3D relativeOffset = null;

				foreach (OffsetKey relatedKey in _offsets[referenceOffsetKey].Keys)
				{
					if (eliminated.Contains(relatedKey))
						continue;
					
					Vector3D offset = GetOffset(relatedKey, key, eliminated);
					if (offset != null)
					{
						//again, find the smallest of all possible offsets.
						offset += _offsets[referenceOffsetKey][relatedKey];
						if (relativeOffset == null || offset.Magnitude < relativeOffset.Magnitude)
							relativeOffset = offset;
					}
				}

				if (relativeOffset != null)
					return relativeOffset;
			}

			return null;
		}

		private static bool NormalsWithinLimits(ImageInfo referenceImageInfo, ImageInfo compareImageInfo)
		{
			float angle = Math.Abs((float)Math.Acos(compareImageInfo.Normal.Dot(referenceImageInfo.Normal)));
			return (angle <= _fiveDegreesInRadians || (Math.PI - angle) <= _fiveDegreesInRadians);
		}

		private int CalculateClosestSlice(IImageBox referenceImageBox, IImageBox imageBox)
		{
			Frame referenceFrame = ((IImageSopProvider) referenceImageBox.TopLeftPresentationImage).Frame;

			ImageInfo referenceImageInfo = _cache.GetImageInformation(referenceFrame);
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
					Frame frame = provider.Frame;

					bool sameFrameOfReference = frame.FrameOfReferenceUid == referenceFrame.FrameOfReferenceUid &&
					                             frame.ParentImageSop.StudyInstanceUID == referenceFrame.ParentImageSop.StudyInstanceUID;

					// When 'studies linked' is false, we only synchronize within the same frame of reference.
					if (this.StudiesLinked || sameFrameOfReference)
					{
						ImageInfo info = _cache.GetImageInformation(frame);
						if (info != null && NormalsWithinLimits(referenceImageInfo, info))
						{
							//Don't bother getting an offset for something in the same frame of reference.
							Vector3D offset = sameFrameOfReference ? Vector3D.Empty : GetOffset(referenceFrame, referenceImageInfo, frame, info);

							Vector3D difference = referenceImageInfo.PositionPatientCenterOfImage + offset - info.PositionPatientCenterOfImage;
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

		private bool SynchronizeImageBoxes(IImageBox referenceImageBox, IImageBox imageBox)
		{
			int index = CalculateClosestSlice(referenceImageBox, imageBox);
			if (index >= 0 && index != imageBox.TopLeftPresentationImageIndex)
			{
				imageBox.TopLeftPresentationImageIndex = index;
				return true;
			}

			return false;
		}

		private IEnumerable<IImageBox> SynchronizeWithImage(IImageBox referenceImageBox, IImageBox ignoreImageBox)
		{
			foreach (IImageBox imageBox in GetImageBoxesToSynchronize(referenceImageBox))
			{
				if (imageBox == ignoreImageBox)
					continue;

				if (SynchronizeImageBoxes(referenceImageBox, imageBox))
					yield return imageBox;
			}
		}

		private IEnumerable<IImageBox> SynchronizeNewDisplaySet(IDisplaySet newDisplaySet)
		{
			if (!SynchronizeActive || newDisplaySet == null)
				yield break;

			IImageBox changedImageBox = newDisplaySet.ImageBox;
			IImageBox selectedImageBox = this.Context.Viewer.SelectedImageBox;

			if (selectedImageBox == null || selectedImageBox == changedImageBox)
			{
				// if there is no selected image box or the new one is selected, try
				// to sync it up with the other ones.

				bool synced = false;
				// Do a reverse synchronization; sync the newly selected one with all the others.
				foreach (IImageBox imageBox in GetImageBoxesToSynchronize(changedImageBox))
				{
						if (SynchronizeImageBoxes(imageBox, changedImageBox))
							synced = true;
				}

				if (synced)
					yield return changedImageBox;
			}
			else
			{
				if (SynchronizeImageBoxes(selectedImageBox, changedImageBox))
					yield return changedImageBox;
			}
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
					//Synchronize everything with everything else, but never with the selected (do it last).
					foreach (IImageBox affectedImageBox in SynchronizeWithImage(imageBox, selectedImageBox))
						yield return affectedImageBox;
				}
			}

			if (selectedImageBox == null)
				yield break;

			//Synchronize with the selected.
			foreach (IImageBox affectedImageBox in SynchronizeWithImage(selectedImageBox))
				yield return affectedImageBox;
		}

		public IEnumerable<IImageBox> SynchronizeWithSelectedImage()
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
