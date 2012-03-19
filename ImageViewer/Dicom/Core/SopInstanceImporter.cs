#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Dicom.Core.Command;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Dicom.Core
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
    public class DicomReceiveImportContext : SopInstanceImporterContext
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceAE">The AE title of the remote application sending the SOP Instances.</param>
        public DicomReceiveImportContext(string sourceAE) : base(sourceAE)
        {    
        }


        /// <summary>
        /// Create a StudyProcessRequest object for a specific SOP Instance.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override StudyProcessRequest CreateRequest(DicomMessageBase message)
        {
            var request = new DicomReceiveRequest
                              {
                                  FromAETitle = SourceAE,
                                  Type = ImportType,
                                  Priority = WorkItemPriorityEnum.Stat,
                                  Patient = new PatientRootPatientIdentifier(message.DataSet),
                                  Study = new StudyRootStudyIdentifier(message.DataSet)
                              };

            return request;
        }
    }

    /// <summary>
    /// A context object used when importing a batch of DICOM SOP Instances from disk.
    /// </summary>
    public class ImportStudyContext : SopInstanceImporterContext
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceAE">The local AE title of the application importing the studies.</param>
        public ImportStudyContext(string sourceAE) : base(sourceAE)
        {
        }

        /// <summary>
        /// Create a <see cref="StudyProcessRequest"/> object for a specific SOP Instnace.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override StudyProcessRequest CreateRequest(DicomMessageBase message)
        {
            var request = new ImportStudyRequest
                              {
                                  Type = ImportType,
                                  Priority = WorkItemPriorityEnum.Stat,
                                  Patient = new PatientRootPatientIdentifier(message.DataSet),
                                  Study = new StudyRootStudyIdentifier(message.DataSet)
                              };
            return request;
        }
    }

    /// <summary>
    /// Encapsulates the context of the application when <see cref="SopInstanceImporter"/> is called.
    /// </summary>
    public abstract class SopInstanceImporterContext
    {
     
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="SopInstanceImporterContext"/> to be used
        /// by <see cref="SopInstanceImporter"/> 
        /// </summary>
        protected SopInstanceImporterContext(string sourceAE)
        {
            StudyWorkItems = new Dictionary<string, WorkItem>();
            ImportType = WorkItemTypeEnum.StudyProcess;
            SourceAE = sourceAE;
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
        public Dictionary<string,WorkItem> StudyWorkItems { get; private set; }

        /// <summary>
        /// Abstract method for creating a <see cref="StudyProcessRequest"/> object for the given DICOM message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract StudyProcessRequest CreateRequest(DicomMessageBase message);
    }

    /// <summary>
    /// Import utility for importing specific SOP Instances, either in memory from the network or on disk.
    /// </summary>
    public class SopInstanceImporter
    {
        #region Private Members
        private readonly SopInstanceImporterContext _context; 
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="SopInstanceImporter"/> to import DICOM object(s)
        /// into the system.
        /// </summary>
        /// <param name="context">The context of the operation.</param>
        public SopInstanceImporter(SopInstanceImporterContext context)
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
        /// <returns>An instance of <see cref="DicomProcessingResult"/> that describes the result of the processing.</returns>
        /// <exception cref="DicomDataException">Thrown when the DICOM object contains invalid data</exception>
        public DicomProcessingResult Import(DicomMessageBase message)
        {
            Platform.CheckForNullReference(message, "message");
            String studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);
            String seriesInstanceUid = message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
            String sopInstanceUid = message.DataSet[DicomTags.SopInstanceUid].GetString(0, string.Empty);
            String accessionNumber = message.DataSet[DicomTags.AccessionNumber].GetString(0, string.Empty);
            String patientsName = message.DataSet[DicomTags.PatientsName].GetString(0, string.Empty);

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

            // Use the command processor for rollback capabilities.
            using (var commandProcessor = new ViewerCommandProcessor(String.Format("Processing Sop Instance {0}", sopInstanceUid)))
            {
                try
                {
                    var studyLocation = new StudyLocation(message);

                	String path = studyLocation.StudyFolder;
                	String finalDest = studyLocation.GetSopInstancePath(seriesInstanceUid, sopInstanceUid);
                    
                    DicomFile file = ConvertToDicomFile(message, finalDest, _context.SourceAE);

                    // Create the filestore Folder, if need be
                    commandProcessor.AddCommand(new CreateDirectoryCommand(studyLocation.StudyFolder));

                    // Create the Study folder
                    commandProcessor.AddCommand(new CreateDirectoryCommand(Path.GetDirectoryName(finalDest)));

                    InsertWorkItemCommand command;

                    if (File.Exists(finalDest))
                    {
                        string dupName = Guid.NewGuid().ToString() + ".dcm";
                        string dupPath = Path.Combine(Path.GetDirectoryName(finalDest), dupName);

                        commandProcessor.AddCommand(new SaveDicomFileCommand(dupPath, file, true));

                        WorkItem workItem;
                        command = _context.StudyWorkItems.TryGetValue(studyInstanceUid, out workItem) 
                            ? new InsertWorkItemCommand(workItem, studyInstanceUid, seriesInstanceUid, sopInstanceUid, dupName) 
                            : new InsertWorkItemCommand(_context.CreateRequest(file), studyInstanceUid, seriesInstanceUid, sopInstanceUid, dupName);
                    }
                    else
                    {
                        commandProcessor.AddCommand(new SaveDicomFileCommand(path, file, true));

                        WorkItem workItem;
                        command = _context.StudyWorkItems.TryGetValue(studyInstanceUid, out workItem) 
                            ? new InsertWorkItemCommand(workItem, studyInstanceUid, seriesInstanceUid, sopInstanceUid) 
                            : new InsertWorkItemCommand(_context.CreateRequest(file), studyInstanceUid, seriesInstanceUid, sopInstanceUid);
                    }

                    commandProcessor.AddCommand(command);

                	if (commandProcessor.Execute())
                	{
                		result.DicomStatus = DicomStatuses.Success;

                	    WorkItem workItem;
                        if (!_context.StudyWorkItems.TryGetValue(studyInstanceUid, out workItem) )
                            _context.StudyWorkItems.Add(studyInstanceUid, command.WorkItem);
                	}
                	else
                	{
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
            //TODO
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
