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
using ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.Tests
{
    internal class TestObject<T> where T :class 
    {
        public T TheProperty;
    }

    internal class TestStringPropertyFilter : StringPropertyFilter<TestObject<string>>
    {
        public bool CalledAddToQuery;
        public bool CalledFilterResults;
        public bool CalledAddToResults;

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
            CalledAddToQuery = false;
            CalledFilterResults = false;
            CalledAddToResults = false;
        }

        protected override System.Linq.IQueryable<TestObject<string>> AddToQuery(System.Linq.IQueryable<TestObject<string>> query)
        {
            CalledAddToQuery = true;
            return base.AddToQuery(query);
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
            Assert.IsFalse(filter.IsCriterionWildcard);

            Assert.IsFalse(filter.CalledAddToQuery);
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
            Assert.IsFalse(filter.IsCriterionWildcard);

            Assert.IsFalse(filter.CalledAddToQuery);
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

            Assert.IsFalse(filter.CalledAddToQuery);
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
            Assert.IsFalse(filter.IsCriterionWildcard);

            Assert.IsTrue(filter.CalledAddToQuery);
            Assert.IsTrue(filter.CalledFilterResults);
            Assert.IsTrue(filter.CalledAddToResults);

            //Should populate the result because it was in the request.
            Assert.AreEqual(testObjects[0].TheProperty, result[DicomTags.PatientId].GetString(0, ""));
        }

        [Test]
        public void TestWildCardCriteria_WildcardAllowed()
        {
            var criteria = new DicomAttributeCollection();
            criteria[DicomTags.PatientId].SetStringValue("test*");

            var filter = new TestStringPropertyFilter(DicomTags.PatientId, criteria);

            Assert.IsTrue(filter.IsWildcardCriterionAllowed);
            Assert.IsTrue(filter.IsCriterionWildcard);
        }

        [Test]
        public void TestWildCardCriteria_WildcardNotAllowed()
        {
            var criteria = new DicomAttributeCollection {ValidateVrLengths = false, ValidateVrValues = false};
            criteria[DicomTags.StudyInstanceUid].SetStringValue("test*");

            var filter = new TestStringPropertyFilter(DicomTags.StudyInstanceUid, criteria);

            Assert.IsFalse(filter.IsWildcardCriterionAllowed);
            Assert.IsFalse(filter.IsCriterionWildcard);
        }
    }
}

#endif