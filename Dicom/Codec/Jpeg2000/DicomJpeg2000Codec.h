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

#ifndef __DCMJPEG2000CODEC_H__
#define __DCMJPEG2000CODEC_H__

#pragma once

using namespace System;

using namespace ClearCanvas::Dicom;
using namespace ClearCanvas::Dicom::Codec;

namespace ClearCanvas {
namespace Dicom {
namespace Codec {
namespace Jpeg2000 {

	public ref class DicomJpeg2000Parameters : public DicomCodecParameters {
	private:
		bool _irreversible;
		int _rate;
		array<int>^ _rates;
		bool _isVerbose;
		bool _enableMct;
		bool _updatePmi;

	public:
		DicomJpeg2000Parameters() {
			_irreversible = true;
			_rate = 20;
			_isVerbose = false;
			_enableMct = true;
			_updatePmi = true;

			_rates = gcnew array<int>(9);
			_rates[0] = 1280;
			_rates[1] = 640;
			_rates[2] = 320;
			_rates[3] = 160;
			_rates[4] = 80;
			_rates[5] = 40;
			_rates[6] = 20;
			_rates[7] = 10;
			_rates[8] = 5;
		}

		property bool Irreversible {
			bool get() { return _irreversible; }
			void set(bool value) { _irreversible = value; }
		}

		property int Rate {
			int get() { return _rate; }
			void set(int value) { _rate = value; }
		}

		property array<int>^ RateLevels {
			array<int>^ get() { return _rates; }
			void set(array<int>^ value) { _rates = value; }
		}

		property bool IsVerbose {
			bool get() { return _isVerbose; }
			void set(bool value) { _isVerbose = value; }
		}

		property bool AllowMCT {
			bool get() { return _enableMct; }
			void set(bool value) { _enableMct = value; }
		}

		property bool UpdatePhotometricInterpretation {
			bool get() { return _updatePmi; }
			void set(bool value) { _updatePmi = value; }
		}
	};


	public ref class DicomJpeg2000Codec abstract : public IDicomCodec
	{
	public:
		virtual property String^ Name { String^ get(); };
		virtual property ClearCanvas::Dicom::TransferSyntax^ CodecTransferSyntax { ClearCanvas::Dicom::TransferSyntax^ get(); };

		virtual DicomCodecParameters^ GetDefaultParameters() {
			return gcnew DicomJpeg2000Parameters();
		}

		virtual void Encode(DicomUncompressedPixelData^ oldPixelData, DicomCompressedPixelData^ newPixelData, DicomCodecParameters^ parameters);
		virtual void Decode(DicomCompressedPixelData^ oldPixelData, DicomUncompressedPixelData^ newPixelData, DicomCodecParameters^ parameters);
		virtual void DecodeFrame(int frame, DicomCompressedPixelData^ oldPixelData, DicomUncompressedPixelData^ newPixelData, DicomCodecParameters^ parameters);

		static void Register();
	};

	public ref class DicomJpeg2000LossyCodec : public DicomJpeg2000Codec
	{
	public:
	    property String^ Name { virtual String^ get() override;}
	    property ClearCanvas::Dicom::TransferSyntax^ CodecTransferSyntax { virtual ClearCanvas::Dicom::TransferSyntax^ get() override; };
	};

	public ref class DicomJpeg2000LosslessCodec : public DicomJpeg2000Codec
	{
	public:
	    property String^ Name { virtual String^ get() override;}
	    property ClearCanvas::Dicom::TransferSyntax^ CodecTransferSyntax { virtual ClearCanvas::Dicom::TransferSyntax^ get() override; };
	};

} // Jpeg2000
} // Codec
} // Dicom
} // ClearCanvas

#endif