#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Codec.Jpeg2000;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Codec.Jpeg2000
{
	/// <summary>
	/// JPEG 2000 lossy codec factory
	/// </summary>
    [ExtensionOf(typeof(DicomCodecFactoryExtensionPoint))]
	public class Jpeg2000LossyFactory : DicomJpeg2000LossyCodecFactory, IImageServerXmlCodecParameters
    {
        public override DicomCodecParameters GetCodecParameters(DicomAttributeCollection dataSet)
        {
            DicomJpeg2000Parameters parms = new DicomJpeg2000Parameters();

            parms.Irreversible = true;
            parms.UpdatePhotometricInterpretation = true;
            parms.Rate = 5.0f; //1 == Lossless
            return parms;
        }

    	public DicomCodecParameters GetCodecParameters(XmlDocument parms)
    	{
			DicomJpeg2000Parameters codecParms = new DicomJpeg2000Parameters();

			codecParms.Irreversible = true;
			codecParms.UpdatePhotometricInterpretation = true;

			XmlElement element = parms.DocumentElement;

			string ratioString = element.Attributes["ratio"].Value;
			float ratio;
			if (false == float.TryParse(ratioString, out ratio))
				throw new ApplicationException("Invalid quality specified for JPEG 2000 Lossy: " + ratioString);

			codecParms.Rate = ratio;

			return codecParms;
    	}
    }
}
