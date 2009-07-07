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
            DateTime? now = DateTime.Now;
            DateTime? today = DateTime.Today;
            DateTime? yesterday = today.Value.AddDays(-1);
            DateTime? tomorrow = today.Value.AddDays(1);
            
            // if input was before yesterday
            // we test for a day before the threshold
            Assert.AreEqual(threshold + " days ago", Format.Date(now.Value.AddDays(-threshold), true));           // within threshold
            Assert.AreEqual(Format.DateTime(now.Value.AddDays(-(threshold + 1))), Format.Date(now.Value.AddDays(-(threshold + 1)), true));  // outside threshold
            
            // if input was yesterday
            Assert.AreEqual("Yesterday " + Format.Time(yesterday), Format.Date(yesterday, true));
            
            // if input is today
            Assert.AreEqual("Today " + Format.Time(today), Format.Date(today, true));
            
            // if input is tomorrow
            Assert.AreEqual("Tomorrow " + Format.Time(tomorrow), Format.Date(tomorrow, true));
            
            // if input is beyond tomorrow
            // we test for a day before the threshold
            Assert.AreEqual(threshold + " days from now", Format.Date(now.Value.AddDays(threshold), true));       // within threshold
            Assert.AreEqual(Format.DateTime(now.Value.AddDays(threshold + 2)), Format.Date(now.Value.AddDays(threshold + 2), true));    // outside threshold
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
            DateTime? now = DateTime.Now;
            DateTime? today = DateTime.Today;
            DateTime? yesterday = today.Value.AddDays(-1);
            DateTime? tomorrow = today.Value.AddDays(1);

            // if input was yesterday
            Assert.AreEqual(yesterday.Value.ToString(FormatSettings.Default.DateTimeFormat), Format.Date(yesterday, true));

            // if input is today
            Assert.AreEqual("Today " + Format.Time(today), Format.Date(today, true));

            // if input is tomorrow
            Assert.AreEqual(tomorrow.Value.ToString(FormatSettings.Default.DateTimeFormat), Format.Date(tomorrow, true));

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
        public void Test_Date_NonDescriptive_API()
        {
            DateTime? today = System.DateTime.Today;

            Assert.AreEqual(today.Value.ToString(FormatSettings.Default.DateTimeFormat), Format.Date(today, false));
        }

        [Test]
        public void Test_Date_NonDescriptive_Admin()
        {
            DateTime? today = System.DateTime.Today;

            FormatSettings.Default.DescriptiveFormattingEnabled = false;

            Assert.AreEqual(today.Value.ToString(FormatSettings.Default.DateTimeFormat), Format.Date(today, true));
        }
    }
}

#endif