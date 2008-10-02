using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Samples
{
	public class Compression
	{
		private string _sourceFilename;
		private DicomFile _dicomFile;

		public Compression(string file)
		{
			_sourceFilename = file;
		}

		public DicomFile DicomFile
		{
			get { return _dicomFile; }
		}

		public void Load()
		{
			_dicomFile = new DicomFile(_sourceFilename);
			try
			{
				_dicomFile.Load();
			}
			catch (Exception e)
			{
				Logger.LogErrorException(e, "Unexpected exception loading DICOM file: {0}", _sourceFilename);
			}
		}

		public void ChangeSyntax(TransferSyntax syntax)
		{
			try
			{
				_dicomFile.ChangeTransferSyntax(syntax);
			}
			catch (Exception e)
			{
				Logger.LogErrorException(e, "Unexpected exception compressing/decompressing DICOM file");
			}
		}

		public void Save(string filename)
		{
			try
			{
				_dicomFile.Save(filename);
			}
			catch (Exception e)
			{
				Logger.LogErrorException(e, "Unexpected exception saving dicom file: {0}", filename);
			}
		}
	}
}
