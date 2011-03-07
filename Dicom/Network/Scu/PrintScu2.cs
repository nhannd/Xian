#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Network.Scu
{
	//TODO (CR March 2011) - High: This is a decent candidate for a unit test
	public partial class PrintScu
	{
		/// <summary>
		/// Delegate for creating a filmBox.
		/// </summary>
		/// <returns></returns>
		public delegate FilmBox CreateFilmBoxDelegate();

		/// <summary>
		/// Delegate for getting pixel data for an imageBox.
		/// </summary>
		public delegate void GetPixelDataDelegate(ImageBox imageBox, ColorMode colorMode,
			out ushort rows, out ushort columns, out byte[] pixelData);

		public class PrintItem
		{
			public PrintItem(object printObject, GetPixelDataDelegate getPixelDataCallback)
			{
				this.PrintObject = printObject;
				this.GetPixelDataCallback = getPixelDataCallback;
			}

			public object PrintObject { get; private set; }

			public GetPixelDataDelegate GetPixelDataCallback { get; private set; }
		}

		public class FilmSession : BasicFilmSessionModuleIod
		{
			internal DicomUid SopInstanceUid { get; set; }
			internal PrintScu PrintScu { get; set; }

			private readonly CreateFilmBoxDelegate _createFilmBoxCallback;

			private readonly List<FilmBox> _filmBoxes;
			private FilmBox _currentFilmBox;

			private readonly List<PrintItem> _printItems;
			private List<PrintItem>.Enumerator _printItemEnumerator;

			public FilmSession(List<PrintItem> printItems, CreateFilmBoxDelegate createFilmBoxCallback)
			{
				_createFilmBoxCallback = createFilmBoxCallback;
				_filmBoxes = new List<FilmBox>();

				_printItems = printItems;
				_printItemEnumerator = _printItems.GetEnumerator();
				_printItemEnumerator.MoveNext();
			}

			public ReadOnlyCollection<FilmBox> FilmBoxes
			{
				get { return _filmBoxes.AsReadOnly(); }
			}

			internal void OnCreated(DicomUid filmSessionUid)
			{
				this.SopInstanceUid = filmSessionUid;

				// Move to the first element.
				_filmBoxes.Add(_currentFilmBox = _createFilmBoxCallback.Invoke());
				this.PrintScu.CreateFilmBox(this, _currentFilmBox);
			}

			internal void OnFilmBoxCreated(DicomUid filmBoxUid, List<DicomUid> imageBoxUids)
			{
				_currentFilmBox.SopInstanceUid = filmBoxUid;

				// The SCP returns a list of imageBoxUids.  Create an imageBox for each UID.
				var imageBoxes = new List<ImageBox>();
				for (var i = 0; i < imageBoxUids.Count; i++)
				{
					var imageBox = new ImageBox(_currentFilmBox, _printItemEnumerator.Current)
						{
							ImageBoxPosition = (ushort) (i+1),  // position is 1-based
							SopInstanceUid = SopInstanceUid = imageBoxUids[i]
						};
					imageBoxes.Add(imageBox);

					// No more print items.  Stop creating imageBoxes
					if (!_printItemEnumerator.MoveNext())
						break;
				}

				// start setting the first imageBox
				_currentFilmBox.SetImageBoxes(imageBoxes);
				var imageBoxToSet = _currentFilmBox.GetNextImageBox();
				imageBoxToSet.OnSet(this.PrintScu.ColorMode);
				this.PrintScu.SetImageBox(imageBoxToSet);
			}

			internal void OnImageBoxSet(DicomUid imageBoxUid)
			{
				var imageBoxToSet = _currentFilmBox.GetNextImageBox();
				if (imageBoxToSet == null)
				{
					// No more imageBox to set.  Print the filmBox.
					this.PrintScu.PrintFilmBox(_currentFilmBox);
				}
				else
				{
					imageBoxToSet.OnSet(this.PrintScu.ColorMode);
					this.PrintScu.SetImageBox(imageBoxToSet);
				}
			}

			internal void OnFilmBoxPrinted(DicomUid filmBoxUid)
			{
				this.PrintScu.DeleteFilmBox(_currentFilmBox);
			}

			internal void OnFilmBoxDeleted()
			{
				_currentFilmBox.SopInstanceUid = null;

				if (_printItemEnumerator.Current == null)
				{
					// No more items to create filmBox for.
					this.PrintScu.DeleteFilmSession(this);
				}
				else
				{
					// Create the next filmBox
					_filmBoxes.Add(_currentFilmBox = _createFilmBoxCallback.Invoke());
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

			private List<ImageBox> _imageBoxes;
			private List<ImageBox>.Enumerator _imageBoxEnumerator;

			private readonly int _standardResolutionDPI;
			private readonly int _highResolutionDPI;

			public FilmBox(int standardResolutionDPI, int highResolutionDPI)
			{
				_standardResolutionDPI = standardResolutionDPI;
				_highResolutionDPI = highResolutionDPI;
			}

			public ReadOnlyCollection<ImageBox> ImageBox
			{
				get { return _imageBoxes.AsReadOnly(); }
			}

			public int FilmDPI
			{
				get
				{
					return this.RequestedResolutionId == RequestedResolution.Standard 
						? _standardResolutionDPI
						: _highResolutionDPI;
				}
			}

			public Size SizeInPixels
			{
				get
				{
					var physicalWidthInInches = this.FilmSizeId.GetWidth(FilmSize.FilmSizeUnit.Inch);
					var physicalHeightInInches = this.FilmSizeId.GetHeight(FilmSize.FilmSizeUnit.Inch);

					var width = (int)Math.Ceiling(physicalWidthInInches * this.FilmDPI);
					var height = (int)Math.Ceiling(physicalHeightInInches * this.FilmDPI);

					return this.FilmOrientation == FilmOrientation.Portrait
						? new Size(width, height)
						: new Size(height, width);
				}
			}

			internal ImageBox GetNextImageBox()
			{
				return _imageBoxEnumerator.MoveNext() ? _imageBoxEnumerator.Current : null;
			}

			internal void SetImageBoxes(List<ImageBox> imageBoxes)
			{
				_imageBoxes = imageBoxes;
				_imageBoxEnumerator = _imageBoxes.GetEnumerator();
			}

		}

		public class ImageBox : ImageBoxPixelModuleIod
		{
			internal DicomUid SopInstanceUid;

			public FilmBox FilmBox { get; private set; }
			public PrintItem PrintItem { get; private set; }

			public ImageBox(FilmBox filmBox, PrintItem printItem)
			{
				this.FilmBox = filmBox;
				this.PrintItem = printItem;
			}

			//TODO (CR March 2011)- High (time permitting): Good candidate for unit tests

			/// <summary>
			/// Get the estimated size in pixel.  This method assumes that the spacing for each rows/columns of imageBoxes on a film are evenly divided.
			/// </summary>
			public static Size GetEstimatedSizeInPixel(Size filmBoxSize, ImageDisplayFormat imageDisplayFormat, int imageBoxPosition)
			{
				switch (imageDisplayFormat.Format)
				{
					case ImageDisplayFormat.FormatEnum.STANDARD:
						{
							var numberOfCols = imageDisplayFormat.Modifiers[0];
							var numberOfRows = imageDisplayFormat.Modifiers[1];
							// Size of rows and columns are uniform, and equals to total width or height divide by the count in either dimension
							return new Size(filmBoxSize.Width / numberOfCols, filmBoxSize.Height / numberOfRows);
						}

					case ImageDisplayFormat.FormatEnum.ROW:
						{
							// Major row order: left-to-right and top-to-bottom
							int rowIndex, colIndex;
							GetRowColumnIndex(imageDisplayFormat, imageBoxPosition, out rowIndex, out colIndex);

							var numberOfRows = imageDisplayFormat.Modifiers.Count;
							var numberOfCols = imageDisplayFormat.Modifiers[rowIndex];  // # of columns for the row the imageBox is in
							return new Size(filmBoxSize.Width / numberOfCols, filmBoxSize.Height / numberOfRows);
						}

					case ImageDisplayFormat.FormatEnum.COL:
						{
							// Major column order: top-to-bottom and left-to-right
							int rowIndex, colIndex;
							GetRowColumnIndex(imageDisplayFormat, imageBoxPosition, out rowIndex, out colIndex);

							var numberOfCols = imageDisplayFormat.Modifiers.Count;
							var numberOfRows = imageDisplayFormat.Modifiers[colIndex];  // # of rows for the column the imageBox is in
							return new Size(filmBoxSize.Width / numberOfCols, filmBoxSize.Height / numberOfRows);
						}

					case ImageDisplayFormat.FormatEnum.SLIDE:
					case ImageDisplayFormat.FormatEnum.SUPERSLIDE:
					case ImageDisplayFormat.FormatEnum.CUSTOM:
					default:
						break;
				}

				throw new NotSupportedException(string.Format("{0} image display format is not supported", imageDisplayFormat.Format));
			}

			internal void OnSet(ColorMode colorMode)
			{
				byte[] pixelData;
				ushort rows;
				ushort columns;

				if (colorMode == ColorMode.Color)
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

					this.PrintItem.GetPixelDataCallback.Invoke(this, colorMode, out rows, out columns, out pixelData);
					image.PixelData = pixelData;
					image.Rows = rows;
					image.Columns = columns;

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

					this.PrintItem.GetPixelDataCallback.Invoke(this, colorMode, out rows, out columns, out pixelData);
					image.PixelData = pixelData;
					image.Rows = rows;
					image.Columns = columns;

					this.BasicGrayscaleImageSequenceList.Add(image);
				}
			}

			#region Private helper methods

			/// <summary>
			/// Get the rowIndex and columnIndex of the specified imageBoxPosition for this ImageDisplayType.
			/// </summary>
			private static void GetRowColumnIndex(ImageDisplayFormat imageDisplayFormat, int imageBoxPosition, out int rowIndex, out int columnIndex)
			{
				rowIndex = 0;
				columnIndex = 0;

				switch (imageDisplayFormat.Format)
				{
					case ImageDisplayFormat.FormatEnum.STANDARD:
						{
							var numberOfColumns = imageDisplayFormat.Modifiers[0];
							var numberOfRows = imageDisplayFormat.Modifiers[1];
							var imageBoxIndex = imageBoxPosition - 1;
							rowIndex = imageBoxIndex / numberOfRows;
							columnIndex = imageBoxIndex % numberOfColumns;
							break;
						}

					case ImageDisplayFormat.FormatEnum.ROW:
						{
							// Major row order: left-to-right and top-to-bottom
							GetIndexForRowColumnFormat(imageDisplayFormat.Modifiers, imageBoxPosition, out rowIndex, out columnIndex);
							break;
						}

					case ImageDisplayFormat.FormatEnum.COL:
						{
							// Major column order: top-to-bottom and left-to-right
							GetIndexForRowColumnFormat(imageDisplayFormat.Modifiers, imageBoxPosition, out columnIndex, out rowIndex);
							break;
						}

					case ImageDisplayFormat.FormatEnum.SLIDE:
					case ImageDisplayFormat.FormatEnum.SUPERSLIDE:
					case ImageDisplayFormat.FormatEnum.CUSTOM:
					default:
						throw new NotSupportedException(string.Format("{0} image display format is not supported", imageDisplayFormat.Format));
				}
			}

			private static void GetIndexForRowColumnFormat(IList<int> imageBoxesPerLines, int imageBoxPosition, out int firstIndex, out int secondIndex)
			{
				firstIndex = 0;
				secondIndex = 0;

				var numberOfImageBoxesBeforeCurrentLine = 0;
				var numberOfLines = imageBoxesPerLines.Count;
				for (var lineIndex = 0; lineIndex < numberOfLines; lineIndex++)
				{
					var numberOfImageBoxesIncludingCurrentLine = numberOfImageBoxesBeforeCurrentLine + imageBoxesPerLines[lineIndex];
					if (imageBoxPosition <= numberOfImageBoxesIncludingCurrentLine)
					{
						// Image is in current line
						firstIndex = lineIndex;
						secondIndex = imageBoxPosition - numberOfImageBoxesBeforeCurrentLine - 1;

						break;
					}

					// Advance the total imageBox count
					numberOfImageBoxesBeforeCurrentLine = numberOfImageBoxesIncludingCurrentLine;
				}
			}

			#endregion
		}
	}
}
