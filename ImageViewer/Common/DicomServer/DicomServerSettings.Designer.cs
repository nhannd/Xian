﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.ImageViewer.Common.DicomServer {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class DicomServerSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static DicomServerSettings defaultInstance = ((DicomServerSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new DicomServerSettings())));
        
        public static DicomServerSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("localhost")]
        public string HostName {
            get {
                return ((string)(this["HostName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("CLEARCANVAS")]
        public string AETitle {
            get {
                return ((string)(this["AETitle"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("104")]
        public int Port {
            get {
                return ((int)(this["Port"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AllowUnknownCaller {
            get {
                return ((bool)(this["AllowUnknownCaller"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool QueryResponsesInUtf8 {
            get {
                return ((bool)(this["QueryResponsesInUtf8"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\r\n                    <ImageSopClassCollection xmlns:xsi=\"http://www.w3.org/2001/" +
            "XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n             " +
            "           <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.1\" Description=\"Computed Radio" +
            "graphy Image Storage\" />\r\n                        <SopClass Uid=\"1.2.840.10008.5" +
            ".1.4.1.1.2\" Description=\"CT Image Storage\" />\r\n                        <SopClass" +
            " Uid=\"1.2.840.10008.5.1.4.1.1.1.3\" Description=\"Digital Intra-oral X-Ray Image S" +
            "torage – For Presentation\" />\r\n                        <SopClass Uid=\"1.2.840.10" +
            "008.5.1.4.1.1.1.3.1\" Description=\"Digital Intra-oral X-Ray Image Storage – For P" +
            "rocessing\" />\r\n                        <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.1." +
            "2\" Description=\"Digital Mammography X-Ray Image Storage – For Presentation\" />\r\n" +
            "                        <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.1.2.1\" Descriptio" +
            "n=\"Digital Mammography X-Ray Image Storage – For Processing\" />\r\n               " +
            "         <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.1.1\" Description=\"Digital X-Ray " +
            "Image Storage – For Presentation\" />\r\n                        <SopClass Uid=\"1.2" +
            ".840.10008.5.1.4.1.1.1.1.1\" Description=\"Digital X-Ray Image Storage – For Proce" +
            "ssing\" />\r\n                        <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.2.1\" D" +
            "escription=\"Enhanced CT Image Storage\" />\r\n                        <SopClass Uid" +
            "=\"1.2.840.10008.5.1.4.1.1.4.1\" Description=\"Enhanced MR Image Storage\" />\r\n     " +
            "                   <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.12.1.1\" Description=\"E" +
            "nhanced XA Image Storage\" />\r\n                        <SopClass Uid=\"1.2.840.100" +
            "08.5.1.4.1.1.12.2.1\" Description=\"Enhanced XRF Image Storage\" />\r\n              " +
            "          <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.4\" Description=\"MR Image Storag" +
            "e\" />\r\n                        <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.7.2\" Descr" +
            "iption=\"Multi-frame Grayscale Byte Secondary Capture Image Storage\" />\r\n        " +
            "                <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.7.3\" Description=\"Multi-f" +
            "rame Grayscale Word Secondary Capture Image Storage\" />\r\n                       " +
            " <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.7.1\" Description=\"Multi-frame Single Bit" +
            " Secondary Capture Image Storage\" />\r\n                        <SopClass Uid=\"1.2" +
            ".840.10008.5.1.4.1.1.7.4\" Description=\"Multi-frame True Color Secondary Capture " +
            "Image Storage\" />\r\n                        <SopClass Uid=\"1.2.840.10008.5.1.4.1." +
            "1.5\" Description=\"Nuclear Medicine Image  Storage (Retired)\" />\r\n               " +
            "         <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.20\" Description=\"Nuclear Medicin" +
            "e Image Storage\" />\r\n                        <SopClass Uid=\"1.2.840.10008.5.1.4." +
            "1.1.77.1.5.2\" Description=\"Ophthalmic Photography 16 Bit Image Storage\" />\r\n    " +
            "                    <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.77.1.5.1\" Description" +
            "=\"Ophthalmic Photography 8 Bit Image Storage\" />\r\n                        <SopCl" +
            "ass Uid=\"1.2.840.10008.5.1.4.1.1.77.1.5.4\" Description=\"Ophthalmic Tomography Im" +
            "age Storage\" />\r\n                        <SopClass Uid=\"1.2.840.10008.5.1.4.1.1." +
            "128\" Description=\"Positron Emission Tomography Image Storage\" />\r\n              " +
            "          <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.481.1\" Description=\"RT Image St" +
            "orage\" />\r\n                        <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.7\" Des" +
            "cription=\"Secondary Capture Image Storage\" />\r\n                        <SopClass" +
            " Uid=\"1.2.840.10008.5.1.4.1.1.6.1\" Description=\"Ultrasound Image Storage\" />\r\n  " +
            "                      <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.6\" Description=\"Ult" +
            "rasound Image Storage (Retired)\" />\r\n                        <SopClass Uid=\"1.2." +
            "840.10008.5.1.4.1.1.3.1\" Description=\"Ultrasound Multi-frame Image Storage\" />\r\n" +
            "                        <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.3\" Description=\"U" +
            "ltrasound Multi-frame Image Storage (Retired)\" />\r\n                        <SopC" +
            "lass Uid=\"1.2.840.10008.5.1.4.1.1.77.1.1.1\" Description=\"Video Endoscopic Image " +
            "Storage\" />\r\n                        <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.77.1" +
            ".2.1\" Description=\"Video Microscopic Image Storage\" />\r\n                        " +
            "<SopClass Uid=\"1.2.840.10008.5.1.4.1.1.77.1.4.1\" Description=\"Video Photographic" +
            " Image Storage\" />\r\n                        <SopClass Uid=\"1.2.840.10008.5.1.4.1" +
            ".1.77.1.1\" Description=\"VL Endoscopic Image Storage\" />\r\n                       " +
            " <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.77.1.2\" Description=\"VL Microscopic Imag" +
            "e Storage\" />\r\n                        <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.77" +
            ".1.4\" Description=\"VL Photographic Image Storage\" />\r\n                        <S" +
            "opClass Uid=\"1.2.840.10008.5.1.4.1.1.77.1.3\" Description=\"VL Slide-Coordinates M" +
            "icroscopic Image Storage\" />\r\n                        <SopClass Uid=\"1.2.840.100" +
            "08.5.1.4.1.1.13.1.1\" Description=\"X-Ray 3D Angiographic Image Storage\" />\r\n     " +
            "                   <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.13.1.2\" Description=\"X" +
            "-Ray 3D Craniofacial Image Storage\" />\r\n                        <SopClass Uid=\"1" +
            ".2.840.10008.5.1.4.1.1.12.3\" Description=\"X-Ray Angiographic Bi-Plane Image Stor" +
            "age (Retired)\" />\r\n                        <SopClass Uid=\"1.2.840.10008.5.1.4.1." +
            "1.12.1\" Description=\"X-Ray Angiographic Image Storage\" />\r\n                     " +
            "   <SopClass Uid=\"1.2.840.10008.5.1.4.1.1.12.2\" Description=\"X-Ray Radiofluorosc" +
            "opic Image Storage\" />\r\n                    </ImageSopClassCollection>\r\n        " +
            "        ")]
        public global::ClearCanvas.ImageViewer.Common.DicomServer.ImageSopClassConfigurationElementCollection ImageStorageSopClasses {
            get {
                return ((global::ClearCanvas.ImageViewer.Common.DicomServer.ImageSopClassConfigurationElementCollection)(this["ImageStorageSopClasses"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"
                    <NonImageSopClassCollection xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                        <SopClass Uid=""1.2.840.10008.5.1.4.1.1.88.59"" Description=""Key Object Selection Document Storage"" />
                        <SopClass Uid=""1.2.840.10008.5.1.4.1.1.11.3"" Description=""Pseudo-Color Softcopy Presentation State Storage SOP Class"" />
                        <SopClass Uid=""1.2.840.10008.5.1.4.1.1.11.2"" Description=""Color Softcopy Presentation State Storage SOP Class"" />
                        <SopClass Uid=""1.2.840.10008.5.1.4.1.1.11.1"" Description=""Grayscale Softcopy Presentation State Storage SOP Class"" />
                    </NonImageSopClassCollection>
                ")]
        public global::ClearCanvas.ImageViewer.Common.DicomServer.NonImageSopClassConfigurationElementCollection NonImageStorageSopClasses {
            get {
                return ((global::ClearCanvas.ImageViewer.Common.DicomServer.NonImageSopClassConfigurationElementCollection)(this["NonImageStorageSopClasses"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\r\n                    <TransferSyntaxCollection xmlns:xsi=\"http://www.w3.org/2001" +
            "/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n            " +
            "            <TransferSyntax Uid=\"1.2.840.10008.1.2.1\" Description=\"Explicit VR L" +
            "ittle Endian\" />\r\n                        <TransferSyntax Uid=\"1.2.840.10008.1.2" +
            "\" Description=\"Implicit VR Little Endian: Default Transfer Syntax for DICOM\" />\r" +
            "\n                        <TransferSyntax Uid=\"1.2.840.10008.1.2.2\" Description=\"" +
            "Explicit VR Big Endian\" />\r\n                        <TransferSyntax Uid=\"1.2.840" +
            ".10008.1.2.4.90\" Description=\"JPEG 2000 Image Compression (Lossless Only)\" />\r\n " +
            "                       <TransferSyntax Uid=\"1.2.840.10008.1.2.4.70\" Description=" +
            "\"JPEG Lossless, Non-Hierarchical, First-Order Prediction (Process 14 [Selection " +
            "Value 1]): Default Transfer Syntax for Lossless JPEG Image Compression\" />\r\n    " +
            "                    <TransferSyntax Uid=\"1.2.840.10008.1.2.5\" Description=\"RLE L" +
            "ossless\" />\r\n                        <TransferSyntax Uid=\"1.2.840.10008.1.2.4.91" +
            "\" Description=\"JPEG 2000 Image Compression\" />\r\n                        <Transfe" +
            "rSyntax Uid=\"1.2.840.10008.1.2.4.50\" Description=\"JPEG Baseline (Process 1): Def" +
            "ault Transfer Syntax for Lossy JPEG 8 Bit Image Compression\" />\r\n               " +
            "         <TransferSyntax Uid=\"1.2.840.10008.1.2.4.51\" Description=\"JPEG Extended" +
            " (Process 2 &amp; 4): Default Transfer Syntax for Lossy JPEG 12 Bit Image Compre" +
            "ssion (Process 4 only)\" />\r\n                    </TransferSyntaxCollection>\r\n   " +
            "             ")]
        public global::ClearCanvas.ImageViewer.Common.DicomServer.TransferSyntaxConfigurationElementCollection StorageTransferSyntaxes {
            get {
                return ((global::ClearCanvas.ImageViewer.Common.DicomServer.TransferSyntaxConfigurationElementCollection)(this["StorageTransferSyntaxes"]));
            }
        }
    }
}
