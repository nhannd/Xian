#if UNIT_TESTS
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;

using NUnit.Framework;

using ClearCanvas.ImageServer.Dicom;
using ClearCanvas.ImageServer.Dicom.Network;
using ClearCanvas.ImageServer.Dicom.Exceptions;


namespace ClearCanvas.ImageServer.Dicom.Tests
{
    public enum TestTypes
    {
        AssociationReject,
        AssociationAbort,
        SendMR
    }

    public class ClientHandler : IDicomClientHandler
    {
        private AbstractTest _test;
        private TestTypes _type;
        public ManualResetEvent _threadStop = new ManualResetEvent(false);

        public bool OnClientConnectedCalled = false;
        public bool OnClientClosedCalled = false;

        public ClientHandler(AbstractTest test, TestTypes type)
        {
            _test = test;
            _type = type;
        }

        #region IDicomClientHandler Members

        public void OnDimseTimeout(DicomClient client)
        {
        }

        public void OnClientClosed(DicomClient client)
        {
            OnClientClosedCalled = true;
        }

        public void OnNetworkError(DicomClient client, Exception e)
        {
            Assert.Fail("Incorrectly received OnNetworkError callback");
        }

        public void OnReceiveAssociateAccept(DicomClient client, AssociationParameters association)
        {
            if (_type == TestTypes.AssociationReject)
            {
                Assert.Fail("Unexpected negotiated association on reject test.");
            }
            else if (_type == TestTypes.SendMR)
            {
                DicomMessage msg = new DicomMessage();

                _test.SetupMR(msg.DataSet);
                byte id = client.Associate.FindAbstractSyntaxWithTransferSyntax(msg.SopClass, TransferSyntax.ExplicitVRLittleEndian);

                client.SendCStoreRequest(id, client.NextMessageID(), DicomPriority.Medium, msg);
            }
            else
            {
                Assert.Fail("Unexpected test type");
            }
        }

        public void OnReceiveRequestMessage(DicomClient client, byte presentationID, DicomMessage message)
        {
            Assert.Fail("Incorrectly received OnReceiveRequestMessage callback");
        }

        public void OnReceiveResponseMessage(DicomClient client, byte presentationID, DicomMessage message)
        {
            client.SendReleaseRequest();
            Assert.AreEqual(message.Status.Code, DicomStatuses.Success.Code, "Incorrect DICOM status returned");
        }

        public void OnReceiveReleaseResponse(DicomClient client)
        {
            // Signal the main thread we're exiting
            _threadStop.Set();
        }

        public void OnReceiveAssociateReject(DicomClient client, DicomRejectResult result, DicomRejectSource source, DicomRejectReason reason)
        {
            if (_type == TestTypes.AssociationReject)
            {
                Assert.IsTrue(source == DicomRejectSource.ServiceProviderACSE);
                Assert.IsTrue(result == DicomRejectResult.Permanent);
                Assert.IsTrue(reason == DicomRejectReason.NoReasonGiven);
                _threadStop.Set();
            }
            else
                Assert.Fail("Incorrectly received OnReceiveAssociateReject callback");
        }

        public void OnReceiveAbort(DicomClient client, DicomAbortSource source, DicomAbortReason reason)
        {
            Assert.Fail("Incorrectly received OnReceiveAbort callback");
        }

        #endregion
    }

    public class ServerHandler : IDicomServerHandler
    {
        AbstractTest _test;
        TestTypes _type;

        public ServerHandler(AbstractTest test, TestTypes type)
        {
            _test = test;
            _type = type;
        }
        #region IDicomServerHandler Members

        public void OnClientConnected(DicomServer server)
        {

        }

        public void OnClientClosed(DicomServer server)
        {
            DicomLogger.LogInfo("Client has closed");
        }

        public void OnNetworkError(DicomServer server, Exception e)
        {
            DicomLogger.LogErrorException(e, "Unexpected network error");

            Assert.Fail("Unexpected network error: " + e.Message);
        }

        public void OnDimseTimeout(DicomServer client)
        {
        }

        public void OnReceiveAssociateRequest(DicomServer server, AssociationParameters association)
        {
            server.SendAssociateAccept(association);
        }

        public void OnReceiveRequestMessage(DicomServer server, byte presentationID, DicomMessage message)
        {
            AttributeCollection testSet = new AttributeCollection();

            _test.SetupMR(testSet);

            bool same = testSet.Equals(message.DataSet);


            server.SendCStoreResponse(presentationID, message.MessageId, message.DataSet[DicomTags.SOPInstanceUID].GetUid(0), DicomStatuses.Success);

        }

        public void OnReceiveResponseMessage(DicomServer server, byte presentationID, DicomMessage message)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void OnReceiveReleaseRequest(DicomServer server)
        {

        }

        public void OnReceiveAbort(DicomServer server, DicomAbortSource source, DicomAbortReason reason)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

       
    [TestFixture]
    public class AssociationTests : AbstractTest
    {
        TestTypes _serverType;

        public IDicomServerHandler ServerHandlerCreator(AssociationParameters assoc)
        {
            return new ServerHandler(this,_serverType);
        }  

        [Test]
        public void RejectTests()
        {
            int port = 2112;

            /* Setup the Server */
            ServerAssociationParameters serverParameters = new ServerAssociationParameters("AssocTestServer", new IPEndPoint(IPAddress.Any, port));
            byte pcid = serverParameters.AddPresentationContext(SopClass.MRImageStorage);
            serverParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            serverParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRBigEndian);
            serverParameters.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            _serverType = TestTypes.AssociationReject;
            DicomServer.StartListening(serverParameters, ServerHandlerCreator);

            /* Setup the client */
            ClientAssociationParameters clientParameters = new ClientAssociationParameters("AssocTestClient", "AssocTestServer",
                                                                                new System.Net.IPEndPoint(IPAddress.Loopback, port));
            pcid = clientParameters.AddPresentationContext(SopClass.CTImageStorage);
            clientParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            clientParameters.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            /* Open the association */
            ClientHandler handler = new ClientHandler(this, TestTypes.AssociationReject);
            DicomClient client = DicomClient.Connect(clientParameters, handler);


            handler._threadStop.WaitOne();
            client.Close();

            _serverType = TestTypes.AssociationReject;

            /* Setup the client */
            clientParameters = new ClientAssociationParameters("AssocTestClient", "AssocTestServer",
                                                                new System.Net.IPEndPoint(IPAddress.Loopback, port));
            pcid = clientParameters.AddPresentationContext(SopClass.MRImageStorage);
            clientParameters.AddTransferSyntax(pcid, TransferSyntax.JPEG2000ImageCompressionLosslessOnly);


            /* Open the association */
            ClientHandler clientHandler = new ClientHandler(this, TestTypes.AssociationReject);
            client = DicomClient.Connect(clientParameters, clientHandler);

            handler._threadStop.WaitOne();
            client.Close();


            DicomServer.StopListening(serverParameters);

        }

       

        [Test]
        public void ServerTest()
        {
            int port = 2112;

            /* Setup the Server */
            ServerAssociationParameters serverParameters = new ServerAssociationParameters("AssocTestServer",new IPEndPoint(IPAddress.Any,port));
            byte pcid = serverParameters.AddPresentationContext(SopClass.MRImageStorage);
            serverParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            serverParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRBigEndian);
            serverParameters.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            _serverType = TestTypes.SendMR;
            DicomServer.StartListening(serverParameters, ServerHandlerCreator);

            /* Setup the client */
            ClientAssociationParameters clientParameters = new ClientAssociationParameters("AssocTestClient","AssocTestServer",
                                                                                new System.Net.IPEndPoint(IPAddress.Loopback,port));
            pcid = clientParameters.AddPresentationContext(SopClass.MRImageStorage);
            clientParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            clientParameters.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            pcid = clientParameters.AddPresentationContext(SopClass.CTImageStorage);
            clientParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVRLittleEndian);
            clientParameters.AddTransferSyntax(pcid, TransferSyntax.ImplicitVRLittleEndian);

            /* Open the association */
            ClientHandler handler = new ClientHandler(this,TestTypes.SendMR);
            DicomClient client = DicomClient.Connect(clientParameters, handler);


            handler._threadStop.WaitOne();

            client.Close();

            DicomServer.StopListening(serverParameters);
        }

    }
}
#endif
