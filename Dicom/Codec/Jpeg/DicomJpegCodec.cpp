// mDCM: A C# DICOM library
//
// Copyright (c) 2008  Colby Dillion
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
//
// Author:
//    Colby Dillion (colby.dillion@gmail.com)

#include "DicomJpegCodec.h"

using namespace System;
using namespace System::IO;

using namespace ClearCanvas::Dicom::Codec;
using namespace ClearCanvas::Dicom;

#include "JpegCodec.h"
#include "DicomJpegParameters.h"

namespace ClearCanvas {
namespace Dicom {
namespace Codec {
namespace Jpeg {

	String^ DicomJpegCodec::Name::get()
	{ 
		return nullptr;
	}
	ClearCanvas::Dicom::TransferSyntax^ DicomJpegCodec::CodecTransferSyntax::get()
	{ 
		return nullptr;
	}
	
	ClearCanvas::Dicom::TransferSyntax^ DicomJpegProcess1Codec::CodecTransferSyntax::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegBaselineProcess1;
	}
	String^ DicomJpegProcess1Codec::Name::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegBaselineProcess1->Name;	
	}

    TransferSyntax^ DicomJpegProcess24Codec::CodecTransferSyntax::get()  {
		return TransferSyntax::JpegExtendedProcess24;
	}
	String^ DicomJpegProcess24Codec::Name::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegExtendedProcess24->Name;	
	}

	TransferSyntax^ DicomJpegLossless14Codec::CodecTransferSyntax::get()  {
		return TransferSyntax::JpegLosslessNonHierarchicalProcess14;
	}
	String^ DicomJpegLossless14Codec::Name::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegLosslessNonHierarchicalProcess14->Name;	
	}

    ClearCanvas::Dicom::TransferSyntax^ DicomJpegLossless14SV1Codec::CodecTransferSyntax::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1;
	}
	String^ DicomJpegLossless14SV1Codec::Name::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1->Name;	
	}
	
	
void DicomJpegCodec::Encode(DicomUncompressedPixelData^ oldPixelData, DicomCompressedPixelData^ newPixelData, DicomCodecParameters^ parameters)
{
	if (parameters == nullptr || parameters->GetType() != DicomJpegParameters::typeid)
		throw gcnew DicomCodecException("Invalid codec parameters");

	DicomJpegParameters^ jparams = (DicomJpegParameters^)parameters;

	IJpegCodec^ codec = GetCodec(oldPixelData->BitsStored, jparams);

	for (int frame = 0; frame < oldPixelData->NumberOfFrames; frame++) {
		codec->Encode(oldPixelData, newPixelData, jparams, frame);
	}

	if (codec->Mode != JpegMode::Lossless) {
		newPixelData->LossyImageCompressionMethod = "ISO_10918_1";
		
		double oldSize = oldPixelData->UncompressedFrameSize;
		double newSize = newPixelData->GetCompressedFrameSize(0);
		String^ ratio = String::Format("{0:0.000}", oldSize / newSize);
		newPixelData->LossyImageCompressionRatio = (float) oldSize / newSize;
	}

	if (oldPixelData->PhotometricInterpretation == "RGB") {
		if (codec->Mode != JpegMode::Lossless) {
			if (jparams->SampleFactor == JpegSampleFactor::SF422)
				newPixelData->PhotometricInterpretation = "YBR_FULL_422";
			else if (jparams->SampleFactor == JpegSampleFactor::SF444)
				newPixelData->PhotometricInterpretation = "YBR_FULL";
		}
	}
}

void DicomJpegCodec::Decode(DicomCompressedPixelData^ oldPixelData, DicomUncompressedPixelData^ newPixelData, DicomCodecParameters^ parameters)
{
	if (parameters == nullptr || parameters->GetType() != DicomJpegParameters::typeid)
		throw gcnew DicomCodecException("Invalid codec parameters");

	DicomJpegParameters^ jparams = (DicomJpegParameters^)parameters;

	IJpegCodec^ codec = GetCodec(oldPixelData->BitsStored, jparams);

	for (int frame = 0; frame < oldPixelData->NumberOfFrames; frame++) {
		codec->Decode(oldPixelData, newPixelData, jparams, frame);
	}

	if (oldPixelData->PhotometricInterpretation->StartsWith("YBR_")) {
		if (jparams->ConvertColorspaceToRGB && codec->Mode != JpegMode::Lossless) {
			newPixelData->PhotometricInterpretation = "RGB";
		}
	}
}

void DicomJpegCodec::DecodeFrame(int frame, DicomCompressedPixelData^ oldPixelData, DicomUncompressedPixelData^ newPixelData, DicomCodecParameters^ parameters)
{
	if (parameters == nullptr || parameters->GetType() != DicomJpegParameters::typeid)
		throw gcnew DicomCodecException("Invalid codec parameters");

	DicomJpegParameters^ jparams = (DicomJpegParameters^)parameters;

	IJpegCodec^ codec = GetCodec(oldPixelData->BitsStored, jparams);

	codec->Decode(oldPixelData, newPixelData, jparams, frame);

	if (oldPixelData->PhotometricInterpretation->StartsWith("YBR_")) {
		if (jparams->ConvertColorspaceToRGB && codec->Mode != JpegMode::Lossless) {
			newPixelData->PhotometricInterpretation = "RGB";
		}
	}
}

//void DicomJpegCodec::Register() {
	//DicomCodec::RegisterCodec(TransferSyntax::JpegBaselineProcess1, DicomJpegProcess1Codec::typeid);
	//DicomCodec::RegisterCodec(TransferSyntax::JpegExtendedProcess24, DicomJpegProcess24Codec::typeid);
	//DicomCodec::RegisterCodec(TransferSyntax::JpegLosslessNonHierarchicalProcess14, DicomJpegLossless14Codec::typeid);
	//DicomCodec::RegisterCodec(TransferSyntax::JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1, DicomJpegLossless14SV1Codec::typeid);
//}

} // Jpeg
} // Codec
} // Dicom
} // ClearCanvas