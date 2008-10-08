﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.832
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.ImageViewer.Shreds {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SR {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SR() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ClearCanvas.ImageViewer.Shreds.SR", typeof(SR).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dicom Server.
        /// </summary>
        internal static string DicomServer {
            get {
                return ResourceManager.GetString("DicomServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This shred hosts the Dicom Server and the WCF Dicom Server service.
        /// </summary>
        internal static string DicomServerDescription {
            get {
                return ResourceManager.GetString("DicomServerDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Diskspace Manager.
        /// </summary>
        internal static string DiskspaceManager {
            get {
                return ResourceManager.GetString("DiskspaceManager", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This shred hosts the Diskspace Manager service.
        /// </summary>
        internal static string DiskspaceManagerDescription {
            get {
                return ResourceManager.GetString("DiskspaceManagerDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;bad file&apos; storage directory is inaccessible.  The service has been disabled..
        /// </summary>
        internal static string ExceptionBadFileStorageDirectoryDoesNotExist {
            get {
                return ResourceManager.GetString("ExceptionBadFileStorageDirectoryDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cancellation of at least one of the specified items has failed..
        /// </summary>
        internal static string ExceptionCancellationOfAtLeastOneItemFailed {
            get {
                return ResourceManager.GetString("ExceptionCancellationOfAtLeastOneItemFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot delete the specified study (StudyInstanceUid = {0}) because it could not be found in the data store..
        /// </summary>
        internal static string ExceptionCannotDeleteStudyDoesNotExist {
            get {
                return ResourceManager.GetString("ExceptionCannotDeleteStudyDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot perform deletion while reindex is in progress..
        /// </summary>
        internal static string ExceptionCannotDeleteWhileReindexIsInProgress {
            get {
                return ResourceManager.GetString("ExceptionCannotDeleteWhileReindexIsInProgress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot perform a reindex operation while a deletion is in progress..
        /// </summary>
        internal static string ExceptionCannotReindexWhileDeletionIsInProgress {
            get {
                return ResourceManager.GetString("ExceptionCannotReindexWhileDeletionIsInProgress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot start a new import operation while reindex is still running..
        /// </summary>
        internal static string ExceptionCannotStartNewImportJobWhileReindexing {
            get {
                return ResourceManager.GetString("ExceptionCannotStartNewImportJobWhileReindexing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified drive &apos;{0}&apos; is not available or does not exist..
        /// </summary>
        internal static string ExceptionDriveIsNotValid {
            get {
                return ResourceManager.GetString("ExceptionDriveIsNotValid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred while attempting to refresh the Local Data Store activities..
        /// </summary>
        internal static string ExceptionErrorAttemptingToRefresh {
            get {
                return ResourceManager.GetString("ExceptionErrorAttemptingToRefresh", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to process delete request..
        /// </summary>
        internal static string ExceptionErrorProcessingDeleteRequest {
            get {
                return ResourceManager.GetString("ExceptionErrorProcessingDeleteRequest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error has occurred while attempting to process the file import request..
        /// </summary>
        internal static string ExceptionErrorProcessingImportRequest {
            get {
                return ResourceManager.GetString("ExceptionErrorProcessingImportRequest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error has occurred while attempting to process a received file ({0}).
        /// </summary>
        internal static string ExceptionErrorProcessingReceivedFile {
            get {
                return ResourceManager.GetString("ExceptionErrorProcessingReceivedFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred while processing the &apos;receive error&apos; operation..
        /// </summary>
        internal static string ExceptionErrorProcessingReceiveError {
            get {
                return ResourceManager.GetString("ExceptionErrorProcessingReceiveError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error has occurred while attempting to process the reindex request..
        /// </summary>
        internal static string ExceptionErrorProcessingReindexRequest {
            get {
                return ResourceManager.GetString("ExceptionErrorProcessingReindexRequest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred while processing the &apos;retrieve started&apos; operation..
        /// </summary>
        internal static string ExceptionErrorProcessingRetrieveStarted {
            get {
                return ResourceManager.GetString("ExceptionErrorProcessingRetrieveStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred while processing the &apos;send error&apos; operation..
        /// </summary>
        internal static string ExceptionErrorProcessingSendError {
            get {
                return ResourceManager.GetString("ExceptionErrorProcessingSendError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred while processing the &apos;send started&apos; operation..
        /// </summary>
        internal static string ExceptionErrorProcessingSendStarted {
            get {
                return ResourceManager.GetString("ExceptionErrorProcessingSendStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error has occurred while attempting to process a sent file ({0}).
        /// </summary>
        internal static string ExceptionErrorProcessingSentFile {
            get {
                return ResourceManager.GetString("ExceptionErrorProcessingSentFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to retrieve the Local Data Store configuration parameters..
        /// </summary>
        internal static string ExceptionErrorRetrievingLocalDataStoreConfiguration {
            get {
                return ResourceManager.GetString("ExceptionErrorRetrievingLocalDataStoreConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred while attempting to clear the inactive items..
        /// </summary>
        internal static string ExceptionErrorWhileAttemptingToClearInactiveItems {
            get {
                return ResourceManager.GetString("ExceptionErrorWhileAttemptingToClearInactiveItems", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to add subscriber to the Local DataStore Activity Monitor service..
        /// </summary>
        internal static string ExceptionFailedToAddSubscriber {
            get {
                return ResourceManager.GetString("ExceptionFailedToAddSubscriber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to delete study (StudyInstanceUid = {0}) from the data store.  Please check the service log for details..
        /// </summary>
        internal static string ExceptionFailedToDeleteStudy {
            get {
                return ResourceManager.GetString("ExceptionFailedToDeleteStudy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to get Server configuration..
        /// </summary>
        internal static string ExceptionFailedToGetServerConfiguration {
            get {
                return ResourceManager.GetString("ExceptionFailedToGetServerConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error has occurred while attempting to import the file into the local file store..
        /// </summary>
        internal static string ExceptionFailedToImportFile {
            get {
                return ResourceManager.GetString("ExceptionFailedToImportFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to initiate a retrieval of the specified items..
        /// </summary>
        internal static string ExceptionFailedToInitiateRetrieve {
            get {
                return ResourceManager.GetString("ExceptionFailedToInitiateRetrieve", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to initiate a send of the specified items..
        /// </summary>
        internal static string ExceptionFailedToInitiateSend {
            get {
                return ResourceManager.GetString("ExceptionFailedToInitiateSend", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to remove subscriber from the Local DataStore Activity Monitor service..
        /// </summary>
        internal static string ExceptionFailedToRemoveSubscriber {
            get {
                return ResourceManager.GetString("ExceptionFailedToRemoveSubscriber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to update Server configuration..
        /// </summary>
        internal static string ExceptionFailedToUpdateServerConfiguration {
            get {
                return ResourceManager.GetString("ExceptionFailedToUpdateServerConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file importer has already been started..
        /// </summary>
        internal static string ExceptionImporterAlreadyStarted {
            get {
                return ResourceManager.GetString("ExceptionImporterAlreadyStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file importer is not running..
        /// </summary>
        internal static string ExceptionImporterNotStarted {
            get {
                return ResourceManager.GetString("ExceptionImporterNotStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Patient Id cannot be empty..
        /// </summary>
        internal static string ExceptionInvalidPatientId {
            get {
                return ResourceManager.GetString("ExceptionInvalidPatientId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified thread pool &apos;{0}&apos; does not exist..
        /// </summary>
        internal static string ExceptionNamedThreadPoolDoesNotExist {
            get {
                return ResourceManager.GetString("ExceptionNamedThreadPoolDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No files have been specified to import..
        /// </summary>
        internal static string ExceptionNoFilesHaveBeenSpecifiedToImport {
            get {
                return ResourceManager.GetString("ExceptionNoFilesHaveBeenSpecifiedToImport", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No valid files have been specified to import..
        /// </summary>
        internal static string ExceptionNoValidFilesHaveBeenSpecifiedToImport {
            get {
                return ResourceManager.GetString("ExceptionNoValidFilesHaveBeenSpecifiedToImport", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only study level deletion is currently supported..
        /// </summary>
        internal static string ExceptionOnlyStudyLevelDeletionCurrentlySupported {
            get {
                return ResourceManager.GetString("ExceptionOnlyStudyLevelDeletionCurrentlySupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to process request. The service has been disabled..
        /// </summary>
        internal static string ExceptionServiceHasBeenDisabled {
            get {
                return ResourceManager.GetString("ExceptionServiceHasBeenDisabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified retrieve level is unsupported at this time..
        /// </summary>
        internal static string ExceptionSpecifiedRetrieveLevelNotSupported {
            get {
                return ResourceManager.GetString("ExceptionSpecifiedRetrieveLevelNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The storage directory is inaccessible.  The service has been disabled..
        /// </summary>
        internal static string ExceptionStorageDirectoryDoesNotExist {
            get {
                return ResourceManager.GetString("ExceptionStorageDirectoryDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unrecoverable Error: unable to determine drive..
        /// </summary>
        internal static string ExceptionUnableToDetermineDrive {
            get {
                return ResourceManager.GetString("ExceptionUnableToDetermineDrive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Checking disk usage - Used Space(%): {0:F2}, High Watermark(%): {1:F2}, Low Watermark(%): {2:F2}.
        /// </summary>
        internal static string FormatCheckUsage {
            get {
                return ResourceManager.GetString("FormatCheckUsage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Requesting deletion of {0} studies; Expected space freed (bytes): {1}.
        /// </summary>
        internal static string FormatDeletionRequest {
            get {
                return ResourceManager.GetString("FormatDeletionRequest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enumerating: {0}.
        /// </summary>
        internal static string FormatEnumeratingFile {
            get {
                return ResourceManager.GetString("FormatEnumeratingFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to commit a file to the datastore ({0}); This file will need to be imported manually..
        /// </summary>
        internal static string FormatFailedToCommitToDatastore {
            get {
                return ResourceManager.GetString("FormatFailedToCommitToDatastore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file cannot be inserted into the Data Store ({0})..
        /// </summary>
        internal static string FormatFileCannotBeInsertedIntoDataStore {
            get {
                return ResourceManager.GetString("FormatFileCannotBeInsertedIntoDataStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to   The file has been moved to {0}.
        /// </summary>
        internal static string FormatFileHasBeenMoved {
            get {
                return ResourceManager.GetString("FormatFileHasBeenMoved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to + {0}.
        /// </summary>
        internal static string FormatMultipleFilesDescription {
            get {
                return ResourceManager.GetString("FormatMultipleFilesDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Processing: {0}.
        /// </summary>
        internal static string FormatProcessingFile {
            get {
                return ResourceManager.GetString("FormatProcessingFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to total errors: {0}, last error: {1}
        ///Please check the service logs for more details..
        /// </summary>
        internal static string FormatReceiveErrorSummary {
            get {
                return ResourceManager.GetString("FormatReceiveErrorSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} service has failed to start.  Please check the log for more details..
        /// </summary>
        internal static string FormatServiceFailedToStart {
            get {
                return ResourceManager.GetString("FormatServiceFailedToStart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} service has started successfully..
        /// </summary>
        internal static string FormatServiceStartedSuccessfully {
            get {
                return ResourceManager.GetString("FormatServiceStartedSuccessfully", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} service has stopped successfully..
        /// </summary>
        internal static string FormatServiceStoppedSuccessfully {
            get {
                return ResourceManager.GetString("FormatServiceStoppedSuccessfully", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dicom Server Shred (Name: {0} AE: {1} Host: {2} Port: {3} Location: {4}).
        /// </summary>
        internal static string FormatTooltipServerDetails {
            get {
                return ResourceManager.GetString("FormatTooltipServerDetails", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to parse the file ({0}); the sent file information may not be accurate for this study..
        /// </summary>
        internal static string FormatUnableToParseFile {
            get {
                return ResourceManager.GetString("FormatUnableToParseFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} WCF service has failed to start.  Please check the log for more details..
        /// </summary>
        internal static string FormatWCFServiceFailedToStart {
            get {
                return ResourceManager.GetString("FormatWCFServiceFailedToStart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} WCF service has started successfully..
        /// </summary>
        internal static string FormatWCFServiceStartedSuccessfully {
            get {
                return ResourceManager.GetString("FormatWCFServiceStartedSuccessfully", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} WCF service has stopped successfully..
        /// </summary>
        internal static string FormatWCFServiceStoppedSuccessfully {
            get {
                return ResourceManager.GetString("FormatWCFServiceStoppedSuccessfully", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Local Data Store.
        /// </summary>
        internal static string LocalDataStore {
            get {
                return ResourceManager.GetString("LocalDataStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Local Data Store Activity Monitor.
        /// </summary>
        internal static string LocalDataStoreActivityMonitor {
            get {
                return ResourceManager.GetString("LocalDataStoreActivityMonitor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hosts the Local Data Store and Activity Monitor Services.
        /// </summary>
        internal static string LocalDataStoreDescription {
            get {
                return ResourceManager.GetString("LocalDataStoreDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to High Watermark reached; begin removing studies.
        /// </summary>
        internal static string MessageBeginDeleting {
            get {
                return ResourceManager.GetString("MessageBeginDeleting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cancelled..
        /// </summary>
        internal static string MessageCancelled {
            get {
                return ResourceManager.GetString("MessageCancelled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Clearing Data Store  ....
        /// </summary>
        internal static string MessageClearingDatabase {
            get {
                return ResourceManager.GetString("MessageClearingDatabase", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Complete..
        /// </summary>
        internal static string MessageComplete {
            get {
                return ResourceManager.GetString("MessageComplete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed: unable to clear data store prior to reindex operation..
        /// </summary>
        internal static string MessageFailedToClearDatabase {
            get {
                return ResourceManager.GetString("MessageFailedToClearDatabase", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Processing of the bad file failed..
        /// </summary>
        internal static string MessageFailedToProcessBadFile {
            get {
                return ResourceManager.GetString("MessageFailedToProcessBadFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to   The file has been deleted..
        /// </summary>
        internal static string MessageFileHasBeenDeleted {
            get {
                return ResourceManager.GetString("MessageFileHasBeenDeleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Import has been paused waiting for delete operation to complete..
        /// </summary>
        internal static string MessageImportPausedForDelete {
            get {
                return ResourceManager.GetString("MessageImportPausedForDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Import has been paused waiting for reindex to complete..
        /// </summary>
        internal static string MessageImportPausedForReindex {
            get {
                return ResourceManager.GetString("MessageImportPausedForReindex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inactive..
        /// </summary>
        internal static string MessageInactive {
            get {
                return ResourceManager.GetString("MessageInactive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Complete.  No files could be found to import..
        /// </summary>
        internal static string MessageNoFilesToImport {
            get {
                return ResourceManager.GetString("MessageNoFilesToImport", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Complete.  No files could be found to reindex..
        /// </summary>
        internal static string MessageNoFilesToReindex {
            get {
                return ResourceManager.GetString("MessageNoFilesToReindex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No studies could be found in the datastore to delete.
        /// </summary>
        internal static string MessageNothingToDelete {
            get {
                return ResourceManager.GetString("MessageNothingToDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pausing active import jobs ....
        /// </summary>
        internal static string MessagePausingActiveImportJobs {
            get {
                return ResourceManager.GetString("MessagePausingActiveImportJobs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pending ....
        /// </summary>
        internal static string MessagePending {
            get {
                return ResourceManager.GetString("MessagePending", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This operation appears to have failed.  This can be caused by a number of problems including an incorrect Dicom configuration (client or server) or the local firewall blocking the inbound Dicom port. .
        /// </summary>
        internal static string MessageRetrieveLikelyFailed {
            get {
                return ResourceManager.GetString("MessageRetrieveLikelyFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Waiting for all files to become available ....
        /// </summary>
        internal static string MessageWaitingForFilesToBecomeAvailable {
            get {
                return ResourceManager.GetString("MessageWaitingForFilesToBecomeAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Study Locator.
        /// </summary>
        internal static string StudyLocator {
            get {
                return ResourceManager.GetString("StudyLocator", resourceCulture);
            }
        }
    }
}
