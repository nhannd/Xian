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
    /// DICOM Storage SCU Sample Application
    /// </summary>
    public class StorageScu : IDicomClientHandler
    {
        #region Private Classes and Structures
        struct FileToSend
        {
            public String filename;
            public SopClass sopClass;
            public DicomUid transferSyntaxUid;
        }
        #endregion

        #region Private Members
        private List<FileToSend> _fileList = new List<FileToSend>();
        private int _fileListIndex = 0;
        private ClientAssociationParameters _assocParams = null;
        private DicomClient _dicomClient = null;
        #endregion

        #region Constructors
        public StorageScu()
        {
        }
        #endregion

        #region Private Methods
        private bool LoadDirectory(DirectoryInfo dir)
        {
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                AddFileToSend(file.FullName);
            }

            String[] subdirectories = Directory.GetDirectories(dir.FullName);
            foreach (String subPath in subdirectories)
            {
                DirectoryInfo subDir = new DirectoryInfo(subPath);
                LoadDirectory(subDir);
            }

            return true;
        }
        #endregion

        #region Public Methods
        public bool AddFileToSend(String file)
        {

            try
            {
                DicomFile dicomFile = new DicomFile(file);

                // Only load to specific character set to reduce amount of data read from file
                dicomFile.Load(DicomTags.SpecificCharacterSet, DicomReadOptions.Default);

                FileToSend fileStruct = new FileToSend();

                fileStruct.filename = file;
                fileStruct.sopClass = dicomFile.SopClass;
                fileStruct.transferSyntaxUid = dicomFile.TransferSyntax.DicomUid;
                if (dicomFile.TransferSyntax.Encapsulated)
                {
                    DicomLogger.LogError("Unsupported encapsulated transfer syntax in file: {0}.  Not sending file.", dicomFile.TransferSyntax.Name);
                    return false;
                }

                _fileList.Add(fileStruct);
            }
            catch (DicomException e)
            {
                DicomLogger.LogErrorException(e, "Unexpected exception when loading file for sending: {0}", file);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add all the files to send.
        /// </summary>
        /// <param name="directory">The path of the directory to scan for DICOM files</param>
        /// <returns></returns>
        public bool AddDirectoryToSend(String directory)
        {
            DirectoryInfo dir = new DirectoryInfo(directory);

			return LoadDirectory(dir);
		}

        /// <summary>
        /// Scan the files to send, and create presentation contexts for each abstract syntax to send.
        /// </summary>
        public void SetPresentationContexts()
        {
            foreach (FileToSend sendStruct in _fileList)
            {
                byte pcid = _assocParams.FindAbstractSyntax(sendStruct.sopClass);

                if (pcid == 0)
                {
                    pcid = _assocParams.AddPresentationContext(sendStruct.sopClass);

                    _assocParams.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
                    _assocParams.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);
                }
            }
        }

        public void Send(String remoteAE, String host, int port)
        {
            if (_dicomClient == null)
            {
                if (_fileList.Count == 0)
                {
                    DicomLogger.LogInfo("Not sending, no files to send.");
                    return;
                }

                DicomLogger.LogInfo("Preparing to connect to AE {0} on host {1} on port {2} and sending {3} images.", remoteAE, host, port, _fileList.Count);

                try
                {
                    IPAddress addr = null;
                    foreach (IPAddress dnsAddr in Dns.GetHostAddresses(host))
                        if (dnsAddr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            addr = dnsAddr;
                            break;
                        }
                    if (addr == null)
                    {
                        DicomLogger.LogError("No Valid IP addresses for host {0}", host);
                        return;
                    }
                    _assocParams = new ClientAssociationParameters("StorageSCU", remoteAE, new IPEndPoint(addr, port));

                    SetPresentationContexts();

                    _dicomClient = DicomClient.Connect(_assocParams, this);
                }
                catch (Exception e)
                {
                    DicomLogger.LogErrorException(e, "Unexpected exception trying to connect to Remote AE {0} on host {1} on port {2}", remoteAE, host, port);
                }
            }
        }

        /// <summary>
        /// Generic routine to send the next C-STORE-RQ message in the _fileList.
        /// </summary>
        /// <param name="client">DICOM Client class</param>
        /// <param name="association">Association Parameters</param>
        public bool SendCStore(DicomClient client, ClientAssociationParameters association)
        {
            FileToSend fileToSend = _fileList[_fileListIndex];

            DicomFile dicomFile = new DicomFile(fileToSend.filename);

            try
            {
                dicomFile.Load(DicomReadOptions.Default);
            }
            catch (DicomException e)
            {
                DicomLogger.LogErrorException(e, "Unexpected exception when loading DICOM file {0}",fileToSend.filename);

                return false;
            }

            DicomMessage msg = new DicomMessage(dicomFile);

            byte pcid = association.FindAbstractSyntax(fileToSend.sopClass);

            client.SendCStoreRequest(pcid, client.NextMessageID(), DicomPriority.Medium, msg);
            return true;
        }
        #endregion


        #region IDicomClientHandler Members

        public void OnReceiveAssociateAccept(DicomClient client, ClientAssociationParameters association)
        {
            DicomLogger.LogInfo("Association Accepted:\r\n{0}", association.ToString());

            _fileListIndex = 0;

            bool ok = SendCStore(client, association);
            while (ok == false)
            {
                _fileListIndex++;
                if (_fileListIndex >= _fileList.Count)
                {
                    DicomLogger.LogInfo("Completed sending C-STORE-RQ messages, releasing association.");
                    client.SendReleaseRequest();
                    return;
                }
                ok = SendCStore(client, association);
            }
        }

        public void OnReceiveAssociateReject(DicomClient client, ClientAssociationParameters association, DicomRejectResult result, DicomRejectSource source, DicomRejectReason reason)
        {
            DicomLogger.LogInfo("Association Rejection when {0} connected to remote AE {1}", association.CallingAE, association.CalledAE);
        }

        public void OnReceiveRequestMessage(DicomClient client, ClientAssociationParameters association, byte presentationID, DicomMessage message)
        {
            DicomLogger.LogError("Unexpected OnReceiveRequestMessage callback on client.");

            throw new Exception("The method or operation is not implemented.");
        }

        public void OnReceiveResponseMessage(DicomClient client, ClientAssociationParameters association, byte presentationID, DicomMessage message)
        {
            if (message.Status.Status != DicomState.Success)
            {
                DicomLogger.LogError("Failure status received in sending C-STORE: {0}", message.Status.Description);
            }

            bool ok = false;
            while (ok == false)
            {
                _fileListIndex++;
                if (_fileListIndex >= _fileList.Count)
                {
                    DicomLogger.LogInfo("Completed sending C-STORE-RQ messages, releasing association.");
                    client.SendReleaseRequest();
                    return;
                }

                ok = SendCStore(client, association);
            }
        }

        public void OnReceiveReleaseResponse(DicomClient client, ClientAssociationParameters association)
        {
            DicomLogger.LogInfo("Association released to {0}", association.CalledAE);
        }

        public void OnReceiveAbort(DicomClient client, ClientAssociationParameters association, DicomAbortSource source, DicomAbortReason reason)
        {
            DicomLogger.LogError("Unexpected association abort received from {0}", association.CalledAE);
        }

        public void OnNetworkError(DicomClient client, ClientAssociationParameters association, Exception e)
        {
            DicomLogger.LogErrorException(e, "Unexpected network error");
        }

        public void OnDimseTimeout(DicomClient client, ClientAssociationParameters association)
        {
            DicomLogger.LogInfo("Timeout waiting for response message, continuing.");
        }

        #endregion
    }
}
