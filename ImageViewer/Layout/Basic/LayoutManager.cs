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
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Configuration;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[ExtensionOf(typeof(LayoutManagerExtensionPoint))]
	public class LayoutManager : ImageViewer.LayoutManager
	{
		private class DisplaySetFactory : ImageViewer.DisplaySetFactory
		{
			private readonly StoredDisplaySetCreationSetting _creationSetting;

			private readonly MREchoDisplaySetFactory _echoFactory;
			private readonly MixedMultiFrameDisplaySetFactory _mixedMultiFrameFactory;
			private readonly BasicDisplaySetFactory _basicFactory;

			public DisplaySetFactory(StoredDisplaySetCreationSetting creationSetting)
			{
				_creationSetting = creationSetting;

				PresentationState defaultPresentationState = new BasicDicomPresentationState 
																{ ShowGrayscaleInverted = creationSetting.ShowGrayscaleInverted };

				PresentationImageFactory imageFactory = (PresentationImageFactory)PresentationImageFactory;
				imageFactory.DefaultPresentationState = defaultPresentationState;

				_basicFactory = new BasicDisplaySetFactory(imageFactory) 
										{ CreateSingleImageDisplaySets = _creationSetting.CreateSingleImageDisplaySets };

				if (creationSetting.SplitMultiEchoSeries)
					_echoFactory = new MREchoDisplaySetFactory(imageFactory);

				if (_creationSetting.SplitMixedMultiframes)
					_mixedMultiFrameFactory = new MixedMultiFrameDisplaySetFactory(imageFactory);
			}

			private bool SplitMultiEchoSeries { get { return _creationSetting.SplitMultiEchoSeries; } }
			private bool ShowOriginalMREchoSeries { get { return _creationSetting.ShowOriginalMultiEchoSeries; } }

			private bool SplitMixedMultiframeSeries { get { return _creationSetting.SplitMixedMultiframes; } }
			private bool ShowOriginalMixedMultiframeSeries { get { return _creationSetting.ShowOriginalMixedMultiframeSeries; } }

			public override void  SetStudyTree(StudyTree studyTree)
			{
				base.SetStudyTree(studyTree);

				_basicFactory.SetStudyTree(studyTree);
				
				if (_echoFactory != null)
					_echoFactory.SetStudyTree(studyTree);

				if (_mixedMultiFrameFactory != null)
					_mixedMultiFrameFactory.SetStudyTree(studyTree);
			}

			public override List<IDisplaySet> CreateDisplaySets(Series series)
			{
				List<IDisplaySet> displaySets = new List<IDisplaySet>();

				bool showOriginal = true;

				if (SplitMultiEchoSeries)
				{
					List<IDisplaySet> echoDisplaySets = _echoFactory.CreateDisplaySets(series);
					if (echoDisplaySets.Count > 0 && !ShowOriginalMREchoSeries)
						showOriginal = false;

					displaySets.AddRange(echoDisplaySets);
				}

				if (SplitMixedMultiframeSeries)
				{
					List<IDisplaySet> multiFrameDisplaySets = _mixedMultiFrameFactory.CreateDisplaySets(series);
					if (multiFrameDisplaySets.Count > 0 && showOriginal && !ShowOriginalMixedMultiframeSeries)
						showOriginal = false;

					displaySets.AddRange(multiFrameDisplaySets);
				}

				if (showOriginal)
				{
					foreach (IDisplaySet displaySet in _basicFactory.CreateDisplaySets(series))
						displaySets.Add(displaySet);
				}

				return displaySets;
			}
		}

		private readonly DefaultPatientReconciliationStrategy _reconciliationStrategy = new DefaultPatientReconciliationStrategy();
		private readonly Dictionary<string, IDisplaySetFactory> _modalityDisplaySetFactories = new Dictionary<string, IDisplaySetFactory>();
		private const string _defaultModality = "";

		public LayoutManager()
		{
			foreach (StoredDisplaySetCreationSetting setting in DisplaySetCreationSettings.Default.GetStoredSettings())
				_modalityDisplaySetFactories[setting.Modality] = new DisplaySetFactory(setting);

			base.AllowEmptyViewer = ViewerLaunchSettings.AllowEmptyViewer;
		}

		public override void SetImageViewer(IImageViewer imageViewer)
		{
			base.SetImageViewer(imageViewer);

			StudyTree studyTree = null;
			if (imageViewer != null)
				studyTree = imageViewer.StudyTree;

			foreach (IDisplaySetFactory displaySetFactory in _modalityDisplaySetFactories.Values)
				displaySetFactory.SetStudyTree(studyTree);

			_modalityDisplaySetFactories[_defaultModality] = new BasicDisplaySetFactory();
		}

		private IDisplaySetFactory GetDisplaySetFactory(string modality)
		{
			modality = modality ?? _defaultModality;

			IDisplaySetFactory factory;
			if (_modalityDisplaySetFactories.TryGetValue(modality, out factory))
				return factory;

			return _modalityDisplaySetFactories[_defaultModality];
		}

		#region Logical Workspace building 

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
			List<IDisplaySet> displaySets = GetDisplaySetFactory(series.Modality).CreateDisplaySets(series);
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
