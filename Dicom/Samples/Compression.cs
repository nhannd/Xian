#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Samples
{
	public class Compression
	{
		private readonly string _sourceFilename;
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
				Platform.Log(LogLevel.Error, e, "Unexpected exception loading DICOM file: {0}", _sourceFilename);
			}
		}

		public void ChangeSyntax(TransferSyntax syntax)
		{
			try
			{
				if (!_dicomFile.TransferSyntax.Encapsulated)
				{
					// Check if Overlay is embedded in pixels
					OverlayPlaneModuleIod overlayIod = new OverlayPlaneModuleIod(_dicomFile.DataSet);
					for (int i = 0; i < 16; i++)
					{
						if (overlayIod.HasOverlayPlane(i))
						{
							OverlayPlane overlay = overlayIod[i];
							if (overlay.OverlayData == null)
							{
								DicomUncompressedPixelData pd = new DicomUncompressedPixelData(_dicomFile);
								overlay.ConvertEmbeddedOverlay(pd);	
							}
						}
					}
				}
				else if (syntax.Encapsulated)
				{
					// Must decompress first.
					_dicomFile.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian);
				}

				_dicomFile.ChangeTransferSyntax(syntax);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception compressing/decompressing DICOM file");
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
				Platform.Log(LogLevel.Error, e, "Unexpected exception saving dicom file: {0}", filename);
			}
		}

		public void SavePixels(string filename)
		{
			DicomPixelData pd = DicomPixelData.CreateFrom(_dicomFile);

			if (File.Exists(filename))
				File.Delete(filename);

			using (FileStream fs = new FileStream(filename, FileMode.CreateNew))
			{
				byte[] ba;
				DicomCompressedPixelData compressed = pd as DicomCompressedPixelData;
				if (compressed != null) 
					ba = compressed.GetFrameFragmentData(0);
				else
					ba = pd.GetFrame(0);

				fs.Write(ba, 0, ba.Length);
				fs.Flush();
				fs.Close();
			}
		}
	}
}
