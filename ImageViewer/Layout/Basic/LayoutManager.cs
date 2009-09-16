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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[ExtensionOf(typeof(LayoutManagerExtensionPoint))]
	public class LayoutManager : ClearCanvas.ImageViewer.LayoutManager
	{
		readonly List<StoredDisplaySetCreationOptions> _options = DisplaySetCreationSettings.Default.GetStoredOptions();

		private readonly MixedMultiFrameDisplaySetFactory _mixedMultiFrameFactory = new MixedMultiFrameDisplaySetFactory();
		private readonly MREchoDisplaySetFactory _echoFactory = new MREchoDisplaySetFactory();
		private readonly BasicDisplaySetFactory _basicFactory = new BasicDisplaySetFactory();

		private readonly DefaultPatientReconciliationStrategy _reconciliationStrategy = new DefaultPatientReconciliationStrategy();

		public LayoutManager()
		{
			List<string> singleImageModalities = CollectionUtils.Map(_options,
				delegate(StoredDisplaySetCreationOptions options)
				{
					if (options.CreateSingleImageDisplaySets)
						return options.Modality;

					return "";
				});

			singleImageModalities = CollectionUtils.Select(singleImageModalities,
				delegate(string modality) { return !String.IsNullOrEmpty(modality); });

			_basicFactory.SingleImageModalities.AddRange(singleImageModalities);
		}

		public override void SetImageViewer(IImageViewer imageViewer)
		{
			base.SetImageViewer(imageViewer);

			StudyTree studyTree = null;
			if (imageViewer != null)
				studyTree = imageViewer.StudyTree;

			_echoFactory.SetStudyTree(studyTree);
			_mixedMultiFrameFactory.SetStudyTree(studyTree);
			_basicFactory.SetStudyTree(studyTree);
		}

		#region Logical Workspace building 

		private bool SplitMixedMultiframeSeries(string modality)
		{
			StoredDisplaySetCreationOptions options = CollectionUtils.SelectFirst(_options,
					delegate(StoredDisplaySetCreationOptions opt) { return opt.Modality == modality; });

			if (options != null)
				return options.SplitMixedMultiframesEnabled && options.SplitMixedMultiframes;
			else
				return true;
		}

		private bool ShowOriginalMixedMultiframeSeries(string modality)
		{
			StoredDisplaySetCreationOptions options = CollectionUtils.SelectFirst(_options,
					delegate(StoredDisplaySetCreationOptions opt) { return opt.Modality == modality; });

			if (options != null)
				return options.ShowOriginalMixedMultiframeSeries;
			else
				return true;
		}

		private bool SplitMultiEchoSeries(string modality)
		{
			StoredDisplaySetCreationOptions options = CollectionUtils.SelectFirst(_options,
					delegate(StoredDisplaySetCreationOptions opt) { return opt.Modality == modality; });

			if (options != null)
				return options.SplitMultiEchoSeries && options.SplitMultiEchoSeriesEnabled;
			else
				return true;
		}

		private bool ShowOriginalMREchoSeries()
		{
			StoredDisplaySetCreationOptions options = CollectionUtils.SelectFirst(_options,
					delegate(StoredDisplaySetCreationOptions opt) { return opt.Modality == "MR"; });

			if (options != null)
				return options.ShowOriginalMultiEchoSeries;
			else
				return true;
		}

		protected override DicomImageSetDescriptor CreateImageSetDescriptor(IStudyRootStudyIdentifier studyRootData)
		{
			PatientInformation info = new PatientInformation();
			info.PatientId = studyRootData.PatientId;
			PatientInformation reconciled = _reconciliationStrategy.ReconcilePatientInformation(info);

			StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier(studyRootData);
			identifier.PatientId = reconciled.PatientId;

			return base.CreateImageSetDescriptor(identifier);
		}

		protected override void UpdateImageSet(IImageSet imageSet, Series series)
		{
			List<IDisplaySet> displaySets = new List<IDisplaySet>();

			bool showOriginal = true;

			if (SplitMultiEchoSeries(series.Modality))
			{
				List<IDisplaySet> echoDisplaySets = _echoFactory.CreateDisplaySets(series);
				if (echoDisplaySets.Count > 0 && !ShowOriginalMREchoSeries())
					showOriginal = false;

				displaySets.AddRange(echoDisplaySets);
			}

			if (SplitMixedMultiframeSeries(series.Modality))
			{
				List<IDisplaySet> multiFrameDisplaySets = _mixedMultiFrameFactory.CreateDisplaySets(series);
				if (multiFrameDisplaySets.Count > 0 && showOriginal && !ShowOriginalMixedMultiframeSeries(series.Modality))
					showOriginal = false;

				displaySets.AddRange(multiFrameDisplaySets);
			}

			if (showOriginal)
			{
				foreach(IDisplaySet displaySet in _basicFactory.CreateDisplaySets(series))
					displaySets.Add(displaySet);
			}

			foreach (IDisplaySet displaySet in displaySets)
				imageSet.DisplaySets.Add(displaySet);
		}

		#endregion

		protected override void LayoutPhysicalWorkspace()
		{
			StoredLayout layout = null;

			//take the first opened study, enumerate the modalities and compute the union of the layout configuration (in case there are multiple modalities in the study).
			if (LogicalWorkspace.ImageSets.Count > 0)
			{
				IImageSet firstImageSet = LogicalWorkspace.ImageSets[0];
				foreach (IDisplaySet displaySet in firstImageSet.DisplaySets)
				{
					if (displaySet.PresentationImages.Count <= 0)
						continue;

					if (layout == null)
						layout = LayoutSettings.GetMinimumLayout();

					StoredLayout storedLayout = LayoutSettings.Default.GetLayout(displaySet.PresentationImages[0] as IImageSopProvider);
					layout.ImageBoxRows = Math.Max(layout.ImageBoxRows, storedLayout.ImageBoxRows);
					layout.ImageBoxColumns = Math.Max(layout.ImageBoxColumns, storedLayout.ImageBoxColumns);
					layout.TileRows = Math.Max(layout.TileRows, storedLayout.TileRows);
					layout.TileColumns = Math.Max(layout.TileColumns, storedLayout.TileColumns);
				}
			}

			if (layout == null)
				layout = LayoutSettings.Default.DefaultLayout;

			PhysicalWorkspace.SetImageBoxGrid(layout.ImageBoxRows, layout.ImageBoxColumns);
			for (int i = 0; i < PhysicalWorkspace.ImageBoxes.Count; ++i)
				PhysicalWorkspace.ImageBoxes[i].SetTileGrid(layout.TileRows, layout.TileColumns);
		}
	}
}
