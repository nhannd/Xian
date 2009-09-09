using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public interface IDisplaySetFactory
	{
		void SetStudyTree(StudyTree studyTree);

		List<IDisplaySet> CreateDisplaySets(Series series);
	}

	public class CompositeDisplaySetFactory : IDisplaySetFactory
	{
		private readonly List<IDisplaySetFactory> _factories;

		public CompositeDisplaySetFactory(IEnumerable<IDisplaySetFactory> factories)
		{
			_factories = new List<IDisplaySetFactory>(factories);
			Platform.CheckTrue(_factories.Count > 0, "_factories > 0");
		}

		#region IDisplaySetFactory Members

		public void SetStudyTree(StudyTree studyTree)
		{
			foreach (IDisplaySetFactory factory in _factories)
				factory.SetStudyTree(studyTree);
		}

		public List<IDisplaySet> CreateDisplaySets(Series series)
		{
			List<IDisplaySet> displaySets = new List<IDisplaySet>();
			foreach (IDisplaySetFactory factory in _factories)
				displaySets.AddRange(factory.CreateDisplaySets(series));

			return displaySets;
		}

		#endregion
	}

	public abstract class DisplaySetFactory : IDisplaySetFactory
	{
		private StudyTree _studyTree;
		private readonly IPresentationImageFactory _presentationImageFactory;

		protected DisplaySetFactory()
			: this(new PresentationImageFactory())
		{
		}

		protected DisplaySetFactory(IPresentationImageFactory presentationImageFactory)
		{
			Platform.CheckForNullReference(presentationImageFactory, "presentationImageFactory");
			_presentationImageFactory = presentationImageFactory;
		}

		protected StudyTree StudyTree
		{
			get { return _studyTree; }
		}

		protected IPresentationImageFactory PresentationImageFactory
		{
			get { return _presentationImageFactory; }	
		}

		#region IDisplaySetFactory Members

		public void SetStudyTree(StudyTree studyTree)
		{
			_studyTree = studyTree;
			PresentationImageFactory.SetStudyTree(_studyTree);
		}

		public abstract List<IDisplaySet> CreateDisplaySets(Series series);

		#endregion

		protected virtual IDisplaySet CreateSeriesDisplaySet(Series series)
		{
			IDisplaySet displaySet = null;
			List<IPresentationImage> images = new List<IPresentationImage>();
			foreach (Sop sop in series.Sops)
				images.AddRange(PresentationImageFactory.CreateImages(sop));

			if (images.Count > 0)
			{
				displaySet = CreateSeriesDisplaySet((ISeriesData)series);
				foreach (IPresentationImage image in images)
					displaySet.PresentationImages.Add(image);
			}

			return displaySet;
		}

		protected virtual List<IDisplaySet> CreateSingleImageDisplaySets(Series series)
		{
			List<IDisplaySet> displaySets = new List<IDisplaySet>();

			foreach (Sop sop in series.Sops)
			{
				if (sop.IsImage)
				{
					IDisplaySet displaySet = CreateSingleImageDisplaySet((ImageSop)sop);
					if (displaySet != null)
						displaySets.Add(displaySet);
				}
			}

			return displaySets;
		}

		protected virtual IDisplaySet CreateSingleImageDisplaySet(ImageSop imageSop)
		{
			List<IPresentationImage> images = PresentationImageFactory.CreateImages(imageSop);

			IDisplaySet displaySet = null;
			if (images.Count > 0)
			{
				displaySet = CreateSingleImageDisplaySet(imageSop.ParentSeries, imageSop);
				foreach (IPresentationImage image in images)
					displaySet.PresentationImages.Add(image);

			}

			return displaySet;
		}

		protected virtual DisplaySet CreateDisplaySet(ISeriesData series)
		{
			return CreateSeriesDisplaySet<DisplaySet>(series);
		}

		public static DisplaySet CreateSingleImageDisplaySet(ISeriesData series, ISopInstanceData sop)
		{
			return CreateSingleImageDisplaySet<DisplaySet>(series, sop);
		}

		public static DisplaySet CreateSingleImageDisplaySet<T>(ISeriesData series, ISopInstanceData sop) where T : DisplaySet, new()
		{
			DisplaySet displaySet = CreateSeriesDisplaySet<T>(series);
			displaySet.Uid = sop.SopInstanceUid;

			string suffix = String.Format(SR.SuffixFormatSingleImageDisplaySet, sop.InstanceNumber);

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

		public static DisplaySet CreateSeriesDisplaySet(ISeriesData series)
		{
			return CreateSeriesDisplaySet<DisplaySet>(series);
		}

		public static DisplaySet CreateSeriesDisplaySet<T>(ISeriesData series) where T : DisplaySet, new()
		{
			string name = String.Format("{0}: {1}", series.SeriesNumber, series.SeriesDescription);
			DisplaySet displaySet = new T();
			displaySet.Name = name;
			displaySet.Uid = series.SeriesInstanceUid;
			displaySet.Number = series.SeriesNumber;
			displaySet.Description = series.SeriesDescription;
			return displaySet;
		}
	}
}