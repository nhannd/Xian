#include "DicomJpegCodecFactory.h"
#include "DicomJpegCodec.h"

using namespace System;
using namespace System::IO;

using namespace ClearCanvas::Dicom::Codec;
using namespace ClearCanvas::Dicom;

#include "DicomJpegParameters.h"

namespace ClearCanvas {
namespace Dicom {
namespace Codec {
namespace Jpeg {
	
	ClearCanvas::Dicom::TransferSyntax^ DicomJpegProcess1CodecFactory::CodecTransferSyntax::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegBaselineProcess1;
	}
	String^ DicomJpegProcess1CodecFactory::Name::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegBaselineProcess1->Name;	
	}
	DicomCodecParameters^ DicomJpegProcess1CodecFactory::GetCodecParameters(DicomAttributeCollection^ dataSet) {
		return gcnew DicomJpegParameters();
	}
	IDicomCodec^ DicomJpegProcess1CodecFactory::GetDicomCodec() {
		return gcnew DicomJpegProcess1Codec();
	}
	void DicomJpegProcess1CodecFactory::Register() {
		DicomCodecRegistry::RegisterCodec(TransferSyntax::JpegBaselineProcess1, gcnew DicomJpegProcess1CodecFactory());
	}


    TransferSyntax^ DicomJpegProcess24CodecFactory::CodecTransferSyntax::get()  {
		return TransferSyntax::JpegExtendedProcess24;
	}
	String^ DicomJpegProcess24CodecFactory::Name::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegExtendedProcess24->Name;	
	}
	DicomCodecParameters^ DicomJpegProcess24CodecFactory::GetCodecParameters(DicomAttributeCollection^ dataSet) {
		return gcnew DicomJpegParameters();
	}
	IDicomCodec^ DicomJpegProcess24CodecFactory::GetDicomCodec() {
		return gcnew DicomJpegProcess24Codec();
	}
	void DicomJpegProcess24CodecFactory::Register() {
		DicomCodecRegistry::RegisterCodec(TransferSyntax::JpegExtendedProcess24, gcnew DicomJpegProcess24CodecFactory());
	}


	TransferSyntax^ DicomJpegLossless14CodecFactory::CodecTransferSyntax::get()  {
		return TransferSyntax::JpegLosslessNonHierarchicalProcess14;
	}
	String^ DicomJpegLossless14CodecFactory::Name::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegLosslessNonHierarchicalProcess14->Name;	
	}
	DicomCodecParameters^ DicomJpegLossless14CodecFactory::GetCodecParameters(DicomAttributeCollection^ dataSet) {
		return gcnew DicomJpegParameters();
	}
	IDicomCodec^ DicomJpegLossless14CodecFactory::GetDicomCodec() {
		return gcnew DicomJpegLossless14Codec();
	}
	void DicomJpegLossless14CodecFactory::Register() {
		DicomCodecRegistry::RegisterCodec(TransferSyntax::JpegLosslessNonHierarchicalProcess14, gcnew DicomJpegLossless14CodecFactory());
	}

    ClearCanvas::Dicom::TransferSyntax^ DicomJpegLossless14SV1CodecFactory::CodecTransferSyntax::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1;
	}
	String^ DicomJpegLossless14SV1CodecFactory::Name::get()  {
		return ClearCanvas::Dicom::TransferSyntax::JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1->Name;	
	}
	DicomCodecParameters^ DicomJpegLossless14SV1CodecFactory::GetCodecParameters(DicomAttributeCollection^ dataSet) {
		return gcnew DicomJpegParameters();
	}
	IDicomCodec^ DicomJpegLossless14SV1CodecFactory::GetDicomCodec() {
		return gcnew DicomJpegLossless14SV1Codec();
	}
	void DicomJpegLossless14SV1CodecFactory::Register() {
		DicomCodecRegistry::RegisterCodec(TransferSyntax::JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1, gcnew DicomJpegLossless14SV1CodecFactory());
	}

} // Jpeg
} // Codec
} // Dicom
} // ClearCanvas