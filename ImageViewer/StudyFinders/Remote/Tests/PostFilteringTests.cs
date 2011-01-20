#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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