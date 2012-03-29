#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

#if UNIT_TESTS

using System;
using ClearCanvas.Dicom.ServiceModel.Query;
using NUnit.Framework;
namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.Tests
{
    [TestFixture]
    public class StudyRootQueryTests : ClearCanvas.Dicom.Tests.AbstractTest
    {
        private const string _utf8 = "ISO_IR 192";
        private int _testStudyCount = 3;

        [TestFixtureSetUp]
        public void Setup()
        {
            InsertTestStudies();
        }

        [Test]
        public void SelectAllStudies()
        {
            using (var context = new DataAccessContext())
            {
                var query = context.GetStudyRootQuery();
                var criteria = new StudyRootStudyIdentifier();

                var results = query.StudyQuery(criteria);
                Assert.AreEqual(_testStudyCount, results.Count);
            }
        }

        [Test]
        public void SelectPatientIdEquals()
        {
            using (var context = new DataAccessContext())
            {
                var query = context.GetStudyRootQuery();
                var criteria = new StudyRootStudyIdentifier
                                   {
                                       PatientId = "1234"
                                   };
                
                var results = query.StudyQuery(criteria);
                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(criteria.PatientId, results[0].PatientId);
            }
        }

        [Test]
        public void SelectPatientIdWildcard()
        {
            using (var context = new DataAccessContext())
            {
                var query = context.GetStudyRootQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    PatientId = "123*"
                };

                var results = query.StudyQuery(criteria);
                Assert.AreEqual(2, results.Count);
                Assert.AreEqual(criteria.PatientId, results[0].PatientId);
            }
        }

        private void InsertTestStudies()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetStudyBroker();
                broker.DeleteAll();
                context.Commit();
            }

            using (var context = new DataAccessContext())
            {
                var broker = context.GetStudyBroker();
                broker.AddStudy(CreateTestStudy1());
                broker.AddStudy(CreateTestStudy2());
                broker.AddStudy(CreateTestStudy3());
                //broker.AddStudy(CreateTestStudy4());
                //broker.AddStudy(CreateTestStudy5());
                //broker.AddStudy(CreateTestStudy6());
                context.Commit();
            }
        }

        private Study CreateTestStudy1()
        {
            var study = new Study();
            var identifier = new StudyRootStudyIdentifier
                                 {
                                     PatientId = "1234",
                                     PatientsName = "Sparky",
                                     PatientsBirthDate = "19760810",
                                     PatientsBirthTime = "110003",
                                     PatientsSex = "M",

                                     PatientSpeciesDescription = "Doggie",
                                     PatientSpeciesCodeSequenceCodingSchemeDesignator = "ASPCA",
                                     PatientSpeciesCodeSequenceCodeValue = "Dog",
                                     PatientSpeciesCodeSequenceCodeMeaning = "A Dog",
                                     PatientBreedDescription = "Labrador Retriever",
                                     PatientBreedCodeSequenceCodingSchemeDesignator = "ASPCA",
                                     PatientBreedCodeSequenceCodeValue = "Lab",
                                     PatientBreedCodeSequenceCodeMeaning = "Labrador Retriever",
                                     ResponsiblePerson = "Guy^Some",
                                     ResponsiblePersonRole = "Owner",
                                     ResponsibleOrganization = "Home town vet",
                                     
                                     ReferringPhysiciansName = "Lafleur^Guy^Dr.",
                                     AccessionNumber = "1256",
                                     StudyDescription = "Chest X-Ray",
                                     StudyId = "abc",
                                     StudyDate = "20120327",
                                     StudyTime = "120102",
                                     ModalitiesInStudy = new string[]{"DX"},
                                     StudyInstanceUid = "1.2.3",
                                     NumberOfStudyRelatedSeries = 3,
                                     NumberOfStudyRelatedInstances = 3,

                                     SpecificCharacterSet = _utf8
                                 };
        
            study.Initialize(identifier.ToDicomAttributeCollection());
            
            study.DeleteTime = DateTime.Now.AddDays(1);
            return study;
        }

        private Study CreateTestStudy2()
        {
            var study = new Study();
            var identifier = new StudyRootStudyIdentifier
            {
                PatientId = "1235",
                PatientsName = "Doe^John",
                PatientsBirthDate = "19860612",
                PatientsBirthTime = "165602",
                PatientsSex = "M",

                //PatientSpeciesDescription = "Doggie",
                //PatientSpeciesCodeSequenceCodingSchemeDesignator = "ASPCA",
                //PatientSpeciesCodeSequenceCodeValue = "Dog",
                //PatientSpeciesCodeSequenceCodeMeaning = "A Dog",
                //PatientBreedDescription = "Labrador Retriever",
                //PatientBreedCodeSequenceCodingSchemeDesignator = "ASPCA",
                //PatientBreedCodeSequenceCodeValue = "Lab",
                //PatientBreedCodeSequenceCodeMeaning = "Labrador Retriever",
                //ResponsiblePerson = "Guy^Some",
                //ResponsiblePersonRole = "Owner",
                //ResponsibleOrganization = "Home town vet",

                ReferringPhysiciansName = "Maher^Bill^Dr.",
                AccessionNumber = "3451",
                StudyDescription = "MR Brain",
                StudyId = "dfer",
                StudyDate = "20110211",
                StudyTime = "172356",
                ModalitiesInStudy = new string[] { "MR" },
                StudyInstanceUid = "1.2.4",
                NumberOfStudyRelatedSeries = 4,
                NumberOfStudyRelatedInstances = 265,

                SpecificCharacterSet = _utf8
            };

            study.Initialize(identifier.ToDicomAttributeCollection());

            study.DeleteTime = DateTime.Now.AddDays(1);
            return study;
        }

        private Study CreateTestStudy3()
        {
            var study = new Study();
            var identifier = new StudyRootStudyIdentifier
            {
                PatientId = "6723",
                PatientsName = "Doe^Jane",
                PatientsBirthDate = "19670512",
                PatientsBirthTime = "",
                PatientsSex = "F",

                //PatientSpeciesDescription = "Doggie",
                //PatientSpeciesCodeSequenceCodingSchemeDesignator = "ASPCA",
                //PatientSpeciesCodeSequenceCodeValue = "Dog",
                //PatientSpeciesCodeSequenceCodeMeaning = "A Dog",
                //PatientBreedDescription = "Labrador Retriever",
                //PatientBreedCodeSequenceCodingSchemeDesignator = "ASPCA",
                //PatientBreedCodeSequenceCodeValue = "Lab",
                //PatientBreedCodeSequenceCodeMeaning = "Labrador Retriever",
                //ResponsiblePerson = "Guy^Some",
                //ResponsiblePersonRole = "Owner",
                //ResponsibleOrganization = "Home town vet",

                ReferringPhysiciansName = "Spok^Leonard^Dr.",
                AccessionNumber = "6413",
                StudyDescription = "CT Chest",
                StudyId = "awrt",
                StudyDate = "20100728",
                StudyTime = "235612",
                ModalitiesInStudy = new string[] { "CT", "KO" },
                StudyInstanceUid = "5.2.3",
                NumberOfStudyRelatedSeries = 6,
                NumberOfStudyRelatedInstances = 26,

                SpecificCharacterSet = _utf8
            };

            study.Initialize(identifier.ToDicomAttributeCollection());

            study.DeleteTime = DateTime.Now.AddDays(1);
            return study;
        }

        private Study CreateTestStudy4()
        {
            throw new System.NotImplementedException();
        }

        private Study CreateTestStudy5()
        {
            throw new System.NotImplementedException();
        }

        private Study CreateTestStudy6()
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif