#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
    using ClearCanvas.Dicom.Network;

    [TestFixture]
    public class DataStoreTests
    {
        private String _connectionString = "Data Source=CLINTONLAPTOP\\SQLEXPRESS;Initial Catalog=ripp_version5;User ID=sa;Password=root";
        private SqlConnection _connection;

        [Test]
        public void TestDicomMappingTable()
        {
            DicomMappingTable table = new DicomMappingTable(new ConnectionString(_connectionString));

            Assert.IsTrue(table.GetColumn(new TagName("StudyInstanceUid")).Path.ToString() == "(0020,000d)");
            Assert.IsTrue(table.GetColumn(new TagName("PatientId")).Path.ToString() == "(0010,0020)");
            Assert.IsTrue(table.GetColumn(new Path("(0008,0060)")).TagName.ToString() == "Modality");
            Assert.IsTrue(table.GetColumn(new Path("(0008,1032)\\(0008,0102)")).TagName.ToString() == "ProcedureCodeSequenceCodingSchemeDesignator");
            Assert.IsTrue(table.GetColumn(new TagName("ProcedureCodeSequenceCodeValue")).Path.ToString() == "(0008,1032)\\(0008,0100)");
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
            DatabaseConnector connector = new DatabaseConnector(new ConnectionString(_connectionString));
            connector.SetupConnector();
            //connector.InsertSopInstance("c:\\temp\\CT.1.2.840.113619.2.30.1.1762295590.1623.978668950.168.dcm");
            //connector.InsertSopInstance("c:\\temp\\movescu_2.cap");
            connector.InsertSopInstance("C:\\DICOM\\9493\\94912\\949226.DCM");
            connector.TeardownConnector();
        }

        [Test]
        public void TestImageReceiveInsert()
        {
            ApplicationEntity myself = new ApplicationEntity(new HostName("localhost"), new AETitle("CCNETTEST"), new ListeningPort(4000));
            ApplicationEntity server = new ApplicationEntity(new HostName("clintondesk"), new AETitle("CONQUESTSRV1"), new ListeningPort(5678));

            DicomClient client = new DicomClient(myself);

            ReadOnlyQueryResultCollection results = client.Query(server, new PatientId("*001*"));

            if (results.Count < 1)
                return;

            client.SopInstanceReceivedEvent += NewImageEventHandler;
            client.Retrieve(server, results[0].StudyInstanceUid, "c:\\temp\\retrieveDB\\images");  
        }

        protected void NewImageEventHandler(Object source, SopInstanceReceivedEventArgs args)
        {
            DateTime start = DateTime.Now;
            DatabaseConnector db = new DatabaseConnector(new ConnectionString(_connectionString));
            db.SetupConnector();
            db.InsertSopInstance(args.SopFileName);
            db.TeardownConnector();
            DateTime stop = DateTime.Now;
            TimeSpan duration = stop-start;

            Console.WriteLine("{0} stored {1}", duration.ToString(), args.SopFileName);
        }
    }
}
