#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.Tests
{
	/// <summary>
	/// Tests for any pre and post query operations performed by the StudyBrowser.
	/// </summary>
	[TestFixture]
	public class StudyQueryTests
	{
		private static IEnumerable<StudyRootStudyIdentifier> CreateTestStudies()
		{
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "01", ModalitiesInStudy = new[] {"MR"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "02", ModalitiesInStudy = new[] {"MR"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "03", ModalitiesInStudy = new[] {"MR"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "04", ModalitiesInStudy = new[] {"MR", "KO"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "05", ModalitiesInStudy = new[] {"MR", "KO"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "06", ModalitiesInStudy = new[] {"MR", "KO", "PR"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "07", ModalitiesInStudy = new[] {"MR", "KO", "PR"}});

			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "11", ModalitiesInStudy = new[] {"CT"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "12", ModalitiesInStudy = new[] {"CT"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "13", ModalitiesInStudy = new[] {"CT"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "14", ModalitiesInStudy = new[] {"CT", "KO"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "15", ModalitiesInStudy = new[] {"CT", "KO", "PR"}});

			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "21", ModalitiesInStudy = new[] {"PT", "CT"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "22", ModalitiesInStudy = new[] {"PT", "CT"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "23", ModalitiesInStudy = new[] {"CT", "PT"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "24", ModalitiesInStudy = new[] {"CT", "PT", "KO", "PR"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "25", ModalitiesInStudy = new[] {"PT", "CT", "KO", "PR"}});

			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "31", ModalitiesInStudy = new[] {"DX"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "32", ModalitiesInStudy = new[] {"DX"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "33", ModalitiesInStudy = new[] {"DX"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "34", ModalitiesInStudy = new[] {"DX", "KO"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "35", ModalitiesInStudy = new[] {"DX", "KO", "PR"}});

			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "41", ModalitiesInStudy = new[] {"CR"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "42", ModalitiesInStudy = new[] {"CR"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "43", ModalitiesInStudy = new[] {"CR"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "44", ModalitiesInStudy = new[] {"CR", "KO"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "45", ModalitiesInStudy = new[] {"CR", "KO", "PR"}});

			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "51", ModalitiesInStudy = new[] {"PT"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "52", ModalitiesInStudy = new[] {"PT"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "53", ModalitiesInStudy = new[] {"PT"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "54", ModalitiesInStudy = new[] {"PT", "KO"}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "55", ModalitiesInStudy = new[] {"PT", "KO", "PR"}});

			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "90", ModalitiesInStudy = new[] {""}});
			yield return (new StudyRootStudyIdentifier {StudyInstanceUid = "91", ModalitiesInStudy = new string[0]});
		}

		[TestFixtureSetUp]
		public void Initialize()
		{
			var list = CreateTestStudies().Select(s => s.StudyInstanceUid).ToList();
			var distinct = list.Distinct().ToList();
			if (list.Count != distinct.Count)
			{
				Console.WriteLine("CreateTestStudies() must create only unique studies, otherwise tests are invalid");
				throw new InvalidOperationException();
			}
		}

		[Test]
		public void TestIdealOpenModalityQuery()
		{
			var server = new TestDicomServiceNode();
			server.Studies.AddRange(CreateTestStudies());
			server.StudyMatcher = IdealMatchModalitiesInStudy;

			var expectedStudies = server.Studies.Select(s => s.StudyInstanceUid).ToList();

			var actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = null}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=null");
			Assert.AreEqual(1, server.QueryCount, "Too many server query calls");

			actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new string[0]}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=[]");
			Assert.AreEqual(2, server.QueryCount, "Too many server query calls");

			actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new[] {""}}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=['']");
			Assert.AreEqual(3, server.QueryCount, "Too many server query calls");
		}

		[Test]
		public void TestIdealSingleModalityQuery()
		{
			var server = new TestDicomServiceNode();
			server.Studies.AddRange(CreateTestStudies());
			server.StudyMatcher = IdealMatchModalitiesInStudy;

			var expectedStudies = new[] {"01", "02", "03", "04", "05", "06", "07"};

			var actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new[] {"MR"}}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=['MR']");
			Assert.AreEqual(1, server.QueryCount, "Too many server query calls");

			actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new[] {"MR", "MR"}}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=['MR','MR']");
			Assert.AreEqual(2, server.QueryCount, "Too many server query calls");
		}

		[Test]
		public void TestIdealMultiModalityQuery()
		{
			var server = new TestDicomServiceNode();
			server.Studies.AddRange(CreateTestStudies());
			server.StudyMatcher = IdealMatchModalitiesInStudy;

			var expectedStudies = new[]
			                      	{
			                      		"11", "12", "13", "14", "15",
			                      		"21", "22", "23", "24", "25",
			                      		"51", "52", "53", "54", "55"
			                      	};

			var actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new[] {"PT", "CT"}}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=['PT','CT']");
			Assert.AreEqual(2, server.QueryCount, "Too many server query calls");

			expectedStudies = new[]
			                  	{
			                  		"04", "05", "06", "07",
			                  		"11", "12", "13", "14", "15",
			                  		"21", "22", "23", "24", "25",
			                  		"34", "35",
			                  		"44", "45",
			                  		"51", "52", "53", "54", "55"
			                  	};

			actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new[] {"PT", "CT", "KO", "PR"}}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=['PT','CT','KO','PR']");
			Assert.AreEqual(6, server.QueryCount, "Too many server query calls");
		}

		[Test]
		public void TestUnindexedOpenModalityQuery()
		{
			var server = new TestDicomServiceNode();
			server.Studies.AddRange(CreateTestStudies());
			server.StudyMatcher = null;

			var expectedStudies = server.Studies.Select(s => s.StudyInstanceUid).ToList();

			var actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = null}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=null");
			Assert.AreEqual(1, server.QueryCount, "Too many server query calls");

			actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new string[0]}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=[]");
			Assert.AreEqual(2, server.QueryCount, "Too many server query calls");

			actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new[] {""}}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=['']");
			Assert.AreEqual(3, server.QueryCount, "Too many server query calls");
		}

		[Test]
		public void TestUnindexedSingleModalityQuery()
		{
			var server = new TestDicomServiceNode();
			server.Studies.AddRange(CreateTestStudies());
			server.StudyMatcher = null;

			var expectedStudies = server.Studies.Select(s => s.StudyInstanceUid).ToList();

			var actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new[] {"MR"}}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=['MR']");
			Assert.AreEqual(1, server.QueryCount, "Too many server query calls");

			actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new[] {"MR", "MR"}}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=['MR','MR']");
			Assert.AreEqual(2, server.QueryCount, "Too many server query calls");
		}

		[Test]
		public void TestUnindexedMultiModalityQuery()
		{
			var server = new TestDicomServiceNode();
			server.Studies.AddRange(CreateTestStudies());
			server.StudyMatcher = null;

			var expectedStudies = server.Studies.Select(s => s.StudyInstanceUid).ToList();

			var actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new[] {"PT", "CT"}}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=['PT','CT']");
			Assert.AreEqual(1, server.QueryCount, "Too many server query calls");

			actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new[] {"PT", "CT", "KO", "PR"}}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=['PT','CT','KO','PR']");
			Assert.AreEqual(2, server.QueryCount, "Too many server query calls");
		}

		[Test]
		public void TestUnsupportedOpenModalityQuery()
		{
			var server = new TestDicomServiceNode();
			server.Studies.AddRange(CreateTestStudies());
			server.StudyMatcher = null;
			foreach (var study in server.Studies) study.ModalitiesInStudy = new string[0]; // blank the modalities field

			var expectedStudies = server.Studies.Select(s => s.StudyInstanceUid).ToList();

			var actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = null}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=null");
			Assert.AreEqual(1, server.QueryCount, "Too many server query calls");

			actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new string[0]}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=[]");
			Assert.AreEqual(2, server.QueryCount, "Too many server query calls");

			actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new[] {""}}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=['']");
			Assert.AreEqual(3, server.QueryCount, "Too many server query calls");
		}

		[Test]
		public void TestUnsupportedSingleModalityQuery()
		{
			var server = new TestDicomServiceNode();
			server.Studies.AddRange(CreateTestStudies());
			server.StudyMatcher = null;
			foreach (var study in server.Studies) study.ModalitiesInStudy = new string[0]; // blank the modalities field

			var expectedStudies = server.Studies.Select(s => s.StudyInstanceUid).ToList();

			var actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new[] {"MR"}}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=['MR']");
			Assert.AreEqual(1, server.QueryCount, "Too many server query calls");

			actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new[] {"MR", "MR"}}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=['MR','MR']");
			Assert.AreEqual(2, server.QueryCount, "Too many server query calls");
		}

		[Test]
		public void TestUnsupportedMultiModalityQuery()
		{
			var server = new TestDicomServiceNode();
			server.Studies.AddRange(CreateTestStudies());
			server.StudyMatcher = null;
			foreach (var study in server.Studies) study.ModalitiesInStudy = new string[0]; // blank the modalities field

			var expectedStudies = server.Studies.Select(s => s.StudyInstanceUid).ToList();

			var actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new[] {"PT", "CT"}}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=['PT','CT']");
			Assert.AreEqual(1, server.QueryCount, "Too many server query calls");

			actualStudies = StudyBrowserComponent.TestGetStudyEntries(server, new StudyRootStudyIdentifier {ModalitiesInStudy = new[] {"PT", "CT", "KO", "PR"}}).Select(s => s.Study.StudyInstanceUid).ToList();
			AssertCollections(expectedStudies, actualStudies, "ModalitiesInStudy=['PT','CT','KO','PR']");
			Assert.AreEqual(2, server.QueryCount, "Too many server query calls");
		}

		private static bool IdealMatchModalitiesInStudy(StudyRootStudyIdentifier s, StudyRootStudyIdentifier q)
		{
			return q.ModalitiesInStudy.Any(string.IsNullOrEmpty)
			       || s.ModalitiesInStudy.Intersect(q.ModalitiesInStudy).Any();
		}

		private static void AssertCollections<T>(IList<T> expectedCollection, IList<T> actualCollection, string message, params object[] args)
		{
			var msg = string.Format(message, args);

			var workingList = new List<T>(actualCollection);
			foreach (var expectedElement in expectedCollection)
			{
				if (!workingList.Contains(expectedElement))
				{
					if (actualCollection.Contains(expectedElement))
					{
						var expectedCount = expectedCollection.Count(e => Equals(e, expectedElement));
						var actualCount = actualCollection.Count(e => Equals(e, expectedElement));
						Assert.Fail("Expected {2} instances of item '{0}' but found {3}: {1}", expectedElement, msg, expectedCount, actualCount);
					}
					else
					{
						Assert.Fail("Expected item '{0}' not found: {1}", expectedElement, msg);
					}
				}
				workingList.Remove(expectedElement);
			}

			foreach (var unexpectedElement in workingList)
			{
				if (expectedCollection.Contains(unexpectedElement))
				{
					var expectedCount = expectedCollection.Count(e => Equals(e, unexpectedElement));
					var actualCount = actualCollection.Count(e => Equals(e, unexpectedElement));
					Assert.Fail("Expected {2} instances of item '{0}' but found {3}: {1}", unexpectedElement, msg, expectedCount, actualCount);
				}
				else
				{
					Assert.Fail("Unexpected item '{0}' found: {1}", unexpectedElement, msg);
				}
			}
		}

		private class TestDicomServiceNode : IDicomServiceNode, IStudyRootQuery
		{
			public readonly List<StudyRootStudyIdentifier> Studies = new List<StudyRootStudyIdentifier>();

			public Func<StudyRootStudyIdentifier, StudyRootStudyIdentifier, bool> StudyMatcher { get; set; }

			public int QueryCount { get; set; }

			#region Implementation of IDicomServiceNode

			public bool IsSupported<T>() where T : class
			{
				return typeof (T) == typeof (IStudyRootQuery);
			}

			public void GetService<T>(Action<T> withService) where T : class
			{
				withService.Invoke(GetService<T>());
			}

			public T GetService<T>() where T : class
			{
				if (typeof (T) == typeof (IStudyRootQuery))
				{
					return (T) (object) this;
				}
				throw new NotSupportedException();
			}

			public string Name
			{
				get { return "FAKE"; }
			}

			public string AETitle
			{
				get { return "FAKE"; }
			}

			public string Description
			{
				get { return "FAKE"; }
			}

			public string Location
			{
				get { return "FAKE"; }
			}

			public IScpParameters ScpParameters
			{
				get { throw new NotImplementedException(); }
			}

			public IStreamingParameters StreamingParameters
			{
				get { throw new NotImplementedException(); }
			}

			public bool IsLocal
			{
				get { throw new NotImplementedException(); }
			}

			#endregion

			#region Implementation of IStudyRootQuery

			public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria)
			{
				QueryCount++;

				if (StudyMatcher == null) return Studies;
				return Studies.Where(s => StudyMatcher.Invoke(s, queryCriteria)).ToList();
			}

			public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
			{
				throw new NotImplementedException();
			}

			public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
			{
				throw new NotImplementedException();
			}

			#endregion
		}
	}
}

#endif