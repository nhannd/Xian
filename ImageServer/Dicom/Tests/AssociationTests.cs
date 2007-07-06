using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using ClearCanvas.ImageServer.Dicom;
using ClearCanvas.ImageServer.Dicom.Network;
using ClearCanvas.ImageServer.Dicom.Exceptions;


namespace ClearCanvas.ImageServer.Dicom.Tests
{
    [TestFixture]
    public class AssociationTests
    {
        [Test]
        public void ClientTest()
        {
        }

        public class ClientHandler : IDicomClientHandler
        {
            public bool OnClientConnectedCalled = false;
            public bool OnClientClosedCalled = false;


            #region IDicomClientHandler Members

            public void OnClientClosed(DicomClient client)
            {
                OnClientClosedCalled = true;
            }

            public void OnNetworkError(DicomClient client, Exception e)
            {

                throw new Exception("The method or operation is not implemented.");
            }

            public void OnReceiveAssociateAccept(DicomClient client, AssociationParameters association)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void OnReceiveRequestMessage(DicomClient client, byte presentationID, ushort messageID, DicomMessage message)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void OnReceiveResponseMessage(DicomClient client, byte presentationID, ushort messageID, DcmStatus status, DicomMessage message)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void OnReceiveReleaseResponse(DicomClient client)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void OnReceiveAssociateReject(DicomClient client, DcmRejectResult result, DcmRejectSource source, DcmRejectReason reason)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void OnReceiveAbort(DicomClient client, DcmAbortSource source, DcmAbortReason reason)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        public class ServerHandler : IDicomServerHandler
        {
            #region IDicomServerHandler Members

            public void OnClientConnected(DicomServer server)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void OnClientClosed(DicomServer server)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void OnNetworkError(DicomServer server, Exception e)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void OnReceiveAssociateRequest(DicomServer server, AssociationParameters association)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void OnReceiveRequestMessage(DicomServer server, byte presentationID, ushort messageID, DicomMessage message)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void OnReceiveResponseMessage(DicomServer server, byte presentationID, ushort messageID, DcmStatus status, DicomMessage message)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void OnReceiveReleaseRequest(DicomServer server)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void OnReceiveAbort(DicomServer server, DcmAbortSource source, DcmAbortReason reason)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        public IDicomServerHandler CreateHandler(AssociationParameters assoc)
        {
            return new ServerHandler();
        }

        [Test]
        public void ServerTest()
        {
            int port = 2112;

            ApplicationEntity serverAe = new ApplicationEntity("AssocTestServer");
            ApplicationEntity clientAe = new ApplicationEntity("AssocTestClient");

            DicomServer.StartListening(port, serverAe, CreateHandler);

            AssociationParameters assoc = new AssociationParameters();

            DicomClient client = DicomClient.Connect("localhost",port,clientAe, assoc, new ClientHandler());

            client.Close();

            DicomServer.StopListening(port, serverAe);
        }

    }
}
