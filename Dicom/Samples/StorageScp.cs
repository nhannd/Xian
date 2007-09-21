using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.Dicom.Samples
{
    /// <summary>
    /// DICOM Storage SCP Sample Application
    /// </summary>
    class StorageScp : IDicomServerHandler
    {
        #region Private Members
        private static bool _started = false;
        private static String _staticStorageLocation;
        private static ServerAssociationParameters _staticAssocParameters;
        private ServerAssociationParameters _assocParameters;
        #endregion

        #region Constructors
        private StorageScp(ServerAssociationParameters assoc)
        {
            _assocParameters = assoc;
        }
        #endregion

        #region Public Properties
        public static bool Started
        {
            get { return _started; }
        }

        /// <summary>
        /// The path (directory) to store incoming images.
        /// </summary>
        public static String StorageLocation
        {
            get { return _staticStorageLocation; }
            set { _staticStorageLocation = value; }
        }

        #endregion

        #region Private Methods
        private static void AddPresentationContexts(ServerAssociationParameters assoc)
        {
            byte pcid = assoc.AddPresentationContext(SopClass.VerificationSopClass);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.MrImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.CtImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.SecondaryCaptureImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.UltrasoundImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.UltrasoundImageStorageRetired);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.UltrasoundMultiFrameImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.UltrasoundMultiFrameImageStorageRetired);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.NuclearMedicineImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.DigitalIntraOralXRayImageStorageForPresentation);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.DigitalIntraOralXRayImageStorageForProcessing);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.DigitalMammographyXRayImageStorageForPresentation);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.DigitalMammographyXRayImageStorageForProcessing);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.DigitalXRayImageStorageForPresentation);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.DigitalXRayImageStorageForProcessing);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.ComputedRadiographyImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.GrayscaleSoftcopyPresentationStateStorageSopClass);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.KeyObjectSelectionDocument);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.OphthalmicPhotography16BitImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.OphthalmicPhotography8BitImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.VideoEndoscopicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.VideoMicroscopicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.VideoPhotographicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.VlEndoscopicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.VlMicroscopicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.VlPhotographicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.VlSlideCoordinatesMicroscopicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.XRayAngiographicBiPlaneImageStorageRetired);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.XRayAngiographicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.XRayRadiofluoroscopicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.XRayRadiationDoseSr);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.ChestCadSr);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);
        }
        #endregion

        #region Static Public Methods
        public static void StartListening(string aeTitle, int port)
        {
            if (_started)
                return;

            _staticAssocParameters = new ServerAssociationParameters(aeTitle, new IPEndPoint(IPAddress.Any, port));

            AddPresentationContexts(_staticAssocParameters);

            DicomServer.StartListening(_staticAssocParameters, delegate(ServerAssociationParameters assoc) 
                {
                    return new StorageScp(assoc);
                } );

            _started = true;
        }

        public static void StopListening(int port)
        {
            if (_started)
            {
                DicomServer.StopListening(_staticAssocParameters);
                _started = false;
            }
        }
        #endregion


        #region IDicomServerHandler Members

        void IDicomServerHandler.OnReceiveAssociateRequest(DicomServer server, ServerAssociationParameters association)
        {
            server.SendAssociateAccept(association);
        }

        void IDicomServerHandler.OnReceiveRequestMessage(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {

            if (message.CommandField == DicomCommandField.CEchoRequest)
            {
                server.SendCEchoResponse(presentationID, message.MessageId, DicomStatuses.Success);
                return;
            }

            String studyInstanceUid = null;
            String seriesInstanceUid = null;
            DicomUid sopInstanceUid = null;
            
            bool ok = message.DataSet[DicomTags.SopInstanceUid].TryGetUid(0, out sopInstanceUid);
            if (ok) ok = message.DataSet[DicomTags.SeriesInstanceUid].TryGetString(0, out seriesInstanceUid);
            if (ok) ok = message.DataSet[DicomTags.StudyInstanceUid].TryGetString(0, out studyInstanceUid);

            if (!ok)
            {
                DicomLogger.LogError("Unable to retrieve UIDs from request message, sending failure status.");

                server.SendCStoreResponse(presentationID, message.MessageId, sopInstanceUid.UID,
                    DicomStatuses.ProcessingFailure);
                return;
            }

            if (!Directory.Exists(StorageScp.StorageLocation))
                Directory.CreateDirectory(StorageScp.StorageLocation);

            StringBuilder path = new StringBuilder();
            path.AppendFormat("{0}{1}{2}{3}{4}", StorageScp.StorageLocation,  Path.DirectorySeparatorChar,
                studyInstanceUid, Path.DirectorySeparatorChar,seriesInstanceUid);

            Directory.CreateDirectory(path.ToString());

            path.AppendFormat("{0}{1}.dcm", Path.DirectorySeparatorChar, sopInstanceUid.UID);

            DicomFile dicomFile = new DicomFile(message, path.ToString());

            dicomFile.TransferSyntaxUid = TransferSyntax.ExplicitVrLittleEndianUid;
            dicomFile.MediaStorageSopInstanceUid = sopInstanceUid.UID;
            dicomFile.ImplementationClassUid = DicomImplementation.ClassUID.UID;
            dicomFile.ImplementationVersionName = DicomImplementation.Version;
            dicomFile.SourceApplicationEntityTitle = association.CallingAE;
            dicomFile.MediaStorageSopClassUid = message.SopClass.Uid;
            
            dicomFile.Save(DicomWriteOptions.None);

            String patientName = dicomFile.DataSet[DicomTags.PatientsName].GetString(0, "");
            DicomLogger.LogInfo("Received SOP Instance: {0} for patient {1}", sopInstanceUid, patientName);

            server.SendCStoreResponse(presentationID, message.MessageId,
                sopInstanceUid.UID, 
                DicomStatuses.Success);
        }

        void IDicomServerHandler.OnReceiveResponseMessage(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
            DicomLogger.LogError("Unexpectedly received response mess on server.");

            server.SendAssociateAbort(DicomAbortSource.ServiceUser, DicomAbortReason.UnrecognizedPDU);
        }

        void IDicomServerHandler.OnReceiveReleaseRequest(DicomServer server, ServerAssociationParameters association)
        {
            DicomLogger.LogInfo("Received association release request from  {0}.", association.CallingAE);
        }

        void IDicomServerHandler.OnReceiveAbort(DicomServer server, ServerAssociationParameters association, DicomAbortSource source, DicomAbortReason reason)
        {
            DicomLogger.LogError("Unexpected association abort received.");
        }

        void IDicomServerHandler.OnNetworkError(DicomServer server, ServerAssociationParameters association, Exception e)
        {
            DicomLogger.LogErrorException(e, "Unexpected network error over association from {0}.", association.CallingAE);
        }

        void IDicomServerHandler.OnDimseTimeout(DicomServer server, ServerAssociationParameters association)
        {
            DicomLogger.LogInfo("Received DIMSE Timeout, continuing listening for messages");
        }

        #endregion
    }
}
