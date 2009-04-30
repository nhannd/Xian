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

#if	UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyFinders.Remote.Tests
{
	internal class MockRemoteStudyFinder : RemoteStudyFinder
	{
		public MockRemoteStudyFinder()
		{ 
		}

		public bool TestFilterResults(IList<DicomAttributeCollection> results, IDictionary<string, DicomAttributeCollection> resultsByStudy, string modalityFilter)
		{
			return FilterResultsByModality(results, resultsByStudy, modalityFilter);
		}
	}

	[TestFixture]
	public class PostFilteringTests
	{
		public PostFilteringTests()
		{ 
		}

		[Test]
		public void TestModalitiesInStudyPostFiltering()
		{
			MockRemoteStudyFinder finder = new MockRemoteStudyFinder();
			Dictionary<string, DicomAttributeCollection> results = new Dictionary<string, DicomAttributeCollection>();
			List<DicomAttributeCollection> serverResults = new List<DicomAttributeCollection>();
			
			string modalityFilter = "";

			serverResults.Add(GetModalityResult("MR", "1"));
			serverResults.Add(GetModalityResult("PT\\CT", "2"));
			serverResults.Add(GetModalityResult("MR\\CT", "3"));

			//empty filter, everything is a match
			Assert.IsTrue(finder.TestFilterResults(serverResults, results, modalityFilter));
			Assert.AreEqual(results.Count, 3);

			//filter on MR, a PT/CT study exists in the results, so everything is a match
			results.Clear();
			modalityFilter = "MR";
			Assert.IsTrue(finder.TestFilterResults(serverResults, results, modalityFilter));
			Assert.AreEqual(results.Count, 2);

			//filter on MR\\CT, each result set contains good matches, so we keep performing queries.
			modalityFilter = "MR";
			results.Clear();
			serverResults.Clear();
			serverResults.Add(GetModalityResult("MR", "1"));
			serverResults.Add(GetModalityResult("MR\\CT", "2"));
			serverResults.Add(GetModalityResult("MR\\CT\\OT", "3"));
			Assert.IsFalse(finder.TestFilterResults(serverResults, results, modalityFilter));
			Assert.AreEqual(results.Count, 3);

			modalityFilter = "CT";
			serverResults.Clear();
			serverResults.Add(GetModalityResult("MR\\CT", "2"));
			serverResults.Add(GetModalityResult("MR\\CT\\OT", "3"));
			serverResults.Add(GetModalityResult("CT", "4"));
			serverResults.Add(GetModalityResult("MR\\CT", "5"));
			serverResults.Add(GetModalityResult("MR\\CT\\SC", "6"));
			Assert.IsFalse(finder.TestFilterResults(serverResults, results, modalityFilter));
			Assert.AreEqual(results.Count, 6);

			//when a server does not even return ModalitiesInStudy, it means it is not supported at all.
			modalityFilter = "MR";
			results.Clear();
			serverResults.Clear();
			serverResults.Add(GetModalityResult(null, "1"));
			serverResults.Add(GetModalityResult(null, "2"));
			serverResults.Add(GetModalityResult(null, "3"));
			Assert.IsTrue(finder.TestFilterResults(serverResults, results, modalityFilter));
			Assert.AreEqual(results.Count, 3);

			//We don't support this right now anyway, but if a wildcard query were done, we would just include all the results.
			modalityFilter = "M*";
			results.Clear();
			serverResults.Clear();
			serverResults.Add(GetModalityResult("MR", "1"));
			serverResults.Add(GetModalityResult("MR\\CT", "2"));
			serverResults.Add(GetModalityResult("MR\\CT\\OT", "3"));
			Assert.IsFalse(finder.TestFilterResults(serverResults, results, modalityFilter));
			Assert.AreEqual(results.Count, 3);
			
		}

		private DicomAttributeCollection GetModalityResult(string modalityResult, string studyUID)
		{
			DicomAttributeCollection result = new DicomAttributeCollection();
			if (modalityResult != null)
				result[DicomTags.ModalitiesInStudy].SetStringValue(modalityResult);
			
			result[DicomTags.StudyInstanceUid].SetStringValue(studyUID);
			return result;
		}
	}
}

#endif