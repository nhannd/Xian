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

#pragma once

using namespace System;
using namespace System::IO;
using namespace System::Xml;

using namespace ClearCanvas::Dicom;
using namespace ClearCanvas::Dicom::Codec;

#include "DicomJpegCodec.h"
#include "DicomJpegParameters.h"

namespace ClearCanvas {
namespace Dicom {
namespace Codec {
namespace Jpeg {

[ClearCanvas::Common::ExtensionOf(DicomCodecFactoryExtensionPoint::typeid)]
public ref class DicomJpegProcess1CodecFactory : public IDicomCodecFactory {
public:
    virtual property String^ Name { String^ get() ;}
    virtual property bool Enabled { bool get(); }
    virtual property ClearCanvas::Dicom::TransferSyntax^ CodecTransferSyntax {  ClearCanvas::Dicom::TransferSyntax^ get(); };

    virtual DicomCodecParameters^ GetCodecParameters(DicomAttributeCollection^ dataSet);
	virtual DicomCodecParameters^ GetCodecParameters(XmlDocument^ parms);
    virtual IDicomCodec^ GetDicomCodec();
};

[ClearCanvas::Common::ExtensionOf(DicomCodecFactoryExtensionPoint::typeid)]
public ref class DicomJpegProcess24CodecFactory : public IDicomCodecFactory {
public:
    virtual property String^ Name { String^ get();}
    virtual property bool Enabled { bool get(); }
    virtual property ClearCanvas::Dicom::TransferSyntax^ CodecTransferSyntax { ClearCanvas::Dicom::TransferSyntax^ get(); };

    virtual DicomCodecParameters^ GetCodecParameters(DicomAttributeCollection^ dataSet);
	virtual DicomCodecParameters^ GetCodecParameters(XmlDocument^ parms);
	virtual IDicomCodec^ GetDicomCodec();
};

[ClearCanvas::Common::ExtensionOf(DicomCodecFactoryExtensionPoint::typeid)]
public ref class DicomJpegLossless14CodecFactory : public IDicomCodecFactory {
public:
    virtual property String^ Name { String^ get();}
    virtual property bool Enabled { bool get(); }
    virtual property ClearCanvas::Dicom::TransferSyntax^ CodecTransferSyntax { ClearCanvas::Dicom::TransferSyntax^ get(); };

    virtual DicomCodecParameters^ GetCodecParameters(DicomAttributeCollection^ dataSet);
    virtual DicomCodecParameters^ GetCodecParameters(XmlDocument^ parms);
	virtual IDicomCodec^ GetDicomCodec();
};

[ClearCanvas::Common::ExtensionOf(DicomCodecFactoryExtensionPoint::typeid)]
public ref class DicomJpegLossless14SV1CodecFactory : public IDicomCodecFactory {
public:
    virtual property String^ Name { String^ get();}
    virtual property bool Enabled { bool get(); }
    virtual property ClearCanvas::Dicom::TransferSyntax^ CodecTransferSyntax { ClearCanvas::Dicom::TransferSyntax^ get(); };

    virtual DicomCodecParameters^ GetCodecParameters(DicomAttributeCollection^ dataSet);
    virtual DicomCodecParameters^ GetCodecParameters(XmlDocument^ parms);
	virtual IDicomCodec^ GetDicomCodec();
};

} // Jpeg
} // Codec
} // Dicom
} // ClearCanvas
