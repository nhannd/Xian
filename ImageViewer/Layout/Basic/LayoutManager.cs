#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.PresentationStates;
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

			private readonly IList<IDisplaySetFactory> _externalFactories;

			public DisplaySetFactory(StoredDisplaySetCreationSetting creationSetting)
			{
				_creationSetting = creationSetting;

				PresentationState defaultPresentationState = new BasicDicomPresentationState 
																{ ShowGrayscaleInverted = creationSetting.ShowGrayscaleInverted };

				var imageFactory = (PresentationImageFactory)PresentationImageFactory;
				imageFactory.DefaultPresentationState = defaultPresentationState;

				_basicFactory = new BasicDisplaySetFactory(imageFactory) 
										{ CreateSingleImageDisplaySets = _creationSetting.CreateSingleImageDisplaySets };

				if (creationSetting.SplitMultiEchoSeries)
					_echoFactory = new MREchoDisplaySetFactory(imageFactory);

				if (_creationSetting.SplitMixedMultiframes)
					_mixedMultiFrameFactory = new MixedMultiFrameDisplaySetFactory(imageFactory);

				var externalFactories = new List<IDisplaySetFactory>();
				foreach (IDisplaySetFactoryProvider provider in new DisplaySetFactoryProviderExtensionPoint().CreateExtensions())
					externalFactories.AddRange(provider.CreateDisplaySetFactories(imageFactory));
				_externalFactories = externalFactories.AsReadOnly();
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

				foreach (var factory in _externalFactories)
					factory.SetStudyTree(studyTree);
			}

			public override List<IDisplaySet> CreateDisplaySets(Series series)
			{
				var displaySets = new List<IDisplaySet>();

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

				foreach (var factory in _externalFactories)
					displaySets.AddRange(factory.CreateDisplaySets(series));

				return displaySets;
			}
		}

		private readonly IPatientReconciliationStrategy _reconciliationStrategy = new DefaultPatientReconciliationStrategy();
		private readonly Dictionary<string, IDisplaySetFactory> _modalityDisplaySetFactories = new Dictionary<string, IDisplaySetFactory>();
		private const string _defaultModality = "";

		public LayoutManager()
		{
			foreach (StoredDisplaySetCreationSetting setting in DisplaySetCreationSettings.Default.GetStoredSettings())
				_modalityDisplaySetFactories[setting.Modality] = new DisplaySetFactory(setting);

			AllowEmptyViewer = ViewerLaunchSettings.AllowEmptyViewer;
		}

		public override void SetImageViewer(IImageViewer imageViewer)
		{
			base.SetImageViewer(imageViewer);

			StudyTree studyTree = null;
			if (imageViewer != null)
				studyTree = imageViewer.StudyTree;

			_reconciliationStrategy.SetStudyTree(studyTree);

			foreach (IDisplaySetFactory displaySetFactory in _modalityDisplaySetFactories.Values)
				displaySetFactory.SetStudyTree(studyTree);

			_modalityDisplaySetFactories[_defaultModality] = new BasicDisplaySetFactory {ShouldCreatePlaceholderImageDisplaySets = true};
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

		protected override IComparer<Series> GetSeriesComparer()
		{
			return new CompositeComparer<Series>(new DXSeriesPresentationIntentComparer(), base.GetSeriesComparer());
		}

		protected override IPatientData ReconcilePatient(Study study)
		{
			var reconciled = _reconciliationStrategy.ReconcilePatientInformation(study.ParentPatient);
			if (reconciled != null)
				return new StudyRootStudyIdentifier(reconciled, study.GetIdentifier());

			return base.ReconcilePatient(study);
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

		#region Comparers

		private class DXSeriesPresentationIntentComparer : DicomSeriesComparer
		{
			public override int Compare(Sop x, Sop y)
			{
				// this sorts FOR PROCESSING series to the end.
				// FOR PRESENTATION and unspecified series are considered equal for the purposes of sorting by intent.
				const string forProcessing = "FOR PROCESSING";
				int presentationIntentX = GetPresentationIntent(x) == forProcessing ? 1 : 0;
				int presentationIntentY = GetPresentationIntent(y) == forProcessing ? 1 : 0;
				int result = presentationIntentX - presentationIntentY;
				if (Reverse)
					return -result;
				return result;
			}

			private static string GetPresentationIntent(Sop sop)
			{
				DicomAttribute attribute;
				if (sop.DataSource.TryGetAttribute(DicomTags.PresentationIntentType, out attribute))
					return (attribute.ToString() ?? string.Empty).ToUpperInvariant();
				return string.Empty;
			}
		}

		private class CompositeComparer<T> : IComparer<T>
		{
			private readonly IList<IComparer<T>> _comparers;

			public CompositeComparer(params IComparer<T>[] comparers)
			{
				Platform.CheckForNullReference(comparers, "comparers");
				_comparers = new List<IComparer<T>>(comparers);
			}

			public int Compare(T x, T y)
			{
				foreach (IComparer<T> comparer in _comparers)
				{
					int result = comparer.Compare(x, y);
					if (result != 0)
						return result;
				}
				return 0;
			}
		}

		#endregion
	}
}
