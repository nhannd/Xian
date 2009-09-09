using System;
using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	#region Default

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
			//TODO: make this work with Key Images.
			if (_singleImageModalities.Contains(series.Modality) && series.NumberOfSeriesRelatedInstances > 1)
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

		internal static List<IDisplaySet> CreateSeriesDisplaySets(Series series, StudyTree studyTree)
		{
			BasicDisplaySetFactory factory = new BasicDisplaySetFactory();
			factory.SetStudyTree(studyTree);
			return factory.CreateDisplaySets(series);
		}
	}
	
	#endregion

	#region MR Echo

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
							IDisplaySet displaySet = CreateDisplaySet(echoImages.Key, series);
							foreach (IPresentationImage image in images)
								displaySet.PresentationImages.Add(image);

							displaySets.Add(displaySet);
						}
					}
				}
			}

			return displaySets;
		}

		private static IDisplaySet CreateDisplaySet(int echoNumber, ISeriesData series)
		{
			DisplaySet displaySet = CreateSeriesDisplaySet<DisplaySet>(series);
			displaySet.Uid += String.Format(":MREcho{0}", echoNumber);

			String suffix = String.Format(SR.SuffixFormatMREchoDisplaySet, echoNumber);

			//NOTE: purposely use description here b/c the name is never empty
			if (String.IsNullOrEmpty(displaySet.Description))
				displaySet.Name += suffix;
			else
				displaySet.Name += " - " + suffix;

			if (String.IsNullOrEmpty(displaySet.Description))
				displaySet.Description += suffix;
			else
				displaySet.Description += " - " + suffix;

			return displaySet;
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
				IDisplaySet singleFrameDisplaySet = CreateSingleFramesDisplaySet(singleFrames);
				if (singleFrameDisplaySet != null)
					displaySets.Add(singleFrameDisplaySet);

				foreach (ImageSop imageSop in multiFrames)
				{
					IDisplaySet multiFrameDisplaySet = CreateMultiFrameDisplaySet(imageSop);
					if (multiFrameDisplaySet != null)
						displaySets.Add(multiFrameDisplaySet);
				}
			}

			return displaySets;
		}

		private IDisplaySet CreateMultiFrameDisplaySet(ImageSop multiFrame)
		{
			DisplaySet displaySet = null;
			List<IPresentationImage> images = PresentationImageFactory.CreateImages(multiFrame);

			if (images.Count > 0)
			{
				displaySet = base.CreateDisplaySet(multiFrame.ParentSeries);
				displaySet.Uid = multiFrame.SopInstanceUid;

				string suffix = String.Format(SR.SuffixFormatMultiframeDisplaySet, multiFrame.InstanceNumber);

				//NOTE: purposely use description here b/c the name is never empty
				if (String.IsNullOrEmpty(displaySet.Description))
					displaySet.Name += suffix;
				else
					displaySet.Name += " - " + suffix;

				if (String.IsNullOrEmpty(displaySet.Description))
					displaySet.Description += suffix;
				else
					displaySet.Description += " - " + suffix;

				foreach (IPresentationImage image in images)
					displaySet.PresentationImages.Add(image);
			}

			return displaySet;
		}

		private IDisplaySet CreateSingleFramesDisplaySet(List<ImageSop> singleFrames)
		{
			DisplaySet displaySet = null;

			List<IPresentationImage> images = new List<IPresentationImage>();
			foreach (ImageSop singleFrame in singleFrames)
				images.AddRange(PresentationImageFactory.CreateImages(singleFrame));

			if (images.Count > 0)
			{
				displaySet = base.CreateDisplaySet(singleFrames[0].ParentSeries);
				displaySet.Uid += ":SingleFrames";

				//NOTE: purposely use description here b/c the name is never empty
				if (String.IsNullOrEmpty(displaySet.Description))
					displaySet.Name += SR.SuffixSingleFramesDisplaySet;
				else
					displaySet.Name += " - " + SR.SuffixSingleFramesDisplaySet;

				if (String.IsNullOrEmpty(displaySet.Description))
					displaySet.Description += SR.SuffixSingleFramesDisplaySet;
				else
					displaySet.Description += " - " + SR.SuffixSingleFramesDisplaySet;

				foreach (IPresentationImage image in images)
					displaySet.PresentationImages.Add(image);
			}

			return displaySet;
		}
	}
	
	#endregion
}