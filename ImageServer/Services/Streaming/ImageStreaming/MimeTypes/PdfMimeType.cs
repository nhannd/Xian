#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.MimeTypes
{
   [ExtensionOf(typeof(ImageMimeTypeProcessorExtensionPoint))]
    public class PdfMimeType : IImageMimeTypeProcessor
    {
        public string OutputMimeType
        {
            get { return "application/pdf"; }
        }

        public MimeTypeProcessorOutput Process(ImageStreamingContext context)
        {
            var output = new MimeTypeProcessorOutput
                             {
                                 ContentType = OutputMimeType
                             };

            var file = new DicomFile(context.ImagePath);
            file.Load(DicomReadOptions.StorePixelDataReferences);

            if (!file.SopClass.Equals(SopClass.EncapsulatedPdfStorage))
                throw new WADOException(HttpStatusCode.NotImplemented, "image/pdf is not supported for this type of object: " + file.SopClass.Name);

            var iod = new EncapsulatedDocumentModuleIod(file.DataSet);

            output.Output = iod.EncapsulatedDocument;
            return output;            
        }
    }
}
