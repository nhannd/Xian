using System;
using System.Text;
using System.Runtime.InteropServices;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.Dicom
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

		private DcmDataset _dataset;
		private bool _isDatasetLoaded = false;
        private bool _isImageParameterSetLoaded = false;

		protected DcmDataset Dataset
		{
			get { return _dataset; }
			set { _dataset = value; }
		}

		public bool IsDatasetLoaded
		{
			get { return _isDatasetLoaded; }
			protected set { _isDatasetLoaded = value; }
		}

        public bool IsImageParameterSetLoaded
        {
            get { return _isImageParameterSetLoaded; }
            protected set { _isImageParameterSetLoaded = value; }
        }

		#region IImage members

		public int Rows 
		{ 
			get 
			{ 
				LoadDataset();
                LoadImageParameterSet();
				return (int) _rows; 
			}
		}

		public int Columns 
		{ 
			get 
			{ 
				LoadDataset();
                LoadImageParameterSet();
				return (int) _columns; 
			} 
		}

		public int BitsAllocated 
		{ 
			get 
			{ 
				LoadDataset();
                LoadImageParameterSet();
				return (int) _bitsAllocated; 
			} 
		}

		public int BitsStored 
		{ 
			get 
			{ 
				LoadDataset();
                LoadImageParameterSet();
				return (int) _bitsStored; 
			} 
		}

		public int HighBit 
		{ 
			get 
			{ 
				LoadDataset();
                LoadImageParameterSet();
				return (int) _highBit; 
			} 
		}

		public int SamplesPerPixel 
		{ 
			get 
			{ 
				LoadDataset();
                LoadImageParameterSet();
				return (int) _samplesPerPixel; 
			} 
		}

		public int PixelRepresentation 
		{ 
			get 
			{ 
				LoadDataset();
                LoadImageParameterSet();
				return (int) _pixelRepresentation; 
			} 
		}

		public int PlanarConfiguration 
		{ 
			get 
			{ 
				LoadDataset();
                LoadImageParameterSet();
				return (int) _planarConfiguration; 
			} 
		}

		public string PhotometricInterpretation 
		{ 
			get 
			{ 
				LoadDataset();
                LoadImageParameterSet();
				return _photometricInterpretation; 
			} 
		}

		public byte[] GetPixelData()
		{
			if (!IsDatasetLoaded)
                LoadDataset();

            if (!IsImageParameterSetLoaded)
                LoadImageParameterSet();

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

				// TODO: Eventually need to check for transfer syntax and decompress here if necessary

				int sizeInBytes = (int)(this.Rows * this.Columns * this.SamplesPerPixel * this.BitsAllocated / 8);
				_pixelData = new byte[sizeInBytes];
				Marshal.Copy(pUnmanagedPixelData, _pixelData, 0, sizeInBytes);
				
				// We don't need the unmanaged pixel data anymore since we've already
				// made a copy so just get rid of it to free up some memory
				_dataset.findAndDeleteElement(Dcm.PixelData);
			}

			return _pixelData;
		}

        public byte[] GetPixelData(int bitsAllocated, int rows, int columns, int samplesPerPixel)
        {
            // assume that if bitsAllocated, etc. are provided, IsImageParameterSetLoaded is true
            if (!IsDatasetLoaded)
                LoadDataset();

            if (_pixelData == null)
            {
                OFCondition status;
                IntPtr pUnmanagedPixelData = IntPtr.Zero;

                if (bitsAllocated == 16)
                    status = _dataset.findAndGetUint16Array(Dcm.PixelData, ref pUnmanagedPixelData);
                else
                    status = _dataset.findAndGetUint8Array(Dcm.PixelData, ref pUnmanagedPixelData);

                bool tagExists;
                DicomHelper.CheckReturnValue(status, Dcm.PixelData, out tagExists);

                // TODO: Eventually need to check for transfer syntax and decompress here if necessary
                int sizeInBytes = (int)(rows * columns * samplesPerPixel * bitsAllocated / 8);
                _pixelData = new byte[sizeInBytes];
                Marshal.Copy(pUnmanagedPixelData, _pixelData, 0, sizeInBytes);

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
			LoadDataset();
			OFCondition status = _dataset.findAndGetUint16(tag, out val);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out ushort val, uint position, out bool tagExists)
		{
			LoadDataset();
			OFCondition status = _dataset.findAndGetUint16(tag, out val, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out double val, out bool tagExists)
		{
			LoadDataset();
			OFCondition status = _dataset.findAndGetFloat64(tag, out val);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out double val, uint position, out bool tagExists)
		{
			LoadDataset();
			OFCondition status = _dataset.findAndGetFloat64(tag, out val, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out string val, out bool tagExists)
		{
			//TODO: shouldn't hard code the buffer length like this
			LoadDataset();
			StringBuilder buffer = new StringBuilder(64);
			OFCondition status = _dataset.findAndGetOFString(tag, buffer);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
			val = buffer.ToString();
		}
		
		#endregion

		protected virtual void LoadDataset()
		{
		}

		protected virtual void UnloadDataset()
		{
		}

        protected virtual void LoadImageParameterSet()
        {
            if (IsImageParameterSetLoaded)
                return;

            if (!IsDatasetLoaded)
                LoadDataset();

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

            IsImageParameterSetLoaded = true;
        }
	}
}
