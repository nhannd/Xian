#if	UNIT_TESTS

using System;
using System.Collections;
using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tests
{
/*
	public class DummyDicomImage : DicomImage
	{
		// Constructor
		public DummyDicomImage()
		{
			_rows = 512;
			_columns = 512;
			_bitsAllocated = 16;
			_bitsStored = 12;
			_highBit = 11;
			_samplesPerPixel = 1;
			_pixelRepresentation = 0;
			_planarConfiguration = 0;
			_photometricInterpretation = "MONOCHROME2";
			_pixelData = new byte[1];

			CalculateOtherImageParameters();
		}

		// Protected methods
		protected override void Load()
		{
		}

		protected override void Unload()
		{
		}
	}



	/// <summary>
	/// Summary description for DisplaySetTest.
	/// </summary>
	[TestFixture]
	public class DisplaySetTest
	{
		DisplaySet _selectedDisplaySet;
		DisplaySet _visibleDisplaySet;
		PresentationImage _presentationImage;
		DisplaySet _displaySet;
		DicomImage _dicomImage;

		public DisplaySetTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
			_displaySet = LogicalWorkspace.CreateDisplaySet();
			_dicomImage = new DummyDicomImage();
		}
		
		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void AddPresentationImages()
		{
			DisplaySet ds = new DisplaySet();
			ds.PresentationImages.ItemAdded += new EventHandler<PresentationImageEventArgs>(OnPresentationImageAdded);
			ds.PresentationImages.ItemRemoved += new EventHandler<PresentationImageEventArgs>(OnPresentationImageRemoved);

			_presentationImage = null;
			PresentationImage pi1 = new StandardImage(_dicomImage);
			ds.PresentationImages.Add(pi1);
			Assert.AreEqual(1, ds.PresentationImages.Count);
			// Verify add event was fired
			Assert.AreEqual(pi1, _presentationImage);

			PresentationImage pi2 = new StandardImage(_dicomImage);
			ds.PresentationImages.Add(pi2);
			Assert.AreEqual(2, ds.PresentationImages.Count);

			Assert.AreEqual(pi1, ds.PresentationImages[0]);
			Assert.AreEqual(pi2, ds.PresentationImages[1]);

			ds.PresentationImages.Remove(pi1);
			Assert.AreEqual(1, ds.PresentationImages.Count);

			_presentationImage = null;
			ds.PresentationImages.Remove(pi2);
			Assert.AreEqual(0, ds.PresentationImages.Count);
			// Verify add event was fired
			Assert.AreEqual(pi2, _presentationImage);
		}

		[Test]
		public void AddDuplicatePresentationImage()
		{
			DisplaySet ds = new DisplaySet();

			PresentationImage pi = new StandardImage(_dicomImage);
			ds.PresentationImages.Add(pi);
			Assert.AreEqual(1, ds.PresentationImages.Count);

			ds.PresentationImages.Add(pi);
			Assert.AreEqual(1, ds.PresentationImages.Count);
		}

		[Test]
		public void ForEachPresentationImage()
		{
			DisplaySet ds = new DisplaySet();

			PresentationImage pi1 = new StandardImage(_dicomImage);
			ds.PresentationImages.Add(pi1);

			PresentationImage pi2 = new StandardImage(_dicomImage);
			ds.PresentationImages.Add(pi2);

			foreach (PresentationImage pi in ds.PresentationImages)
				pi.ToString();
		}

		[Test]
		public void SelectPresentationImages()
		{
			_selectedDisplaySet = null;

			DisplaySet ds = new DisplaySet();
			//Assert.IsFalse(ds.Selected);

			//ds.SelectionChangedEvent += new EventHandler(OnSelectionChangedEvent);

			PresentationImage pi1 = new StandardImage(_dicomImage);
			ds.PresentationImages.Add(pi1);

			//pi1.SetSelected(true, SelectionType.Single);
			//Assert.IsTrue(ds.Selected);

			// Confirm that the selection event fired
			Assert.AreSame(ds, _selectedDisplaySet);
			//Assert.IsTrue(_selectedDisplaySet.Selected);

			// Selecting the presentation image also selects the display set that contains it
			PresentationImage[] piList = null; //ds.SelectedPresentationImages;
			Assert.AreEqual(1, piList.Length);
			Assert.AreEqual(pi1, piList[0]);

			PresentationImage pi2 = new StandardImage(_dicomImage);
			ds.PresentationImages.Add(pi2);

			//pi2.SetSelected(true, SelectionType.Single);
			piList = null; // ds.SelectedPresentationImages;
			Assert.AreEqual(1, piList.Length);
			Assert.AreEqual(pi2, piList[0]);
		}

		[Test]
		public void SetVisibility()
		{
			_visibleDisplaySet = null;

			DisplaySet ds = new DisplaySet();
			Assert.IsFalse(ds.Visible);

			//ds.VisibilityChangedEvent += new EventHandler(OnVisibilityChangedEvent);

			PresentationImage pi1 = new StandardImage(_dicomImage);
			ds.PresentationImages.Add(pi1);

			ds.Visible = true;
			Assert.AreSame(ds, _visibleDisplaySet);
			Assert.IsTrue(_visibleDisplaySet.Visible);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddNullPresentationImage()
		{
			DisplaySet ds = new DisplaySet();
			ds.PresentationImages.Add(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RemoveNullPresentationImage()
		{
			DisplaySet ds = new DisplaySet();
			ds.PresentationImages.Remove(null);
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooLow()
		{
			DisplaySet ds = new DisplaySet();
			Assert.AreEqual(0, ds.PresentationImages.Count);

			PresentationImage pi1 = new StandardImage(_dicomImage);
			ds.PresentationImages.Add(pi1);
			Assert.AreEqual(1, ds.PresentationImages.Count);

			PresentationImage pi2 = ds.PresentationImages[-1];
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeTooHigh()
		{
			DisplaySet ds = new DisplaySet();
			Assert.AreEqual(0, ds.PresentationImages.Count);

			PresentationImage pi1 = new StandardImage(_dicomImage);
			ds.PresentationImages.Add(pi1);
			Assert.AreEqual(1, ds.PresentationImages.Count);

			PresentationImage pi2 = ds.PresentationImages[1];
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IndexOutOfRangeEmptyWorkspace()
		{
			DisplaySet ds = new DisplaySet();
			Assert.AreEqual(0, ds.PresentationImages.Count);

			PresentationImage pi2 = ds.PresentationImages[0];
		}

		private void OnSelectionChangedEvent(object sender, EventArgs e)
		{
			_selectedDisplaySet = (DisplaySet) sender;
		}

		private void OnVisibilityChangedEvent(object sender, EventArgs e)
		{
			_visibleDisplaySet = (DisplaySet) sender;
		}

		private void OnPresentationImageAdded(object sender, PresentationImageEventArgs e)
		{
			_presentationImage = e.PresentationImage;
		}

		private void OnPresentationImageRemoved(object sender, PresentationImageEventArgs e)
		{
			_presentationImage = e.PresentationImage;
		}
	}*/
}

#endif