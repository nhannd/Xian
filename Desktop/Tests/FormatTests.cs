#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace ClearCanvas.Desktop.Tests
{
    [TestFixture]
    public class FormatTests
    {
        /// <summary>
        /// We use the boundary or "threshold" as the test here to determine
        /// assume that if case is that we have friendly terms just inside the boundary
        /// and regular dates just outside that the threshold does indeed hold.
        /// 
        /// We override the default setting here by manually setting it to something known
        /// that can be tested against.
        /// </summary>
        [Test]
        public void Test_Date_Descriptive()
        {
            int threshold = 5;
            FormatSettings.Default.DescriptiveDateThresholdInDays = threshold;
            DateTime? today = System.DateTime.Today;
            DateTime? yesterday = today.Value.AddDays(-1);
            DateTime? tomorrow = today.Value.AddDays(1);
            DateTime? afterTomorrow = tomorrow.Value.AddDays(1);
            
            // if input was before yesterday
            // we test for a day before the threshold
            Assert.AreEqual(threshold - 1 + " days ago", Format.Date(today.Value.AddDays(-(threshold - 1)), true));           // within threshold
            Assert.AreEqual(Format.DateTime(today.Value.AddDays(-threshold)), Format.Date(today.Value.AddDays(-threshold), true));  // outside threshold
            
            // if input was yesterday
            Assert.AreEqual("Yesterday " + Format.Time(yesterday), Format.Date(yesterday, true));
            
            // if input is today
            Assert.AreEqual("Today " + Format.Time(today), Format.Date(today, true));
            
            // if input is tomorrow
            Assert.AreEqual("Tomorrow " + Format.Time(tomorrow), Format.Date(tomorrow, true));
            
            // if input is beyond tomorrow
            // we test for a day before the threshold
            Assert.AreEqual(threshold - 1 + " days from now", Format.Date(today.Value.AddDays(threshold - 1), true));       // within threshold
            Assert.AreEqual(Format.DateTime(today.Value.AddDays(threshold)), Format.Date(today.Value.AddDays(threshold), true));    // outside threshold
        }

        /// <summary>
        /// premise of this test is that if the following assertions succeed, 
        /// then that means that the execution path never directs to the "friendly terms"
        /// 
        /// We override the default setting here by manually setting it to something known
        /// that can be tested against.
        /// </summary>
        [Test]
        public void Test_Date_Descriptive_MininumThresholds()
        {
            int threshold = 0;
            FormatSettings.Default.DescriptiveDateThresholdInDays = threshold;
            DateTime? today = System.DateTime.Today;
            DateTime? yesterday = today.Value.AddDays(-1);
            DateTime? tomorrow = today.Value.AddDays(1);
            DateTime? afterTomorrow = tomorrow.Value.AddDays(1);

            // if input was yesterday
            Assert.AreEqual("Yesterday " + Format.Time(yesterday), Format.Date(yesterday, true));

            // if input is today
            Assert.AreEqual("Today " + Format.Time(today), Format.Date(today, true));

            // if input is tomorrow
            Assert.AreEqual("Tomorrow " + Format.Time(tomorrow), Format.Date(tomorrow, true));

            threshold = 1;
            FormatSettings.Default.DescriptiveDateThresholdInDays = threshold;

            // if input was yesterday
            Assert.AreEqual("Yesterday " + Format.Time(yesterday), Format.Date(yesterday, true));

            // if input is today
            Assert.AreEqual("Today " + Format.Time(today), Format.Date(today, true));

            // if input is tomorrow
            Assert.AreEqual("Tomorrow " + Format.Time(tomorrow), Format.Date(tomorrow, true));
        }

        [Test]
        public void Test_Date_NonDescriptive()
        {
            DateTime? today = System.DateTime.Today;

            Assert.AreEqual(today.Value.ToString(FormatSettings.Default.DateFormat), Format.Date(today, false));
        }
    }
}

#endif