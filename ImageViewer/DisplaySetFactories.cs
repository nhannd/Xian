using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer
{
	#region Default

	[Cloneable(false)]
	internal class SeriesDisplaySetDescriptor : DicomDisplaySetDescriptor
	{
		public SeriesDisplaySetDescriptor(ISeriesIdentifier sourceSeries, IPresentationImageFactory presentationImageFactory)
			: base(sourceSeries, presentationImageFactory)
		{
			Platform.CheckForNullReference(presentationImageFactory, "presentationImageFactory");
		}

		protected SeriesDisplaySetDescriptor(SeriesDisplaySetDescriptor source, ICloningContext context)
			: base(source, context)
		{
		}

		protected override string GetName()
		{
			return String.Format("{0}: {1}", SourceSeries.SeriesNumber, SourceSeries.SeriesDescription);
		}

		protected override string GetDescription()
		{
			return SourceSeries.SeriesDescription;
		}

		protected override string GetUid()
		{
			return SourceSeries.SeriesInstanceUid;
		}

		protected override bool ShouldAddSop(Sop sop)
		{
			return (sop.SeriesInstanceUid == SourceSeries.SeriesInstanceUid && sop.IsImage);
		}
	}

	[Cloneable(false)]
	internal class SingleFrameDisplaySetDescriptor : DicomDisplaySetDescriptor
	{
		private readonly string _suffix;
		private readonly string _sopInstanceUid;
		private readonly int _frameNumber;

		public SingleFrameDisplaySetDescriptor(ISeriesIdentifier sourceSeries, string sopInstanceUid, int instanceNumber, int frameNumber)
			: base(sourceSeries)
		{
			_sopInstanceUid = sopInstanceUid;
			_frameNumber = frameNumber;
			_suffix = String.Format(SR.SuffixFormatSingleFrameDisplaySet, instanceNumber, _frameNumber);
		}

		protected SingleFrameDisplaySetDescriptor(SingleFrameDisplaySetDescriptor source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override string GetName()
		{
			if (String.IsNullOrEmpty(SourceSeries.SeriesDescription))
				return String.Format("{0}: {1}", SourceSeries.SeriesNumber, _suffix);
			else
				return String.Format("{0}: {1} - {2}", SourceSeries.SeriesNumber, SourceSeries.SeriesDescription, _suffix);
		}

		protected override string GetDescription()
		{
			if (String.IsNullOrEmpty(SourceSeries.SeriesDescription))
				return _suffix;
			else
				return String.Format("{0} - {1}", SourceSeries.SeriesDescription, _suffix);
		}

		protected override string GetUid()
		{
			return String.Format("{0}:{1}:{2}", SourceSeries.SeriesInstanceUid, _sopInstanceUid, _frameNumber);
		}
	}

	[Cloneable(false)]
	internal class SingleImageDisplaySetDescriptor : DicomDisplaySetDescriptor
	{
		private readonly string _suffix;
		private readonly string _sopInstanceUid;

		public SingleImageDisplaySetDescriptor(ISeriesIdentifier sourceSeries, string sopInstanceUid, int instanceNumber)
			: base(sourceSeries)
		{
			_sopInstanceUid = sopInstanceUid;
			_suffix = String.Format(SR.SuffixFormatSingleImageDisplaySet, instanceNumber);
		}

		protected SingleImageDisplaySetDescriptor(SingleImageDisplaySetDescriptor source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override string GetName()
		{
			if (String.IsNullOrEmpty(SourceSeries.SeriesDescription))
				return String.Format("{0}: {1}", SourceSeries.SeriesNumber, _suffix);
			else
				return String.Format("{0}: {1} - {2}", SourceSeries.SeriesNumber, SourceSeries.SeriesDescription, _suffix);
		}

		protected override string GetDescription()
		{
			if (String.IsNullOrEmpty(SourceSeries.SeriesDescription))
				return _suffix;
			else
				return String.Format("{0} - {1}", SourceSeries.SeriesDescription, _suffix);
		}

		protected override string GetUid()
		{
			return String.Format("{0}:{1}", SourceSeries.SeriesInstanceUid, _sopInstanceUid);
		}
	}

	public class BasicDisplaySetFactory : DisplaySetFactory
	{
		private readonly List<string> _singleImageModalities = new List<string>();

		public BasicDisplaySetFactory()
		{
		}

		public BasicDisplaySetFactory(IPresentationImageFactory presentationImageFactory)
			:base(presentationImageFactory)
		{
		}

		public List<string> SingleImageModalities
		{
			get { return _singleImageModalities; }
		}


		public override List<IDisplaySet> CreateDisplaySets(Series series)
		{
			if (_singleImageModalities.Contains(series.Modality))
			{
				return CreateSingleImageDisplaySets(series);
			}
			else
			{
				List<IDisplaySet> displaySets = new List<IDisplaySet>();
				IDisplaySet displaySet = CreateSeriesDisplaySet(series);
				if (displaySet != null)
					displaySets.Add(displaySet);
				return displaySets;
			}
		}

		private IDisplaySet CreateSeriesDisplaySet(Series series)
		{
			IDisplaySet displaySet = null;
			List<IPresentationImage> images = new List<IPresentationImage>();
			foreach (Sop sop in series.Sops)
				images.AddRange(PresentationImageFactory.CreateImages(sop));

			if (images.Count > 0)
			{
				displaySet = new DisplaySet(new SeriesDisplaySetDescriptor(series.GetIdentifier(), PresentationImageFactory));
				foreach (IPresentationImage image in images)
					displaySet.PresentationImages.Add(image);
			}

			return displaySet;
		}

		private List<IDisplaySet> CreateSingleImageDisplaySets(Series series)
		{
			List<IDisplaySet> displaySets = new List<IDisplaySet>();

			foreach (Sop sop in series.Sops)
			{
				List<IPresentationImage> images = PresentationImageFactory.CreateImages(sop);
				if (images.Count == 0)
					continue;

				if (sop.IsImage)
				{
					ImageSop imageSop = (ImageSop)sop;
					DisplaySetDescriptor descriptor;

					if (imageSop.NumberOfFrames == 1)
						descriptor = new SingleImageDisplaySetDescriptor(series.GetIdentifier(), sop.SopInstanceUid, sop.InstanceNumber);
					else
						descriptor = new MultiframeDisplaySetDescriptor(series.GetIdentifier(), sop.SopInstanceUid, sop.InstanceNumber);

					DisplaySet displaySet = new DisplaySet(descriptor);
					foreach (IPresentationImage image in images)
						displaySet.PresentationImages.Add(image);

					displaySets.Add(displaySet);
				}
				else
				{
					if (series.Sops.Count == 1 && images.Count == 1)
					{
						//The sop is actually a container for other referenced sops, like key images, but there's only one image to show, so it's a complete series.
						DisplaySet displaySet = new DisplaySet(new SeriesDisplaySetDescriptor(series.GetIdentifier(), PresentationImageFactory));
						displaySet.PresentationImages.Add(images[0]);
						displaySets.Add(displaySet);
					}
					else
					{
						//The sop is actually a container for other referenced sops, like key images.
						foreach (IPresentationImage image in images)
						{
							IImageSopProvider provider = (IImageSopProvider)image;
							DisplaySetDescriptor descriptor;
							if (provider.ImageSop.NumberOfFrames == 1)
								descriptor = new SingleImageDisplaySetDescriptor(series.GetIdentifier(), provider.ImageSop.SopInstanceUid, provider.ImageSop.InstanceNumber);
							else
								descriptor = new SingleFrameDisplaySetDescriptor(series.GetIdentifier(), provider.ImageSop.SopInstanceUid, provider.ImageSop.InstanceNumber, provider.Frame.FrameNumber);

							DisplaySet displaySet = new DisplaySet(descriptor);
							displaySet.PresentationImages.Add(image);
							displaySets.Add(displaySet);
						}
					}
				}
			}

			return displaySets;
		}

		internal static IEnumerable<IDisplaySet> CreateSeriesDisplaySets(Series series, StudyTree studyTree)
		{
			BasicDisplaySetFactory factory = new BasicDisplaySetFactory();
			factory.SetStudyTree(studyTree);
			return factory.CreateDisplaySets(series);
		}
	}
	
	#endregion

	#region MR Echo

	[Cloneable(false)]
	internal class MREchoDisplaySetDescriptor : DicomDisplaySetDescriptor
	{
		private readonly int _echoNumber;
		private readonly string _suffix;

		public MREchoDisplaySetDescriptor(ISeriesIdentifier sourceSeries, int echoNumber, IPresentationImageFactory presentationImageFactory)
			: base(sourceSeries, presentationImageFactory)
		{
			Platform.CheckForNullReference(presentationImageFactory, "presentationImageFactory");
			_echoNumber = echoNumber;
			_suffix = String.Format(SR.SuffixFormatMREchoDisplaySet, echoNumber);
		}

		protected MREchoDisplaySetDescriptor(MREchoDisplaySetDescriptor source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override string GetName()
		{
			if (String.IsNullOrEmpty(base.SourceSeries.SeriesDescription))
				return String.Format("{0}: {1}", SourceSeries.SeriesNumber, _suffix);
			else
				return String.Format("{0}: {1} - {2}", SourceSeries.SeriesNumber, SourceSeries.SeriesDescription, _suffix);
		
		}

		protected override string GetDescription()
		{
			if (String.IsNullOrEmpty(base.SourceSeries.SeriesDescription))
				return _suffix;
			else
				return String.Format("{0} - {1}", SourceSeries.SeriesDescription, _suffix);
		}

		protected override string GetUid()
		{
			return String.Format("{0}:Echo{1}", SourceSeries.SeriesInstanceUid, _echoNumber);
		}

		protected override bool ShouldAddSop(Sop sop)
		{
			if (sop.IsImage)
			{
				DicomAttribute echoAttribute = sop.DataSource[DicomTags.EchoNumbers];
				if (!echoAttribute.IsEmpty)
				{
					int echoNumber = echoAttribute.GetInt32(0, 0);
					return echoNumber == _echoNumber;
				}
			}

			return false;
		}
	}

	public class MREchoDisplaySetFactory : DisplaySetFactory
	{
		public MREchoDisplaySetFactory()
		{}

		public MREchoDisplaySetFactory(IPresentationImageFactory presentationImageFactory)
			: base(presentationImageFactory)
		{ }

		public override List<IDisplaySet> CreateDisplaySets(Series series)
		{
			List<IDisplaySet> displaySets = new List<IDisplaySet>();

			if (series.Modality == "MR")
			{
				SortedDictionary<int, List<Sop>> imagesByEchoNumber = SplitMREchos(series.Sops);
				if (imagesByEchoNumber.Count > 1)
				{
					foreach (KeyValuePair<int, List<Sop>> echoImages in imagesByEchoNumber)
					{
						List<IPresentationImage> images = new List<IPresentationImage>();
						foreach (ImageSop sop in echoImages.Value)
							images.AddRange(PresentationImageFactory.CreateImages(sop));

						if (images.Count > 0)
						{
							IDisplaySet displaySet = new DisplaySet(new MREchoDisplaySetDescriptor(series.GetIdentifier(), echoImages.Key, PresentationImageFactory));
							foreach (IPresentationImage image in images)
								displaySet.PresentationImages.Add(image);

							displaySets.Add(displaySet);
						}
					}
				}
			}

			return displaySets;
		}

		private static SortedDictionary<int, List<Sop>> SplitMREchos(IEnumerable<Sop> sops)
		{
			SortedDictionary<int, List<Sop>> imagesByEchoNumber = new SortedDictionary<int, List<Sop>>();

			foreach (Sop sop in sops)
			{
				if (sop.IsImage)
				{
					DicomAttribute echoAttribute = sop.DataSource[DicomTags.EchoNumbers];
					if (!echoAttribute.IsEmpty)
					{
						int echoNumber = echoAttribute.GetInt32(0, 0);
						if (!imagesByEchoNumber.ContainsKey(echoNumber))
							imagesByEchoNumber[echoNumber] = new List<Sop>();

						imagesByEchoNumber[echoNumber].Add(sop);
					}
				}
			}

			return imagesByEchoNumber;
		}
	}

	#endregion

	#region Mixed Multi-frame

	[Cloneable(false)]
	internal class MultiframeDisplaySetDescriptor : DicomDisplaySetDescriptor
	{
		private readonly string _sopInstanceUid;
		private readonly string _suffix;

		public MultiframeDisplaySetDescriptor(ISeriesIdentifier sourceSeries, string sopInstanceUid, int instanceNumber)
			: base(sourceSeries)
		{
			_sopInstanceUid = sopInstanceUid;
			_suffix = String.Format(SR.SuffixFormatMultiframeDisplaySet, instanceNumber);
		}

		protected MultiframeDisplaySetDescriptor(MultiframeDisplaySetDescriptor source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override string GetName()
		{
			if (String.IsNullOrEmpty(base.SourceSeries.SeriesDescription))
				return String.Format("{0}: {1}", SourceSeries.SeriesNumber, _suffix);
			else
				return String.Format("{0}: {1} - {2}", SourceSeries.SeriesNumber, SourceSeries.SeriesDescription, _suffix);
		}
		
		protected override string GetDescription()
		{
			if (String.IsNullOrEmpty(base.SourceSeries.SeriesDescription))
				return _suffix;
			else
				return String.Format("{0} - {1}", SourceSeries.SeriesDescription, _suffix);
		}
		
		protected override string GetUid()
		{
			return _sopInstanceUid;
		}
	}

	[Cloneable(false)]
	internal class SingleImagesDisplaySetDescriptor : DicomDisplaySetDescriptor
	{
		private readonly string _suffix;

		public SingleImagesDisplaySetDescriptor(ISeriesIdentifier sourceSeries, IPresentationImageFactory presentationImageFactory)
			: base(sourceSeries, presentationImageFactory)
		{
			_suffix = SR.SuffixSingleImagesDisplaySet;
		}

		protected SingleImagesDisplaySetDescriptor(SingleImagesDisplaySetDescriptor source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override string GetName()
		{
			if (String.IsNullOrEmpty(base.SourceSeries.SeriesDescription))
				return String.Format("{0}: {1}", SourceSeries.SeriesNumber, _suffix);
			else
				return String.Format("{0}: {1} - {2}", SourceSeries.SeriesNumber, SourceSeries.SeriesDescription, _suffix);
		}
	
		protected override string GetDescription()
		{
			if (String.IsNullOrEmpty(base.SourceSeries.SeriesDescription))
				return _suffix;
			else
				return String.Format("{0} - {1}", SourceSeries.SeriesDescription, _suffix);
		}

		protected override string GetUid()
		{
			return String.Format("{0}:SingleImages", SourceSeries.SeriesInstanceUid);
		}

		protected override bool ShouldAddSop(Sop sop)
		{
			return sop.SeriesInstanceUid == SourceSeries.SeriesInstanceUid && sop.IsImage && ((ImageSop)sop).NumberOfFrames == 1;
		}
	}

	public class MixedMultiFrameDisplaySetFactory : DisplaySetFactory
	{
		public MixedMultiFrameDisplaySetFactory()
		{}

		public MixedMultiFrameDisplaySetFactory(IPresentationImageFactory presentationImageFactory)
			: base(presentationImageFactory)
		{ }

		public override List<IDisplaySet> CreateDisplaySets(Series series)
		{
			List<IDisplaySet> displaySets = new List<IDisplaySet>();

			List<ImageSop> singleFrames = new List<ImageSop>();
			List<ImageSop> multiFrames = new List<ImageSop>();

			foreach (Sop sop in series.Sops)
			{
				if (sop.IsImage)
				{
					ImageSop imageSop = (ImageSop)sop;
					if (imageSop.NumberOfFrames > 1)
						multiFrames.Add(imageSop);
					else
						singleFrames.Add(imageSop);
				}
			}

			if (multiFrames.Count > 1 || (singleFrames.Count > 0 && multiFrames.Count > 0))
			{
				if (singleFrames.Count > 0)
				{
					List<IPresentationImage> singleFrameImages = new List<IPresentationImage>();
					foreach (ImageSop singleFrame in singleFrames)
						singleFrameImages.AddRange(PresentationImageFactory.CreateImages(singleFrame));

					if (singleFrameImages.Count > 0)
					{
						SingleImagesDisplaySetDescriptor descriptor =
							new SingleImagesDisplaySetDescriptor(series.GetIdentifier(), PresentationImageFactory);
						DisplaySet singleImagesDisplaySet = new DisplaySet(descriptor);

						foreach (IPresentationImage singleFrameImage in singleFrameImages)
							singleImagesDisplaySet.PresentationImages.Add(singleFrameImage);

						displaySets.Add(singleImagesDisplaySet);
					}
				}

				foreach (ImageSop multiFrame in multiFrames)
				{
					List<IPresentationImage> multiFrameImages = PresentationImageFactory.CreateImages(multiFrame);
					if (multiFrameImages.Count > 0)
					{
						MultiframeDisplaySetDescriptor descriptor =
							new MultiframeDisplaySetDescriptor(multiFrame.ParentSeries.GetIdentifier(), multiFrame.SopInstanceUid, multiFrame.InstanceNumber);
						DisplaySet displaySet = new DisplaySet(descriptor);

						foreach (IPresentationImage multiFrameImage in multiFrameImages)
							displaySet.PresentationImages.Add(multiFrameImage);

						displaySets.Add(displaySet);
					}
				}
			}

			return displaySets;
		}
	}
	
	#endregion
}