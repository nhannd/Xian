#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Anonymization;
using ClearCanvas.Common;
using System.IO;
using ClearCanvas.ImageViewer.Services.Auditing;
using System.Threading;
using Path=System.IO.Path;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Export
{
	public class DicomFileExporter
	{
		private readonly ICollection<string> _files;
		private DicomAnonymizer _anonymizer;
		private volatile bool _overwrite;
		private volatile bool _canceled;
		private volatile AuditedInstances _exportedInstances;
		private SynchronizationContext _synchronizationContext;

		public DicomFileExporter(ICollection<string> files)
		{
			Platform.CheckPositive(files.Count, "files.Count");
			_files = files;
		}

		public bool Anonymize { get; set; }
		public string OutputPath { get; set; }

		public bool Export()
		{
			if (!Initialize())
				return false;

			EventResult result = EventResult.Success;

			try
			{
				BackgroundTask task = new BackgroundTask(DoExport, true);
				ProgressDialog.Show(task, Application.ActiveDesktopWindow, true, ProgressBarStyle.Continuous);
				
				if (_canceled)
					result = EventResult.MinorFailure;

				return !_canceled;
			}
			catch
			{
				result = EventResult.MinorFailure;
				throw;
			}
			finally
			{
				AuditHelper.LogExportStudies(_exportedInstances, EventSource.CurrentUser, result);
			}
		}

		private bool Initialize()
		{
			_synchronizationContext = SynchronizationContext.Current;

			_exportedInstances = new AuditedInstances();
			_canceled = false;
			_overwrite = false;

			if (Anonymize)
			{
				ExportComponent component = new ExportComponent();
				component.OutputPath = OutputPath;

				if (DialogBoxAction.Ok != Application.ActiveDesktopWindow.ShowDialogBox(component, SR.Export))
					return false;

				OutputPath = component.OutputPath;

				StudyData studyData = new StudyData
				{
					PatientId = component.PatientId,
					PatientsNameRaw = component.PatientsName,
					PatientsBirthDate = component.PatientsDateOfBirth,
					StudyId = component.StudyId,
					StudyDescription = component.StudyDescription,
					AccessionNumber = component.AccessionNumber,
					StudyDate = component.StudyDate
				};

				_anonymizer = new DicomAnonymizer();
				_anonymizer.ValidationOptions = ValidationOptions.RelaxAllChecks;
				_anonymizer.StudyDataPrototype = studyData;
			}
			else
			{
				SelectFolderDialogCreationArgs args = new SelectFolderDialogCreationArgs();
				args.Prompt = SR.MessageSelectOutputLocation;
				args.Path = OutputPath;

				FileDialogResult result = Application.ActiveDesktopWindow.ShowSelectFolderDialogBox(args);
				if (result.Action != DialogBoxAction.Ok)
					return false;

				OutputPath = result.FileName;
			}

			return true;
		}

		private void SaveFile(string filename)
		{
			if (_anonymizer != null)
			{
				DicomFile dicomFile = new DicomFile(filename);
				dicomFile.Load(); 

				_anonymizer.Anonymize(dicomFile);

				//anonymize first, then audit, since this is what gets exported.
				_exportedInstances.AddInstance(
				dicomFile.DataSet[DicomTags.PatientId].ToString(),
				dicomFile.DataSet[DicomTags.PatientsName].ToString(),
				dicomFile.DataSet[DicomTags.StudyInstanceUid].ToString(),
				filename);
				
				string fileName = System.IO.Path.Combine(OutputPath, dicomFile.MediaStorageSopInstanceUid);
				fileName += ".dcm";
				CheckFileExists(fileName); // this will never happen for anonymized images.
				if (_canceled)
					return;

				dicomFile.Save(fileName);
			}
			else
			{
				_exportedInstances.AddPath(filename, false);

				string destination = Path.Combine(OutputPath, Path.GetFileName(filename));
				CheckFileExists(destination);
				if (_canceled)
					return;

				File.Copy(filename, destination, true);
			}
		}

		private void CheckFileExists(string fileName)
		{
			if (_overwrite || !File.Exists(fileName))
				return;

			_synchronizationContext.Send(
				delegate
				{
					_canceled = DialogBoxAction.No == Application.ActiveDesktopWindow.ShowMessageBox(SR.MessageConfirmOverwriteFiles, MessageBoxActions.YesNo);
				}, null);

			_overwrite = !_canceled;
		}

		private void DoExport(IBackgroundTaskContext context)
		{
			try
			{
				int i = 0;
				int fileCount = _files.Count;

				foreach (string filename in _files)
				{
					string message = String.Format(SR.MessageFormatExportingFiles, i + 1, fileCount);
					BackgroundTaskProgress progress = new BackgroundTaskProgress(i, fileCount, message);
					context.ReportProgress(progress);

					SaveFile(filename);
					
					if (_canceled || context.CancelRequested)
					{
						_canceled = true;
						context.Cancel();
						return;
					}

					i++;
				}

				context.Complete();
			}
			catch (Exception e)
			{
				context.Error(e);
			}
		}
	}
}
