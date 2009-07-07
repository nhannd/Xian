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
using ClearCanvas.Common;
using NUnit.Framework;

namespace ClearCanvas.Workflow.Tests
{
    [TestFixture]
    public class PerformedStepTests
    {
        class ConcretePerformedStep : PerformedStep
        {
            public ConcretePerformedStep(ConcreteActivityPerformer performer, DateTime? startTime)
                : base(performer, startTime, new PerformedStepStatusTransitionLogic())
            {
                
            }
        }
        class ConcreteActivityPerformer : ActivityPerformer
        {
            
        }

        #region Method Tests

        [Test]
        public void Test_Ctor()
        {
            DateTime? now = DateTime.Now;
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
            ConcretePerformedStep performedStep = new ConcretePerformedStep(activityPerformer, now);
            Assert.IsNotNull(performedStep.Activities);
            Assert.AreEqual(now, performedStep.StartTime);
            Assert.AreEqual(performedStep.Performer, activityPerformer);
        }

        [Test]
        public void Test_Discontinue()
        {
            DateTime? now = DateTime.Now;
            DateTime? end = now + TimeSpan.FromDays(3);
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
            ConcretePerformedStep performedStep = new ConcretePerformedStep(activityPerformer, now);
            Assert.AreEqual(PerformedStepStatus.IP, performedStep.State);

            performedStep.Discontinue(end); // Perform event

            // Make assertions
            Assert.AreEqual(PerformedStepStatus.DC, performedStep.State);
            Assert.IsTrue(RoughlyEqual(performedStep.LastStateChangeTime, Platform.Time));
            Assert.AreEqual(end, performedStep.EndTime);
        }

        [Test]
        public void Test_Discontinue_NullEndTime()
        {
            DateTime? now = DateTime.Now;
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
            ConcretePerformedStep performedStep = new ConcretePerformedStep(activityPerformer, now);
            Assert.AreEqual(PerformedStepStatus.IP, performedStep.State);

            performedStep.Discontinue(); // Perform event

            // Make assertions
            Assert.AreEqual(PerformedStepStatus.DC, performedStep.State);
            Assert.IsTrue(RoughlyEqual(performedStep.LastStateChangeTime, Platform.Time));
            Assert.IsTrue(RoughlyEqual(performedStep.EndTime, Platform.Time));
        }

        [Test]
        public void Test_Complete()
        {
            DateTime? now = DateTime.Now;
            DateTime? end = now + TimeSpan.FromDays(3);
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
            ConcretePerformedStep performedStep = new ConcretePerformedStep(activityPerformer, now);
            Assert.AreEqual(PerformedStepStatus.IP, performedStep.State);

            performedStep.Complete(end); // Perform event

            // Make assertions
            Assert.AreEqual(PerformedStepStatus.CM, performedStep.State);
            Assert.IsTrue(RoughlyEqual(performedStep.LastStateChangeTime, Platform.Time));
            Assert.AreEqual(end, performedStep.EndTime);
        }

        [Test]
        public void Test_Complete_NullEndTime()
        {
            DateTime? now = DateTime.Now;
            ConcreteActivityPerformer activityPerformer = new ConcreteActivityPerformer();
            ConcretePerformedStep performedStep = new ConcretePerformedStep(activityPerformer, now);
            Assert.AreEqual(PerformedStepStatus.IP, performedStep.State);

            performedStep.Complete(); // Perform event

            // Make assertions
            Assert.AreEqual(PerformedStepStatus.CM, performedStep.State);
            Assert.IsTrue(RoughlyEqual(performedStep.LastStateChangeTime, Platform.Time));
            Assert.IsTrue(RoughlyEqual(performedStep.EndTime, Platform.Time));
        }

        #endregion

        private static bool RoughlyEqual(DateTime? x, DateTime? y)
        {
            if (!x.HasValue && !y.HasValue)
                return true;

            if (!x.HasValue || !y.HasValue)
                return false;

            DateTime xx = x.Value;
            DateTime yy = y.Value;

            // for these purposes, if the times are within 1 second, that is good enough
            return Math.Abs((xx - yy).TotalSeconds) < 1;
        }
    }
}

#endif