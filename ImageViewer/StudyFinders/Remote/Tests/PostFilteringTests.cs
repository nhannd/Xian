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

		public bool TestFilterResults(ReadOnlyQueryResultCollection results, IDictionary<string, QueryResult> resultsByStudy, string modalityFilter)
		{
			return base.FilterResultsByModality(results, resultsByStudy, modalityFilter);
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
			Dictionary<string, QueryResult> results = new Dictionary<string, QueryResult>();
			List<QueryResult> serverResults = new List<QueryResult>();
			
			string modalityFilter = "";

			serverResults.Add(GetModalityResult("MR", "1"));
			serverResults.Add(GetModalityResult("PT\\CT", "2"));
			serverResults.Add(GetModalityResult("MR\\CT", "3"));

			//empty filter, everything is a match
			Assert.IsTrue(finder.TestFilterResults(new ReadOnlyQueryResultCollection(serverResults), results, modalityFilter));
			Assert.AreEqual(results.Count, 3);

			//filter on MR, a PT/CT study exists in the results, so everything is a match
			results.Clear();
			modalityFilter = "MR";
			Assert.IsTrue(finder.TestFilterResults(new ReadOnlyQueryResultCollection(serverResults), results, modalityFilter));
			Assert.AreEqual(results.Count, 2);

			//filter on MR\\CT, each result set contains good matches, so we keep performing queries.
			modalityFilter = "MR";
			results.Clear();
			serverResults.Clear();
			serverResults.Add(GetModalityResult("MR", "1"));
			serverResults.Add(GetModalityResult("MR\\CT", "2"));
			serverResults.Add(GetModalityResult("MR\\CT\\OT", "3"));
			Assert.IsFalse(finder.TestFilterResults(new ReadOnlyQueryResultCollection(serverResults), results, modalityFilter));
			Assert.AreEqual(results.Count, 3);

			modalityFilter = "CT";
			serverResults.Clear();
			serverResults.Add(GetModalityResult("MR\\CT", "2"));
			serverResults.Add(GetModalityResult("MR\\CT\\OT", "3"));
			serverResults.Add(GetModalityResult("CT", "4"));
			serverResults.Add(GetModalityResult("MR\\CT", "5"));
			serverResults.Add(GetModalityResult("MR\\CT\\SC", "6"));
			Assert.IsFalse(finder.TestFilterResults(new ReadOnlyQueryResultCollection(serverResults), results, modalityFilter));
			Assert.AreEqual(results.Count, 6);

			//when a server does not even return ModalitiesInStudy, it means it is not supported at all.
			modalityFilter = "MR";
			results.Clear();
			serverResults.Clear();
			serverResults.Add(GetModalityResult(null, "1"));
			serverResults.Add(GetModalityResult(null, "2"));
			serverResults.Add(GetModalityResult(null, "3"));
			Assert.IsTrue(finder.TestFilterResults(new ReadOnlyQueryResultCollection(serverResults), results, modalityFilter));
			Assert.AreEqual(results.Count, 3);

			//We don't support this right now anyway, but if a wildcard query were done, we would just include all the results.
			modalityFilter = "M*";
			results.Clear();
			serverResults.Clear();
			serverResults.Add(GetModalityResult("MR", "1"));
			serverResults.Add(GetModalityResult("MR\\CT", "2"));
			serverResults.Add(GetModalityResult("MR\\CT\\OT", "3"));
			Assert.IsFalse(finder.TestFilterResults(new ReadOnlyQueryResultCollection(serverResults), results, modalityFilter));
			Assert.AreEqual(results.Count, 3);
			
		}

		private QueryResult GetModalityResult(string modalityResult, string studyUID)
		{
			QueryResult result = new QueryResult();
			if (modalityResult != null)
				result.Add(DicomTag.ModalitiesInStudy, modalityResult);
			
			result.Add(DicomTag.StudyInstanceUID, studyUID);
			return result;
		}
	}
}
