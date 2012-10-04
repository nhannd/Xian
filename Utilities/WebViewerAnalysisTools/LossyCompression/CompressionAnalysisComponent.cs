#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.StudyManagement;
using Encoder=System.Drawing.Imaging.Encoder;

namespace ClearCanvas.Utilities.WebViewerAnalysisTools.LossyCompression
{
    /// <summary>
    /// CompressionAnalysisComponent class.
    /// </summary>
    [AssociateView(typeof(CompressionAnalysisComponentViewExtensionPoint))]
    public class CompressionAnalysisComponent: ApplicationComponent
    {
        protected internal IPresentationImage Image { get; set; }

        protected internal IDesktopWindow DesktopWindow { get; set; }

        internal static void Launch(IDesktopWindow desktopWindow, IPresentationImage image)
        {
            Platform.CheckForNullReference(desktopWindow, "desktopWindow");
            Platform.CheckForNullReference(image, "image");

            //initialize the component.
            CompressionAnalysisComponent component = new CompressionAnalysisComponent();
            
            // Load the image from disk instead so that the text overlay 
            // will be the same as other images
            DicomMessageSopDataSource ds = (image as IDicomPresentationImage).Sop.DataSource as DicomMessageSopDataSource;
            DicomFile file = ds.SourceMessage as DicomFile;
            ISopDataSource sopDS = new LocalSopDataSource(file);
            component.Image = PresentationImageFactory.Create(new ImageSop(sopDS))[0];

            if (ApplicationComponentExitCode.Accepted != LaunchAsDialog(desktopWindow, component, "Compression Analysis"))
                return;
        }


        private ImageComparisonResult Compare(string description, Bitmap bmp1, Bitmap bmp2)
        {
            ImageComparisonResult result = BitmapComparison.Compare(ref bmp1, ref bmp2);
            result.Description = description;
            return result;
        }


        private Bitmap ConvertToJpeg(IDicomPresentationImage image, Bitmap losslessBitmap, int quality, out float compressionRatio)
        {
            ImageCodecInfo[] availableCodecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo imageCodec in availableCodecs)
            {
                if (imageCodec.MimeType.Equals("image/jpeg"))
                {
                    EncoderParameters encodeParams = new EncoderParameters(1);
                    encodeParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);

                    Stream ms = new MemoryStream();
                    losslessBitmap.Save(ms, imageCodec, encodeParams);
                    ms.Position = 0;

                    long originalSize = image.Frame.Rows*image.Frame.Columns*image.Frame.SamplesPerPixel*
                                        image.Frame.BitsAllocated/8;

                    compressionRatio = (float)originalSize / ms.Length;
                    return new Bitmap(ms);
                }
            }
            compressionRatio = 0;
            return null;
        }


        /// <summary>
        /// Compare Bitmap of the lossless image and bitmap of the lossy compressed image
        /// </summary>
        /// <param name="dicomCompressQuality"></param>
        public void CompareLosslessBMPVsLossyBMP(int dicomCompressQuality)
        {
            IDicomPresentationImage originalDicomImage = (IDicomPresentationImage)(Image as IDicomPresentationImage).Clone();
            
            IDicomPresentationImage lossyDicomImage = originalDicomImage.Clone() as IDicomPresentationImage;
            DicomMessageSopDataSource ds = lossyDicomImage.Sop.DataSource as DicomMessageSopDataSource;
            
            IDicomCodecFactory[] factories = Dicom.Codec.DicomCodecRegistry.GetCodecFactories();
            IDicomCodec codec = null;
            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement("Root");
            XmlAttribute ratio = xml.CreateAttribute("ratio");
            ratio.Value = dicomCompressQuality.ToString();
            root.Attributes.Append(ratio);
            xml.AppendChild(root);

            DicomCodecParameters codecParams = null;
            foreach (IDicomCodecFactory factory in factories)
            {
                if (factory.CodecTransferSyntax.Equals(TransferSyntax.Jpeg2000ImageCompression))
                {
                    codec = factory.GetDicomCodec();
                    codecParams = factory.GetCodecParameters(xml);
                    break;
                }
            }

            if (codec == null)
            {
                throw new ApplicationException("Unable to find codec for Jpeg2000ImageCompression");
            }

            DicomFile file = new DicomFile((ds.SourceMessage as DicomFile).Filename);
            DicomMessageSopDataSource newDS = new LocalSopDataSource(file);

            DicomUid newSopUid = DicomUid.GenerateUid();
            newDS.SourceMessage.MetaInfo[DicomTags.MediaStorageSopInstanceUid].SetStringValue(newSopUid.UID);
            newDS.SourceMessage.DataSet[DicomTags.SopInstanceUid].SetUid(0, newSopUid);
            newDS.SourceMessage.ChangeTransferSyntax(TransferSyntax.Jpeg2000ImageCompression, codec, codecParams);
            ImageSop newSop = new ImageSop(newDS);
            IPresentationImage lossyImage = PresentationImageFactory.Create(newSop)[0];

            Bitmap losslessBitmap = originalDicomImage.DrawToBitmap(originalDicomImage.Frame.Columns, originalDicomImage.Frame.Rows);
            Bitmap lossyBitmap = lossyImage.DrawToBitmap(originalDicomImage.Frame.Columns, originalDicomImage.Frame.Rows);

            
            ImageComparisonResult result = Compare(String.Format("J2K LOSSY COMPRESSION EFFECT: Comparing bitmap of the lossless image and bitmap of the lossy J2K compressed image (Compression Ratio={0})",
                dicomCompressQuality), losslessBitmap, lossyBitmap);

            
            ImageComparisonResultControl ctrl = new ImageComparisonResultControl();
            ctrl.SetImageComparisonResult(result);

            Form form = new Form();
            form.AutoSize = true;
            form.Controls.Add(ctrl);
            form.Show();
            
        }


        /// <summary>
        /// Compares the bitmap and the jpeg of the lossless image.
        /// </summary>
        /// <param name="jpegQuality"></param>
        public void CompareBitmapAndJPEGOfLosslessImage(int jpegQuality)
        {
            IDicomPresentationImage originalDicomImage = (IDicomPresentationImage)(Image as IDicomPresentationImage).Clone();
            
            Bitmap losslessBitmap = originalDicomImage.DrawToBitmap(originalDicomImage.Frame.Columns, originalDicomImage.Frame.Rows);
            float compressionRatio;
            Bitmap losslessBitmapToJPEG = ConvertToJpeg(originalDicomImage, losslessBitmap, jpegQuality, out compressionRatio);

            ImageComparisonResult result = Compare(String.Format("JPEG COMPRESSION EFFECT: Comparing the bitmap and the jpeg of the lossless DICOM image. (JPEG Q={0})",
                jpegQuality), losslessBitmap, losslessBitmapToJPEG);

            result.CompressionRatio = compressionRatio;

            ImageComparisonResultControl ctrl = new ImageComparisonResultControl();
            ctrl.SetImageComparisonResult(result);

            Form form = new Form();
            form.AutoSize = true;
            form.Controls.Add(ctrl);
            form.Show();
        }

        /// <summary>
        /// Compare the bitmap of the lossless image and JPEG of the lossy image
        /// </summary>
        /// <param name="dicomCompressionQuality"></param>
        /// <param name="jpegQuality"></param>
        public void CompareLosslessBMPvsLossyJPEG(int dicomCompressionQuality, int jpegQuality)
        {
            IDicomPresentationImage originalDicomImage = (IDicomPresentationImage)(Image as IDicomPresentationImage).Clone();
           
            IDicomPresentationImage lossyDicomImage = originalDicomImage.Clone() as IDicomPresentationImage;
            DicomMessageSopDataSource ds = lossyDicomImage.Sop.DataSource as DicomMessageSopDataSource;

            IDicomCodecFactory[] factories = Dicom.Codec.DicomCodecRegistry.GetCodecFactories();
            IDicomCodec codec = null;

            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement("Root");
            XmlAttribute ratio = xml.CreateAttribute("ratio");
            ratio.Value = dicomCompressionQuality.ToString();
            root.Attributes.Append(ratio);
            xml.AppendChild( root);

            DicomCodecParameters codecParams = null;
            foreach (IDicomCodecFactory factory in factories)
            {
                if (factory.CodecTransferSyntax.Equals(TransferSyntax.Jpeg2000ImageCompression))
                {
                    codec = factory.GetDicomCodec();
                    codecParams = factory.GetCodecParameters(xml);
                    break;
                }
            }
            
            if (codec==null)
            {
                throw new ApplicationException("Unable to find codec for Jpeg2000ImageCompression");
            }

            DicomFile file = new DicomFile((ds.SourceMessage as DicomFile).Filename);
            DicomMessageSopDataSource newDS = new LocalSopDataSource(file);
            

            DicomUid newSopUid = DicomUid.GenerateUid();
            newDS.SourceMessage.MetaInfo[DicomTags.MediaStorageSopInstanceUid].SetStringValue(newSopUid.UID);
            newDS.SourceMessage.DataSet[DicomTags.SopInstanceUid].SetUid(0, newSopUid);

            newDS.SourceMessage.ChangeTransferSyntax(TransferSyntax.Jpeg2000ImageCompression, codec, codecParams);
            ImageSop newSop = new ImageSop(newDS);
            IPresentationImage lossyImage = PresentationImageFactory.Create(newSop)[0];
           
            Bitmap losslessBitmap = originalDicomImage.DrawToBitmap(originalDicomImage.Frame.Columns, originalDicomImage.Frame.Rows);
            Bitmap losslyBitmap = lossyImage.DrawToBitmap(originalDicomImage.Frame.Columns, originalDicomImage.Frame.Rows);
            //Bitmap losslessBitmapToJPEG = ConvertToJpeg(losslessBitmap, jpegQuality);
            float compressionRatio;
            Bitmap losslyBitmapToJPEG = ConvertToJpeg(lossyDicomImage, losslyBitmap, jpegQuality, out compressionRatio);


            ImageComparisonResult result = Compare(String.Format("DICOM + JPEG COMPRESSION COMBINED EFFECT: Comparing the bitmap of the lossless image and JPEG of the lossy DICOM image (DICOM Compression Q={0}, JPEG Q={1})",
                dicomCompressionQuality, jpegQuality), losslessBitmap, losslyBitmapToJPEG);

            result.CompressionRatio = compressionRatio;

            ImageComparisonResultControl ctrl = new ImageComparisonResultControl();
            ctrl.SetImageComparisonResult(result);

            Form form = new Form();
            form.AutoSize = true;
            form.Controls.Add(ctrl);
            form.Show();
        }
    }
}