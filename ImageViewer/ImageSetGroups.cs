using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer
{
	#region Basic Specifications

	public class PatientIdSpecification : ImageSetSpecification
	{
		private readonly IImageSet _referenceImageSet;

		public PatientIdSpecification(IImageSet referenceImageSet)
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

	public class ContainsModalitySpecification : ImageSetSpecification
	{
		private readonly string _modality;

		public ContainsModalitySpecification(string modality)
		{
			Platform.CheckForEmptyString(modality, "modality");
			_modality = modality;
		}

		public override TestResult Test(IImageSet imageSet)
		{
			//TODO: this doesn't actually test 'contains'.
			IImageSopProvider provider = GetFirstImage<IImageSopProvider>(imageSet);
			if (provider != null && provider.ImageSop.Modality == _modality)
				return new TestResult(true);
			else
				return new TestResult(false);
		}
	}

	public class StudyDateSpecification : ImageSetSpecification
	{
		private readonly Predicate<DateTime> _innerTest;

		private StudyDateSpecification(Predicate<DateTime> innerTest)
		{
			Platform.CheckForNullReference(innerTest, "innerTest");
			_innerTest = innerTest;
		}

		public override TestResult Test(IImageSet imageSet)
		{
			IImageSopProvider provider = GetFirstImage<IImageSopProvider>(imageSet);
			if (provider != null)
			{
				DateTime? studyDate = DateParser.Parse(provider.ImageSop.StudyDate);
				if (studyDate != null && _innerTest(studyDate.Value))
					return new TestResult(true);
			}
		
			return new TestResult(false);
		}

		public static StudyDateSpecification CreateBeforeSpecification(DateTime date)
		{
			return new StudyDateSpecification(delegate(DateTime dateTime) { return dateTime < date; });
		}
		
		public static StudyDateSpecification CreateAfterSpecification(DateTime date)
		{
			return new StudyDateSpecification(delegate(DateTime dateTime) { return dateTime > date; });
		}

		public static StudyDateSpecification CreateBetweenSpecification(DateTime begin, DateTime end)
		{
			return new StudyDateSpecification(delegate(DateTime dateTime) { return dateTime < end && dateTime > begin; });
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

	#endregion

	#region Specialized Groups

	public class PatientImageSetGroup : FilteredGroup<IImageSet>
	{
		internal PatientImageSetGroup(IImageSet sourceImageSet)
			: base("Patient", sourceImageSet.PatientInfo, new PatientIdSpecification(sourceImageSet))
		{
		}
	}

	public class PatientsImageSetGroup : FilteredGroups<IImageSet>
	{
		public PatientsImageSetGroup()
		{
		}

		protected override bool AddItemToChildren(IImageSet imageSet)
		{
			if (!base.AddItemToChildren(imageSet))
			{
				PatientImageSetGroup newGroup = new PatientImageSetGroup(imageSet);
				ChildGroups.Add(newGroup);
				if (!AddItemToChild(imageSet, newGroup))
					Debug.Assert(false, "Item should be guaranteed to have been inserted.");
			}

			return true;
		}

		protected override void RemoveItem(IImageSet imageSet)
		{
			List<FilteredGroup<IImageSet>> allGroups = new List<FilteredGroup<IImageSet>>(ChildGroups);
			foreach (FilteredGroup<IImageSet> group in allGroups)
			{
				if (group.Items.Count == 0)
					base.ChildGroups.Remove(group);
			}
		}
	}

	#endregion

	#region Filtered Image Set Groups

	public class ImageSetGroups
	{
		private readonly PatientsImageSetGroup _root;
		private ImageSetCollection _sourceImageSetCollection;

		public ImageSetGroups()
		{
			_root = new PatientsImageSetGroup();
		}

		public ImageSetGroups(ImageSetCollection sourceImageSets)
			: this()
		{
			SetSourceCollection(sourceImageSets);
		}

		public PatientsImageSetGroup Root
		{
			get { return _root; }	
		}

		public void SetSourceCollection(ImageSetCollection sourceImageSets)
		{
			if (_sourceImageSetCollection == sourceImageSets)
				return;

			if (_sourceImageSetCollection != null)
			{
				_sourceImageSetCollection.ItemAdded -= OnImageSetAdded;
				_sourceImageSetCollection.ItemChanging -= OnImageSetChanging;
				_sourceImageSetCollection.ItemChanged -= OnImageSetChanged;
				_sourceImageSetCollection.ItemRemoved -= OnImageSetRemoved;

				_root.Clear();
			}

			_sourceImageSetCollection = sourceImageSets;
			if (_sourceImageSetCollection != null)
			{
				_root.Add(_sourceImageSetCollection);

				_sourceImageSetCollection.ItemAdded += OnImageSetAdded;
				_sourceImageSetCollection.ItemChanging += OnImageSetChanging;
				_sourceImageSetCollection.ItemChanged += OnImageSetChanged;
				_sourceImageSetCollection.ItemRemoved += OnImageSetRemoved;
			}
		}

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

	#endregion
}
