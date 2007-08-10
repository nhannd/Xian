#if UNIT_TESTS
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using ClearCanvas.ImageServer.Streaming;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Streaming.Tests
{
    [TestFixture]
    public class GeneralStreamingTest
    {

        [Test]
        public void ConstructorTest()
        {
            StudyStream stream = new StudyStream();

            stream = new StudyStream("1.1.1");



        }

    }
}
#endif