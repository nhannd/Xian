#if UNIT_TESTS

namespace ClearCanvas.Dicom.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClearCanvas.Dicom.Network;
    using NUnit.Framework;

    [TestFixture]
    public class DcmNetTest
    {
        [TestFixtureSetUp]
        public void Init()
        {
            // check to see if OFFIS storescp.exe exists
            const string debugProgramLocation = "..\\Offis\\dcmtk-3.5.3\\dcmnet\\apps\\debug\\storescp.exe";
            const string releaseProgramLocation = "..\\Offis\\dcmtk-3.5.3\\dcmnet\\apps\\release\\storescp.exe";
            string programToRun = null;

            if (!System.IO.File.Exists(debugProgramLocation))
            {   
                if (!System.IO.File.Exists(releaseProgramLocation))
                {
                    throw new Exception("Could not find the DICOM server program to run tests against");
                }
               
                programToRun = releaseProgramLocation;
            }
            else
            {
                programToRun = debugProgramLocation;
            }

            // build the command line then run
            programToRun += "-v 104";
            System.Diagnostics.Process.Start(programToRun);
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
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
    }
}

#endif