using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Codec.Jpeg2000;
using ClearCanvas.DicomServices.Codec;

namespace ClearCanvas.ImageServer.Codec.Jpeg2000
{
    [ExtensionOf(typeof(DicomCodecFactoryExtensionPoint))]
    public class Jpeg2000LossyFactory : DicomJpeg2000LossyCodecFactory
    {
        public override DicomCodecParameters GetCodecParameters(DicomAttributeCollection dataSet)
        {
            DicomJpeg2000Parameters parms = new DicomJpeg2000Parameters();

            parms.Irreversible = true;
            parms.UpdatePhotometricInterpretation = true;
            parms.Rate = 5; //1 == Lossless
            return parms;
        }
    }
}
