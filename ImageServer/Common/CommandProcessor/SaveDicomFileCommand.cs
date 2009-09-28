#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom;
using System;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
	/// <summary>
	/// Specific exception if a file exists when saving.
	/// </summary>
	public class InstanceAlreadyExistsException : Exception
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message">Message describing the exception.</param>
		public InstanceAlreadyExistsException(string message)
			: base(message)
		{}

        /// <summary>
        /// The Series Instance UID of the existing instance.
        /// </summary>        
		public string SeriesInstanceUid { get; set; }
		
        /// <summary>
        /// The SOP Instance UID of the existing instance.
        /// </summary>
		public string SopInstanceUid { get; set; }
	}

	/// <summary>
	/// Class for saving a DicomFile instance in memory to disk.
	/// </summary>
	public class SaveDicomFileCommand : ServerCommand, IDisposable, IAggregateServerCommand
	{
        public delegate string GetFilePathDelegateMethod();

		#region Private Members
		private string _path;
        private string _backupPath;
		private readonly DicomFile _file;
		private readonly bool _failOnExists;
		private readonly bool _saveTemp;
		private bool _fileCreated = false;
		private readonly StudyStorageLocation _storageLocation = null;
	    readonly RateStatistics _backupSpeed = new RateStatistics("BackupSpeed", RateType.BYTES);
	    readonly RateStatistics _saveSpeed = new RateStatistics("SaveSpeed", RateType.BYTES);
		private readonly Stack<IServerCommand> _aggregateStack = new Stack<IServerCommand>();
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="path">The path to save the file.</param>
		/// <param name="file">The file to save.</param>
		/// <param name="failOnExists">If the file already exists, the file will save.</param>
		/// <param name="saveTemp">Save the file to a temporary file first, then move to the final file.  This 
		/// reduces the likelyhood of having partial files if a crash in the service occurs when the file is being written.</param>
		public SaveDicomFileCommand(string path, DicomFile file, bool failOnExists, bool saveTemp)
			: base("Save DICOM Message", true)
		{
			Platform.CheckForNullReference(path, "File name");
			Platform.CheckForNullReference(file, "Dicom File object");

			_path = path;
			_file = file;
			_failOnExists = failOnExists;
			_saveTemp = saveTemp;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="file">The file to save.</param>
		/// <param name="failOnExists">If the file already exists, the file will save.</param>
		/// <param name="saveTemp">Save the file to a temporary file first, then move to the final file.  This 
		/// reduces the likelyhood of having partial files if a crash in the service occurs when the file is being written.</param>
		/// <param name="location">The storage location for the file.</param>
		public SaveDicomFileCommand(StudyStorageLocation location, DicomFile file, bool failOnExists, bool saveTemp)
			: base("Save DICOM Message", true)
		{
			Platform.CheckForNullReference(file, "Dicom File object");
			_storageLocation = location;
			_path = null;
			_file = file;
			_failOnExists = failOnExists;
			_saveTemp = saveTemp;
		}

		public Stack<IServerCommand> AggregateCommands
		{
			get { return _aggregateStack; }
		}

	    private void Backup()
	    {
            if (File.Exists(_path))
            {
				if (_failOnExists)
					throw new InstanceAlreadyExistsException(String.Format("DICOM File unexpectedly already exists: {0}", _path));
				try
				{

                    _backupSpeed.Start();
                    FileInfo fi = new FileInfo(_path);
                    _backupPath = FileUtils.Backup(_path);
                    _backupSpeed.SetData(fi.Length);
                    _backupSpeed.End();
				}
				catch (IOException)
				{
					_backupPath = null;
					throw;
				}
            }
	    }
		private string GetTempPath()
		{
			int count = 0;
			string path;
			
			path = String.Format("{0}_tmp", _path);

			while (File.Exists(path))
			{
				DateTime creationTime = File.GetCreationTime(path);
				DateTime currentTime = Platform.Time;
				// Arbitrary check of 12 hour old file.  if the file is more than 12 hours old,
				// we're assuming its an orphan, and an error occured when creating, so it can 
				// be overwritten.
				if (creationTime < currentTime.AddHours(-12.0d))
				{
					//TODO: FileUtils.Delete does throw exceptions ... shouldn't we just keep iterating?
					FileUtils.Delete(path);
					return path;
				}

				count++;
				path = String.Format("{0}_{1}tmp", _path, count);
			}

			return path;
		}

		protected override void OnExecute(ServerCommandProcessor theProcessor)
		{
			if (_path == null)
			{
				String seriesUid = _file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
				String sopUid = _file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);
				_path = _storageLocation.GetSopInstancePath(seriesUid, sopUid);
			}

			// Make sure the directory exists where we're storing the file.
			if (!Directory.Exists(Path.GetDirectoryName(_path)))
			{
				if (!theProcessor.ExecuteSubCommand(this, new CreateDirectoryCommand(Path.GetDirectoryName(_path))))
					throw new ApplicationException(theProcessor.FailureReason);
			}

            if (RequiresRollback)
                Backup();

	    	string path;
			if (_saveTemp)
			{
				path = GetTempPath();
			}
			else
				path = _path;

	    	using (FileStream stream = FileStreamOpener.OpenForSoleUpdate(path, FileMode.Create))
			{
				// Set _fileCreated here, because the file has been opened.
				if (!_saveTemp)
					_fileCreated = true;

                _saveSpeed.Start();
				_file.Save(stream, DicomWriteOptions.Default);
				stream.Flush();
				stream.Close();
                _saveSpeed.End();

                FileInfo fi = new FileInfo(path);
                _saveSpeed.SetData(fi.Length);
                
			}

			if (_saveTemp)
			{
				if (File.Exists(_path))
				{
					if (_failOnExists)
					{
						try
						{
							FileUtils.Delete(path);
						}
						catch
						{}
						throw new ApplicationException(String.Format("DICOM File unexpectedly already exists: {0}", _path));
					}
					else
						FileUtils.Delete(_path);
				}

				File.Move(path, _path);
				_fileCreated = true;
			}
		}

		protected override void OnUndo()
		{
            if (File.Exists(_path) && _fileCreated) 
                File.Delete(_path); 
            
            if (false==String.IsNullOrEmpty(_backupPath) && File.Exists(_backupPath))
            {
                // restore original file
                File.Copy(_backupPath, _path, true);
            }            
		}

        #region IDisposable Members

        public void Dispose()
        {
            if (false == String.IsNullOrEmpty(_backupPath) && File.Exists(_backupPath))
            {
                File.Delete(_backupPath);
            }
        }

        #endregion
    }
}