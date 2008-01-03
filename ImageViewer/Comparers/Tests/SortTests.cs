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

#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;
using ClearCanvas.ImageViewer.Tests;
using NUnit.Framework;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Comparers.Tests
{
	[TestFixture]
	public class SortTests
	{
		[TestFixtureSetUp]
		public void Init()
		{
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{ 
		}

		[Test]
		public void TestSortingDicomImagesByInstanceNumber()
		{
			TestSortingDicomImagesByInstanceNumber(false);
		}

		[Test]
		public void TestSortingDicomImagesByInstanceNumberReverse()
		{
			TestSortingDicomImagesByInstanceNumber(true);
		}

		[Test]
		public void TestSortingDisplaySetsBySeriesNumber()
		{
			TestSortingDisplaySetsBySeriesNumber(false);
		}

		[Test]
		public void TestSortingDisplaySetsBySeriesNumberReverse()
		{
			TestSortingDisplaySetsBySeriesNumber(true);
		}

		[Test]
		public void TestSortingImageSetsByStudyDate()
		{
			TestSortingImageSetsByStudyDate(false);
		}

		[Test]
		public void TestSortingImageSetsByStudyDateReverse()
		{
			TestSortingImageSetsByStudyDate(true);
		}

		void TestSortingImageSetsByStudyDate(bool reverse)
		{
			ImageSetCollection orderedCollection = new ImageSetCollection();
			ImageSetCollection nonOrderedCollection = new ImageSetCollection();

			for (int i = 0; i <= 20; ++i)
			{
				string id = i.ToString();
				ImageSet imageSet = new ImageSet();
				imageSet.Name = id;

				DisplaySet displaySet = new DisplaySet(id, id);
				IPresentationImage image = new DicomGrayscalePresentationImage(NewMockImageSop(id, id, i));
				IImageSopProvider sopProvider = (IImageSopProvider)image;
				((IMockImageSopSetters)sopProvider.ImageSop).StudyDate = i == 0 ? "" : String.Format("200801{0}", i.ToString("00"));

				imageSet.DisplaySets.Add(displaySet);
				displaySet.PresentationImages.Add(image);
				orderedCollection.Add(imageSet);
			}

			Randomize(orderedCollection, nonOrderedCollection);

			Debug.WriteLine("Before Sort\n------------------------\n");
			CollectionUtils.ForEach(nonOrderedCollection, delegate(IImageSet imageSet)
			                                              	{
																Debug.WriteLine(String.Format("name: {0}, date: {1}", imageSet.Name, 
																	((IImageSopProvider)(imageSet.DisplaySets[0].PresentationImages[0])).ImageSop.StudyDate));
			                                              	});

			nonOrderedCollection.Sort(new StudyDateComparer(reverse));

			Debug.WriteLine("\nAfter Sort\n------------------------\n");
			CollectionUtils.ForEach(nonOrderedCollection, delegate(IImageSet imageSet)
															{
																Debug.WriteLine(String.Format("name: {0}, date: {1}", imageSet.Name,
																	((IImageSopProvider)(imageSet.DisplaySets[0].PresentationImages[0])).ImageSop.StudyDate));
															});

			if (reverse)nonOrderedCollection.RemoveAt(20);
			else nonOrderedCollection.RemoveAt(0);

			int j = reverse ? 20 : 1;
			foreach (IImageSet set in nonOrderedCollection)
			{
				Assert.AreEqual(j.ToString(), set.Name);
				j += reverse ? -1 : 1;
			}
		}

		void TestSortingDisplaySetsBySeriesNumber(bool reverse)
		{
			DisplaySetCollection orderedCollection = new DisplaySetCollection();
			DisplaySetCollection nonOrderedCollection = new DisplaySetCollection();

			for (int i = 1; i <= 20; ++i)
			{
				string id = i.ToString();
				DisplaySet displaySet = new DisplaySet(id, id);
				IPresentationImage image = new DicomGrayscalePresentationImage(NewMockImageSop(id, id, i));
				IImageSopProvider sopProvider = (IImageSopProvider)image;
				((IMockImageSopSetters)sopProvider.ImageSop).SeriesNumber = i;

				displaySet.PresentationImages.Add(image);
				orderedCollection.Add(displaySet);
			}

			Randomize(orderedCollection, nonOrderedCollection);

			Debug.WriteLine("Before Sort\n------------------------\n");
			CollectionUtils.ForEach(nonOrderedCollection, delegate(IDisplaySet displaySet) { Debug.WriteLine(String.Format("name: {0}", displaySet.Name)); });

			nonOrderedCollection.Sort(new SeriesNumberComparer(reverse));

			Debug.WriteLine("\nAfter Sort\n------------------------\n");
			CollectionUtils.ForEach(nonOrderedCollection, delegate(IDisplaySet displaySet) { Debug.WriteLine(String.Format("name: {0}", displaySet.Name)); });

			int j = reverse ? 20 : 1;
			foreach (IDisplaySet set in nonOrderedCollection)
			{
				Assert.AreEqual(j.ToString(), set.Name);
				j += reverse ? -1 : 1;
			}
		}

		void TestSortingDicomImagesByInstanceNumber(bool reverse)
		{ 
			PresentationImageCollection orderedCollection = new PresentationImageCollection();
			PresentationImageCollection nonOrderedCollection = new PresentationImageCollection();

			MockImageSop junkImageSop = NewMockImageSop("123", "1", 0);
			orderedCollection.Add(new DicomGrayscalePresentationImage(junkImageSop));

			AppendCollection(NewDicomSeries("123", "1", 1, 25), orderedCollection);
			
			AppendCollection(NewDicomSeries("123", "10", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("123", "111", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("123", "456", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("123", "789", 1, 25), orderedCollection);

			//Note that the seriesUID are *not* in numerical order.  This is because
			//it is a string comparison that is being done.
			AppendCollection(NewDicomSeries("a", "1", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("a", "11", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("a", "12", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("a", "6", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("a", "7", 1, 25), orderedCollection);

			AppendCollection(NewDicomSeries("b", "20", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("b", "21", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("b", "33", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("b", "34", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("b", "40", 1, 25), orderedCollection);

			//just put one of these at the end, it's enough.  We just want to see
			// that non-Dicom images get pushed to one end (depending on forward/reverse).
			orderedCollection.Add(new MockPresentationImage());

			if (reverse)
			{
				PresentationImageCollection reversedCollection = new PresentationImageCollection();
				for (int i = orderedCollection.Count - 1; i >= 0; --i)
					reversedCollection.Add(orderedCollection[i]);

				orderedCollection = reversedCollection;
			}

			Randomize(orderedCollection, nonOrderedCollection);

			Debug.WriteLine("NON-ORDERED COLLECTION (PRE-SORT)\n");
			Trace(nonOrderedCollection);
			
			//Be certain it is currently *not* in order.
			Assert.IsFalse(VerifyOrdered(orderedCollection, nonOrderedCollection));

			//Sort it.
			nonOrderedCollection.Sort(new InstanceNumberComparer(reverse));
			
			Debug.WriteLine("NON-ORDERED COLLECTION (POST-SORT)");
			Trace(nonOrderedCollection);

			//It should now be in the proper order.
			Assert.IsTrue(VerifyOrdered(orderedCollection, nonOrderedCollection));
		}

		void Trace(IEnumerable<IPresentationImage> collection)
		{
			foreach (IPresentationImage image in collection)
			{
				if (image is DicomGrayscalePresentationImage)
				{
					DicomGrayscalePresentationImage dicomImage = (DicomGrayscalePresentationImage)image;
					string line = string.Format("StudyUID: {0}, Series: {1}, Instance: {2}", dicomImage.ImageSop.StudyInstanceUID,
																			dicomImage.ImageSop.SeriesInstanceUID,
																			dicomImage.ImageSop.InstanceNumber);
					Debug.WriteLine(line);
				}
				else
				{
					Debug.WriteLine("** Non-Dicom Image **");
				}
			}
		}

		void Randomize<T>(ICollection<T> orderedCollection, ICollection<T> nonOrderedCollection)
		{ 
			ArrayList tempCollection = new ArrayList();
			foreach (T obj in orderedCollection)
				tempCollection.Add(obj);

			Random random = new Random();
			while (tempCollection.Count > 0)
			{
				int index = random.Next(tempCollection.Count);
				T obj = (T)tempCollection[index];
				nonOrderedCollection.Add(obj);
				tempCollection.Remove(obj);
			}
		}

		bool VerifyOrdered(
			PresentationImageCollection orderedCollection,
			PresentationImageCollection nonOrderedCollection)
		{
			Assert.AreEqual(orderedCollection.Count, nonOrderedCollection.Count);

			int index = 0;
			foreach (PresentationImage orderedImage in orderedCollection)
			{
				IPresentationImage nonOrderedImage = nonOrderedCollection[index];

				if (!(orderedImage is DicomGrayscalePresentationImage) && !(nonOrderedImage is DicomGrayscalePresentationImage))
				{
					++index;
					continue;
				}

				if (!(orderedImage is DicomGrayscalePresentationImage) && (nonOrderedImage is DicomGrayscalePresentationImage))
					return false;

				if ((orderedImage is DicomGrayscalePresentationImage) && !(nonOrderedImage is DicomGrayscalePresentationImage))
					return false;

				DicomGrayscalePresentationImage dicomOrdered = orderedImage as DicomGrayscalePresentationImage;
				DicomGrayscalePresentationImage dicomNonOrdered = nonOrderedImage as DicomGrayscalePresentationImage;

				if (dicomOrdered.ImageSop.StudyInstanceUID != dicomNonOrdered.ImageSop.StudyInstanceUID ||
					dicomOrdered.ImageSop.SeriesInstanceUID != dicomNonOrdered.ImageSop.SeriesInstanceUID ||
					dicomOrdered.ImageSop.InstanceNumber != dicomNonOrdered.ImageSop.InstanceNumber)
					return false;

				++index;
			}

			return true;
		}

		void AppendCollection(List<PresentationImage> listImages, PresentationImageCollection collection)
		{
			foreach (PresentationImage image in listImages)
				collection.Add(image);
		}

		List<PresentationImage> NewDicomSeries(string studyUID, string seriesUID, int startInstanceNumber, uint numberInstances)
		{
			List<PresentationImage> listImages = new List<PresentationImage>();
			int instanceNumber = (int)startInstanceNumber;
			while (instanceNumber < (startInstanceNumber + numberInstances))
			{
				listImages.Add(NewDicomImage(studyUID, seriesUID, instanceNumber));
				++instanceNumber;
			}

			return listImages;
		}

		DicomGrayscalePresentationImage NewDicomImage(string studyUID, string seriesUID, int instanceNumber)
		{
			return new DicomGrayscalePresentationImage(NewMockImageSop(studyUID, seriesUID, instanceNumber));
		}

		MockImageSop NewMockImageSop(string studyUID, string seriesUID, int instanceNumber)
		{
			MockImageSop newImageSop = new MockImageSop();
			IMockImageSopSetters setters = (IMockImageSopSetters)newImageSop;

			setters.StudyInstanceUid = studyUID;
			setters.SeriesInstanceUid = seriesUID;
			setters.InstanceNumber = instanceNumber;

			return newImageSop;
		}
	}
}

#endif