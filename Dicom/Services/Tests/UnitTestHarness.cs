#if UNIT_TESTS

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.DataStore;
using NMock2;
using NHibernate;

namespace ClearCanvas.Dicom.Services.Tests
{
    [TestFixture]
    public class SenderTests
    {
        public class TestSender : Sender
        {
            private ISendQueue _mockSendQueue;

            public TestSender(ApplicationEntity myAE, ISendQueue mockSendQueue)
                : base(myAE)
            {
                _mockSendQueue = mockSendQueue;
            }

            public override ISendQueue SendQueue
            {
                get
                {
                    return _mockSendQueue;
                }
            }
        }

        private Mockery _mocks;
        private ISender _sender;
        private ISendQueue _mockSendQueue;
        private IParcel _mockParcel;
        private ApplicationEntity _testingAE = new ApplicationEntity(new HostName("localhost"),
                                                    new AETitle("TEST_AE"),
                                                    new ListeningPort(12000));

        [SetUp]
        public void Setup()
        {
            _mocks = new Mockery();
            _mockSendQueue = _mocks.NewMock<ISendQueue>();
            _mockParcel = _mocks.NewMock<IParcel>();

            _sender = new TestSender(_testingAE, _mockSendQueue);
        }

        [Test]
        public void CreateASender()
        {

            Sender aSender = new TestSender(_testingAE, _mockSendQueue);

            _mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void CreateParcelAndPutOnQueue()
        {
            ApplicationEntity destinationAE = new ApplicationEntity(new HostName("testhost"),
                                                                        new AETitle("TEST_SERVER"),
                                                                        new ListeningPort(13000));

            Uid sopInstance = new Uid("9.9.9.9");

            Expect.Once.On(_mockSendQueue).Method("CreateNewParcel").With(new Matcher[] { Is.Same(_testingAE), Is.Same(destinationAE), Is.StringContaining("Test Send") }).Will(Return.Value(_mockParcel));
            Expect.Once.On(_mockParcel).Method("Include").With(Is.EqualTo(sopInstance)).Will(Return.Value(1));
            Expect.Once.On(_mockSendQueue).Method("Add").With(Is.Same(_mockParcel));

            _sender.Send(sopInstance, destinationAE, "Test Send");

            _mocks.VerifyAllExpectationsHaveBeenMet();
        }
    }

    [TestFixture]
    public class ParcelTests
    {
        private Mockery _mocks;
        private IStudy _mockStudy;
        private IDataStoreReader _mockDataStore;
        private IDicomSender _mockDicomSender;
        private ISendQueue _mockSendQueue;
        private ISeries _mockSeries;
        private ApplicationEntity _testingAE = new ApplicationEntity(new HostName("localhost"),
                                            new AETitle("TEST_AE"),
                                            new ListeningPort(12000));

        public class TestParcel : Parcel
        {
            public TestParcel(ApplicationEntity sourceAE, ApplicationEntity destinationAE, IDataStoreReader dataStore, IDicomSender dicomSender, ISendQueue sendQueue) : base(sourceAE, destinationAE, "Test Parcel")
            {
                _mockDataStoreReader = dataStore;
                _mockDicomSender = dicomSender;
                _mockSendQueue = sendQueue;
            }

            public override IDataStoreReader DataStoreReader
            {
                get { return _mockDataStoreReader; }
            }

            public override IDicomSender DicomSender
            {
                get { return _mockDicomSender; }
            }

            public override ISendQueue SendQueue
            {
                get { return _mockSendQueue; }
            }

            private IDataStoreReader _mockDataStoreReader;
            private IDicomSender _mockDicomSender;
            private ISendQueue _mockSendQueue;
        }

        [SetUp]
        public void Setup()
        {
            _mocks = new Mockery();
            _mockStudy = _mocks.NewMock<IStudy>();
            _mockDataStore = _mocks.NewMock<IDataStoreReader>();
            _mockDicomSender = _mocks.NewMock<IDicomSender>();
            _mockSendQueue = _mocks.NewMock<ISendQueue>();
            _mockSeries = _mocks.NewMock<ISeries>();
        }

        [Test]
        public void TestInclusionOfSopInstanceObject()
        {
            ApplicationEntity destinationAE = new ApplicationEntity(new HostName("testhost"),
                                                            new AETitle("TEST_SERVER"),
                                                            new ListeningPort(13000));

            IParcel aParcel = new TestParcel(_testingAE, destinationAE, _mockDataStore, _mockDicomSender, _mockSendQueue);
    
            Uid sopInstanceReference = new Uid("1.2.8.999.999.999.999.1111.1111.1111");
            ISopInstance mockSopInstance1 = _mocks.NewMock<ISopInstance>();
            ISopInstance mockSopInstance2 = _mocks.NewMock<ISopInstance>();
            ISopInstance[] sopInstances = new ISopInstance[] { mockSopInstance1, mockSopInstance2 };

            // try to find the object referenced in the data store
            Expect.Once.On(_mockDataStore).Method("StudyExists").With(Is.EqualTo(sopInstanceReference)).Will(Return.Value(false));
            Expect.Once.On(_mockDataStore).Method("SeriesExists").With(Is.EqualTo(sopInstanceReference)).Will(Return.Value(false));
            Expect.Once.On(_mockDataStore).Method("SopInstanceExists").With(Is.EqualTo(sopInstanceReference)).Will(Return.Value(true));

            // found the sop instance, now figure out the Transfer Syntax and SOP Class parameters
            Expect.Once.On(_mockDataStore).Method("GetSopInstance").With(Is.EqualTo(sopInstanceReference)).Will(Return.Value(sopInstances[0]));

            // get the stored transfer syntax and sop class of instance
            Uid transferSyntax = new Uid("1.2.840.10008.1.2");
            Uid sopClass = new Uid("1.2.840.10008.5.1.4.1.1.1");
            Expect.Once.On(sopInstances[0]).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntax));
            Expect.Once.On(sopInstances[0]).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClass));

            int includedObjectCount = aParcel.Include(sopInstanceReference);
            Assert.IsTrue(includedObjectCount == 1);

            // we want to acces some internal properties for testing purposes only
            TestParcel parcelObject = aParcel as TestParcel;

            // state-based verifications
            IEnumerable<String> transferSyntaxList = (IEnumerable<string>) parcelObject.TransferSyntaxes;
            IEnumerable<String> sopClassList = (IEnumerable<string>) parcelObject.SopClasses;

            Assert.IsTrue((transferSyntaxList as List<string>).Count == 1);
            Assert.IsTrue((sopClassList as List<string>).Count == 1);

            foreach (string syntax in transferSyntaxList)
            {
                Assert.IsTrue(syntax == transferSyntax);
            }

            foreach (string localSopClass in sopClassList)
            {
                Assert.IsTrue(localSopClass == sopClass);
            }

            _mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void TestInclusionOfSopInstanceObjectsWithMultipleTransferSyntaxes()
        {
            ApplicationEntity destinationAE = new ApplicationEntity(new HostName("testhost"),
                                                            new AETitle("TEST_SERVER"),
                                                            new ListeningPort(13000));

            IParcel aParcel = new TestParcel(_testingAE, destinationAE, _mockDataStore, _mockDicomSender, _mockSendQueue);

            // first SOP instance
            Uid sopInstanceReference = new Uid("1.2.8.999.999.999.999.1111.1111.1111");
            ISopInstance mockSopInstance1 = _mocks.NewMock<ISopInstance>();
            ISopInstance mockSopInstance2 = _mocks.NewMock<ISopInstance>();
            ISopInstance[] sopInstances = new ISopInstance[] { mockSopInstance1, mockSopInstance2 };

            Uid[] transferSyntaxes = new Uid[] { new Uid("1.2.840.10008.1.2"), new Uid("1.2.840.10008.1.2.1") };
            Uid sopClass = new Uid("1.2.840.10008.5.1.4.1.1.1");

            Expect.Once.On(_mockDataStore).Method("StudyExists").With(Is.EqualTo(sopInstanceReference)).Will(Return.Value(false));
            Expect.Once.On(_mockDataStore).Method("SeriesExists").With(Is.EqualTo(sopInstanceReference)).Will(Return.Value(false));
            Expect.Once.On(_mockDataStore).Method("SopInstanceExists").With(Is.EqualTo(sopInstanceReference)).Will(Return.Value(true));
            Expect.Once.On(_mockDataStore).Method("GetSopInstance").With(Is.EqualTo(sopInstanceReference)).Will(Return.Value(sopInstances[0]));
            Expect.Once.On(sopInstances[0]).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntaxes[0]));
            Expect.Once.On(sopInstances[0]).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClass));

            int includedObjectCount = aParcel.Include(sopInstanceReference);
            Assert.IsTrue(includedObjectCount == 1);

            // second SOP instance
            Uid sopInstanceReference2 = new Uid("1.2.8.999.999.999.999.1111.1111.1112");

            Expect.Once.On(_mockDataStore).Method("StudyExists").With(Is.EqualTo(sopInstanceReference2)).Will(Return.Value(false));
            Expect.Once.On(_mockDataStore).Method("SeriesExists").With(Is.EqualTo(sopInstanceReference2)).Will(Return.Value(false));
            Expect.Once.On(_mockDataStore).Method("SopInstanceExists").With(Is.EqualTo(sopInstanceReference2)).Will(Return.Value(true));
            Expect.Once.On(_mockDataStore).Method("GetSopInstance").With(Is.EqualTo(sopInstanceReference2)).Will(Return.Value(sopInstances[1]));
            Expect.Once.On(sopInstances[1]).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntaxes[1]));
            Expect.Once.On(sopInstances[1]).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClass));
            Expect.Once.On(sopInstances[1]).Method("IsIdenticalTo").With(Is.EqualTo(sopInstances[0])).Will(Return.Value(false));

            includedObjectCount = aParcel.Include(sopInstanceReference2);
            Assert.IsTrue(includedObjectCount == 1);

            // we want to acces some internal properties for testing purposes only
            TestParcel parcelObject = aParcel as TestParcel;

            // state-based verifications
            IEnumerable<String> transferSyntaxList = (IEnumerable<string>) parcelObject.TransferSyntaxes;
            IEnumerable<String> sopClassList = (IEnumerable<string>) parcelObject.SopClasses;

            Assert.IsTrue((transferSyntaxList as List<string>).Count == 2);
            Assert.IsTrue((sopClassList as List<string>).Count == 1);

            int i = 0;
            foreach (string syntax in transferSyntaxList)
            {
                Assert.IsTrue(syntax == transferSyntaxes[i++]);
            }

            foreach (string localSopClass in sopClassList)
            {
                Assert.IsTrue(localSopClass == sopClass);
            }

            _mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void TestInclusionOfSopInstanceObjectsWithMultipleSopClasses()
        {
            ApplicationEntity destinationAE = new ApplicationEntity(new HostName("testhost"),
                                                            new AETitle("TEST_SERVER"),
                                                            new ListeningPort(13000));

            IParcel aParcel = new TestParcel(_testingAE, destinationAE, _mockDataStore, _mockDicomSender, _mockSendQueue);

            // first SOP instance
            Uid sopInstanceReference = new Uid("1.2.8.999.999.999.999.1111.1111.1111");
            ISopInstance mockSopInstance1 = _mocks.NewMock<ISopInstance>();
            ISopInstance mockSopInstance2 = _mocks.NewMock<ISopInstance>();
            ISopInstance[] sopInstances = new ISopInstance[] { mockSopInstance1, mockSopInstance2 };
            Uid[] transferSyntaxes = new Uid[] { new Uid("1.2.840.10008.1.2"), new Uid("1.2.840.10008.1.2.1") };
            Uid[] sopClasses = new Uid[] { new Uid("1.2.840.10008.5.1.4.1.1.1"), new Uid("1.2.840.10008.5.1.4.1.1.2") };

            Expect.Once.On(_mockDataStore).Method("StudyExists").With(Is.EqualTo(sopInstanceReference)).Will(Return.Value(false));
            Expect.Once.On(_mockDataStore).Method("SeriesExists").With(Is.EqualTo(sopInstanceReference)).Will(Return.Value(false));
            Expect.Once.On(_mockDataStore).Method("SopInstanceExists").With(Is.EqualTo(sopInstanceReference)).Will(Return.Value(true));
            Expect.Once.On(_mockDataStore).Method("GetSopInstance").With(Is.EqualTo(sopInstanceReference)).Will(Return.Value(sopInstances[0]));
            Expect.Once.On(sopInstances[0]).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntaxes[0]));
            Expect.Once.On(sopInstances[0]).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClasses[0]));

            int includedObjectCount = aParcel.Include(sopInstanceReference);
            Assert.IsTrue(includedObjectCount == 1);

            // second SOP instance
            Uid sopInstanceReference2 = new Uid("1.2.8.999.999.999.999.1111.1111.1112");

            Expect.Once.On(_mockDataStore).Method("StudyExists").With(Is.EqualTo(sopInstanceReference2)).Will(Return.Value(false));
            Expect.Once.On(_mockDataStore).Method("SeriesExists").With(Is.EqualTo(sopInstanceReference2)).Will(Return.Value(false));
            Expect.Once.On(_mockDataStore).Method("SopInstanceExists").With(Is.EqualTo(sopInstanceReference2)).Will(Return.Value(true));
            Expect.Once.On(_mockDataStore).Method("GetSopInstance").With(Is.EqualTo(sopInstanceReference2)).Will(Return.Value(sopInstances[1]));
            Expect.Once.On(sopInstances[1]).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntaxes[1]));
            Expect.Once.On(sopInstances[1]).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClasses[1]));
            Expect.Once.On(sopInstances[1]).Method("IsIdenticalTo").With(Is.EqualTo(sopInstances[0])).Will(Return.Value(false));

            includedObjectCount = aParcel.Include(sopInstanceReference2);
            Assert.IsTrue(includedObjectCount == 1);

            // we want to acces some internal properties for testing purposes only
            TestParcel parcelObject = aParcel as TestParcel;

            // state-based verifications
            IEnumerable<String> transferSyntaxList = (IEnumerable<string>) parcelObject.TransferSyntaxes;
            IEnumerable<String> sopClassList = (IEnumerable<string>) parcelObject.SopClasses;

            Assert.IsTrue((transferSyntaxList as List<string>).Count == 2);
            Assert.IsTrue((sopClassList as List<string>).Count == 2);

            int i = 0;
            foreach (string syntax in transferSyntaxList)
            {
                Assert.IsTrue(syntax == transferSyntaxes[i++]);
            }

            i = 0;
            foreach (string localSopClass in sopClassList)
            {
                Assert.IsTrue(localSopClass == sopClasses[i++]);
            }

            _mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void TestInclusionOfSeriesObject()
        {
            ApplicationEntity destinationAE = new ApplicationEntity(new HostName("testhost"),
                                                new AETitle("TEST_SERVER"),
                                                new ListeningPort(13000));

            IParcel aParcel = new TestParcel(_testingAE, destinationAE, _mockDataStore, _mockDicomSender, _mockSendQueue);

            Uid seriesReference = new Uid("1.2.8.999.999.999.999.2222.1111");
            ISopInstance mockSopInstance1 = _mocks.NewMock<ISopInstance>();
            ISopInstance mockSopInstance2 = _mocks.NewMock<ISopInstance>();
            ISopInstance mockSopInstance3 = _mocks.NewMock<ISopInstance>();
            ISopInstance[] sopInstances = new ISopInstance[] { mockSopInstance1, mockSopInstance2, mockSopInstance3 };
            Uid transferSyntax = new Uid("1.2.840.10008.1.2");
            Uid sopClass = new Uid("1.2.840.10008.5.1.4.1.1.1");

            // try to find the object referenced in the data store
            Expect.Once.On(_mockDataStore).Method("StudyExists").With(Is.EqualTo(seriesReference)).Will(Return.Value(false));
            Expect.Once.On(_mockDataStore).Method("SeriesExists").With(Is.EqualTo(seriesReference)).Will(Return.Value(true));
            Expect.Once.On(_mockDataStore).Method("GetSeries").With(Is.EqualTo(seriesReference)).Will(Return.Value(_mockSeries));
            Expect.Once.On(_mockSeries).Method("GetSopInstances").WithNoArguments().Will(Return.Value(sopInstances));
            Expect.Once.On(mockSopInstance1).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntax));
            Expect.Once.On(mockSopInstance1).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClass));
            Expect.Once.On(mockSopInstance2).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntax));
            Expect.Once.On(mockSopInstance2).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClass));
            Expect.Once.On(mockSopInstance2).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance1)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance3).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntax));
            Expect.Once.On(mockSopInstance3).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClass));
            Expect.Once.On(mockSopInstance3).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance1)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance3).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance2)).Will(Return.Value(false));

            // found the sop instance, now figure out the Transfer Syntax and SOP Class parameters
            int includedObjectCount = aParcel.Include(seriesReference);
            Assert.IsTrue(includedObjectCount == 3);

            _mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void TestInclusionOfStudyObject()
        {
            ApplicationEntity destinationAE = new ApplicationEntity(new HostName("testhost"),
                                                new AETitle("TEST_SERVER"),
                                                new ListeningPort(13000));

            IParcel aParcel = new TestParcel(_testingAE, destinationAE, _mockDataStore, _mockDicomSender, _mockSendQueue);

            Uid studyReference = new Uid("1.2.8.999.999.999.999.3333");
            ISopInstance mockSopInstance1 = _mocks.NewMock<ISopInstance>();
            ISopInstance mockSopInstance2 = _mocks.NewMock<ISopInstance>();
            ISopInstance mockSopInstance3 = _mocks.NewMock<ISopInstance>();
            ISopInstance mockSopInstance4 = _mocks.NewMock<ISopInstance>();
            ISopInstance mockSopInstance5 = _mocks.NewMock<ISopInstance>();
            ISopInstance[] sopInstances = new ISopInstance[] { mockSopInstance1, mockSopInstance2, mockSopInstance3,
                mockSopInstance4, mockSopInstance5 };
            Uid transferSyntax = new Uid("1.2.840.10008.1.2");
            Uid sopClass = new Uid("1.2.840.10008.5.1.4.1.1.1");

            // try to find the object referenced in the data store
            Expect.Once.On(_mockDataStore).Method("StudyExists").With(Is.EqualTo(studyReference)).Will(Return.Value(true));
            Expect.Once.On(_mockDataStore).Method("GetStudy").With(Is.EqualTo(studyReference)).Will(Return.Value(_mockStudy));
            Expect.Once.On(_mockStudy).Method("GetSopInstances").WithNoArguments().Will(Return.Value(sopInstances));
            Expect.Once.On(mockSopInstance1).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntax));
            Expect.Once.On(mockSopInstance1).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClass));
            Expect.Once.On(mockSopInstance2).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntax));
            Expect.Once.On(mockSopInstance2).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClass));
            Expect.Once.On(mockSopInstance2).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance1)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance3).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntax));
            Expect.Once.On(mockSopInstance3).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClass));
            Expect.Once.On(mockSopInstance3).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance1)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance3).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance2)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance4).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntax));
            Expect.Once.On(mockSopInstance4).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClass));
            Expect.Once.On(mockSopInstance4).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance1)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance4).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance2)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance4).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance3)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance5).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntax));
            Expect.Once.On(mockSopInstance5).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClass));
            Expect.Once.On(mockSopInstance5).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance1)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance5).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance2)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance5).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance3)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance5).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance4)).Will(Return.Value(false));
            
            int includedObjectCount = aParcel.Include(studyReference);
            Assert.IsTrue(includedObjectCount == 5);

            _mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void TestParcelSend()
        {
            ApplicationEntity destinationAE = new ApplicationEntity(new HostName("testhost"),
                                    new AETitle("TEST_SERVER"),
                                    new ListeningPort(13000));

            IParcel aParcel = new TestParcel(_testingAE, destinationAE, _mockDataStore, _mockDicomSender, _mockSendQueue);

            Uid studyReference = new Uid("1.2.8.999.999.999.999.3333");
            ISopInstance mockSopInstance1 = _mocks.NewMock<ISopInstance>();
            ISopInstance mockSopInstance2 = _mocks.NewMock<ISopInstance>();
            ISopInstance mockSopInstance3 = _mocks.NewMock<ISopInstance>();
            ISopInstance mockSopInstance4 = _mocks.NewMock<ISopInstance>();
            ISopInstance mockSopInstance5 = _mocks.NewMock<ISopInstance>();
            ISopInstance[] sopInstances = new ISopInstance[] { mockSopInstance1, mockSopInstance2, mockSopInstance3,
                mockSopInstance4, mockSopInstance5 };
            Uid transferSyntax = new Uid("1.2.840.10008.1.2");
            Uid sopClass = new Uid("1.2.840.10008.5.1.4.1.1.1");

            Expect.Once.On(_mockDataStore).Method("StudyExists").With(Is.EqualTo(studyReference)).Will(Return.Value(true));
            Expect.Once.On(_mockDataStore).Method("GetStudy").With(Is.EqualTo(studyReference)).Will(Return.Value(_mockStudy));
            Expect.Once.On(_mockStudy).Method("GetSopInstances").WithNoArguments().Will(Return.Value(sopInstances));
            Expect.Once.On(_mockDicomSender).Method("SetSourceApplicationEntity").With(Is.EqualTo(_testingAE));
            Expect.Once.On(_mockDicomSender).Method("SetDestinationApplicationEntity").With(Is.EqualTo(destinationAE));
            Expect.Once.On(_mockDicomSender).Method("Send").WithAnyArguments();
            Expect.Exactly(2).On(_mockSendQueue).Method("UpdateParcel").With(Is.EqualTo(aParcel));
            Expect.Once.On(mockSopInstance1).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntax));
            Expect.Once.On(mockSopInstance1).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClass));
            Expect.Once.On(mockSopInstance2).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntax));
            Expect.Once.On(mockSopInstance2).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClass));
            Expect.Once.On(mockSopInstance2).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance1)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance3).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntax));
            Expect.Once.On(mockSopInstance3).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClass));
            Expect.Once.On(mockSopInstance3).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance1)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance3).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance2)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance4).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntax));
            Expect.Once.On(mockSopInstance4).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClass));
            Expect.Once.On(mockSopInstance4).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance1)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance4).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance2)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance4).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance3)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance5).Method("GetTransferSyntaxUid").WithNoArguments().Will(Return.Value(transferSyntax));
            Expect.Once.On(mockSopInstance5).Method("GetSopClassUid").WithNoArguments().Will(Return.Value(sopClass));
            Expect.Once.On(mockSopInstance5).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance1)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance5).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance2)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance5).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance3)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance5).Method("IsIdenticalTo").With(Is.EqualTo(mockSopInstance4)).Will(Return.Value(false));
            Expect.Once.On(mockSopInstance1).Method("GetLocationUri").WithNoArguments().Will(Return.Value(new DicomUri("file://localhost/C:/temp/file1")));
            Expect.Once.On(mockSopInstance2).Method("GetLocationUri").WithNoArguments().Will(Return.Value(new DicomUri("file://localhost/C:/temp/file2")));
            Expect.Once.On(mockSopInstance3).Method("GetLocationUri").WithNoArguments().Will(Return.Value(new DicomUri("file://localhost/C:/temp/file3")));
            Expect.Once.On(mockSopInstance4).Method("GetLocationUri").WithNoArguments().Will(Return.Value(new DicomUri("file://localhost/C:/temp/file4")));
            Expect.Once.On(mockSopInstance5).Method("GetLocationUri").WithNoArguments().Will(Return.Value(new DicomUri("file://localhost/C:/temp/file5")));

            int includedObjectCount = aParcel.Include(studyReference);
            aParcel.StartSend();
            _mocks.VerifyAllExpectationsHaveBeenMet();
        }
    }

    [TestFixture]
    public class SendQueueTests
    {
        private Mockery _mocks;
        private ISessionFactory _mockSessionFactory;
        private ISession _mockSession;
        private IParcel _mockParcel;
        private ApplicationEntity _testingAE = new ApplicationEntity(new HostName("localhost"),
                                                    new AETitle("TEST_AE"),
                                                    new ListeningPort(12000));

        [SetUp]
        public void Setup()
        {
            _mocks = new Mockery();
            _mockSessionFactory = _mocks.NewMock<ISessionFactory>();
            _mockSession = _mocks.NewMock<ISession>();
            _mockParcel = _mocks.NewMock<IParcel>();
        }

        [Test]
        public void GetBackAListOfParcels()
        {
            IList mockList = new ArrayList();
            mockList.Add(_mocks.NewMock<IParcel>());
            mockList.Add(_mocks.NewMock<IParcel>());
            mockList.Add(_mocks.NewMock<IParcel>());

            Expect.Once.On(_mockSessionFactory).Method("OpenSession").WithNoArguments().Will(Return.Value(_mockSession));
            Expect.Once.On(_mockSession).Method("Find").With(Is.StringContaining("from Parcel")).Will(Return.Value(mockList));
            Expect.Once.On(_mockSession).Method("Close").WithNoArguments();
            ISendQueue sendQueue = new SendQueue(_mockSessionFactory);
            IEnumerable<IParcel> listOfParcels = sendQueue.GetParcels();

            int count = 0;
            foreach (IParcel p in listOfParcels) ++count;
            Assert.IsTrue(3 == count);
            _mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void GetBackAListOfIncompleteParcels()
        {
            IList mockList = new ArrayList();
            mockList.Add(_mocks.NewMock<IParcel>());
            mockList.Add(_mocks.NewMock<IParcel>());
            mockList.Add(_mocks.NewMock<IParcel>());

            Expect.Once.On(_mockSessionFactory).Method("OpenSession").WithNoArguments().Will(Return.Value(_mockSession));
            Expect.Once.On(_mockSession).Method("Find").With(Is.StringContaining("from Parcel as parcel where parcel.ParcelTransferState != ?"), ParcelTransferState.Completed, NHibernateUtil.Int16);//.Will(Return.Value(mockList));
            Expect.Once.On(_mockSession).Method("Close").WithNoArguments();
            ISendQueue sendQueue = new SendQueue(_mockSessionFactory);
            IEnumerable<IParcel> listOfParcels = sendQueue.GetSendIncompleteParcels();

            int count = 0;
            foreach (IParcel p in listOfParcels) ++count;
            Assert.IsTrue(3 == count);
            _mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void UpdateParcelStateToDatabase()
        {
            ApplicationEntity destination = new ApplicationEntity(new HostName("testhost"),
                new AETitle("TEST_SERVER"),
                new ListeningPort(13000));
            ApplicationEntity source = new ApplicationEntity(new HostName("tester"),
                new AETitle("TEST_SOURCE"),
                new ListeningPort(14000));

            IParcel aParcel = DicomServicesLayer.GetISendQueue().CreateNewParcel(source, destination, "Test Parcel");
            ITransaction mockTransaction = _mocks.NewMock<ITransaction>();

            Expect.Once.On(_mockSessionFactory).Method("OpenSession").WithNoArguments().Will(Return.Value(_mockSession));
            Expect.Once.On(_mockSession).Method("BeginTransaction").WithNoArguments().Will(Return.Value(mockTransaction));
            Expect.Once.On(_mockSession).Method("Update").With(Is.EqualTo(aParcel));
            Expect.Once.On(mockTransaction).Method("Commit").WithNoArguments();
            Expect.Once.On(_mockSession).Method("Close").WithNoArguments();
            ISendQueue sendQueue = new SendQueue(_mockSessionFactory);
            sendQueue.UpdateParcel(aParcel);

            _mocks.VerifyAllExpectationsHaveBeenMet();

        }
    }
    
    [TestFixture]
    public class ZebraResourceDependentTests
    {
        [Test]
        public void TestPackagingOfParcel()
        {
            ApplicationEntity destinationAE = new ApplicationEntity(new HostName("testhost"),
                new AETitle("TEST_SERVER"),
                new ListeningPort(13000));

            ApplicationEntity sourceAE = new ApplicationEntity(new HostName("tester"),
                new AETitle("TESTER"),
                new ListeningPort(12000));

            IParcel aParcel = new Parcel(sourceAE, destinationAE, "Test Parcel");

            // populate database with test study
            Study study = new Study();
            study.StudyDescription = "Test Study";
            study.StudyInstanceUid = "3.1.4.1.5.9.2";

            Series series = new Series();
            series.SeriesDescription = "Test Series for Test Study #1";
            series.SeriesInstanceUid = "1.2.3.4.5.6.7.8";

            ImageSopInstance sop1 = new ImageSopInstance();
            sop1.SopInstanceUid = "9.9.9.1";
            sop1.SopClassUid = "1.2.840.10008.5.1.4.1.1.1";
            sop1.TransferSyntaxUid = "1.2.840.10008.1.2";

            ImageSopInstance sop2 = new ImageSopInstance();
            sop2.SopInstanceUid = "9.9.9.2";
            sop2.SopClassUid = "1.2.840.10008.5.1.4.1.1.1";
            sop2.TransferSyntaxUid = "1.2.840.10008.1.2";

            series.AddSopInstance(sop1);
            series.AddSopInstance(sop2);

            Series series2 = new Series();
            series2.SeriesDescription = "Test Series for Test Study #2";
            series2.SeriesInstanceUid = "1.2.3.4.5.6.7.8.9";

            ImageSopInstance sop3 = new ImageSopInstance();
            sop3.SopInstanceUid = "9.9.9.3";
            sop3.SopClassUid = "1.2.840.10008.5.1.4.1.1.2";
            sop3.TransferSyntaxUid = "1.2.840.10008.1.2.1";

            ImageSopInstance sop4 = new ImageSopInstance();
            sop4.SopInstanceUid = "9.9.9.4";
            sop4.SopClassUid = "1.2.840.10008.5.1.4.1.1.2";
            sop4.TransferSyntaxUid = "1.2.840.10008.1.2.1";

            ImageSopInstance sop5 = new ImageSopInstance();
            sop5.SopInstanceUid = "9.9.9.5";
            sop5.SopClassUid = "1.2.840.10008.5.1.4.1.1.2";
            sop5.TransferSyntaxUid = "1.2.840.10008.1.2.1";

            series2.AddSopInstance(sop3);
            series2.AddSopInstance(sop4);
            series2.AddSopInstance(sop5);

            study.AddSeries(series);
            study.AddSeries(series2);

            DataAccessLayer.GetIDataStoreWriter().StoreStudy(study);

            aParcel.Include(new Uid("3.1.4.1.5.9.2"));

            DicomServicesLayer.GetISendQueue().Add(aParcel);

            IStudy studyFound = DataAccessLayer.GetIDataStoreReader().GetStudy(new Uid("3.1.4.1.5.9.2"));

            // we're done, get rid of study
            DataAccessLayer.GetIDataStoreWriter().RemoveStudy(studyFound);

            DicomServicesLayer.GetISendQueue().Remove(aParcel);
        }

        [Test]
        public void TestSend()
        {
            while (true)
            {
                IEnumerable<IParcel> parcels = DicomServicesLayer.GetISendQueue().GetSendIncompleteParcels();

                if (null != parcels)
                {
                    foreach (IParcel parcel in parcels)
                    {
                        ParcelTransferState state = parcel.GetState();
                        switch (state)
                        {
                            case ParcelTransferState.CancelRequested:
                                break;
                            case ParcelTransferState.InProgress:
                                break;
                            case ParcelTransferState.PauseRequested:
                                break;
                            case ParcelTransferState.Pending:
                                parcel.StartSend();
                                break;
                            case ParcelTransferState.Unknown:
                                break;
                        }
                    }
                }

                System.Threading.Thread.Sleep(500);
            }
        }
    }
}

#endif