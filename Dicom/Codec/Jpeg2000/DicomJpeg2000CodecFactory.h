
#ifndef __DICOMJPEGCODECFACTORY_H__
#define __DICOMJPEGCODECFACTORY_H__

#pragma once

using namespace System;
using namespace System::IO;
using namespace System::Xml;

using namespace ClearCanvas::Dicom;
using namespace ClearCanvas::Dicom::Codec;

#include "DicomJpeg2000Codec.h"

namespace ClearCanvas {
namespace Dicom {
namespace Codec {
namespace Jpeg2000 {

[ClearCanvas::Common::ExtensionOf(DicomCodecFactoryExtensionPoint::typeid)]
public ref class DicomJpeg2000LosslessCodecFactory : public IDicomCodecFactory {
public:
    virtual property String^ Name { String^ get() ;}
    virtual property ClearCanvas::Dicom::TransferSyntax^ CodecTransferSyntax {  ClearCanvas::Dicom::TransferSyntax^ get(); };

    virtual DicomCodecParameters^ GetCodecParameters(DicomAttributeCollection^ dataSet);
    virtual DicomCodecParameters^ GetCodecParameters(XmlDocument^ parms);
	virtual IDicomCodec^ GetDicomCodec();
};

[ClearCanvas::Common::ExtensionOf(DicomCodecFactoryExtensionPoint::typeid)]
public ref class DicomJpeg2000LossyCodecFactory : public IDicomCodecFactory {
public:
    virtual property String^ Name { String^ get();}
    virtual property ClearCanvas::Dicom::TransferSyntax^ CodecTransferSyntax { ClearCanvas::Dicom::TransferSyntax^ get(); };

    virtual DicomCodecParameters^ GetCodecParameters(DicomAttributeCollection^ dataSet);
    virtual DicomCodecParameters^ GetCodecParameters(XmlDocument^ parms);
	virtual IDicomCodec^ GetDicomCodec();
};



} // Jpeg2000
} // Codec
} // Dicom
} // ClearCanvas

#endif
