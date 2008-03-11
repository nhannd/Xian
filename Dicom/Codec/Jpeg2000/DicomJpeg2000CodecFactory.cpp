#include "DicomJpeg2000CodecFactory.h"
#include "DicomJpeg2000Codec.h"

using namespace System;
using namespace System::IO;

using namespace ClearCanvas::Dicom::Codec;
using namespace ClearCanvas::Dicom;

namespace ClearCanvas {
namespace Dicom {
namespace Codec {
namespace Jpeg2000 {
	
	ClearCanvas::Dicom::TransferSyntax^ DicomJpeg2000LosslessCodecFactory::CodecTransferSyntax::get()  {
		return ClearCanvas::Dicom::TransferSyntax::Jpeg2000ImageCompressionLosslessOnly;
	}
	String^ DicomJpeg2000LosslessCodecFactory::Name::get()  {
		return ClearCanvas::Dicom::TransferSyntax::Jpeg2000ImageCompressionLosslessOnly->Name;	
	}
	DicomCodecParameters^ DicomJpeg2000LosslessCodecFactory::GetCodecParameters(DicomAttributeCollection^ dataSet) {
		return gcnew DicomJpeg2000Parameters();
	}
	IDicomCodec^ DicomJpeg2000LosslessCodecFactory::GetDicomCodec() {
		return gcnew DicomJpeg2000LosslessCodec();
	}
	void DicomJpeg2000LosslessCodecFactory::Register() {
		DicomCodecRegistry::RegisterCodec(TransferSyntax::Jpeg2000ImageCompressionLosslessOnly, gcnew DicomJpeg2000LosslessCodecFactory());
	}


	TransferSyntax^ DicomJpeg2000LossyCodecFactory::CodecTransferSyntax::get()  {
		return TransferSyntax::Jpeg2000ImageCompression;
	}
	String^ DicomJpeg2000LossyCodecFactory::Name::get()  {
		return ClearCanvas::Dicom::TransferSyntax::Jpeg2000ImageCompression->Name;	
	}
	DicomCodecParameters^ DicomJpeg2000LossyCodecFactory::GetCodecParameters(DicomAttributeCollection^ dataSet) {
		return gcnew DicomJpeg2000Parameters();
	}
	IDicomCodec^ DicomJpeg2000LossyCodecFactory::GetDicomCodec() {
		return gcnew DicomJpeg2000LossyCodec();
	}

	void DicomJpeg2000LossyCodecFactory::Register() {
		DicomCodecRegistry::RegisterCodec(TransferSyntax::Jpeg2000ImageCompression, gcnew DicomJpeg2000LossyCodecFactory());
	}

} // Jpeg2000
} // Codec
} // Dicom
} // ClearCanvas
