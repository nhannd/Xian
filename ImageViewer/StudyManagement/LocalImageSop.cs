using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A local, file-based implementation of <see cref="ImageSop"/>.
	/// </summary>
	public class LocalImageSop : ImageSop
	{
		private FileDicomImage _dicomImage;
		private byte[] _pixelData;

		/// <summary>
		/// Initializes a new instance of <see cref="LocalImageSop"/> with
		/// a specified filename.
		/// </summary>
		/// <param name="filename"></param>
		public LocalImageSop(string filename)
		{
			_dicomImage = new FileDicomImage(filename);
		}

		protected LocalImageSop()
		{

		}

        public override object NativeDicomObject
        {
            get { return _dicomImage; }
        }

		public override int SamplesPerPixel
		{
			get
			{
				return _dicomImage.SamplesPerPixel;
			}
		}

		public override PhotometricInterpretation PhotometricInterpretation
		{
			get
			{
				return _dicomImage.PhotometricInterpretation;
			}
		}

		public override int Rows
		{
			get
			{
				return _dicomImage.Rows;
			}
		}

		public override int Columns
		{
			get
			{
				return _dicomImage.Columns;
			}
		}

		public override int BitsAllocated
		{
			get
			{
				return _dicomImage.BitsAllocated;
			}
		}

		public override int BitsStored
		{
			get
			{
				return _dicomImage.BitsStored;
			}
		}

		public override int HighBit
		{
			get
			{
				return _dicomImage.HighBit;
			}
		}

		public override int PixelRepresentation
		{
			get
			{
				return _dicomImage.PixelRepresentation;
			}
		}

		public override int PlanarConfiguration
		{
			get
			{
				return _dicomImage.PlanarConfiguration;
			}
		}

		public override byte[] GetNormalizedPixelData()
		{
			if (_pixelData == null)
			{
				// Decompress the pixel data (if pixel data is already uncompressed,
				// this is a pass-through, a no-op.)
				//
				// TODO: When the pixel data is compressed, we should delete the
				// compressed buffer and just leave the uncompressed buffer, so as to
				// save memory.  If a memory management mechanism decides to unload the
				// pixel data, the next time this method is called should again decompress
				// the data as if it were doing so for the first time.
				_pixelData = ImageSopHelper.DecompressPixelData(this, _dicomImage.GetPixelData());

				// If it's a colour image, we want to change the colour space ARGB
				// so that it's easily consumed downstream
				if (this.PhotometricInterpretation != PhotometricInterpretation.Monochrome1 &&
					this.PhotometricInterpretation != PhotometricInterpretation.Monochrome2)
				{
					int sizeInBytes = this.Rows * this.Columns * 4;
					byte[] newPixelData = new byte[sizeInBytes];
					
					ColorSpaceConverter.ToArgb(
						this.PhotometricInterpretation,
						this.PlanarConfiguration,
						_pixelData,
						newPixelData);

					_pixelData = newPixelData;
				}
			}

			return _pixelData;
		}

		public override void GetTag(DcmTagKey tag, out ushort value, out bool tagExists)
		{
			_dicomImage.GetTag(tag, out value, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out ushort value, uint position, out bool tagExists)
		{
			_dicomImage.GetTag(tag, out value, position, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out int value, out bool tagExists)
		{
			_dicomImage.GetTag(tag, out value, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out int value, uint position, out bool tagExists)
		{
			_dicomImage.GetTag(tag, out value, position, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out double value, out bool tagExists)
		{
			_dicomImage.GetTag(tag, out value, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out double value, uint position, out bool tagExists)
		{
			_dicomImage.GetTag(tag, out value, position, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out string value, out bool tagExists)
		{
			_dicomImage.GetTag(tag, out value, out tagExists);
		}

		public override void GetTag(DcmTagKey tag, out string value, uint position, out bool tagExists)
		{
			_dicomImage.GetTag(tag, out value, position, out tagExists);
		}

		public override void GetTagArray(DcmTagKey tag, out string value, out bool tagExists)
		{
			_dicomImage.GetTagArray(tag, out value, out tagExists);
		}

		protected override void Dispose(bool disposing)
		{
			_dicomImage.Dispose();

			base.Dispose(disposing);
		}
	}
}
