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