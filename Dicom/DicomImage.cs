using System;
using System.Text;
using System.Runtime.InteropServices;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Representation of a DicomImage object.
    /// </summary>
	public abstract class DicomImage : IDisposable
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

        /// <summary>
        /// The underlying DcmDataset object of this image.
        /// </summary>
		public DcmDataset Dataset
		{
			get 
            {
                Load();
                return _dataset; 
            }
			protected set { _dataset = value; }
		}

        /// <summary>
        /// The underlying DcmMetaInfo object of this image, that
        /// represents the meta-header describing the dataset. The DcmMetaInfo
        /// may not exist, as it typically only exists if the dataset is stored
        /// onto media, where the meta-header is used to describe the format
        /// of the storage.
        /// </summary>
        public DcmMetaInfo MetaInfo
		{
			get 
            {
                Load();
                return _metaInfo; 
            }
			protected set { _metaInfo = value; }
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
				Load();
                LoadImageParameterSet();
				return (int) _rows; 
			}
		}

		public int Columns 
		{ 
			get 
			{ 
				Load();
                LoadImageParameterSet();
				return (int) _columns; 
			} 
		}

		public int BitsAllocated 
		{ 
			get 
			{ 
				Load();
                LoadImageParameterSet();
				return (int) _bitsAllocated; 
			} 
		}

		public int BitsStored 
		{ 
			get 
			{ 
				Load();
                LoadImageParameterSet();
				return (int) _bitsStored; 
			} 
		}

		public int HighBit 
		{ 
			get 
			{ 
				Load();
                LoadImageParameterSet();
				return (int) _highBit; 
			} 
		}

		public int SamplesPerPixel 
		{ 
			get 
			{ 
				Load();
                LoadImageParameterSet();
				return (int) _samplesPerPixel; 
			} 
		}

		public int PixelRepresentation 
		{ 
			get 
			{ 
				Load();
                LoadImageParameterSet();
				return (int) _pixelRepresentation; 
			} 
		}

		public int PlanarConfiguration 
		{ 
			get 
			{ 
				Load();
                LoadImageParameterSet();
				return (int) _planarConfiguration; 
			} 
		}

		public PhotometricInterpretation PhotometricInterpretation 
		{ 
			get 
			{ 
				Load();
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
                Load();

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
                Load();

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

				int sizeInBytes = (int)(rows * columns * samplesPerPixel * bitsAllocated / 8);
				_pixelData = new byte[sizeInBytes];
				Marshal.Copy(pUnmanagedPixelData, _pixelData, 0, sizeInBytes);

				// We don't need the unmanaged pixel data anymore since we've already
				// made a copy so just get rid of it to free up some memory
				status = _dataset.findAndDeleteElement(Dcm.PixelData);
				DicomHelper.CheckReturnValue(status, Dcm.PixelData, out tagExists);
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
			Load();
			OFCondition status = _dataset.findAndGetUint16(tag, out val);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);

			if (tagExists)
				return;

			status = _metaInfo .findAndGetUint16(tag, out val);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out ushort val, uint position, out bool tagExists)
		{
			Load();
			OFCondition status = _dataset.findAndGetUint16(tag, out val, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);

			if (tagExists)
				return;

			status = _metaInfo.findAndGetUint16(tag, out val, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out int val, out bool tagExists)
		{
			Load();
			OFCondition status = _dataset.findAndGetSint32(tag, out val);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);

			if (tagExists)
				return;

			status = _metaInfo.findAndGetSint32(tag, out val);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out int val, uint position, out bool tagExists)
		{
			Load();
			OFCondition status = _dataset.findAndGetSint32(tag, out val, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);

			if (tagExists)
				return;

			status = _metaInfo.findAndGetSint32(tag, out val, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out double val, out bool tagExists)
		{
			Load();
			OFCondition status = _dataset.findAndGetFloat64(tag, out val);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);

			if (tagExists)
				return;

			status = _metaInfo.findAndGetFloat64(tag, out val);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out double val, uint position, out bool tagExists)
		{
			Load();
			OFCondition status = _dataset.findAndGetFloat64(tag, out val, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);

			if (tagExists)
				return;

			status = _metaInfo.findAndGetFloat64(tag, out val, position);
			DicomHelper.CheckReturnValue(status, tag, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out string value, out bool tagExists)
		{
			Load();

			DicomHelper.FindAndGetOFString(_dataset, tag, out value, out tagExists);
			if (tagExists)
				return;

			DicomHelper.FindAndGetOFString(_metaInfo, tag, out value, out tagExists);
		}

		public void GetTag(DcmTagKey tag, out string value, uint position, out bool tagExists)
		{
			Load();

			DicomHelper.FindAndGetOFString(_dataset, tag, position, out value, out tagExists);
			if (tagExists)
				return;

			DicomHelper.FindAndGetOFString(_metaInfo, tag, position, out value, out tagExists);
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
			Load();

			DicomHelper.FindAndGetOFStringArray(_dataset, tag, out valueArray, out tagExists);
			if (tagExists)
				return;

			DicomHelper.FindAndGetOFStringArray(_metaInfo, tag, out valueArray, out tagExists);
		}

		public abstract void Load();
        public abstract void WriteToMedia(E_TransferSyntax TransferSyntax);

        /// <summary>
        /// Loads the minmum set of image parameter tags that is
        /// necessary for rendering the pixel data to the display.
        /// </summary>
        protected virtual void LoadImageParameterSet()
        {
            if (IsImageParameterSetLoaded)
                return;

            if (!IsDatasetLoaded)
                Load();

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

			string value;
			status = DicomHelper.TryFindAndGetOFString(_dataset, Dcm.PhotometricInterpretation, out value);
			DicomHelper.CheckReturnValue(status, Dcm.PhotometricInterpretation, out tagExists);
			_photometricInterpretation = PhotometricInterpretationHelper.FromString(value);

			status = DicomHelper.TryFindAndGetOFString(_metaInfo, Dcm.TransferSyntaxUID, out value);
			DicomHelper.CheckReturnValue(status, Dcm.TransferSyntaxUID, out tagExists);
			_transferSyntaxUid = value;
			
			IsImageParameterSetLoaded = true;
        }

		#region Disposal

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(e);
			}
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected abstract void Dispose(bool disposing);

		#endregion 

	}
}
