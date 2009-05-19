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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// Commands for updating a Dicom file.
	/// </summary>
	/// <remark>
	/// A dicom file level update command consists of one or more <see cref="IDicomFileUpdateCommandAction"/>
	/// </remark>
	public class UpdateDicomFileCommand : ServerCommand, IDisposable
	{
		#region Private Members
		private readonly DicomFileUpdateCommandActionList _actionList;
		private string _outputFilePath;
		private string _backupExistingFileName;
		private bool _backup = false;
		bool _saved;
		private DicomFile _file;
		private List<DicomAttribute> _affectedAttributes;
		#endregion

		#region Construtors
		/// <summary>
		/// Creates an instance of <see cref="UpdateDicomFileCommand"/> with the specified actions.
		/// </summary>
		/// <param name="actionList">The actions to be applied to the Dicom file</param>
		public UpdateDicomFileCommand(DicomFileUpdateCommandActionList actionList)
			: base("Update Dicom File", true)
		{
			_actionList = actionList;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Sets or gets the Dicom file to be updated
		/// </summary>
		public DicomFile DicomFile
		{
			get { return _file; }
			set { _file = value; }
		}

		/// <summary>
		/// Sets or gets the path where the updated file will be saved. 
		/// </summary>
		/// <remarks>
		/// If this property is not set, the <see cref="DicomFile"/> will be updated but will not be saved.
		/// </remarks>
		public string OutputFilePath
		{
			get { return _outputFilePath; }
			set { _outputFilePath = value; }
		}

		public IEnumerable<DicomAttribute> AffectedAttributes
		{
			get { return _affectedAttributes; }
		}

		#endregion

		public  void Dispose()
		{
			if (File.Exists(_backupExistingFileName))
				FileUtils.Delete(_backupExistingFileName); 
		}

		protected override void OnExecute()
		{
			Platform.CheckForNullReference(DicomFile, "DicomFile");

			_affectedAttributes = new List<DicomAttribute>();
			foreach(IDicomFileUpdateCommandAction action in _actionList)
			{
				_affectedAttributes.AddRange(action.Apply(DicomFile));
			}

			if (!String.IsNullOrEmpty(OutputFilePath))
			{
				if (File.Exists(OutputFilePath))
				{
					// backup the file first for undo purpose
					_backupExistingFileName = OutputFilePath + "." + Path.GetRandomFileName();
					File.Move(OutputFilePath, _backupExistingFileName);
					_backup = true;
				} 
                
				DicomFile.Save(OutputFilePath);

				_saved = true;
				Debug.Assert(_backupExistingFileName != null);
			}

			Platform.Log(LogLevel.Debug, "Dicom file updated");
		}


		protected override void OnUndo()
		{
			if (!String.IsNullOrEmpty(OutputFilePath) && _saved)
			{
				FileUtils.Delete(OutputFilePath);

				if (_backup)
				{
					// restore the backup file
					Debug.Assert(_backupExistingFileName != null);
					File.Move(_backupExistingFileName, OutputFilePath);
				}
			}

			//TODO: We may want to also undo changes the DicomFile object.
		}
	}

	/// <summary>
	/// Defines the interface of an action at the Dicom file level.
	/// </summary>
	public interface IDicomFileUpdateCommandAction
	{
		IEnumerable<DicomAttribute> Apply(DicomFile file);
	}

	/// <summary>
	/// List of <see cref="IDicomFileUpdateCommandAction"/>
	/// </summary>
	public class DicomFileUpdateCommandActionList : List<IDicomFileUpdateCommandAction> { }
}