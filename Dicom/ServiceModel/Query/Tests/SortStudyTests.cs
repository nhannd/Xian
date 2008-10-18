#if UNIT_TESTS

using System.Collections.Generic;
using NUnit.Framework;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.Dicom.ServiceModel.Query.Tests
{
	[TestFixture]
	public class SortStudyTests
	{
		public SortStudyTests()
		{
		}

		[Test]
		public void Test()
		{
			List<StudyRootStudyIdentifier> identifiers = new List<StudyRootStudyIdentifier>();

			identifiers.Add(CreateStudyIdentifier("3", "20080101", "112300"));
			identifiers.Add(CreateStudyIdentifier("4", "20080101", ""));
			identifiers.Add(CreateStudyIdentifier("2", "20080104", "184400"));
			identifiers.Add(CreateStudyIdentifier("1", "20080104", "184500"));
			identifiers.Add(CreateStudyIdentifier("5", "", ""));
			identifiers.Add(CreateStudyIdentifier("6", "", ""));

			identifiers.Sort(new StudyDateTimeComparer());

			int i = 1;
			foreach (StudyRootStudyIdentifier identifier in identifiers)
			{
				Assert.AreEqual(i.ToString(), identifier.StudyInstanceUid);
				++i;
			}
		}

		private static StudyRootStudyIdentifier CreateStudyIdentifier(string uid, string date, string time)
		{
			StudyRootStudyIdentifier id = new StudyRootStudyIdentifier();
			id.StudyInstanceUid = uid;
			id.StudyDate = date;
			id.StudyTime = time;
			return id;
		}
	}
}

#endif