#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

//#if	UNIT_TESTS

//#pragma warning disable 1591,0419,1574,1587

//using System;
//using System.Collections;
////using MbUnit.Core.Framework;
////using MbUnit.Framework;
//using NUnit.Framework;
//using ClearCanvas.ImageViewer.Imaging;

//namespace ClearCanvas.ImageViewer.Imaging.Tests
//{
//    [TestFixture]
//    public class GrayscaleLUTTest
//    {
//        public GrayscaleLUTTest()
//        {
//        }

//        [TestFixtureSetUp]
//        public void Init()
//        {
//        }
	
//        [TestFixtureTearDown]
//        public void Cleanup()
//        {
//        }


//        [Test]
//        public void CreateLUTRangePositive()
//        {
//            ComposableLut lut = new ComposableLut(10,73);
			
//            Assert.IsTrue(lut.Length == 64);
//            Assert.IsTrue(lut.MinInputValue == 10);
//            Assert.IsTrue(lut.MaxInputValue == 73);
		
//            lut[10] = 100;
//            lut[73] = 200;

//            Assert.IsTrue(lut[10] == 100);
//            Assert.IsTrue(lut[73] == 200);
//        }

//        [Test]
//        public void CreateLUTRangeSigned()
//        {
//            ComposableLut lut = new ComposableLut(-10,53);
			
//            Assert.IsTrue(lut.Length == 64);
//            Assert.IsTrue(lut.MinInputValue == -10);
//            Assert.IsTrue(lut.MaxInputValue == 53);
		
//            lut[-10] = 100;
//            lut[53] = 200;

//            Assert.IsTrue(lut[-10] == 100);
//            Assert.IsTrue(lut[53] == 200);
//        }
		
//        [Test]
//        public void CreateLUTRangeNegative()
//        {
//            ComposableLut lut = new ComposableLut(-74,-11);
			
//            Assert.IsTrue(lut.Length == 64);
//            Assert.IsTrue(lut.MinInputValue == -74);
//            Assert.IsTrue(lut.MaxInputValue == -11);
		
//            lut[-74] = 100;
//            lut[-11] = 200;

//            Assert.IsTrue(lut[-74] == 100);
//            Assert.IsTrue(lut[-11] == 200);
//        }

//        [Test]
//        public void CreateLUTRangeNegativeZero()
//        {
//            ComposableLut lut = new ComposableLut(-63,0);
			
//            Assert.IsTrue(lut.Length == 64);
//            Assert.IsTrue(lut.MinInputValue == -63);
//            Assert.IsTrue(lut.MaxInputValue == 0);
		
//            lut[-63] = 100;
//            lut[0] = 200;

//            Assert.IsTrue(lut[-63] == 100);
//            Assert.IsTrue(lut[0] == 200);
//        }
//    }
//}

//#endif