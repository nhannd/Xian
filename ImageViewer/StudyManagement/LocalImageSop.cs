using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A local, file-based implementation of <see cref="ImageSop"/>.
	/// </summary>
	public class LocalImageSop : ImageSop
	{
		private delegate void GetTagDelegate<T>(DicomAttribute attribute, uint position, out T value);
 
		private DicomFile _dicomFile;
		private byte[] _pixelData;
		private bool _loaded;

		/// <summary>
		/// Initializes a new instance of <see cref="LocalImageSop"/> with
		/// a specified filename.
		/// </summary>
		/// <param name="filename"></param>
		public LocalImageSop(string filename)
		{
			_dicomFile = new DicomFile(filename);
			_loaded = false;
		}

		private LocalImageSop()
		{
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

        public override DicomMessageBase NativeDicomObject
        {
            get { return _dicomFile; }
        }
		
		public override byte[] GetNormalizedPixelData()
		{
			Load();
			
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
				object pixelData = _dicomFile.DataSet[DicomTags.PixelData].Values;
				if (pixelData is byte[])
				{
					_pixelData = ImageSopHelper.DecompressPixelData(this, (byte[])pixelData);
				}
				else
				{
					ushort[] originalPixelData = (ushort[])pixelData;
					byte[] newPixelData = new byte[originalPixelData.Length * 2];
					Buffer.BlockCopy(originalPixelData, 0, newPixelData, 0, newPixelData.Length);
					_pixelData = ImageSopHelper.DecompressPixelData(this, newPixelData);
				}

				//To save on memory, we remove this reference.
				_dicomFile.DataSet[DicomTags.PixelData] = null;

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

		public override void GetTag(uint tag, out ushort value, out bool tagExists)
		{
			GetTag(tag, out value, 0, out tagExists);
		}

		public override void GetTag(uint tag, out ushort value, uint position, out bool tagExists)
		{
			GetTag<ushort>(tag, out value, position, out tagExists, GetUint16FromAttribute);
		}

		public override void GetTag(uint tag, out int value, out bool tagExists)
		{
			GetTag(tag, out value, 0, out tagExists);
		}

		public override void GetTag(uint tag, out int value, uint position, out bool tagExists)
		{
			GetTag<int>(tag, out value, position, out tagExists, GetInt32FromAttribute);
		}

		public override void GetTag(uint tag, out double value, out bool tagExists)
		{
			GetTag(tag, out value, 0, out tagExists);
		}

		public override void GetTag(uint tag, out double value, uint position, out bool tagExists)
		{
			GetTag<double>(tag, out value, position, out tagExists, GetFloat64FromAttribute);
		}

		public override void GetTag(uint tag, out string value, out bool tagExists)
		{
			GetTag(tag, out value, 0, out tagExists);
		}

		public override void GetTag(uint tag, out string value, uint position, out bool tagExists)
		{
			GetTag<string>(tag, out value, position, out tagExists, GetStringFromAttribute);
			value = value ?? "";
		}

		public override void GetTagArray(uint tag, out string value, out bool tagExists)
		{
			GetTag<string>(tag, out value, 0, out tagExists, GetStringArrayFromAttribute);
			value = value ?? "";
		}

		private void GetUint16FromAttribute(DicomAttribute attribute, uint position, out ushort value)
		{
			attribute.TryGetUInt16((int)position, out value);
		}

		private void GetInt32FromAttribute(DicomAttribute attribute, uint position, out int value)
		{
			attribute.TryGetInt32((int)position, out value);
		}

		private void GetFloat64FromAttribute(DicomAttribute attribute, uint position, out double value)
		{
			attribute.TryGetFloat64((int)position, out value);
		}

		private void GetStringFromAttribute(DicomAttribute attribute, uint position, out string value)
		{
			attribute.TryGetString((int)position, out value);
		}

		private void GetStringArrayFromAttribute(DicomAttribute attribute, uint position, out string value)
		{
			value = attribute.ToString();
		}

		private void GetTag<T>(uint tag, out T value, uint position, out bool tagExists, GetTagDelegate<T> getter)
		{
			Load();
			value = default(T);
			tagExists = false;

			DicomAttribute dicomAttribute;
			if(_dicomFile.DataSet.Contains(tag))
			{
				dicomAttribute = _dicomFile.DataSet[tag];
				tagExists = !dicomAttribute.IsEmpty && dicomAttribute.Count > position;
				if (tagExists)
				{
					getter(dicomAttribute, position, out value);
					return;
				}
			}

			if (_dicomFile.MetaInfo.Contains(tag))
			{
				dicomAttribute = _dicomFile.MetaInfo[tag];
				tagExists = !dicomAttribute.IsEmpty && dicomAttribute.Count > position;
				if (tagExists)
					getter(dicomAttribute, position, out value);
			}
		}

		private void Load()
		{
			if (_loaded)
				return;

			_loaded = true;
			_dicomFile.Load(DicomReadOptions.Default);
		}
	}
}
