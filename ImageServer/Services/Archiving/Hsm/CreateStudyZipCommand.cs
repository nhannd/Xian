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
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using Ionic.Zip;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	/// <summary>
	/// <see cref="CommandBase"/> to create Zip file containing all the dcm files in a study
	/// </summary>
	public class CreateStudyZipCommand : CommandBase
	{
		private readonly string _zipFile;
		private readonly StudyXml _studyXml;
		private readonly string _studyFolder;
		private readonly string _tempFolder;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="zipFile">The path of the zip file to create</param>
		/// <param name="studyXml">The <see cref="StudyXml"/> file describing the contents of the study.</param>
		/// <param name="studyFolder">The folder the study is stored in.</param>
		/// <param name="tempFolder">The folder for tempory files when creating the zip file.</param>
		public CreateStudyZipCommand(string zipFile, StudyXml studyXml, string studyFolder, string tempFolder) : base("Create study zip file",true)
		{
			_zipFile = zipFile;
			_studyXml = studyXml;
			_studyFolder = studyFolder;
			_tempFolder = tempFolder;
		}

		/// <summary>
		/// Do the work.
		/// </summary>
		protected override void OnExecute(CommandProcessor theProcessor)
		{
			using (var zip = new ZipFile(_zipFile))
			{
				zip.ForceNoCompression = !HsmSettings.Default.CompressZipFiles;
				zip.TempFileFolder = _tempFolder;
				zip.Comment = String.Format("Archive for study {0}", _studyXml.StudyInstanceUid);
				zip.UseZip64WhenSaving = Zip64Option.AsNecessary;

				// Add the studyXml file
				zip.AddFile(Path.Combine(_studyFolder,String.Format("{0}.xml",_studyXml.StudyInstanceUid)), String.Empty);

				// Add the studyXml.gz file
				zip.AddFile(Path.Combine(_studyFolder, String.Format("{0}.xml.gz", _studyXml.StudyInstanceUid)), String.Empty);

			    string uidMapXmlPath = Path.Combine(_studyFolder, "UidMap.xml");
                if (File.Exists(uidMapXmlPath))
                    zip.AddFile(uidMapXmlPath, String.Empty);

				// Add each sop from the StudyXmlFile
				foreach (SeriesXml seriesXml in _studyXml)
					foreach (InstanceXml instanceXml in seriesXml)
					{
						string filename = Path.Combine(_studyFolder, seriesXml.SeriesInstanceUid);
						filename = Path.Combine(filename, String.Format("{0}.dcm", instanceXml.SopInstanceUid));

						zip.AddFile(filename, seriesXml.SeriesInstanceUid);
					}

				zip.Save();
			}
		}

		/// <summary>
		/// Undo the work.
		/// </summary>
		protected override void OnUndo()
		{
			if (File.Exists(_zipFile))
				File.Delete(_zipFile);
		}
	}
}
