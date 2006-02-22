using System;
using System.Text;
using System.Runtime.InteropServices;
using ClearCanvas.Dicom.Data;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.Dicom.Data
{
	public abstract class DicomImage
	{
		private ushort _rows;
		private ushort _columns;
		private ushort _bitsAllocated;
		private ushort _bitsStored;
		private ushort _highBit;
		private ushort _samplesPerPixel;
		private ushort _pixelRepresentation;
		private ushort _planarConfiguration;
		private string _photometricInterpretation;
		private byte[] _pixelData;

		private int _sizeInBytes;
		private int _stride;

		private DcmDataset _dataset;
		private bool _isLoaded = false;

		protected DcmDataset Dataset
		{
			get { return _dataset; }
			set { _dataset = value; }
		}

		public bool IsLoaded
		{
			get { return _isLoaded; }
			protected set { _isLoaded = value; }
		}

		#region IImage members

		public int Rows 
		{ 
			get 
			{ 
				Load();
				return (int) _rows; 
			}
		}

		public int Columns 
		{ 
			get 
			{ 
				Load();
				return (int) _columns; 
			} 
		}

		public int BitsAllocated 
		{ 
			get 
			{ 
				Load();
				return (int) _bitsAllocated; 
			} 
		}

		public int BitsStored 
		{ 
			get 
			{ 
				Load();
				return (int) _bitsStored; 
			} 
		}

		public int HighBit 
		{ 
			get 
			{ 
				Load();
				return (int) _highBit; 
			} 
		}

		public int SamplesPerPixel 
		{ 
			get 
			{ 
				Load();
				return (int) _samplesPerPixel; 
			} 
		}

		public int PixelRepresentation 
		{ 
			get 
			{ 
				Load();
				return (int) _pixelRepresentation; 
			} 
		}

		public int PlanarConfiguration 
		{ 
			get 
			{ 
				Load();
				return (int) _planarConfiguration; 
			} 
		}

		public string PhotometricInterpretation 
		{ 
			get 
			{ 
				Load();
				return _photometricInterpretation; 
			} 
		}

		public int SizeInBytes 
		{ 
			get 
			{ 
				Load();
				return _sizeInBytes; 
			} 
		}

		public int Stride 
		{ 
			get 
			{ 
				Load();
				return _stride; 
			}
		}

		public byte[] GetPixelData()
		{
			Load();

			if (_pixelData == null)
			{
				OFCondition status;
				IntPtr pUnmanagedPixelData = IntPtr.Zero;

				if (_bitsAllocated == 16)
					status = _dataset.findAndGetUint16Array(Dcm.PixelData, ref pUnmanagedPixelData);
				else
					status = _dataset.findAndGetUint8Array(Dcm.PixelData, ref pUnmanagedPixelData);

				bool tagExists;
				DicomHelper.CheckReturnValue(status, Dcm.PixelData, out tagExists);

				// Eventually need to check for transfer syntax and decompress here if necessary
				_pixelData = new byte[_sizeInBytes];
				Marshal.Copy(pUnmanagedPixelData, _pixelData, 0, _sizeInBytes);
				
				// We don't need the unmanaged pixel data anymore since we've already
				// made a copy so just get rid of it to free up some memory
				_dataset.findAndDeleteElement(Dcm.PixelData);
			}

			return _pixelData;
		}

		#endregion

		#region IDicomImage members

		public void GetTag(DcmTagKey tag, out ushort val, out bool tagExists)
		{
			Load();
			OFCondition status = _dataset.findAndGetUint16(tag, out val);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out ushort val, uint position, out bool tagExists)
		{
			Load();
			OFCondition status = _dataset.findAndGetUint16(tag, out val, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out double val, out bool tagExists)
		{
			Load();
			OFCondition status = _dataset.findAndGetFloat64(tag, out val);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out double val, uint position, out bool tagExists)
		{
			Load();
			OFCondition status = _dataset.findAndGetFloat64(tag, out val, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out string val, out bool tagExists)
		{
			//TODO: shouldn't hard code the buffer length like this
			Load();
			StringBuilder buffer = new StringBuilder(64);
			OFCondition status = _dataset.findAndGetOFString(tag, buffer);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
			val = buffer.ToString();
		}
		
		#endregion

		protected virtual void Load()
		{
			RetrieveImageParameters();
			CalculateOtherImageParameters();
		}

		protected virtual void Unload()
		{
		}

		private void RetrieveImageParameters()
		{
			OFCondition status;
			bool tagExists;

			status = _dataset.findAndGetUint16(Dcm.Rows, out _rows);
			DicomHelper.CheckReturnValue(status, Dcm.Rows, out tagExists);

			status = _dataset.findAndGetUint16(Dcm.Columns, out _columns);
			DicomHelper.CheckReturnValue(status, Dcm.Columns, out tagExists);

			status = _dataset.findAndGetUint16(Dcm.BitsAllocated, out _bitsAllocated);
			DicomHelper.CheckReturnValue(status, Dcm.BitsAllocated, out tagExists);

			status = _dataset.findAndGetUint16(Dcm.BitsStored, out _bitsStored);
			DicomHelper.CheckReturnValue(status, Dcm.BitsStored, out tagExists);

			status = _dataset.findAndGetUint16(Dcm.HighBit, out _highBit);
			DicomHelper.CheckReturnValue(status, Dcm.HighBit, out tagExists);

			status = _dataset.findAndGetUint16(Dcm.SamplesPerPixel, out _samplesPerPixel);
			DicomHelper.CheckReturnValue(status, Dcm.SamplesPerPixel, out tagExists);

			status = _dataset.findAndGetUint16(Dcm.PixelRepresentation, out _pixelRepresentation);
			DicomHelper.CheckReturnValue(status, Dcm.PixelRepresentation, out tagExists);

			if (_samplesPerPixel > 1)
			{
				status = _dataset.findAndGetUint16(Dcm.PlanarConfiguration, out _planarConfiguration);
				DicomHelper.CheckReturnValue(status, Dcm.PlanarConfiguration, out tagExists);
			}

			StringBuilder buf = new StringBuilder(64);
			status = _dataset.findAndGetOFString(Dcm.PhotometricInterpretation, buf);
			DicomHelper.CheckReturnValue(status, Dcm.PhotometricInterpretation, out tagExists);
			_photometricInterpretation = buf.ToString();
		}

		private void CalculateOtherImageParameters()
		{
			_sizeInBytes = (int) (_rows * _columns * _samplesPerPixel * _bitsAllocated / 8);

			// If we're not on a 4-byte boundary, round up to the next multiple of 4
			// using integer division
			if (_columns % 4 != 0)
				_stride = _columns / 4 * 4 + 4;
			else
				_stride = _columns;
		}
	}
}
