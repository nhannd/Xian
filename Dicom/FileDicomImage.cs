using System;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.Dicom
{
	public class FileDicomImage : DicomImage
	{
		// Private attributes
		private DcmFileFormat _fileFormat = new DcmFileFormat();
		private string _filename;

		// Properties
		public string Filename { get { return _filename; } }

		// Constructor
		public FileDicomImage(string filename)
		{
			_filename = filename;
		}

		// Protected methods
		protected override void LoadDataset()
		{
			if (!base.IsDatasetLoaded)
			{
				OFCondition status = _fileFormat.loadFile(_filename, E_TransferSyntax.EXS_Unknown, E_GrpLenEncoding.EGL_noChange, 64, false);
				DicomHelper.CheckReturnValue(status, _filename);

				base.IsDatasetLoaded = true;
				base.Dataset = _fileFormat.getDataset();
			}
		}

		protected override void UnloadDataset()
		{
			_fileFormat.Dispose();
		}
	}

}
