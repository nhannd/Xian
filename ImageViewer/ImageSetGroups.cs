using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer
{
	internal class PatientInfoSpecification : ImageSetSpecification
	{
		private readonly IImageSet _referenceImageSet;

		public PatientInfoSpecification(IImageSet referenceImageSet)
		{
			_referenceImageSet = referenceImageSet;
		}

		public override TestResult Test(IImageSet imageSet)
		{
			if (imageSet.PatientInfo == _referenceImageSet.PatientInfo)
				return new TestResult(true);
			else
				return new TestResult(false);
		}
	}

	public abstract class ImageSetSpecification : ISpecification
	{
		public ImageSetSpecification()
		{
		}

		#region ISpecification Members

		public TestResult Test(object obj)
		{
			if (obj is IImageSet)
				return Test(obj as IImageSet);
			else 
				return new TestResult(false);
		}

		#endregion

		protected static IEnumerable<T> GetImages<T>(IImageSet imageSet) where T : class
		{
			foreach (IPresentationImage image in GetImages(imageSet))
			{
				T provider = image as T;
				if (provider != null)
					yield return provider;
			}
		}

		protected static IEnumerable<IPresentationImage> GetImages(IImageSet imageSet)
		{
			foreach (IDisplaySet displaySet in imageSet.DisplaySets)
			{
				foreach (IPresentationImage image in displaySet.PresentationImages)
				{
					yield return image;
				}
			}
		}

		protected static IPresentationImage GetFirstImage(IImageSet imageSet)
		{
			foreach (IPresentationImage image in GetImages(imageSet))
				return image;

			return null;
		}

		protected static T GetFirstImage<T>(IImageSet imageSet) where T : class
		{
			return GetFirstImage<T>(imageSet, delegate { return true; });
		}

		protected static T GetFirstImage<T>(IImageSet imageSet, Predicate<T> test) where T : class
		{
			foreach (IPresentationImage image in GetImageAsProvider<T>(imageSet))
			{
				T provider = image as T;
				if (provider != null && test(provider))
					return image as T;
			}

			return null;
		}

		protected static IEnumerable<T> GetImageAsProvider<T>(IImageSet imageSet) where T : class
		{
			foreach (IPresentationImage image in GetImages(imageSet))
			{
				if (image is T)
					yield return image as T;
			}
		}

		public abstract TestResult Test(IImageSet imageSet);
	}

	public class PatientImageSetGroup : FilteredGroup<IImageSet>
	{
		internal PatientImageSetGroup(IImageSet sourceImageSet)
			: base("Patient", sourceImageSet.PatientInfo, new PatientInfoSpecification(sourceImageSet))
		{
		}
	}

	public class SimplePatientImageSetGroupFactory : IFilteredGroupFactory<IImageSet>
	{
		public SimplePatientImageSetGroupFactory()
		{
		}

		#region IFilteredGroupFactory<IImageSet> Members

		public FilteredGroup<IImageSet> Create(IImageSet item)
		{
			return new PatientImageSetGroup(item);
		}

		#endregion
	}

	public class ImageSetGroups : IDisposable
	{
		private readonly FilteredGroups<IImageSet> _root;
		private ObservableList<IImageSet> _sourceImageSets;

		public ImageSetGroups()
		{
			_root = new FilteredGroups<IImageSet>("Root", "All Patients", new SimplePatientImageSetGroupFactory());
		}

		public ImageSetGroups(ObservableList<IImageSet> sourceImageSets)
			: this()
		{
			SourceImageSets = sourceImageSets;
		}

		public FilteredGroups<IImageSet> Root
		{
			get { return _root; }	
		}

		public ObservableList<IImageSet> SourceImageSets
		{
			set
			{
				if (_sourceImageSets == value)
					return;

				if (_sourceImageSets != null)
				{
					_sourceImageSets.ItemAdded -= OnImageSetAdded;
					_sourceImageSets.ItemChanging -= OnImageSetChanging;
					_sourceImageSets.ItemChanged -= OnImageSetChanged;
					_sourceImageSets.ItemRemoved -= OnImageSetRemoved;

					_root.Clear();
				}

				_sourceImageSets = value;
				if (_sourceImageSets != null)
				{
					_root.Add(_sourceImageSets);

					_sourceImageSets.ItemAdded += OnImageSetAdded;
					_sourceImageSets.ItemChanging += OnImageSetChanging;
					_sourceImageSets.ItemChanged += OnImageSetChanged;
					_sourceImageSets.ItemRemoved += OnImageSetRemoved;
				}
			}
			get
			{
				return _sourceImageSets;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				SourceImageSets = null;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "An unexpected error has occurred.");
			}
		}

		#endregion

		private void OnImageSetAdded(object sender, ListEventArgs<IImageSet> e)
		{
			_root.Add(e.Item);
		}
		private void OnImageSetChanging(object sender, ListEventArgs<IImageSet> e)
		{
			_root.Remove(e.Item);
		}
		private void OnImageSetChanged(object sender, ListEventArgs<IImageSet> e)
		{
			_root.Add(e.Item);
		}
		private void OnImageSetRemoved(object sender, ListEventArgs<IImageSet> e)
		{
			_root.Remove(e.Item);
		}
	}
}