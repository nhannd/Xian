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
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.Dicom.Utilities.Command
{
	/// <summary>
	/// Insert DICOM file into a <see cref="StudyXml"/> file and save to disk.
	/// </summary>
	public class InsertInstanceXmlCommand : CommandBase
	{
		#region Private Members

		private readonly StudyXml _stream;
		private readonly string _path;
	    private string _seriesInstanceUid;
	    private string _sopInstanceUid;
		#endregion


		#region Constructors

		public InsertInstanceXmlCommand(StudyXml stream, string path)
			: base("Insert into Study XML", true)
		{
			Platform.CheckForNullReference(stream, "StudyStream object");
			Platform.CheckForNullReference(path, "Path to DICOM File");

			_stream = stream;
			_path = path;
		}

		#endregion

		
		#region Overridden Protected Methods

		protected override void OnExecute(CommandProcessor theProcessor)
		{
		    if (!File.Exists(_path))
			{
				Platform.Log(LogLevel.Error, "Unexpected error finding file to add to XML {0}", _path);
				throw new ApplicationException("Unexpected error finding file to add to XML {0}" + _path);
			}

			var finfo = new FileInfo(_path);
			long fileSize = finfo.Length;

			var dicomFile = new DicomFile(_path);
			dicomFile.Load(DicomReadOptions.StorePixelDataReferences | DicomReadOptions.Default);

		    _sopInstanceUid = dicomFile.DataSet[DicomTags.SopInstanceUid];
		    _seriesInstanceUid = dicomFile.DataSet[DicomTags.SeriesInstanceUid];

			// Setup the insert parameters
			if (false == _stream.AddFile(dicomFile, fileSize))
			{
				Platform.Log(LogLevel.Error, "Unexpected error adding SOP to XML Study Descriptor for file {0}",
				             _path);
				throw new ApplicationException("Unexpected error adding SOP to XML Study Descriptor for SOP: " +
				                               dicomFile.MediaStorageSopInstanceUid);
			}
		}

		protected override void OnUndo()
		{
		    _stream.RemoveInstance(_seriesInstanceUid, _sopInstanceUid);
		}

		#endregion
	}
}