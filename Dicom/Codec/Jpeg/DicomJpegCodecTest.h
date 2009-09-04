#pragma once

#if _DEBUG
using namespace System;
using namespace NUnit::Framework;
using namespace ClearCanvas::Dicom;
using namespace ClearCanvas::Dicom::Codec;
using namespace ClearCanvas::Dicom::Codec::Tests;

namespace ClearCanvas {
namespace Dicom {
namespace Codec {
namespace Jpeg {

[NUnit::Framework::TestFixture]
public ref class DicomJpegCodecTest : AbstractCodecTest
{
public:

	[NUnit::Framework::Test]
	void DicomJpegCodecTest::DicomJpegProcess1CodecTest();

	[NUnit::Framework::Test]
	void DicomJpegCodecTest::DicomJpegProcess24CodecTest();

	[NUnit::Framework::Test]
	void DicomJpegCodecTest::DicomJpegLossless14CodecTest();

	[NUnit::Framework::Test]
	void DicomJpegCodecTest::DicomJpegLossless14SV1CodecTest();

};

}
}
}
}

#endif
