using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

using ClearCanvas.ImageServer.Dicom;
using ClearCanvas.ImageServer.Dicom.Network;

namespace ClearCanvas.ImageServer.Dicom.Samples
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
            byte pcid = assoc.AddPresentationContext(SopClass.MRImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.CTImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.SecondaryCaptureImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.UltrasoundImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.UltrasoundImageStorageRetired);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.UltrasoundMultiframeImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.UltrasoundMultiframeImageStorageRetired);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.NuclearMedicineImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.DigitalIntraoralXRayImageStorageForPresentation);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.DigitalIntraoralXRayImageStorageForProcessing);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.DigitalMammographyXRayImageStorageForPresentation);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.DigitalMammographyXRayImageStorageForProcessing);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.DigitalXRayImageStorageForPresentation);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.DigitalXRayImageStorageForProcessing);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.ComputedRadiographyImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.GrayscaleSoftcopyPresentationStateStorageSOPClass);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.KeyObjectSelectionDocument);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.OphthalmicPhotography16BitImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.OphthalmicPhotography8BitImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.VideoEndoscopicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.VideoMicroscopicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.VideoPhotographicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.VLEndoscopicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.VLMicroscopicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.VLPhotographicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.VLSlideCoordinatesMicroscopicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.XRayAngiographicBiPlaneImageStorageRetired);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.XRayAngiographicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.XRayRadiofluoroscopicImageStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.XRayRadiationDoseSR);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.ChestCADSR);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);


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

            String studyInstanceUid = null;
            String seriesInstanceUid = null;
            DicomUid sopInstanceUid = null;
            
            bool ok = message.DataSet[DicomTags.SOPInstanceUID].TryGetUid(0, out sopInstanceUid);
            if (ok) ok = message.DataSet[DicomTags.SeriesInstanceUID].TryGetString(0, out seriesInstanceUid);
            if (ok) ok = message.DataSet[DicomTags.StudyInstanceUID].TryGetString(0, out studyInstanceUid);

            if (!ok)
            {
                DicomLogger.LogError("Unable to retrieve UIDs from request message, sending failure status.");

                server.SendCStoreResponse(presentationID, message.MessageId, sopInstanceUid,
                    DicomStatuses.ProcessingFailure);
                return;
            }


            StringBuilder path = new StringBuilder();
            path.AppendFormat("{0}{1}{2}{3}{4}", StorageScp.StorageLocation,  Path.DirectorySeparatorChar,
                studyInstanceUid, Path.DirectorySeparatorChar,seriesInstanceUid);

            Directory.CreateDirectory(path.ToString());

            path.AppendFormat("{0}{1}.dcm", Path.DirectorySeparatorChar, sopInstanceUid.UID);

            DicomFile dicomFile = new DicomFile(message, path.ToString());

            dicomFile.TransferSyntaxUid = TransferSyntax.ExplicitVRLittleEndianUid;
            dicomFile.MediaStorageSopInstanceUid = sopInstanceUid.UID;
            dicomFile.ImplementationClassUid = DicomImplementation.ClassUID.UID;
            dicomFile.ImplementationVersionName = DicomImplementation.Version;
            dicomFile.SourceApplicationEntityTitle = association.CallingAE;
            dicomFile.MediaStorageSopClassUid = message.SopClass.Uid;
            
            dicomFile.Save(DicomWriteOptions.None);

            DicomLogger.LogInfo("Received SOP Instance: {0}", sopInstanceUid);

            server.SendCStoreResponse(presentationID, message.MessageId,
                sopInstanceUid, 
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
