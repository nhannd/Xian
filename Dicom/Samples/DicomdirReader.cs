#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;

namespace ClearCanvas.Dicom.Samples
{
	/// <summary>
	/// Simple class for reading DICOMDIR files and sending the images they reference to a remote AE.
	/// </summary>
	public class DicomdirReader
	{
		private readonly string _aeTitle;
		private DicomDirectory _dir;
		private int _patientRecords = 0;
		private int _studyRecords = 0;
		private int _seriesRecords = 0;
		private int _instanceRecords = 0;

		public DicomdirReader(string aeTitle)
		{
			_aeTitle = aeTitle;
		}

		public DicomDirectory Dicomdir
		{
			get { return _dir;}
		}

		public int PatientRecords
		{
			get { return _patientRecords; }
			set { _patientRecords = value; }
		}

		public int StudyRecords
		{
			get { return _studyRecords; }
			set { _studyRecords = value; }
		}

		public int SeriesRecords
		{
			get { return _seriesRecords; }
			set { _seriesRecords = value; }
		}

		public int InstanceRecords
		{
			get { return _instanceRecords; }
			set { _instanceRecords = value; }
		}

		/// <summary>
		/// Load a DICOMDIR
		/// </summary>
		/// <param name="filename"></param>
		public void Load(string filename)
		{
			try
			{
				_dir = new DicomDirectory(_aeTitle);

				_dir.Load(filename);


				// Show a simple traversal
				foreach (DirectoryRecordSequenceItem patientRecord in _dir.RootDirectoryRecordCollection)
				{
					PatientRecords++;
					foreach (DirectoryRecordSequenceItem studyRecord in patientRecord.LowerLevelDirectoryRecordCollection)
					{
						StudyRecords++;
						foreach (DirectoryRecordSequenceItem seriesRecord in studyRecord.LowerLevelDirectoryRecordCollection)
						{
							SeriesRecords++;
							foreach (DirectoryRecordSequenceItem instanceRecord in seriesRecord.LowerLevelDirectoryRecordCollection)
							{
								InstanceRecords++;
							}
						}
					}
				}

				Logger.LogInfo("Loaded DICOMDIR with {0} Patient Records, {1} Study Records, {2} Series Records, and {3} Image Records",
					PatientRecords,StudyRecords,SeriesRecords,InstanceRecords);

			}
			catch (Exception e)
			{
				Logger.LogErrorException(e, "Unexpected exception reading DICOMDIR: {0}", filename);
			}
		}

		/// <summary>
		/// Send the images of a loaded DICOMDIR to a remote AE.
		/// </summary>
		/// <param name="rootPath"></param>
		/// <param name="aeTitle"></param>
		/// <param name="host"></param>
		/// <param name="port"></param>
		public void Send(string rootPath, string aeTitle, string host, int port)
		{
			if (_dir == null) return;

			StorageScu scu = new StorageScu();

			foreach (DirectoryRecordSequenceItem patientRecord in _dir.RootDirectoryRecordCollection)
			{
				foreach (DirectoryRecordSequenceItem studyRecord in patientRecord.LowerLevelDirectoryRecordCollection)
				{
					foreach (DirectoryRecordSequenceItem seriesRecord in studyRecord.LowerLevelDirectoryRecordCollection)
					{
						foreach (DirectoryRecordSequenceItem instanceRecord in seriesRecord.LowerLevelDirectoryRecordCollection)
						{
							string path = rootPath;

							foreach (string subpath in instanceRecord[DicomTags.ReferencedFileId].Values as string[])
								path = Path.Combine(path, subpath);

							scu.AddFileToSend(path);
						}
					}
				}
			}

			// Do the send
			scu.Send("DICOMDIR", aeTitle, host, port);

		}
	}
}
