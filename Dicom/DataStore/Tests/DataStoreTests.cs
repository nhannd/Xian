using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using NUnit.Framework;
    using System.Data.SqlClient;
    using ClearCanvas.Dicom.OffisWrapper;
    using ClearCanvas.Dicom.Data;
    using ClearCanvas.Dicom;

    [TestFixture]
    public class DataStoreTests
    {
        private String _connectionString = "Data Source=CLINTONLAPTOP\\SQLEXPRESS;Initial Catalog=ripp_version5;User ID=sa;Password=root";
        private SqlConnection _connection;

        [TestFixtureSetUp]
        public void Setup()
        {
            _connection = new SqlConnection(_connectionString);    
        }

        [Test]
        public void TestDicomMappingTable()
        {
            //DicomMappingTable table = new DicomMappingTable(new ConnectionString("Data Source=CLINTONLAPTOP\\SQLEXPRESS;Initial Catalog=ripp_version5;User ID=sa;Password=root"));
            DicomMappingTable table = new DicomMappingTable(_connection);

            Assert.IsTrue(table.GetColumn(new TagName("StudyInstanceUid")).Path.ToString() == "(0020,000d)");
            Assert.IsTrue(table.GetColumn(new TagName("PatientId")).Path.ToString() == "(0010,0020)");
            Assert.IsTrue(table.GetColumn(new Path("(0008,0060)")).TagName.ToString() == "Modality");
            Assert.IsTrue(table.GetColumn(new Path("(0008,1032)\\(0008,0102)")).TagName.ToString() == "ProcedureCodeSequence.CodingSchemeDesignator");
            Assert.IsTrue(table.GetColumn(new TagName("ProcedureCodeSequence.CodeValue")).Path.ToString() == "(0008,1032)\\(0008,0100)");
        }

        [Test]
        public void TestPath()
        {
            Path p1 = new Path("(0010,0010)");
            Assert.IsTrue(p1.GetPathElementAsInt32(0) == 0x00100010);
            Assert.IsTrue(p1.GetPathElementAsString(0) == "(0010,0010)");

            Path p2 = new Path("(0008,1032)\\(0008,0100)");
            Assert.IsTrue(p2.GetPathElementAsInt32(1) == 0x00080100);
            Assert.IsTrue(p2.GetPathElementAsInt32(0) == 0x00081032);
            Assert.IsTrue(p2.GetPathElementAsString(0) == "(0008,1032)");

        }

        [Test]
        public void TestDatabaseConnector()
        {
            DatabaseConnector connector = new DatabaseConnector(_connection);
            connector.StartImageInsertion();
            //connector.InsertSopInstance("c:\\temp\\CT.1.2.840.113619.2.30.1.1762295590.1623.978668950.168.dcm");
            //connector.InsertSopInstance("c:\\temp\\movescu_2.cap");
            connector.InsertSopInstance("C:\\DICOM\\9493\\94912\\949226.DCM");
            connector.StopImageInsertion();


        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
