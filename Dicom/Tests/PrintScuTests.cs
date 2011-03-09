#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using NUnit.Framework;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	public class PrintScuTests
	{
		[Test]
		public void FilmBox_FilmDPI_Test()
		{
			var filmBox = new PrintScu.FilmBox(300, 600);
			Assert.AreEqual(filmBox.RequestedResolutionId, RequestedResolution.None);

			// Default DPI is used when RequestedResolution.None.
			Assert.AreEqual(filmBox.FilmDPI, 300);

			filmBox.RequestedResolutionId = RequestedResolution.High;
			Assert.AreEqual(filmBox.FilmDPI, 600);

			filmBox.RequestedResolutionId = RequestedResolution.Standard;
			Assert.AreEqual(filmBox.FilmDPI, 300);
		}

		[Test]
		public void FilmBox_SizeInPixels_Test()
		{
			var filmBox = new PrintScu.FilmBox(300, 600);
			Assert.IsTrue(filmBox.SizeInPixels.IsEmpty);

			// Portrait orientation is used, even if FilmOrientation.None.
			filmBox.FilmSizeId = FilmSize.Dimension_8in_x_10in;
			Assert.AreEqual(filmBox.FilmOrientation, FilmOrientation.None);
			Assert.AreEqual(filmBox.SizeInPixels.Width, 8 * 300);
			Assert.AreEqual(filmBox.SizeInPixels.Height, 10 * 300);

			// Test Portrait orientation
			filmBox.FilmOrientation = FilmOrientation.Portrait;
			Assert.AreEqual(filmBox.FilmOrientation, FilmOrientation.Portrait);
			Assert.AreEqual(filmBox.SizeInPixels.Width, 8 * 300);
			Assert.AreEqual(filmBox.SizeInPixels.Height, 10 * 300);

			// Test Landscape orientation
			filmBox.FilmOrientation = FilmOrientation.Landscape;
			Assert.AreEqual(filmBox.FilmOrientation, FilmOrientation.Landscape);
			Assert.AreEqual(filmBox.SizeInPixels.Width, 10 * 300);
			Assert.AreEqual(filmBox.SizeInPixels.Height, 8 * 300);
		}

		[Test]
		public void ImageBox_SizeInPixel_Standard_Format_Test()
		{
			var filmBox = new PrintScu.FilmBox(300, 600)
				{
					FilmOrientation = FilmOrientation.Portrait,
					FilmSizeId = FilmSize.Dimension_8in_x_10in,
					ImageDisplayFormat = ImageDisplayFormat.Standard_2x4
				};

			var imageBox = new PrintScu.ImageBox(filmBox, null);
			var filmBoxSize = filmBox.SizeInPixels;

			// Layout is 2x4, meaning 2 columns, 4 rows.
			// ImageBoxes are ordered top->bottom, left->right
			imageBox.ImageBoxPosition = 1;
			var imageBoxSize = imageBox.EstimatedSizeInPixel;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 4);

			imageBox.ImageBoxPosition = 8;
			imageBoxSize = imageBox.EstimatedSizeInPixel;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 4);

			// Now flip it around to landscape orientation
			filmBox.FilmOrientation = FilmOrientation.Landscape;
			filmBoxSize = filmBox.SizeInPixels;

			imageBox.ImageBoxPosition = 1;
			imageBoxSize = imageBox.EstimatedSizeInPixel;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 4);

			imageBox.ImageBoxPosition = 8;
			imageBoxSize = imageBox.EstimatedSizeInPixel;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 4);
		}

		[Test]
		public void ImageBox_SizeInPixel_Row_Format_Test()
		{
			var filmBox = new PrintScu.FilmBox(300, 600)
				{
					FilmOrientation = FilmOrientation.Portrait,
					FilmSizeId = FilmSize.Dimension_8in_x_10in,
					ImageDisplayFormat = ImageDisplayFormat.Row_1_2
				};

			var imageBox = new PrintScu.ImageBox(filmBox, null);
			var filmBoxSize = filmBox.SizeInPixels;

			// Layout is Row 1,2, meaning 1 column in top row and 2 columns in bottom row
			imageBox.ImageBoxPosition = 1;
			var imageBoxSize = imageBox.EstimatedSizeInPixel;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);

			imageBox.ImageBoxPosition = 2;
			imageBoxSize = imageBox.EstimatedSizeInPixel;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);

			imageBox.ImageBoxPosition = 3;
			imageBoxSize = imageBox.EstimatedSizeInPixel;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);

			// Now flip it around to landscape orientation
			filmBox.FilmOrientation = FilmOrientation.Landscape;
			filmBoxSize = filmBox.SizeInPixels;

			imageBox.ImageBoxPosition = 1;
			imageBoxSize = imageBox.EstimatedSizeInPixel;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);

			imageBox.ImageBoxPosition = 2;
			imageBoxSize = imageBox.EstimatedSizeInPixel;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);

			imageBox.ImageBoxPosition = 3;
			imageBoxSize = imageBox.EstimatedSizeInPixel;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);
		}

		[Test]
		public void ImageBox_SizeInPixel_Column_Format_Test()
		{
			var filmBox = new PrintScu.FilmBox(300, 600)
				{
					FilmOrientation = FilmOrientation.Portrait,
					FilmSizeId = FilmSize.Dimension_8in_x_10in,
					ImageDisplayFormat = ImageDisplayFormat.COL_1_2
				};

			var imageBox = new PrintScu.ImageBox(filmBox, null);
			var filmBoxSize = filmBox.SizeInPixels;

			// Layout is Column 1,2, meaning 1 row on the left column and 2 rows in the right column
			imageBox.ImageBoxPosition = 1;
			var imageBoxSize = imageBox.EstimatedSizeInPixel;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height);

			imageBox.ImageBoxPosition = 2;
			imageBoxSize = imageBox.EstimatedSizeInPixel;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);

			imageBox.ImageBoxPosition = 3;
			imageBoxSize = imageBox.EstimatedSizeInPixel;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);

			// Now flip it around to landscape orientation
			filmBox.FilmOrientation = FilmOrientation.Landscape;
			filmBoxSize = filmBox.SizeInPixels;

			imageBox.ImageBoxPosition = 1;
			imageBoxSize = imageBox.EstimatedSizeInPixel;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height);

			imageBox.ImageBoxPosition = 2;
			imageBoxSize = imageBox.EstimatedSizeInPixel;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);

			imageBox.ImageBoxPosition = 3;
			imageBoxSize = imageBox.EstimatedSizeInPixel;
			Assert.AreEqual(imageBoxSize.Width, filmBoxSize.Width / 2);
			Assert.AreEqual(imageBoxSize.Height, filmBoxSize.Height / 2);
		}
	}
}

#endif