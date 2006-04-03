#if UNIT_TESTS

namespace ClearCanvas.Dicom.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using ClearCanvas.Dicom.Network;
    using NUnit.Framework;
    using ClearCanvas.Dicom;
    using System.Threading;

    [TestFixture]
    public class DcmNetTest
    {   
        private Process _dicomServer = null;

        [TestFixtureSetUp]
        public void Init()
        {
            // check to see if OFFIS storescp.exe exists
            //string programToRun = "C:\\Windows\\storescp.exe";
            //string arguments = " -v 104";

            //if (!System.IO.File.Exists(programToRun))
            //    throw new Exception("Could not find the DICOM server program to run tests against");
            
            ///* Note: I haven't figured out how to determine whether the dcmnet.dll 
            // * dependency is somewhere that NUnit can access and properly load into
            // * memory
            // * 
            //// check to see if the dependent C++ library file exists
            //string dependentLibraryFile = "\\dcmnet.dll";
            //if (!System.IO.File.Exists(dependentLibraryFile))
            //    throw new Exception("Could not find the dependent C++ library");
            // */

            //try
            //{
            //    ProcessStartInfo startInfo = new ProcessStartInfo(programToRun);
            //    startInfo.Arguments = arguments;

            //    _dicomServer = Process.Start(startInfo);
            //}
            //catch (System.Exception e)
            //{
            //    throw new Exception("Could not start the DICOM server", e);
            //}
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            //if (null != _dicomServer)
            //{
            //    _dicomServer.Kill();
            //}
        }

        [Test]
        public void Verify()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(4000));
            ApplicationEntity serverAE = new ApplicationEntity(new HostName("192.168.0.103"),
                new AETitle("CONQUESTSRV1"), new ListeningPort(5678));

            DicomClient dicomClient = new DicomClient(myOwnAEParameters);

            bool successVerify = dicomClient.Verify(serverAE);

            Assert.IsTrue(successVerify);
        }

        [Test]
        [ExpectedException(typeof(NetworkDicomException))]
        public void VerifyCausesRejection()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(4000));
            ApplicationEntity serverAE = new ApplicationEntity(new HostName("clintondesk"),
                new AETitle("WRONGAE"), new ListeningPort(5678));

            DicomClient dicomClient = new DicomClient(myOwnAEParameters);

            bool successVerify = dicomClient.Verify(serverAE);
            Assert.IsFalse(successVerify);
        }

        [Test]
        [ExpectedException(typeof(NetworkDicomException))]
        public void VerifyCausesException()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(4000));
            ApplicationEntity serverAE = new ApplicationEntity(new HostName("192.168.0.101"),
                new AETitle("WRONGAE"), new ListeningPort(5678));

            DicomClient dicomClient = new DicomClient(myOwnAEParameters);

            bool successVerify = dicomClient.Verify(serverAE);
            Assert.IsFalse(successVerify);
        }

        [Test]
        public void QueryByPatientId()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(4000));
            ApplicationEntity serverAE = new ApplicationEntity(new HostName("192.168.0.100"),
                new AETitle("CONQUESTSRV1"), new ListeningPort(5678));

            DicomClient dicomClient = new DicomClient(myOwnAEParameters);

            if (!dicomClient.Verify(serverAE))
                throw new Exception("Target server is not running");

            ReadOnlyQueryResultCollection results = dicomClient.Query(serverAE, new PatientId("0000002"));

            Assert.IsTrue(results.Count > 0);

            foreach (QueryResult qr in results)
            {   
                foreach (DicomTag dicomTag in qr.DicomTags)
                {
                    Console.WriteLine("{0} - {1}", dicomTag.ToString(), qr[dicomTag]);
                }
                Console.WriteLine("Patient's Name: {0}", qr.PatientsName);
                Console.WriteLine("Patient ID: {0}", qr.PatientId);
            }
        }

        [Test]
        public void QueryByPatientsName()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(4000));
            ApplicationEntity serverAE = new ApplicationEntity(new HostName("192.168.0.100"),
                new AETitle("CONQUESTSRV1"), new ListeningPort(5678));

            DicomClient dicomClient = new DicomClient(myOwnAEParameters);

            if (!dicomClient.Verify(serverAE))
                throw new Exception("Target server is not running");

            ReadOnlyQueryResultCollection results = dicomClient.Query(serverAE, new PatientsName("PATIENT1"));

            Assert.IsTrue(results.Count > 0);

            foreach (QueryResult qr in results)
            {
                foreach (DicomTag dicomTag in qr.DicomTags)
                {
                    Console.WriteLine("{0} - {1}", dicomTag.ToString(), qr[dicomTag]);
                }
                Console.WriteLine("Patient's Name: {0}", qr.PatientsName);
                Console.WriteLine("Patient ID: {0}", qr.PatientId);
            }
        }

        [Test]
        public void QueryByPatientIdWithEvent()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(110));
            ApplicationEntity serverAE = new ApplicationEntity(new HostName("192.168.0.100"),
                new AETitle("CONQUESTSRV1"), new ListeningPort(5678));

            DicomClient dicomClient = new DicomClient(myOwnAEParameters);

            if (!dicomClient.Verify(serverAE))
                throw new Exception("Target server is not running");

            dicomClient.QueryResultReceivedEvent += QueryResultReceivedEventHandler;
            dicomClient.QueryCompletedEvent += QueryCompletedEventHandler;

            ReadOnlyQueryResultCollection results = dicomClient.Query(serverAE, new PatientId(""), new PatientsName(""));

            dicomClient.QueryResultReceivedEvent -= QueryResultReceivedEventHandler;
            dicomClient.QueryCompletedEvent -= QueryCompletedEventHandler;

            Assert.IsTrue(results.Count > 0);
        }

 
        [Test]
        public void QueryByAccessionNumber()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(4000));
            ApplicationEntity serverAE = new ApplicationEntity(new HostName("192.168.0.100"),
                new AETitle("CONQUESTSRV1"), new ListeningPort(5678));

            DicomClient dicomClient = new DicomClient(myOwnAEParameters);

            if (!dicomClient.Verify(serverAE))
                throw new Exception("Target server is not running");

            ReadOnlyQueryResultCollection results = dicomClient.Query(serverAE, new PatientId("0000007"), new Accession(""));

            Assert.IsTrue(results.Count > 0);

            foreach (QueryResult qr in results)
            {
                foreach (DicomTag dicomTag in qr.DicomTags)
                {
                    Console.WriteLine("{0} - {1}", dicomTag.ToString(), qr[dicomTag]);
                }
                Console.WriteLine("Patient's Name: {0}", qr.PatientsName);
                Console.WriteLine("Patient ID: {0}", qr.PatientId);
            }
        }

        [Test]
        public void QueryByAccessionNumber2()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(4000));
            ApplicationEntity serverAE = new ApplicationEntity(new HostName("192.168.0.100"),
                new AETitle("CONQUESTSRV1"), new ListeningPort(5678));

            DicomClient dicomClient = new DicomClient(myOwnAEParameters);

            if (!dicomClient.Verify(serverAE))
                throw new Exception("Target server is not running");

            ReadOnlyQueryResultCollection results = dicomClient.Query(serverAE, new PatientId("0000007"), new PatientsName("PATIENT7"), new Accession("0000000007"));

            Assert.IsTrue(results.Count > 0);

            foreach (QueryResult qr in results)
            {
                foreach (DicomTag dicomTag in qr.DicomTags)
                {
                    Console.WriteLine("{0} - {1}", dicomTag.ToString(), qr[dicomTag]);
                }
                Console.WriteLine("Patient's Name: {0}", qr.PatientsName);
                Console.WriteLine("Patient ID: {0}", qr.PatientId);
            }
        }

        [Test]
        public void QueryByStudyInstanceUid()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(110));
            ApplicationEntity serverAE = new ApplicationEntity(new HostName("192.168.0.100"),
                new AETitle("CONQUESTSRV1"), new ListeningPort(5678));

            DicomClient dicomClient = new DicomClient(myOwnAEParameters);

            if (!dicomClient.Verify(serverAE))
                throw new Exception("Target server is not running");

            //ReadOnlyQueryResultCollection results = dicomClient.Query(serverAE, new Uid("1.3.46.670589.5.2.10.2156913941.892665384.993397"));
            ReadOnlyQueryResultCollection results = dicomClient.Query(serverAE, new Uid("1.2.840.113619.2.30.1.1762295590.1623.978668949.886"));
            Assert.IsTrue(results.Count > 0);

            foreach (QueryResult qr in results)
            {   
                foreach (DicomTag dicomTag in qr.DicomTags)
                {
                    Console.WriteLine("{0} - {1}", dicomTag.ToString(), qr[dicomTag]);
                }
            }
        }

        [Test]
        public void QueryForSeries()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(4000));
            ApplicationEntity serverAE = new ApplicationEntity(new HostName("192.168.0.100"),
                new AETitle("CONQUESTSRV1"), new ListeningPort(5678));

            DicomClient dicomClient = new DicomClient(myOwnAEParameters);

            if (!dicomClient.Verify(serverAE))
                throw new Exception("Target server is not running");

            //ReadOnlyQueryResultCollection results = dicomClient.Query(serverAE, new Uid("1.3.46.670589.5.2.10.2156913941.892665384.993397"));
            ReadOnlyQueryResultCollection results = dicomClient.QuerySeries(serverAE, new Uid("1.2.840.113619.2.30.1.1762295590.1623.978668949.886"));
            Assert.IsTrue(results.Count > 0);

            foreach (QueryResult qr in results)
            {
                foreach (DicomTag dicomTag in qr.DicomTags)
                {
                    Console.WriteLine("{0} - {1}", dicomTag.ToString(), qr[dicomTag]);
                }
            }
        }

        [Test]
        public void QueryByMultipleKeys()
        {
        }

        [Test]
        public void Retrieve()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(4000));
            ApplicationEntity serverAE = new ApplicationEntity(new HostName("192.168.0.100"),
                new AETitle("CONQUESTSRV1"), new ListeningPort(5678));

            DicomClient dicomClient = new DicomClient(myOwnAEParameters);

            if (!dicomClient.Verify(serverAE))
                throw new Exception("Target server is not running");

            dicomClient.SopInstanceReceivedEvent += SopInstanceReceivedEventHandler;
            dicomClient.Retrieve(serverAE, new Uid("1.3.46.670589.5.2.10.2156913941.892665384.993397"), @"C:\Temp\retrieveTest");
            dicomClient.SopInstanceReceivedEvent -= SopInstanceReceivedEventHandler;
        }

        [Test]
        public void RetrieveSeries()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(4000));
            ApplicationEntity serverAE = new ApplicationEntity(new HostName("192.168.0.100"),
                new AETitle("CONQUESTSRV1"), new ListeningPort(5678));

            DicomClient dicomClient = new DicomClient(myOwnAEParameters);

            if (!dicomClient.Verify(serverAE))
                throw new Exception("Target server is not running");

            dicomClient.SopInstanceReceivedEvent += SopInstanceReceivedEventHandler;
            dicomClient.RetrieveSeries(serverAE, new Uid("1.2.840.113619.2.30.1.1762295590.1623.978668949.887"), @"C:\Temp\retrieveTest\");
            dicomClient.SopInstanceReceivedEvent -= SopInstanceReceivedEventHandler;
        }

        [Test]
        public void ServerTest()
        {
            ApplicationEntity myOwnAEParameters = new ApplicationEntity(new HostName("localhost"),
                new AETitle("CCNETTEST"), new ListeningPort(4000));

            DicomServer dicomServer = new DicomServer(myOwnAEParameters, @"C:\Temp\ServerObjects\");

            dicomServer.Start();

            Thread.Sleep(TimeSpan.FromMinutes(5));

            dicomServer.Stop();
        }

        #region Non-test utility methods

        public static void SopInstanceReceivedEventHandler(object source, SopInstanceReceivedEventArgs args)
        {
            Console.WriteLine("Beg of SopInstanceResultReceivedEventHandler-------------");
            Console.WriteLine("       File name of SOP: {0}", args.SopFileName);
            Console.WriteLine("End of SopInstanceResultReceivedEventHandler-------------");
        }

        public static void QueryResultReceivedEventHandler(object source, QueryResultReceivedEventArgs args)
        {
            Console.WriteLine("Beg of QueryResultReceivedEventHandler-------------");
            foreach (DicomTag tag in args.Result.DicomTags)
            {
                Console.WriteLine("{0} - {1}", tag.ToString(), args.Result[tag]);
            }
            Console.WriteLine("End of QueryResultReceivedEventHandler-------------");
        }

        public static void QueryCompletedEventHandler(object source, QueryCompletedEventArgs args)
        {
            Console.WriteLine("Beg of QueryCompletedEventHandler-------------");
            foreach (QueryResult qr in args.Results)
            {
                foreach (DicomTag dicomTag in qr.DicomTags)
                {
                    Console.WriteLine("{0} - {1}", dicomTag.ToString(), qr[dicomTag]);
                }
            }
            Console.WriteLine("End of QueryCompletedEventHandler-------------");

        }

        #endregion
    }
}

#endif
