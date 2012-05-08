using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.Dicom.Core
{
    public class DicomFilePublishingException : Exception
    {
        internal DicomFilePublishingException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public static class DicomFilePublisher
    {
        private static void DeleteEmptyFolders(string directory)
        {
            var directoryInfo = new DirectoryInfo(directory);
            if (!directoryInfo.Exists)
                return;

            foreach (DirectoryInfo subDirectory in directoryInfo.GetDirectories())
            {
                try
                {
                    FileInfo[] files = subDirectory.GetFiles();
                    if (files.Length == 0)
                        subDirectory.Delete();
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Info, e, "Failed to delete old temp directory ({0}).", subDirectory);
                }
            }
        }

        private static string FixTempDirectoryPrefix(string prefix)
        {
            //replace directory separators with spaces.
            prefix = prefix.Replace('\\', ' ');
            prefix = prefix.Replace('/', ' ');

            //Replace the wildcard characters as well.
            prefix = prefix.Replace('*', ' ');
            prefix = prefix.Replace('?', ' ');

            char[] invalidChars = Path.GetInvalidPathChars();
            foreach (char invalidChar in invalidChars)
                prefix = prefix.Replace(invalidChar, ' '); //replace invalid characters with spaces.

            return prefix;
        }

        private static string GetTempDirectory(string path, string prefix)
        {
            prefix = FixTempDirectoryPrefix(prefix);
            DateTime dateTime = DateTime.Now;

            int number = 0;
            string tempDirectory;
            do
            {
                tempDirectory = String.Format("{0}.{1}.{2}", prefix, dateTime.Date.ToString("yyyyMMdd"), ++number);
                tempDirectory = Path.Combine(path, tempDirectory);
            } while (Directory.Exists(tempDirectory));

            return tempDirectory;
        }

        private static void SaveFiles(IEnumerable<DicomFile> files, string tempDirectoryPrefix, out string tempDirectory, out List<string> savedFiles)
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "ClearCanvas");
            tempDirectory = Path.Combine(tempDirectory, "Publishing");

            DeleteEmptyFolders(tempDirectory);
            tempDirectory = GetTempDirectory(tempDirectory, tempDirectoryPrefix);
            Directory.CreateDirectory(tempDirectory);
            savedFiles = new List<string>();

            foreach (DicomFile file in files)
            {
                string savePath = Path.Combine(tempDirectory, file.DataSet[DicomTags.SopInstanceUid] + ".dcm");
                file.Save(savePath);
                savedFiles.Add(savePath);
            }
        }

        private static AuditedInstances GetAuditedInstances(IEnumerable<DicomFile> files, bool includeFilename)
        {
            var instances = new AuditedInstances();
            foreach (var file in files)
            {
                var patientId = file.DataSet[DicomTags.PatientId].ToString();
                var patientsName = file.DataSet[DicomTags.PatientsName].ToString();
                var studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].ToString();
                instances.AddInstance(patientId, patientsName, studyInstanceUid, includeFilename ? file.Filename : null);
            }
            return instances;
        }

        private static DicomServerConfiguration GetServerConfiguration()
        {
            DicomServerConfiguration configuration = null;
            var request = new GetDicomServerConfigurationRequest();
            Platform.GetService<IDicomServerConfiguration>(s =>
                                                           configuration = s.GetConfiguration(request).Configuration);

            return configuration;
        }

        public static void PublishLocal(ICollection<DicomFile> files)
        {
            if (files == null || files.Count == 0)
                return;

            var configuration = GetServerConfiguration();
            var context = new ImportStudyContext(configuration.AETitle, StudyStore.GetConfiguration());
            var utility = new ImportFilesUtility(context);

            // setup auditing information
            var result = EventResult.Success;
            var auditedInstances = GetAuditedInstances(files, true);
            try
            {
                foreach (var file in files)
                {
                    var importResult = utility.Import(file, BadFileBehaviourEnum.Ignore, FileImportBehaviourEnum.Save);
                    if (importResult.DicomStatus != DicomStatuses.Success)
                    {
                        Platform.Log(LogLevel.Warn, "Unable to import published file: {0}", importResult.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {                
                result = EventResult.MajorFailure;

                var message = string.Format("Failed to import files");
                throw new DicomFilePublishingException(message, ex);
            }
            finally
            {
                //TODO (Marmot) Move this to the SopInstanceImporter & pass the current user through the Request?
                // audit attempt to import these instances to local store
                AuditHelper.LogImportStudies(auditedInstances, EventSource.CurrentUser, result);
            }            
        }

        public static void PublishRemote(ICollection<DicomFile> files, IDicomServiceNode destinationServer)
        {
            if (files == null || files.Count == 0)
                return;

            // cache files to temporary storage
            string tempFileDirectory;
            List<string> savedFiles;
            SaveFiles(files, destinationServer.AETitle, out tempFileDirectory, out savedFiles);

            try
            {
                var client = new DicomSendClient();
                client.PublishFiles(destinationServer, new StudyRootStudyIdentifier(CollectionUtils.FirstElement(files).DataSet),
                                    DeletionBehaviour.DeleteOnSuccess, savedFiles);
            }
            catch (Exception ex)
            {
                var message = string.Format("Failed to connect to the dicom send service to send files.  The files must be published manually (location: {0})", tempFileDirectory);
                throw new DicomFilePublishingException(message, ex);
            }
        }
    }
}
