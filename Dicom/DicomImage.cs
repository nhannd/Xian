using System;
using System.Text;
using System.Runtime.InteropServices;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Codecs;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Representation of a DicomImage object.
    /// </summary>
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
		private PhotometricInterpretation _photometricInterpretation;
		private byte[] _pixelData;

		private DcmDataset _dataset;
		private DcmMetaInfo _metaInfo;
		private bool _isDatasetLoaded = false;
        private bool _isImageParameterSetLoaded = false;
		private string _transferSyntaxUid;
		private static ImageCodecMap _imageCodecMap;

        /// <summary>
        /// The underlying DcmDataset object of this image.
        /// </summary>
		protected DcmDataset Dataset
		{
			get { return _dataset; }
			set { _dataset = value; }
		}

        /// <summary>
        /// The underlying DcmMetaInfo object of this image, that
        /// represents the meta-header describing the dataset. The DcmMetaInfo
        /// may not exist, as it typically only exists if the dataset is stored
        /// onto media, where the meta-header is used to describe the format
        /// of the storage.
        /// </summary>
		protected DcmMetaInfo MetaInfo
		{
			get { return _metaInfo; }
			set { _metaInfo = value; }
		}

        /// <summary>
        /// Flag indicating whether the underlying dataset has been loaded into memory.
        /// </summary>
		public bool IsDatasetLoaded
		{
			get { return _isDatasetLoaded; }
			protected set { _isDatasetLoaded = value; }
		}

        /// <summary>
        /// Flag indicating whether or not the parameters that define an image for
        /// rendering has been loaded into memory.
        /// </summary>
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

		public PhotometricInterpretation PhotometricInterpretation 
		{ 
			get 
			{ 
				LoadDataset();
                LoadImageParameterSet();
				return _photometricInterpretation; 
			} 
		}

        /// <summary>
        /// Obtains the pixel data for this image dataset.
        /// </summary>
        /// <returns>A byte array representing the pixel data. This array's interpretation
        /// should be subject to the various image-defining parameters such as Samples Per Pixel
        /// and Bits Stored.</returns>
		public byte[] GetPixelData()
		{
			if (!IsDatasetLoaded)
                LoadDataset();

            if (!IsImageParameterSetLoaded)
                LoadImageParameterSet();

			return GetPixelData(
				this.Rows,
				this.Columns,
				this.BitsAllocated,
				this.BitsStored,
				this.PixelRepresentation,
				this.PhotometricInterpretation,
				this.SamplesPerPixel,
				this.PlanarConfiguration,
                _transferSyntaxUid);
		}

        /// <summary>
        /// Obtains the pixel data for this image dataset, given a set of
        /// image-defining parameters. This method allows for slight optimization if
        /// these parameters are already known, since the need to parse the
        /// image's dataset for these values is avoided.
        /// </summary>
        /// <param name="bitsAllocated">The Bits Allocated tag of the image.</param>
        /// <param name="rows">The Rows tag of the image.</param>
        /// <param name="columns">The Columns tag of the image.</param>
        /// <param name="samplesPerPixel">The Samples Per Pixel tag of the image.</param>
        /// <param name="bitsStored">The number of bits actually used in the storage of pixel information.</param>
        /// <param name="pixelRepresentation">The representation of pixels as signed or unsigned using 2's complement.</param>
        /// <param name="photometricInterpretation">The interpretation of the pixel values into different colour spaces.</param>
        /// <param name="planarConfiguration">For coloured pixels, the interpretation of the layout of storage, either by colour planes,
        /// or as RGB triplets.</param>
        /// <param name="transferSyntaxUid">Transfer Syntax of the image encoding.</param>
        /// <returns>A byte array representing the pixel data. This array's interpretation
        /// should be subject to the various image-defining parameters such as Samples Per Pixel
        /// and Bits Stored.</returns>
        public byte[] GetPixelData(
			int rows, 
			int columns,
			int bitsAllocated,
			int bitsStored,
			int pixelRepresentation,
			PhotometricInterpretation photometricInterpretation,
			int samplesPerPixel,
			int planarConfiguration, 
            string transferSyntaxUid)
        {
            // assume that if bitsAllocated, etc. are provided, IsImageParameterSetLoaded is true
            if (!IsDatasetLoaded)
                LoadDataset();

            if (_pixelData == null)
            {
				CreateImageCodecMap();
				
				OFCondition status;
                IntPtr pUnmanagedPixelData = IntPtr.Zero;

                if (bitsAllocated == 16)
                    status = _dataset.findAndGetUint16Array(Dcm.PixelData, ref pUnmanagedPixelData);
                else
                    status = _dataset.findAndGetUint8Array(Dcm.PixelData, ref pUnmanagedPixelData);

                bool tagExists;
                DicomHelper.CheckReturnValue(status, Dcm.PixelData, out tagExists);

                int sizeInBytes = (int)(rows * columns * samplesPerPixel * bitsAllocated / 8);
                byte[] compressedPixelData = new byte[sizeInBytes];
				Marshal.Copy(pUnmanagedPixelData, compressedPixelData, 0, sizeInBytes);

                // We don't need the unmanaged pixel data anymore since we've already
                // made a copy so just get rid of it to free up some memory
                status = _dataset.findAndDeleteElement(Dcm.PixelData);
				DicomHelper.CheckReturnValue(status, Dcm.PixelData, out tagExists);
				
				if (!_imageCodecMap.IsTransferSyntaxSupported(transferSyntaxUid))
					throw new Exception("Transfer syntax not supported");

				try
				{
					_pixelData = _imageCodecMap[transferSyntaxUid].Decode(
						compressedPixelData,
						rows,
						columns,
						bitsAllocated,
						bitsStored,
						pixelRepresentation,
						PhotometricInterpretationHelper.GetString(photometricInterpretation),
						samplesPerPixel,
						planarConfiguration,
						null);
				}
				catch (Exception e)
				{
					Platform.Log(e);
					throw new Exception("Unable to decode pixel data", e);
				}
			}

            return _pixelData;
        }

		#endregion

        /// <summary>
        /// One of several overloads that allows the client to obtain a particular tag from
        /// the image file's dataset. Each overload mirrors an overload of the findAndGetxxx()
        /// functions in the underlying OFFIS DICOM Toolkit.
        /// </summary>
        /// <param name="tag">The tag that will be obtained</param>
        /// <param name="val">The object that the value will be stored in.</param>
        /// <param name="tagExists">An indicator whether the tag exists in the dataset or not.</param>
		public void GetTag(DcmTagKey tag, out ushort val, out bool tagExists)
		{
			LoadDataset();
			OFCondition status = _dataset.findAndGetUint16(tag, out val);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);

			if (tagExists)
				return;

			status = _metaInfo .findAndGetUint16(tag, out val);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out ushort val, uint position, out bool tagExists)
		{
			LoadDataset();
			OFCondition status = _dataset.findAndGetUint16(tag, out val, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);

			if (tagExists)
				return;

			status = _metaInfo.findAndGetUint16(tag, out val, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out int val, out bool tagExists)
		{
			LoadDataset();
			OFCondition status = _dataset.findAndGetSint32(tag, out val);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);

			if (tagExists)
				return;

			status = _metaInfo.findAndGetSint32(tag, out val);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out int val, uint position, out bool tagExists)
		{
			LoadDataset();
			OFCondition status = _dataset.findAndGetSint32(tag, out val, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);

			if (tagExists)
				return;

			status = _metaInfo.findAndGetSint32(tag, out val, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out double val, out bool tagExists)
		{
			LoadDataset();
			OFCondition status = _dataset.findAndGetFloat64(tag, out val);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);

			if (tagExists)
				return;

			status = _metaInfo.findAndGetFloat64(tag, out val);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out double val, uint position, out bool tagExists)
		{
			LoadDataset();
			OFCondition status = _dataset.findAndGetFloat64(tag, out val, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);

			if (tagExists)
				return;

			status = _metaInfo.findAndGetFloat64(tag, out val, position);
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

			if (tagExists)
				return;

			buffer = new StringBuilder(64);
			status = _metaInfo.findAndGetOFString(tag, buffer);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
			val = buffer.ToString();
		}

		public void GetTag(DcmTagKey tag, out string value, uint position, out bool tagExists)
		{
			//TODO: shouldn't hard code the buffer length like this
			LoadDataset();
			StringBuilder buffer = new StringBuilder(64);
			OFCondition status = _dataset.findAndGetOFString(tag, buffer, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
			value = buffer.ToString();

			if (tagExists)
				return;

			buffer = new StringBuilder(64);
			status = _metaInfo.findAndGetOFString(tag, buffer, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
			value = buffer.ToString();
		}

		/// <summary>
		/// This method will obtain an entire tag, regardless of it's Value Multiplicity (VM)
		/// as a string.  This is useful, for example, in the case of the "Image Type" tag,
		/// where the individual values are not necessarily as useful as the unparsed tag.
		/// </summary>
		/// <param name="tag">The tag that will be obtained</param>
		/// <param name="valueArray">The object that the value will be stored in.</param>
		/// <param name="tagExists">An indicator whether the tag exists in the dataset or not.</param>
		public void GetTagArray(DcmTagKey tag, out string valueArray, out bool tagExists)
		{
			LoadDataset();
			StringBuilder buffer = new StringBuilder(512);
			OFCondition status = _dataset.findAndGetOFStringArray(tag, buffer);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
			valueArray = buffer.ToString();

			if (tagExists)
				return;

			buffer = new StringBuilder(512);
			status = _metaInfo.findAndGetOFStringArray(tag, buffer);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
			valueArray = buffer.ToString();
		}

		protected abstract void LoadDataset();
		protected abstract void UnloadDataset();

        /// <summary>
        /// Loads the minmum set of image parameter tags that is
        /// necessary for rendering the pixel data to the display.
        /// </summary>
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

            StringBuilder buffer = new StringBuilder(64);
            status = _dataset.findAndGetOFString(Dcm.PhotometricInterpretation, buffer);
            DicomHelper.CheckReturnValue(status, Dcm.PhotometricInterpretation, out tagExists);
			_photometricInterpretation = PhotometricInterpretationHelper.FromString(buffer.ToString());

			buffer = new StringBuilder(64);
			status = _metaInfo.findAndGetOFString(Dcm.TransferSyntaxUID, buffer);
			DicomHelper.CheckReturnValue(status, Dcm.TransferSyntaxUID, out tagExists);
			_transferSyntaxUid = buffer.ToString();
			
			IsImageParameterSetLoaded = true;
        }

		private static void CreateImageCodecMap()
		{
			if (_imageCodecMap == null)
				_imageCodecMap = new ImageCodecMap();
		}
	}
}
