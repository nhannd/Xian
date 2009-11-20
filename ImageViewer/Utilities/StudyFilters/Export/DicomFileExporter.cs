using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Anonymization;
using ClearCanvas.Common;
using System.IO;
using ClearCanvas.ImageViewer.Services.Auditing;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Export
{
	public class DicomFileExporter
	{
		private readonly ICollection<FileInfo> _files;
		private DicomAnonymizer _anonymizer;
		private volatile bool _canceled;
		private volatile AuditedInstances _exportedInstances;

		public DicomFileExporter(ICollection<FileInfo> files)
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
				if (_files.Count > 10)
				{
					BackgroundTask task = new BackgroundTask(DoExport, true);
					ProgressDialog.Show(task, Application.ActiveDesktopWindow, true, ProgressBarStyle.Continuous);
					
					if (_canceled)
						result = EventResult.MinorFailure;

					return !_canceled;
				}
				else
				{
					BlockingOperation.Run(DoExport);
					return true;
				}
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
			_exportedInstances = new AuditedInstances();
			_canceled = false;

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

		private void SaveFile(FileInfo file)
		{
			if (_anonymizer != null)
			{
				DicomFile dicomFile = new DicomFile(file.FullName);
				dicomFile.Load(); 

				_anonymizer.Anonymize(dicomFile);

				//anonymize first, then audit, since this is what gets exported.
				_exportedInstances.AddInstance(
				dicomFile.DataSet[DicomTags.PatientId].ToString(),
				dicomFile.DataSet[DicomTags.PatientsName].ToString(),
				dicomFile.DataSet[DicomTags.StudyInstanceUid].ToString(),
				file.FullName);
				
				string fileName = System.IO.Path.Combine(OutputPath, dicomFile.MediaStorageSopInstanceUid);
				fileName += ".dcm";
				dicomFile.Save(fileName);
			}
			else
			{
				_exportedInstances.AddPath(file.FullName, false);

				string newpath = System.IO.Path.Combine(OutputPath, file.Name);
				if (!File.Exists(newpath))
					file.CopyTo(newpath);
			}
		}

		private void DoExport()
		{
			foreach (FileInfo file in _files)
				SaveFile(file);
		}

		private void DoExport(IBackgroundTaskContext context)
		{
			try
			{
				int i = 0;
				int fileCount = _files.Count;

				foreach (FileInfo file in _files)
				{
					string message = String.Format(SR.MessageFormatExportingFiles, i + 1, fileCount);
					BackgroundTaskProgress progress = new BackgroundTaskProgress(i, fileCount, message);

					SaveFile(file);

					context.ReportProgress(progress);
					if (context.CancelRequested)
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
