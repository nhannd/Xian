#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Network.Scu
{
	public partial class PrintScu
	{
		public class FilmSession : BasicFilmSessionModuleIod
		{
			private FilmBox _currentFilmBox;

			internal DicomUid SopInstanceUid { get; set; }
			internal PrintScu PrintScu { get; set; }

			public List<FilmBox> FilmBoxes { get; set; }

			public FilmSession()
			{
				this.FilmBoxes = new List<FilmBox>();
			}

			internal void OnCreated(DicomUid filmSessionUid)
			{
				this.SopInstanceUid = filmSessionUid;
				_currentFilmBox = this.FilmBoxes[0];
				this.PrintScu.CreateFilmBox(this, _currentFilmBox);
			}

			internal void OnFilmBoxCreated(DicomUid filmBoxUid, List<DicomUid> imageBoxUids)
			{
				_currentFilmBox.SopInstanceUid = filmBoxUid;
				for (var i = 0; i < _currentFilmBox.ImageBoxes.Count; i++)
					_currentFilmBox.ImageBoxes[i].SopInstanceUid = imageBoxUids[i];

				// start setting the first imageBox
				var firstImageBox = _currentFilmBox.ImageBoxes[0];
				firstImageBox.OnSet(this.PrintScu.ColorPrinting);
				this.PrintScu.SetImageBox(firstImageBox);
			}

			internal void OnImageBoxSet(DicomUid imageBoxUid)
			{
				var currentImageBox = CollectionUtils.SelectFirst(_currentFilmBox.ImageBoxes, ib => Equals(ib.SopInstanceUid, imageBoxUid));
				var nextImageBoxIndex = _currentFilmBox.ImageBoxes.IndexOf(currentImageBox) + 1;

				if (nextImageBoxIndex == _currentFilmBox.ImageBoxes.Count)
				{
					// No more imageBox to set.  Print the filmBox.
					this.PrintScu.PrintFilmBox(_currentFilmBox);
				}
				else
				{
					var nextImageBox = _currentFilmBox.ImageBoxes[nextImageBoxIndex];
					nextImageBox.OnSet(this.PrintScu.ColorPrinting);
					this.PrintScu.SetImageBox(nextImageBox);
				}
			}

			internal void OnFilmBoxPrinted(DicomUid filmBoxUid)
			{
				this.PrintScu.DeleteFilmBox(_currentFilmBox);
			}

			internal void OnFilmBoxDeleted()
			{
				_currentFilmBox.SopInstanceUid = null;
				var nextFilmBoxIndex = this.FilmBoxes.IndexOf(_currentFilmBox) + 1;
				if (nextFilmBoxIndex == this.FilmBoxes.Count)
				{
					this.PrintScu.DeleteFilmSession(this);
				}
				else
				{
					// Create the next filmBox
					_currentFilmBox = this.FilmBoxes[nextFilmBoxIndex];
					this.PrintScu.CreateFilmBox(this, _currentFilmBox);
				}
			}

			internal void OnDeleted()
			{
				this.SopInstanceUid = null;
				this.PrintScu = null;
			}
		}

		public class FilmBox : BasicFilmBoxModuleIod
		{
			internal DicomUid SopInstanceUid { get; set; }

			public List<ImageBox> ImageBoxes { get; set; }

			public FilmBox()
			{
				this.ImageBoxes = new List<ImageBox>();
			}
		}

		public class ImageBox : ImageBoxPixelModuleIod
		{
			internal DicomUid SopInstanceUid;

			/// <summary>
			/// Delegate for getting pixel data.
			/// </summary>
			public delegate byte[] PixelDataDelegate(Size imageBoxSSize, out Size pixelSize, bool isPrintingColor);

			protected readonly Size _imageBoxSize;
			protected readonly PixelDataDelegate _pixelDataGetter;

			public ImageBox(ushort imageBoxPosition, Size imageBoxSize, PixelDataDelegate pixelDataGetter)
			{
				this.ImageBoxPosition = imageBoxPosition;
				_imageBoxSize = imageBoxSize;
				_pixelDataGetter = pixelDataGetter;
			}

			public void OnSet(bool isPrintingColor)
			{
				if (isPrintingColor)
				{
					var image = new BasicColorImageSequenceIod
						{
							SamplesPerPixel = 3,
							PhotometricInterpretation = PhotometricInterpretation.Rgb,
							PixelRepresentation = 0,
							PixelAspectRatio = new PixelAspectRatio(1, 1),
							PlanarConfiguration = 1,
							BitsStored = 8,
							BitsAllocated = 8,
							HighBit = 7
						};

					Size pixelSize;
					image.PixelData = _pixelDataGetter.Invoke(_imageBoxSize, out pixelSize, true);
					image.Rows = (ushort) pixelSize.Height;
					image.Columns = (ushort) pixelSize.Width;

					this.BasicColorImageSequenceList.Add(image);
				}
				else
				{
					var image = new BasicGrayscaleImageSequenceIod
						{
							SamplesPerPixel = 1,
							PhotometricInterpretation = PhotometricInterpretation.Monochrome2,
							PixelRepresentation = 0,
							PixelAspectRatio = new PixelAspectRatio(1, 1),
							BitsStored = 8,
							BitsAllocated = 8,
							HighBit = 7
						};

					Size pixelSize;
					image.PixelData = _pixelDataGetter.Invoke(_imageBoxSize, out pixelSize, false);
					image.Rows = (ushort)pixelSize.Height;
					image.Columns = (ushort)pixelSize.Width;

					this.BasicGrayscaleImageSequenceList.Add(image);
				}
			}
		}
	}
}
