using System;
using ClearCanvas.Dicom.Data;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.Dicom.Data
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
		protected override void Load()
		{
			if (!base.IsLoaded)
			{
				OFCondition status = _fileFormat.loadFile(_filename, E_TransferSyntax.EXS_Unknown, E_GrpLenEncoding.EGL_noChange, OffisDcm.DCM_MaxReadLength, false);
				DicomHelper.CheckReturnValue(status, _filename);

				base.IsLoaded = true;
				base.Dataset = _fileFormat.getDataset();

				base.Load();
			}
		}

		protected override void Unload()
		{
			_fileFormat.Dispose();
		}
	}

}
