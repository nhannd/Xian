#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

#if UNIT_TESTS

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.Tests
{
    internal class TestObject<T> where T :class 
    {
        public T TheProperty;
    }

    internal class TestStringPropertyFilter : StringDicomPropertyFilter<TestObject<string>>
    {
        public bool CalledAddEqualsToQuery;
        public bool CalledAddLikeToQuery;
        public bool CalledFilterResults;
        public bool CalledAddToResults;
        public string EqualsCriterion = "";
        public string LikeCriterion = "";

        public TestStringPropertyFilter(DicomTagPath path, DicomAttributeCollection criteria)
            : base(path, criteria)
        {
        }

        public TestStringPropertyFilter(DicomTag tag, DicomAttributeCollection criteria)
            : base(tag, criteria)
        {
        }

        public TestStringPropertyFilter(uint tag, DicomAttributeCollection criteria) : base(tag, criteria)
        {
        }

        public void Reset()
        {
            CalledAddEqualsToQuery = false;
            CalledAddLikeToQuery = false;
            CalledFilterResults = false;
            CalledAddToResults = false;
            EqualsCriterion = LikeCriterion = "";
        }

        protected override System.Linq.IQueryable<TestObject<string>> AddEqualsToQuery(System.Linq.IQueryable<TestObject<string>> query, string criterion)
        {
            CalledAddEqualsToQuery = true;
            EqualsCriterion = criterion;
            return query;
        }

        protected override System.Linq.IQueryable<TestObject<string>> AddLikeToQuery(System.Linq.IQueryable<TestObject<string>> query, string criterion)
        {
            CalledAddLikeToQuery = true;
            LikeCriterion = criterion;
            return query;
        }

        protected override System.Collections.Generic.IEnumerable<TestObject<string>> FilterResults(System.Collections.Generic.IEnumerable<TestObject<string>> results)
        {
            CalledFilterResults = true;
            return base.FilterResults(results);
        }

        protected override void AddValueToResult(TestObject<string> item, DicomAttribute resultAttribute)
        {
            CalledAddToResults = true;
            resultAttribute.SetStringValue(item.TheProperty);
        }
    }

    [TestFixture]
    public class PropertyFilterTests
    {
        [Test]
        public void TestBasicString_NoCriteria()
        {
            var testObjects = new[] {new TestObject<string> {TheProperty = "test"}};
            var criteria = new DicomAttributeCollection();
            var result = new DicomAttributeCollection();

            var filter = new TestStringPropertyFilter(DicomTags.PatientId, criteria);
            var iFilter = (IPropertyFilter<TestObject<string>>) filter;
            iFilter.AddToQuery(null);
            iFilter.FilterResults(testObjects);
            iFilter.SetAttributeValue(testObjects[0], result);

            Assert.IsTrue(filter.IsCriterionEmpty);
            Assert.IsFalse(filter.IsCriterionNull);
            Assert.IsFalse(filter.ShouldAddToQuery);
            Assert.IsFalse(filter.ShouldAddToResult);
            Assert.IsTrue(filter.IsWildcardCriterionAllowed);
            Assert.IsFalse(filter.IsWildcardCriterion(filter.CriterionValue));

            Assert.IsFalse(filter.CalledAddEqualsToQuery);
            Assert.IsFalse(filter.CalledAddLikeToQuery);
            Assert.IsFalse(filter.CalledFilterResults);
            Assert.IsFalse(filter.CalledAddToResults);
        }

        [Test]
        public void TestBasicString_NullCriteria()
        {
            var testObjects = new[] { new TestObject<string> { TheProperty = "test" } };
            var criteria = new DicomAttributeCollection();
            criteria[DicomTags.PatientId].SetNullValue();
            var result = new DicomAttributeCollection();

            var filter = new TestStringPropertyFilter(DicomTags.PatientId, criteria);
            var iFilter = (IPropertyFilter<TestObject<string>>)filter;
            iFilter.AddToQuery(null);
            iFilter.FilterResults(testObjects);
            iFilter.SetAttributeValue(testObjects[0], result);

            Assert.IsFalse(filter.IsCriterionEmpty);
            Assert.IsTrue(filter.IsCriterionNull);
            Assert.IsFalse(filter.ShouldAddToQuery);
            Assert.IsTrue(filter.ShouldAddToResult);
            Assert.IsTrue(filter.IsWildcardCriterionAllowed);
            Assert.IsFalse(filter.IsWildcardCriterion(filter.CriterionValue));

            Assert.IsFalse(filter.CalledAddEqualsToQuery);
            Assert.IsFalse(filter.CalledAddLikeToQuery);
            Assert.IsFalse(filter.CalledFilterResults);
            Assert.IsTrue(filter.CalledAddToResults);

            //Should populate the result because it was in the request.
            Assert.AreEqual(testObjects[0].TheProperty, result[DicomTags.PatientId].GetString(0, ""));

            filter.Reset();
            criteria[DicomTags.PatientId].SetStringValue("");
            result[DicomTags.PatientId].SetEmptyValue();

            iFilter.AddToQuery(null);
            iFilter.FilterResults(testObjects);
            iFilter.SetAttributeValue(testObjects[0], result);

            Assert.IsFalse(filter.IsCriterionEmpty);
            Assert.IsTrue(filter.IsCriterionNull);
            Assert.IsFalse(filter.ShouldAddToQuery);
            Assert.IsTrue(filter.ShouldAddToResult);
            Assert.IsTrue(filter.IsWildcardCriterionAllowed);

            Assert.IsFalse(filter.CalledAddEqualsToQuery);
            Assert.IsFalse(filter.CalledAddLikeToQuery);
            Assert.IsFalse(filter.CalledFilterResults);
            Assert.IsTrue(filter.CalledAddToResults);

            //Should populate the result because it was in the request.
            Assert.AreEqual(testObjects[0].TheProperty, result[DicomTags.PatientId].GetString(0, ""));
        }

        [Test]
        public void TestBasicString_SimpleCriteria()
        {
            var testObjects = new[] { new TestObject<string> { TheProperty = "test" } };
            var criteria = new DicomAttributeCollection();
            criteria[DicomTags.PatientId].SetStringValue("test");
            var result = new DicomAttributeCollection();

            var filter = new TestStringPropertyFilter(DicomTags.PatientId, criteria);
            var iFilter = (IPropertyFilter<TestObject<string>>)filter;
            iFilter.AddToQuery(null);
            iFilter.FilterResults(testObjects);
            iFilter.SetAttributeValue(testObjects[0], result);

            Assert.IsFalse(filter.IsCriterionEmpty);
            Assert.IsFalse(filter.IsCriterionNull);
            Assert.IsTrue(filter.ShouldAddToQuery);
            Assert.IsTrue(filter.ShouldAddToResult);
            Assert.IsTrue(filter.IsWildcardCriterionAllowed);
            Assert.IsFalse(filter.IsWildcardCriterion(filter.CriterionValue));

            Assert.IsTrue(filter.CalledAddEqualsToQuery);
            Assert.IsFalse(filter.CalledAddLikeToQuery);
            //Only if enabled.
            Assert.IsFalse(filter.CalledFilterResults);
            Assert.IsTrue(filter.CalledAddToResults);

            //Should populate the result because it was in the request.
            Assert.AreEqual(testObjects[0].TheProperty, result[DicomTags.PatientId].GetString(0, ""));
        }

        [Test]
        public void TestWildCardCriteria_WildcardAllowed()
        {
            var testObjects = new[] { new TestObject<string> { TheProperty = "test" } };
            var criteria = new DicomAttributeCollection();
            criteria[DicomTags.PatientId].SetStringValue("test*");
            var result = new DicomAttributeCollection();

            var filter = new TestStringPropertyFilter(DicomTags.PatientId, criteria);
            var iFilter = (IPropertyFilter<TestObject<string>>)filter;
            iFilter.AddToQuery(null);
            iFilter.FilterResults(testObjects);
            iFilter.SetAttributeValue(testObjects[0], result);

            Assert.IsFalse(filter.IsCriterionEmpty);
            Assert.IsFalse(filter.IsCriterionNull);
            Assert.IsTrue(filter.ShouldAddToQuery);
            Assert.IsTrue(filter.ShouldAddToResult);
            Assert.IsTrue(filter.IsWildcardCriterionAllowed);
            Assert.IsTrue(filter.IsWildcardCriterion(filter.CriterionValue));

            Assert.AreEqual(filter.EqualsCriterion ?? "", "");
            Assert.AreEqual(filter.LikeCriterion, "test%");

            Assert.IsFalse(filter.CalledAddEqualsToQuery);
            Assert.IsTrue(filter.CalledAddLikeToQuery);
            //Only if enabled.
            Assert.IsFalse(filter.CalledFilterResults);
            Assert.IsTrue(filter.CalledAddToResults);

            //Should populate the result because it was in the request.
            Assert.AreEqual(testObjects[0].TheProperty, result[DicomTags.PatientId].GetString(0, ""));
        }

        [Test]
        public void TestWildCardCriteria_WildcardNotAllowed()
        {
            var testObjects = new[] { new TestObject<string> { TheProperty = "test" } };
            var criteria = new DicomAttributeCollection();
            criteria[DicomTags.StudyInstanceUid].SetStringValue("test*");
            var result = new DicomAttributeCollection();

            var filter = new TestStringPropertyFilter(DicomTags.StudyInstanceUid, criteria);
            var iFilter = (IPropertyFilter<TestObject<string>>)filter;
            iFilter.AddToQuery(null);
            iFilter.FilterResults(testObjects);
            iFilter.SetAttributeValue(testObjects[0], result);

            Assert.IsFalse(filter.IsCriterionEmpty);
            Assert.IsFalse(filter.IsCriterionNull);
            Assert.IsTrue(filter.ShouldAddToQuery);
            Assert.IsTrue(filter.ShouldAddToResult);
            Assert.IsFalse(filter.IsWildcardCriterionAllowed);
            Assert.IsFalse(filter.IsWildcardCriterion(filter.CriterionValue));

            Assert.AreEqual(filter.EqualsCriterion, "test*");
            Assert.AreEqual(filter.LikeCriterion ?? "", "");

            Assert.IsTrue(filter.CalledAddEqualsToQuery);
            Assert.IsFalse(filter.CalledAddLikeToQuery);
            //Only if enabled.
            Assert.IsFalse(filter.CalledFilterResults);
            Assert.IsTrue(filter.CalledAddToResults);

            //Should populate the result because it was in the request.
            Assert.AreEqual(testObjects[0].TheProperty, result[DicomTags.StudyInstanceUid].GetString(0, ""));
        }
    }
}

#endif