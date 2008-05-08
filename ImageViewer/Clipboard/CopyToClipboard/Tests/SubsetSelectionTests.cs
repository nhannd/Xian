#if	UNIT_TESTS

#pragma warning disable 0419,1574,1587,1591

using System.Collections.Generic;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Clipboard.CopyToClipboard.Tests
{
	[TestFixture]
	public class SubsetSelectionTests
	{
		public SubsetSelectionTests()
		{
		}

		[Test]
		public void TestSuccess1()
		{
			string test = "1, 3, 5, 7-9, 12-";
			List<int> expected = new List<int>(new int[] {1, 1, 3, 3, 5, 5, 7, 9, 12, 20});

			List<int> values;
			bool result = CopySubsetToClipboardComponent.CustomImageSelectionStrategy.Parse(test, 1, 20, out values);
			Assert.IsTrue(result, "Parse failed.");

			CompareLists(expected, values);
		}

		[Test]
		public void TestSuccess2()
		{
			string test = "1, 3, 5, 7-";
			List<int> expected = new List<int>(new int[] {1, 1, 3, 3, 5, 5, 7, 20});

			List<int> values;
			bool result = CopySubsetToClipboardComponent.CustomImageSelectionStrategy.Parse(test, 1, 20, out values);
			Assert.IsTrue(result, "Parse failed.");

			CompareLists(expected, values);
		}

		[Test]
		public void TestSuccess3()
		{
			string test = "-7, 10, 15, 20";
			List<int> expected = new List<int>(new int[] {1, 7, 10, 10, 15, 15, 20, 20});

			List<int> values;
			bool result = CopySubsetToClipboardComponent.CustomImageSelectionStrategy.Parse(test, 1, 20, out values);
			Assert.IsTrue(result, "Parse failed.");

			CompareLists(expected, values);
		}

		[Test]
		public void TestFail1()
		{
			string test = "-7, 10, 15, 2a";
			List<int> values;
			bool result = CopySubsetToClipboardComponent.CustomImageSelectionStrategy.Parse(test, 1, 20, out values);
			Assert.IsFalse(result, "Parse succeeded when it should have failed.");
		}

		[Test]
		public void TestFail2()
		{
			string test = "3, 5,6,-";
			List<int> values;
			bool result = CopySubsetToClipboardComponent.CustomImageSelectionStrategy.Parse(test, 1, 20, out values);
			Assert.IsFalse(result, "Parse succeeded when it should have failed.");
		}

		[Test]
		public void TestFail3()
		{
			string test = "3,, 5,6";
			List<int> values;
			bool result = CopySubsetToClipboardComponent.CustomImageSelectionStrategy.Parse(test, 1, 20, out values);
			Assert.IsFalse(result, "Parse succeeded when it should have failed.");
		}

		[Test]
		public void TestFail4()
		{
			string test = "3, 5,6,12-a";
			List<int> values;
			bool result = CopySubsetToClipboardComponent.CustomImageSelectionStrategy.Parse(test, 1, 20, out values);
			Assert.IsFalse(result, "Parse succeeded when it should have failed.");
		}

		private void CompareLists(List<int> expected, List<int> values)
		{
			Assert.AreEqual(expected.Count, values.Count, "The two lists are not the same size.");
			for(int i = 0; i < expected.Count; ++i)
			{
				if (expected[i] != values[i])
					Assert.Fail("The two lists are not identical.");
			}
		}
	}
}

#endif