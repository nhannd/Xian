#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
 	public class DicomDirectoryTests : AbstractTest
	{

		[Test]
		public void CreateDicomdirTest()
		{
			DicomFile file = new DicomFile("CreateFileTest.dcm");

			SetupMR(file.DataSet);
			SetupMetaInfo(file);

			DicomDirectory writer = new DicomDirectory("");

			writer.AddFile(file, "DIR001\\FILE001");

			file = new DicomFile("CreateXaFileTest.dcm");
			SetupMultiframeXA(file.DataSet, 256, 256, 10);
			SetupMetaInfo(file);
			writer.AddFile(file, "DIR001\\FILE002");

			int fileCount = 2;
			IList<DicomAttributeCollection> seriesList = SetupMRSeries(2, 2, DicomUid.GenerateUid().UID);
			foreach (DicomAttributeCollection collection in seriesList)
			{
				file = new DicomFile("test.dcm", new DicomAttributeCollection(), collection);
				fileCount++;
				SetupMetaInfo(file);

				writer.AddFile(file, String.Format("DIR001\\FILE{0}", fileCount));
			}

			writer.FileSetId = "TestDicomdir";
			writer.Save("DICOMDIR");


			DicomDirectory reader = new DicomDirectory("");

			reader.Load("DICOMDIR");

			DirectoryRecordSequenceItem root = reader.RootDirectoryRecord;

		}
	}
}
