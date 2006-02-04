#if UNIT_TESTS

namespace ClearCanvas.Dicom.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using ClearCanvas.Dicom.Network;
    using NUnit.Framework;
using ClearCanvas.Dicom.Exceptions;

    [TestFixture]
    public class DcmNetTest
    {   
        private Process _dicomServer = null;

        [TestFixtureSetUp]
        public void Init()
        {
            // check to see if OFFIS storescp.exe exists
            string programToRun = "C:\\Windows\\storescp.exe";
            string arguments = " -v 104";

            if (!System.IO.File.Exists(programToRun))
                throw new Exception("Could not find the DICOM server program to run tests against");
            
            /* Note: I haven't figured out how to determine whether the dcmnet.dll 
             * dependency is somewhere that NUnit can access and properly load into
             * memory
             * 
            // check to see if the dependent C++ library file exists
            string dependentLibraryFile = "\\dcmnet.dll";
            if (!System.IO.File.Exists(dependentLibraryFile))
                throw new Exception("Could not find the dependent C++ library");
             */

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(programToRun);
                startInfo.Arguments = arguments;

                _dicomServer = Process.Start(startInfo);
            }
            catch (System.Exception e)
            {
                throw new Exception("Could not start the DICOM server", e);
            }
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            if (null != _dicomServer)
            {
                _dicomServer.Kill();
            }
        }

        [Test]
        public void Verify()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(110));
            ApplicationEntity serverAE = new ApplicationEntity(new HostName("localhost"),
                new AETitle("STORESCP"), new ListeningPort(104));

            DicomClient dicomClient = new DicomClient(myOwnAEParameters);

            bool successVerify = dicomClient.Verify(serverAE);

            Assert.IsTrue(successVerify);
        }

        [Test]
        [ExpectedException(typeof(NetworkDicomException))]
        public void VerifyCausesRejection()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(110));
            ApplicationEntity serverAE = new ApplicationEntity(new HostName("localhost"),
                new AETitle("WRONGAE"), new ListeningPort(104));

            DicomClient dicomClient = new DicomClient(myOwnAEParameters);

            bool successVerify = dicomClient.Verify(serverAE);
            Assert.IsFalse(successVerify);
        }

        [Test]
        [ExpectedException(typeof(NetworkDicomException))]
        public void VerifyCausesException()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(110));
            ApplicationEntity serverAE = new ApplicationEntity(new HostName("192.168.0.101"),
                new AETitle("WRONGAE"), new ListeningPort(104));

            DicomClient dicomClient = new DicomClient(myOwnAEParameters);

            bool successVerify = dicomClient.Verify(serverAE);
            Assert.IsFalse(successVerify);
        }

        [Test]
        public void QueryByPatientID()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(110));
            ApplicationEntity serverAE = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CONQUESTSRV1"), new ListeningPort(5678));

            DicomClient dicomClient = new DicomClient(myOwnAEParameters);

            //if (!dicomClient.Verify(serverAE))
            //    throw new Exception("Target server is not running");

            ReadOnlyQueryResultCollection results = dicomClient.Query(serverAE, new PatientID("*"), new PatientsName("*"));

            Assert.IsTrue(results.Count == 1);

            foreach (QueryResult qr in results)
            {
                
                foreach (DicomTag dcmTag in qr.DicomTags)
                {

                }
            }
        }

        [Test]
        public void QueryByAccessionNumber()
        {
        }

        [Test]
        public void QueryByStudyInstanceUid()
        {
        }

        [Test]
        public void QueryByMultipleKeys()
        {
        }
    }
}

#endif