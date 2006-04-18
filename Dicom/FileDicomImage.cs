namespace ClearCanvas.Dicom
{
    using System;
    using ClearCanvas.Dicom;
    using ClearCanvas.Dicom.OffisWrapper;

    /// <summary>
    /// Representation of a DICOM image object that is stored on disk
    /// media.
    /// </summary>
	public class FileDicomImage : DicomImage
	{
		// Private attributes
		private DcmFileFormat _fileFormat = new DcmFileFormat();
		private string _filename;

		// Properties
        /// <summary>
        /// The full filesystem path to this image file.
        /// </summary>
		public string Filename { get { return _filename; } }

		// Constructor
        /// <summary>
        /// Construct this object with the filesystem path to the file.
        /// </summary>
        /// <param name="filename">Filesystem path to the location of the image file.</param>
		public FileDicomImage(string filename)
		{
			_filename = filename;
		}

		// Protected methods
        /// <summary>
        /// Loads the dataset from the filesystem into memory.
        /// </summary>
		protected override void LoadDataset()
		{
			if (!base.IsDatasetLoaded)
			{
				OFCondition status = _fileFormat.loadFile(_filename, E_TransferSyntax.EXS_Unknown, E_GrpLenEncoding.EGL_noChange, 64, false);
				DicomHelper.CheckReturnValue(status, _filename);

				base.IsDatasetLoaded = true;
				base.Dataset = _fileFormat.getDataset();
				base.MetaInfo = _fileFormat.getMetaInfo();
			}
		}

        /// <summary>
        /// Unload the dataset from memory, and release system resources.
        /// </summary>
		protected override void UnloadDataset()
		{
			_fileFormat.Dispose();
		}
	}

}
