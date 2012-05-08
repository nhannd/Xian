#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core.Command;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core
{

    /// <summary>
    /// Class used for returning result information when processing.  Used for importing.
    /// </summary>
    public class DicomProcessingResult
    {
        public String AccessionNumber;
        public String StudyInstanceUid;
        public String SeriesInstanceUid;
        public String SopInstanceUid;
        public bool Successful;
        public String ErrorMessage;
        public DicomStatus DicomStatus;
        public bool RestoreRequested;        

        public void SetError(DicomStatus status, String message)
        {
            Successful = false;
            DicomStatus = status;
            ErrorMessage = message;
        }
    }

    /// <summary>
    /// A context object used when importing a batch of DICOM SOP Instances from a DICOM association.
    /// </summary>
    public class DicomReceiveImportContext : ImportFilesContext
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceAE">The AE title of the remote application sending the SOP Instances.</param>
        /// <param name="configuration">Storage configuration. </param>
        public DicomReceiveImportContext(string sourceAE, StorageConfiguration configuration) : base(sourceAE, configuration)
        {    
        }


        /// <summary>
        /// Create a StudyProcessRequest object for a specific SOP Instance.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override ProcessStudyRequest CreateRequest(DicomMessageBase message)
        {
            var request = new DicomReceiveRequest
                              {
                                  FromAETitle = SourceAE,
                                  Type = ImportType,
                                  Priority = WorkItemPriorityEnum.High,
                                  Patient = new WorkItemPatient(message.DataSet),
                                  Study = new WorkItemStudy(message.DataSet)
                              };

            return request;
        }
    }

    /// <summary>
    /// A context object used when importing a batch of DICOM SOP Instances from disk.
    /// </summary>
    public class ImportStudyContext : ImportFilesContext
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceAE">The local AE title of the application importing the studies.</param>
        /// <param name="configuration">The storage configuration. </param>
        public ImportStudyContext(string sourceAE, StorageConfiguration configuration)
            : base(sourceAE, configuration)
        {
        }

        /// <summary>
        /// Create a <see cref="ProcessStudyRequest"/> object for a specific SOP Instnace.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override ProcessStudyRequest CreateRequest(DicomMessageBase message)
        {
            var request = new ImportStudyRequest
                              {
                                  Type = ImportType,
                                  Priority = WorkItemPriorityEnum.High,
                                  Patient = new WorkItemPatient(message.DataSet),
                                  Study = new WorkItemStudy(message.DataSet)
                              };
            return request;
        }
    }

    /// <summary>
    /// Encapsulates the context of the application when <see cref="ImportFilesUtility"/> is called.
    /// </summary>
    public abstract class ImportFilesContext
    {
     
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ImportFilesContext"/> to be used
        /// by <see cref="ImportFilesUtility"/> 
        /// </summary>
        protected ImportFilesContext(string sourceAE, StorageConfiguration configuration)
        {
            StudyWorkItems = new ObservableDictionary<string, WorkItem>();
            ImportType = WorkItemTypeEnum.ProcessStudy;
            SourceAE = sourceAE;
            StorageConfiguration = configuration;
            ExpirationDelaySeconds = 45;
        }

        #endregion

        /// <summary>
        /// Gets the source AE title where the image(s) are imported from
        /// </summary>
        public string SourceAE { get; private set; }

        /// <summary>
        /// The Type of WorkItem
        /// </summary>
        public WorkItemTypeEnum ImportType { get; private set; }

        /// <summary>
        /// Map of the studies and corresponding WorkItems items for the current context
        /// </summary>
        public ObservableDictionary<string,WorkItem> StudyWorkItems { get; private set; }

        /// <summary>
        /// Abstract method for creating a <see cref="ProcessStudyRequest"/> object for the given DICOM message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract ProcessStudyRequest CreateRequest(DicomMessageBase message);

        /// <summary>
        /// Storage configuration.
        /// </summary>
        public StorageConfiguration  StorageConfiguration { get; private set; }

        /// <summary>
        /// Delay to expire inserted WorkItems
        /// </summary>
        public int ExpirationDelaySeconds { get; set; }
    }

    /// <summary>
    /// Import utility for importing specific SOP Instances, either in memory from the network or on disk.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that the files being imported do not have to belong to the same study.  ImportFilesUtility will 
    /// automatically detect the study the files belong to, and import them to the proper location.
    /// </para>
    /// </remarks>
    public class ImportFilesUtility
    {
        #region Private Members
        private readonly ImportFilesContext _context; 
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ImportFilesUtility"/> to import DICOM object(s)
        /// into the system.
        /// </summary>
        /// <param name="context">The context of the operation.</param>
        public ImportFilesUtility(ImportFilesContext context)
        {
            Platform.CheckForNullReference(context, "context");
            _context = context;
        } 
        #endregion

        #region Public Methods

        /// <summary>
        /// Imports the specified <see cref="DicomMessageBase"/> object into the system.
        /// The object will be inserted into the <see cref="WorkItem"/> for processing
        /// </summary>
        /// <param name="message">The DICOM object to be imported.</param>
        /// <param name="badFileBehavior"> </param>
        /// <param name="fileImportBehaviour"> </param>
        /// <returns>An instance of <see cref="DicomProcessingResult"/> that describes the result of the processing.</returns>
        /// <exception cref="DicomDataException">Thrown when the DICOM object contains invalid data</exception>
        public DicomProcessingResult Import(DicomMessageBase message, BadFileBehaviourEnum badFileBehavior, FileImportBehaviourEnum fileImportBehaviour)
        {
            Platform.CheckForNullReference(message, "message");
            String studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);
            String seriesInstanceUid = message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
            String sopInstanceUid = message.DataSet[DicomTags.SopInstanceUid].GetString(0, string.Empty);
            String accessionNumber = message.DataSet[DicomTags.AccessionNumber].GetString(0, string.Empty);
            String patientsName = message.DataSet[DicomTags.PatientsName].GetString(0, string.Empty);

            // TODO (Marmot) - Not sure if we want to do anythign about this.
            // Scrub the name for invalid characters.
            //string newName = XmlUtils.XmlCharacterScrub(patientsName);
            string newName = patientsName;
            if (!newName.Equals(patientsName))
                message.DataSet[DicomTags.PatientsName].SetStringValue(newName);

			var result = new DicomProcessingResult
			                               	{
			                               		Successful = true,
			                               		StudyInstanceUid = studyInstanceUid,
			                               		SeriesInstanceUid = seriesInstanceUid,
			                               		SopInstanceUid = sopInstanceUid,
			                               		AccessionNumber = accessionNumber
			                               	};

        	try
			{
				Validate(message);
			}
			catch (DicomDataException e)
			{
				result.SetError(DicomStatuses.ProcessingFailure, e.Message);
				return result;
			}

            if (_context.StorageConfiguration.IsMaximumUsedSpaceExceeded)
            {
                result.SetError(DicomStatuses.StorageStorageOutOfResources,
                                string.Format("Unable to import, file store used percent: {0}, maximum used percent: {1}",
                                              _context.StorageConfiguration.FileStoreDiskSpace.UsedSpacePercent.ToString("00.000"),
                                              _context.StorageConfiguration.MaximumUsedSpacePercent.ToString("00.000")));
                return result;
            }

            // Use the command processor for rollback capabilities.
            using (var commandProcessor = new ViewerCommandProcessor(String.Format("Processing Sop Instance {0}", sopInstanceUid)))
            {
                try
                {
                    var studyLocation = new StudyLocation(message);

                	String destinationFile = studyLocation.GetSopInstancePath(seriesInstanceUid, sopInstanceUid);
                    
                    DicomFile file = ConvertToDicomFile(message, destinationFile, _context.SourceAE);

                    // Create the Study Folder, if need be
                    commandProcessor.AddCommand(new CreateDirectoryCommand(studyLocation.StudyFolder));

                    bool duplicateFile = false;
                    string dupName = Guid.NewGuid().ToString() + ".dcm";

                    if (File.Exists(destinationFile))
                    {
                        duplicateFile = true;
                        destinationFile = Path.Combine(Path.GetDirectoryName(destinationFile), dupName);
                    }

                    if (fileImportBehaviour == FileImportBehaviourEnum.Move)
                    {
                        commandProcessor.AddCommand(new RenameFileCommand(file.Filename, destinationFile, true));
                    }
                    else if (fileImportBehaviour == FileImportBehaviourEnum.Copy)
                    {
                        commandProcessor.AddCommand(new CopyFileCommand(file.Filename, destinationFile, true));
                    }
                    else if (fileImportBehaviour == FileImportBehaviourEnum.Save)
                    {
                        commandProcessor.AddCommand(new SaveDicomFileCommand(destinationFile, file, true));
                    }

                    InsertWorkItemCommand command;                 
                    if (duplicateFile)
                    {
                        WorkItem workItem;
                        command = _context.StudyWorkItems.TryGetValue(studyInstanceUid, out workItem)
                            ? new InsertWorkItemCommand(workItem, studyInstanceUid, seriesInstanceUid, sopInstanceUid, dupName)
                            : new InsertWorkItemCommand(_context.CreateRequest(file), new ProcessStudyProgress(),  studyInstanceUid, seriesInstanceUid, sopInstanceUid, dupName);             
                    }
                    else
                    {
                        WorkItem workItem;
                        command = _context.StudyWorkItems.TryGetValue(studyInstanceUid, out workItem)
                            ? new InsertWorkItemCommand(workItem, studyInstanceUid, seriesInstanceUid, sopInstanceUid)
                            : new InsertWorkItemCommand(_context.CreateRequest(file), new ProcessStudyProgress {TotalFilesToProcess = 1}, studyInstanceUid, seriesInstanceUid, sopInstanceUid);                        
                    }
                    command.ExpirationDelaySeconds = _context.ExpirationDelaySeconds;
                    commandProcessor.AddCommand(command);

                	if (commandProcessor.Execute())
                	{
                		result.DicomStatus = DicomStatuses.Success;

                	    WorkItem workItem;
                        if (!_context.StudyWorkItems.TryGetValue(studyInstanceUid, out workItem))
                        {
                            _context.StudyWorkItems.Add(studyInstanceUid, command.WorkItem);

                            WorkItemPublishSubscribeHelper.PublishWorkItemChanged(WorkItemHelper.FromWorkItem(command.WorkItem));
                        }
                        else
                        {
                            var progress = command.WorkItem.Progress as ProcessStudyProgress;
                            if (progress != null)
                            {
                                progress.TotalFilesToProcess++;
                                command.WorkItem.Progress = progress;
                                WorkItemPublishSubscribeHelper.PublishWorkItemChanged(WorkItemHelper.FromWorkItem(command.WorkItem));
                            }
                            // Save the updated WorkItem
                            _context.StudyWorkItems[studyInstanceUid] = command.WorkItem;
                        }
                	}
                	else
                	{
                        Platform.Log(LogLevel.Warn, "Failure Importing file: {0}", file.Filename);
                		string failureMessage = String.Format("Failure processing message: {0}. Sending failure status.",
                		                                      commandProcessor.FailureReason);
                		result.SetError(DicomStatuses.ProcessingFailure, failureMessage);
                		// processor already rolled back
                		return result;
                	}
                }
               
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.", commandProcessor.Description);
                    commandProcessor.Rollback();
                    result.SetError(result.DicomStatus ?? DicomStatuses.ProcessingFailure, e.Message);
                }
            }

            return result;
        }

        #endregion

        #region Private Methods
       
        private static void Validate(DicomMessageBase message)
        {
            //TODO (Marmot)
        }

        static private DicomFile ConvertToDicomFile(DicomMessageBase message, string filename, string sourceAE)
        {
            // This routine sets some of the group 0x0002 elements.
            DicomFile file;
            if (message is DicomFile)
            {
                file = message as DicomFile;
            }
            else if (message is DicomMessage)
            {
                file = new DicomFile(message as DicomMessage, filename);
            }
            else
            {
                throw new NotSupportedException(String.Format("Cannot convert {0} to DicomFile", message.GetType()));
            }

            file.SourceApplicationEntityTitle = sourceAE;
            file.TransferSyntax = message.TransferSyntax.Encapsulated ? 
				message.TransferSyntax : TransferSyntax.ExplicitVrLittleEndian;

            return file;
        }
        
        #endregion
	}
}
