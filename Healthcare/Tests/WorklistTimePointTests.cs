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
using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Tests
{
    [TestFixture]
    public class WorklistTimePointTests
    {

        [Test]
        public void CreateFixed()
        {
            WorklistTimePoint t1 = new WorklistTimePoint(DateTime.Now, WorklistTimePoint.Resolutions.Day);

            Assert.IsTrue(t1.IsFixed);

            // even though we initialized it with DateTime.Now, it should be rounded to the nearest day
            Assert.AreEqual(DateTime.Today, t1.FixedValue);
        }

        [Test]
        public void CreateRelative()
        {
            WorklistTimePoint t1 = new WorklistTimePoint(TimeSpan.FromHours(0), WorklistTimePoint.Resolutions.RealTime);

            Assert.IsTrue(t1.IsRelative);
            Assert.AreEqual(TimeSpan.FromHours(0), t1.RelativeValue);
        }

        [Test]
        public void ResolveFixedWithDayResolution()
        {
            DateTime now = DateTime.Now;

            WorklistTimePoint t1 = new WorklistTimePoint(now.AddDays(1), WorklistTimePoint.Resolutions.Day);
            Assert.AreEqual(now.Date.AddDays(1), t1.ResolveDown(now));
            Assert.AreEqual(now.Date.AddDays(2), t1.ResolveUp(now));

            WorklistTimePoint t2 = new WorklistTimePoint(now.AddDays(-1), WorklistTimePoint.Resolutions.Day);
            Assert.AreEqual(now.Date.AddDays(-1), t2.ResolveDown(now));
            Assert.AreEqual(now.Date.AddDays(0), t2.ResolveUp(now));
        }

        [Test]
        public void ResolveRelativeWithDayResolution()
        {
            DateTime now = DateTime.Now;

            WorklistTimePoint t1 = new WorklistTimePoint(TimeSpan.FromDays(1), WorklistTimePoint.Resolutions.Day);
            Assert.AreEqual(now.Date.AddDays(1), t1.ResolveDown(now));
            Assert.AreEqual(now.Date.AddDays(2), t1.ResolveUp(now));

            WorklistTimePoint t2 = new WorklistTimePoint(TimeSpan.FromDays(-1), WorklistTimePoint.Resolutions.Day);
            Assert.AreEqual(now.Date.AddDays(-1), t2.ResolveDown(now));
            Assert.AreEqual(now.Date.AddDays(0), t2.ResolveUp(now));

            // what if "now" happens to be right on the boundary??
            DateTime midnight = DateTime.Now.Date;

            WorklistTimePoint t3 = new WorklistTimePoint(TimeSpan.FromDays(1), WorklistTimePoint.Resolutions.Day);
            Assert.AreEqual(midnight.AddDays(1), t1.ResolveDown(now));
            Assert.AreEqual(midnight.AddDays(2), t1.ResolveUp(now));

            WorklistTimePoint t4 = new WorklistTimePoint(TimeSpan.FromDays(-1), WorklistTimePoint.Resolutions.Day);
            Assert.AreEqual(midnight.AddDays(-1), t2.ResolveDown(now));
            Assert.AreEqual(midnight.AddDays(0), t2.ResolveUp(now));
        }

        [Test]
        public void ResolveRelativeWithRealTimeResolution()
        {
            DateTime now = DateTime.Now;

            WorklistTimePoint t1 = new WorklistTimePoint(TimeSpan.FromDays(1), WorklistTimePoint.Resolutions.RealTime);
            Assert.AreEqual(now.AddDays(1), t1.ResolveDown(now));
            Assert.AreEqual(now.AddDays(1), t1.ResolveUp(now));

            WorklistTimePoint t2 = new WorklistTimePoint(TimeSpan.FromDays(-1), WorklistTimePoint.Resolutions.RealTime);
            Assert.AreEqual(now.AddDays(-1), t2.ResolveDown(now));
            Assert.AreEqual(now.AddDays(-1), t2.ResolveUp(now));
        }
    }
}

#endif

