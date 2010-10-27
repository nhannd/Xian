#pragma region License (non-CC)

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

#pragma endregion

#ifdef _DEBUG

#include "DicomJpeg2000CodecTest.h"

using namespace System;
using namespace NUnit::Framework;
using namespace ClearCanvas::Dicom;
using namespace ClearCanvas::Dicom::Codec;
using namespace ClearCanvas::Dicom::Codec::Tests;

namespace ClearCanvas {
namespace Dicom {
namespace Codec {
namespace Jpeg2000 {

void DicomJpeg2000CodecTest::LosslessCodecTest()
{
	TransferSyntax^ syntax = TransferSyntax::Jpeg2000ImageCompressionLosslessOnly;

	DicomFile^ file = CreateFile(512, 512, "MONOCHROME1", 12, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(512, 512, "MONOCHROME1", 12, 16, true, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 8, 8, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 8, 8, true, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 255, "MONOCHROME2", 16, 16, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 255, "MONOCHROME2", 16, 16, true, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(256, 256, "MONOCHROME1", 12, 16, true, 5);
	LosslessImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 8, 8, true, 5);
	LosslessImageTest(syntax, file);

	file = CreateFile(255, 255, "RGB", 8, 8, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(255, 255, "RGB", 8, 8, false, 5);
	LosslessImageTest(syntax, file);

	file = CreateFile(512, 512, "RGB", 8, 8, false, 1);
	LosslessImageTest(syntax, file);

	file = CreateFile(512, 512, "RGB", 8, 8, false, 5);
	LosslessImageTest(syntax, file);
}

void DicomJpeg2000CodecTest::LossyCodecTest()
{
	TransferSyntax^ syntax = TransferSyntax::Jpeg2000ImageCompression;

	DicomFile^ file = CreateFile(512, 512, "MONOCHROME1", 12, 16, false, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(512, 512, "MONOCHROME1", 12, 16, true, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 8, 8, false, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 8, 8, true, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(256, 255, "MONOCHROME2", 16, 16, false, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(256, 255, "MONOCHROME2", 16, 16, true, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(256, 256, "MONOCHROME1", 12, 16, true, 5);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "MONOCHROME1", 8, 8, true, 5);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "RGB", 8, 8, false, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(255, 255, "RGB", 8, 8, false, 5);
	LossyImageTest(syntax, file);

	file = CreateFile(512, 512, "RGB", 8, 8, false, 1);
	LossyImageTest(syntax, file);

	file = CreateFile(512, 512, "RGB", 8, 8, false, 5);
	LossyImageTest(syntax, file);
}

}
}
}
}

#endif