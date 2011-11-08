using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    partial class LayoutManager
    {
        internal class ImageSetFiller
        {
            private readonly Dictionary<string, DisplaySetFactory> _modalityDisplaySetFactories = new Dictionary<string, DisplaySetFactory>();
            private readonly ModalityDisplaySetFactory _modalityDisplaySetFactory;
            private readonly IDisplaySetFactory _defaultDisplaySetFactory;

            public ImageSetFiller(StudyTree studyTree)
            {
                foreach (StoredDisplaySetCreationSetting setting in DisplaySetCreationSettings.Default.GetStoredSettings())
                    _modalityDisplaySetFactories[setting.Modality] = new DisplaySetFactory(setting);

                _modalityDisplaySetFactory = new ModalityDisplaySetFactory();
                _defaultDisplaySetFactory = new BasicDisplaySetFactory();

                foreach (IDisplaySetFactory displaySetFactory in _modalityDisplaySetFactories.Values)
                    displaySetFactory.SetStudyTree(studyTree);

                _modalityDisplaySetFactory.SetStudyTree(studyTree);
                _defaultDisplaySetFactory.SetStudyTree(studyTree);
            }

            public void AddMultiSeriesDisplaySets(IImageSet imageSet, Study study)
            {
                var studyModalities = (from setting in DisplaySetCreationSettings.Default.GetStoredSettings()
                                       where setting.CreateAllImagesDisplaySet
                                       join allowedModality in DisplaySetCreationSettings.Default.GetAllImagesModalities()
                                           on setting.Modality equals allowedModality
                                       join series in study.Series
                                           on setting.Modality equals series.Modality
                                       select setting.Modality).Distinct();

                foreach (var modality in studyModalities)
                {
                    //Add all the "all images" per modality display sets for the entire study at the top of the list.
                    var displaySet = _modalityDisplaySetFactory.CreateDisplaySet(study, modality);
                    if (displaySet != null)
                        imageSet.DisplaySets.Add(displaySet);
                }
            }

            public void AddSeriesDisplaySets(IImageSet imageSet, Series series)
            {
                var factory = GetDisplaySetFactory(series.Modality);
                if (factory == null)
                {
                    factory = _defaultDisplaySetFactory;
                }
                else
                {
                    bool modalityDisplaySetExists = null != imageSet.DisplaySets.FirstOrDefault(
                                                                d => d.Descriptor is ModalityDisplaySetDescriptor
                                                                    && ((ModalityDisplaySetDescriptor)d.Descriptor).Modality == series.Modality);

                    //Tell the factory whether we've created an "all images" display set containing
                    //all the images in the entire study for the given modality.

                    ((DisplaySetFactory)factory).ModalityDisplaySetExists = modalityDisplaySetExists;
                }

                //Add all the display sets derived from single series next.
                List<IDisplaySet> displaySets = factory.CreateDisplaySets(series);
                foreach (IDisplaySet displaySet in displaySets)
                    imageSet.DisplaySets.Add(displaySet);
            }

            private IDisplaySetFactory GetDisplaySetFactory(string modality)
            {
                modality = modality ?? String.Empty;

                DisplaySetFactory factory;
                if (_modalityDisplaySetFactories.TryGetValue(modality, out factory))
                    return factory;

                return null;
            }
        }

        internal class DisplaySetFactory : ImageViewer.DisplaySetFactory
        {
            private readonly StoredDisplaySetCreationSetting _creationSetting;

            private readonly MREchoDisplaySetFactory _echoFactory;
            private readonly MixedMultiFrameDisplaySetFactory _mixedMultiFrameFactory;
            private readonly BasicDisplaySetFactory _basicFactory;
            private readonly IDisplaySetFactory _placeholderDisplaySetFactory;

            private readonly IList<IDisplaySetFactory> _externalFactories;

            public DisplaySetFactory(StoredDisplaySetCreationSetting creationSetting)
            {
                _creationSetting = creationSetting;

                PresentationState defaultPresentationState = new DicomPresentationState { ShowGrayscaleInverted = creationSetting.ShowGrayscaleInverted };

                var imageFactory = (PresentationImageFactory)PresentationImageFactory;
                imageFactory.DefaultPresentationState = defaultPresentationState;

                _basicFactory = new BasicDisplaySetFactory(imageFactory) { CreateSingleImageDisplaySets = _creationSetting.CreateSingleImageDisplaySets };

                if (creationSetting.SplitMultiEchoSeries)
                    _echoFactory = new MREchoDisplaySetFactory(imageFactory);

                if (_creationSetting.SplitMixedMultiframes)
                    _mixedMultiFrameFactory = new MixedMultiFrameDisplaySetFactory(imageFactory);

                var externalFactories = new List<IDisplaySetFactory>();
                foreach (IDisplaySetFactoryProvider provider in new DisplaySetFactoryProviderExtensionPoint().CreateExtensions())
                    externalFactories.AddRange(provider.CreateDisplaySetFactories(imageFactory));

                _externalFactories = externalFactories.AsReadOnly();

                _placeholderDisplaySetFactory = new PlaceholderDisplaySetFactory();
            }

            public bool ModalityDisplaySetExists { get; set; }

            private bool ShowOriginalSeries { get { return _creationSetting.ShowOriginalSeries; } }
            private bool CreateSingleImageDisplaySets { get { return _creationSetting.CreateSingleImageDisplaySets; } }

            private bool SplitMultiEchoSeries { get { return _creationSetting.SplitMultiEchoSeries; } }
            private bool ShowOriginalMREchoSeries { get { return _creationSetting.ShowOriginalMultiEchoSeries; } }

            private bool SplitMixedMultiframeSeries { get { return _creationSetting.SplitMixedMultiframes; } }
            private bool ShowOriginalMixedMultiframeSeries { get { return _creationSetting.ShowOriginalMixedMultiframeSeries; } }

            public override void SetStudyTree(StudyTree studyTree)
            {
                base.SetStudyTree(studyTree);

                _basicFactory.SetStudyTree(studyTree);

                if (_echoFactory != null)
                    _echoFactory.SetStudyTree(studyTree);

                if (_mixedMultiFrameFactory != null)
                    _mixedMultiFrameFactory.SetStudyTree(studyTree);

                _placeholderDisplaySetFactory.SetStudyTree(studyTree);

                foreach (var factory in _externalFactories)
                    factory.SetStudyTree(studyTree);
            }

            public override List<IDisplaySet> CreateDisplaySets(Series series)
            {
                var displaySets = new List<IDisplaySet>();

                bool showOriginal = true;

                /// TODO (CR Oct 2011): This disables the degenerate "single image" case ...
                if (ModalityDisplaySetExists && !ShowOriginalSeries)
                    showOriginal = false;

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

                if (CreateSingleImageDisplaySets)
                {
                    //The factory will only create single image display sets and will not create a series 
                    //display set for the degenerate case of one image in a series.
                    _basicFactory.CreateSingleImageDisplaySets = true;
                    foreach (IDisplaySet displaySet in _basicFactory.CreateDisplaySets(series))
                        displaySets.Add(displaySet);

                    //Degenerate case for "single images" is to create the series display set.
                    showOriginal = displaySets.Count <= 0 || ShowOriginalSeries;
                }

                if (showOriginal)
                {
                    //The factory will create series display sets only.
                    _basicFactory.CreateSingleImageDisplaySets = false;
                    foreach (IDisplaySet displaySet in _basicFactory.CreateDisplaySets(series))
                        displaySets.Add(displaySet);
                }

                if (displaySets.Count == 0)
                    displaySets.AddRange(_placeholderDisplaySetFactory.CreateDisplaySets(series));

                foreach (var factory in _externalFactories)
                    displaySets.AddRange(factory.CreateDisplaySets(series));

                return displaySets;
            }
        }
    }
}