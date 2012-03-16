#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// Commands for updating a Dicom file.
	/// </summary>
	/// <remark>
	/// A dicom file level update command consists of one or more <see cref="IDicomFileUpdateCommandAction"/>
	/// </remark>
	public class UpdateDicomFileCommand : CommandBase, IDisposable
	{
		#region Private Members
		private readonly DicomFileUpdateCommandActionList _actionList;
		private string _backupExistingFileName;
		private bool _backup;
		bool _saved;

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
		public DicomFile DicomFile { get; set; }

		/// <summary>
		/// Sets or gets the path where the updated file will be saved. 
		/// </summary>
		/// <remarks>
		/// If this property is not set, the <see cref="DicomFile"/> will be updated but will not be saved.
		/// </remarks>
		public string OutputFilePath { get; set; }

		public List<DicomAttribute> AffectedAttributes { get; private set; }

		#endregion

		public  void Dispose()
		{
			if (File.Exists(_backupExistingFileName))
				FileUtils.Delete(_backupExistingFileName); 
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			Platform.CheckForNullReference(DicomFile, "DicomFile");

			AffectedAttributes = new List<DicomAttribute>();
			foreach(IDicomFileUpdateCommandAction action in _actionList)
			{
				AffectedAttributes.AddRange(action.Apply(DicomFile));
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