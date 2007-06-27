using System;
using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.ImageServer.Dicom
{
    public enum Endian
    {
        Little,
        Big
    }

    /// <summary>
    /// This class contains transfer synatx definitions.
    /// </summary>
    public class TransferSyntax
    {
        /// <summary>
        /// <para>Deflated Explicit VR Little Endian</para>
        /// <para>UID: 1.2.840.10008.1.2.1.99</para>
        /// </summary>
        public static readonly String DeflatedExplicitVRLittleEndian = "1.2.840.10008.1.2.1.99";

        /// <summary>
        /// <para>Explicit VR Big Endian</para>
        /// <para>UID: 1.2.840.10008.1.2.2</para>
        /// </summary>
        public static readonly String ExplicitVRBigEndian = "1.2.840.10008.1.2.2";

        /// <summary>
        /// <para>Explicit VR Little Endian</para>
        /// <para>UID: 1.2.840.10008.1.2.1</para>
        /// </summary>
        public static readonly String ExplicitVRLittleEndian = "1.2.840.10008.1.2.1";

        /// <summary>
        /// <para>Implicit VR Little Endian: Default Transfer Syntax for DICOM</para>
        /// <para>UID: 1.2.840.10008.1.2</para>
        /// </summary>
        public static readonly String ImplicitVRLittleEndian = "1.2.840.10008.1.2";

        /// <summary>
        /// <para>JPEG 2000 Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.91</para>
        /// </summary>
        public static readonly String JPEG2000ImageCompression = "1.2.840.10008.1.2.4.91";

        /// <summary>
        /// <para>JPEG 2000 Image Compression (Lossless Only)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.90</para>
        /// </summary>
        public static readonly String JPEG2000ImageCompressionLosslessOnly = "1.2.840.10008.1.2.4.90";

        /// <summary>
        /// <para>JPEG 2000 Part 2 Multi-component  Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.93</para>
        /// </summary>
        public static readonly String JPEG2000Part2MulticomponentImageCompression = "1.2.840.10008.1.2.4.93";

        /// <summary>
        /// <para>JPEG 2000 Part 2 Multi-component  Image Compression (Lossless Only)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.92</para>
        /// </summary>
        public static readonly String JPEG2000Part2MulticomponentImageCompressionLosslessOnly = "1.2.840.10008.1.2.4.92";

        /// <summary>
        /// <para>JPEG Baseline (Process 1): Default Transfer Syntax for Lossy JPEG 8 Bit Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.50</para>
        /// </summary>
        public static readonly String JPEGBaselineProcess1 = "1.2.840.10008.1.2.4.50";

        /// <summary>
        /// <para>JPEG Extended (Process 2 & 4): Default Transfer Syntax for Lossy JPEG 12 Bit Image Compression (Process 4 only)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.51</para>
        /// </summary>
        public static readonly String JPEGExtendedProcess24 = "1.2.840.10008.1.2.4.51";

        /// <summary>
        /// <para>JPEG Extended (Process 3 & 5) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.52</para>
        /// </summary>
        public static readonly String JPEGExtendedProcess35Retired = "1.2.840.10008.1.2.4.52";

        /// <summary>
        /// <para>JPEG Extended, Hierarchical (Process 16 & 18) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.59</para>
        /// </summary>
        public static readonly String JPEGExtendedHierarchicalProcess1618Retired = "1.2.840.10008.1.2.4.59";

        /// <summary>
        /// <para>JPEG Extended, Hierarchical (Process 17 & 19) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.60</para>
        /// </summary>
        public static readonly String JPEGExtendedHierarchicalProcess1719Retired = "1.2.840.10008.1.2.4.60";

        /// <summary>
        /// <para>JPEG Full Progression, Hierarchical (Process 24 & 26) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.63</para>
        /// </summary>
        public static readonly String JPEGFullProgressionHierarchicalProcess2426Retired = "1.2.840.10008.1.2.4.63";

        /// <summary>
        /// <para>JPEG Full Progression, Hierarchical (Process 25 & 27) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.64</para>
        /// </summary>
        public static readonly String JPEGFullProgressionHierarchicalProcess2527Retired = "1.2.840.10008.1.2.4.64";

        /// <summary>
        /// <para>JPEG Full Progression, Non-Hierarchical (Process 10 & 12) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.55</para>
        /// </summary>
        public static readonly String JPEGFullProgressionNonHierarchicalProcess1012Retired = "1.2.840.10008.1.2.4.55";

        /// <summary>
        /// <para>JPEG Full Progression, Non-Hierarchical (Process 11 & 13) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.56</para>
        /// </summary>
        public static readonly String JPEGFullProgressionNonHierarchicalProcess1113Retired = "1.2.840.10008.1.2.4.56";

        /// <summary>
        /// <para>JPEG Lossless, Hierarchical (Process 28) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.65</para>
        /// </summary>
        public static readonly String JPEGLosslessHierarchicalProcess28Retired = "1.2.840.10008.1.2.4.65";

        /// <summary>
        /// <para>JPEG Lossless, Hierarchical (Process 29) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.66</para>
        /// </summary>
        public static readonly String JPEGLosslessHierarchicalProcess29Retired = "1.2.840.10008.1.2.4.66";

        /// <summary>
        /// <para>JPEG Lossless, Non-Hierarchical (Process 14)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.57</para>
        /// </summary>
        public static readonly String JPEGLosslessNonHierarchicalProcess14 = "1.2.840.10008.1.2.4.57";

        /// <summary>
        /// <para>JPEG Lossless, Non-Hierarchical (Process 15) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.58</para>
        /// </summary>
        public static readonly String JPEGLosslessNonHierarchicalProcess15Retired = "1.2.840.10008.1.2.4.58";

        /// <summary>
        /// <para>JPEG Lossless, Non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1]): Default Transfer Syntax for Lossless JPEG Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.70</para>
        /// </summary>
        public static readonly String JPEGLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1 = "1.2.840.10008.1.2.4.70";

        /// <summary>
        /// <para>JPEG Spectral Selection, Hierarchical (Process 20 & 22) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.61</para>
        /// </summary>
        public static readonly String JPEGSpectralSelectionHierarchicalProcess2022Retired = "1.2.840.10008.1.2.4.61";

        /// <summary>
        /// <para>JPEG Spectral Selection, Hierarchical (Process 21 & 23) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.62</para>
        /// </summary>
        public static readonly String JPEGSpectralSelectionHierarchicalProcess2123Retired = "1.2.840.10008.1.2.4.62";

        /// <summary>
        /// <para>JPEG Spectral Selection, Non-Hierarchical (Process 6 & 8) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.53</para>
        /// </summary>
        public static readonly String JPEGSpectralSelectionNonHierarchicalProcess68Retired = "1.2.840.10008.1.2.4.53";

        /// <summary>
        /// <para>JPEG Spectral Selection, Non-Hierarchical (Process 7 & 9) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.54</para>
        /// </summary>
        public static readonly String JPEGSpectralSelectionNonHierarchicalProcess79Retired = "1.2.840.10008.1.2.4.54";

        /// <summary>
        /// <para>JPEG-LS Lossless Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.80</para>
        /// </summary>
        public static readonly String JPEGLSLosslessImageCompression = "1.2.840.10008.1.2.4.80";

        /// <summary>
        /// <para>JPEG-LS Lossy (Near-Lossless) Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.81</para>
        /// </summary>
        public static readonly String JPEGLSLossyNearLosslessImageCompression = "1.2.840.10008.1.2.4.81";

        /// <summary>
        /// <para>JPIP Referenced</para>
        /// <para>UID: 1.2.840.10008.1.2.4.94</para>
        /// </summary>
        public static readonly String JPIPReferenced = "1.2.840.10008.1.2.4.94";

        /// <summary>
        /// <para>JPIP Referenced Deflate</para>
        /// <para>UID: 1.2.840.10008.1.2.4.95</para>
        /// </summary>
        public static readonly String JPIPReferencedDeflate = "1.2.840.10008.1.2.4.95";

        /// <summary>
        /// <para>MPEG2 Main Profile @ Main Level</para>
        /// <para>UID: 1.2.840.10008.1.2.4.100</para>
        /// </summary>
        public static readonly String MPEG2MainProfileMainLevel = "1.2.840.10008.1.2.4.100";

        /// <summary>
        /// <para>RFC 2557 MIME encapsulation</para>
        /// <para>UID: 1.2.840.10008.1.2.6.1</para>
        /// </summary>
        public static readonly String RFC2557MIMEencapsulation = "1.2.840.10008.1.2.6.1";

        /// <summary>
        /// <para>RLE Lossless</para>
        /// <para>UID: 1.2.840.10008.1.2.5</para>
        /// </summary>
        public static readonly String RLELossless = "1.2.840.10008.1.2.5";

        // Internal members
        private static Dictionary<String,TransferSyntax> _transferSyntaxes = new Dictionary<String,TransferSyntax>();
        private static bool _listInit = false;
        private bool _littleEndian;
        private bool _encapsulated;
        private bool _explicitVr;
        private bool _deflate;
        private String _name;
        private String _uid;

        ///<summary>
        /// Constructor for transfer syntax objects
        ///</summary>
        public TransferSyntax(String name, String uid, bool bLittleEndian, bool bEncapsulated, bool bExplicitVr, bool bDeflate)
        {
            this._uid = uid;
            this._name = name;
            this._littleEndian = bLittleEndian;
            this._encapsulated = bEncapsulated;
            this._explicitVr = bExplicitVr;
            this._deflate = bDeflate;
        }

        ///<summary>Override to the ToString() method, returns the name of the transfer syntax.</summary>
        public override String ToString()
        {
            return _name;
        }

        ///<summary>Property representing UID of transfer syntax.</summary>
        public String UidString
        {
            get { return _uid; }
        }

        public DicomUid UID
        {
            get
            {
                return new DicomUid(_uid, _name, UidType.TransferSyntax);
            }
        }

        ///<summary>Property representing the name of the transfer syntax.</summary>
        public String Name
        {
            get { return _name; }
        }

        ///<summary>Property representing if the transfer syntax is encoded as little endian.</summary>
        public bool LittleEndian
        {
            get { return _littleEndian; }
        }
        public Endian Endian
        {
            get
            {
                if (_littleEndian)
                    return Endian.Little;

                return Endian.Big;
            }
        }

        ///<summary>Property representing if the transfer syntax is encoded as encapsulated.</summary>
        public bool Encapsulated
        {
            get { return _encapsulated; }
        }

        ///<summary>Property representing if the transfer syntax is encoded as explicit Value Representation.</summary>
        public bool ExplicitVr
        {
            get { return _explicitVr; }
        }

        ///<summary>Property representing if the transfer syntax is encoded in deflate format.</summary>
        public bool Deflate
        {
            get { return _deflate; }
        }

        /// <summary>
        /// Get a TransferSyntax object for a specific transfer syntax UID.
        /// </summary>
        public static TransferSyntax GetTransferSyntax(String uid)
        {
            if (_listInit == false)
            {
                _listInit = true;

            _transferSyntaxes.Add(TransferSyntax.DeflatedExplicitVRLittleEndian,
                    new TransferSyntax("Deflated Explicit VR Little Endian",
                                 TransferSyntax.DeflatedExplicitVRLittleEndian,
                                 true, // Little Endian?
                                 false, // Encapsulated?
                                 true, // Explicit VR?
                                 true // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.ExplicitVRBigEndian,
                    new TransferSyntax("Explicit VR Big Endian",
                                 TransferSyntax.ExplicitVRBigEndian,
                                 false, // Little Endian?
                                 false, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.ExplicitVRLittleEndian,
                    new TransferSyntax("Explicit VR Little Endian",
                                 TransferSyntax.ExplicitVRLittleEndian,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.ImplicitVRLittleEndian,
                    new TransferSyntax("Implicit VR Little Endian: Default Transfer Syntax for DICOM",
                                 TransferSyntax.ImplicitVRLittleEndian,
                                 true, // Little Endian?
                                 false, // Encapsulated?
                                 false, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEG2000ImageCompression,
                    new TransferSyntax("JPEG 2000 Image Compression",
                                 TransferSyntax.JPEG2000ImageCompression,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEG2000ImageCompressionLosslessOnly,
                    new TransferSyntax("JPEG 2000 Image Compression (Lossless Only)",
                                 TransferSyntax.JPEG2000ImageCompressionLosslessOnly,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEG2000Part2MulticomponentImageCompression,
                    new TransferSyntax("JPEG 2000 Part 2 Multi-component  Image Compression",
                                 TransferSyntax.JPEG2000Part2MulticomponentImageCompression,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEG2000Part2MulticomponentImageCompressionLosslessOnly,
                    new TransferSyntax("JPEG 2000 Part 2 Multi-component  Image Compression (Lossless Only)",
                                 TransferSyntax.JPEG2000Part2MulticomponentImageCompressionLosslessOnly,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGBaselineProcess1,
                    new TransferSyntax("JPEG Baseline (Process 1): Default Transfer Syntax for Lossy JPEG 8 Bit Image Compression",
                                 TransferSyntax.JPEGBaselineProcess1,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGExtendedProcess24,
                    new TransferSyntax("JPEG Extended (Process 2 & 4): Default Transfer Syntax for Lossy JPEG 12 Bit Image Compression (Process 4 only)",
                                 TransferSyntax.JPEGExtendedProcess24,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGExtendedProcess35Retired,
                    new TransferSyntax("JPEG Extended (Process 3 & 5) (Retired)",
                                 TransferSyntax.JPEGExtendedProcess35Retired,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGExtendedHierarchicalProcess1618Retired,
                    new TransferSyntax("JPEG Extended, Hierarchical (Process 16 & 18) (Retired)",
                                 TransferSyntax.JPEGExtendedHierarchicalProcess1618Retired,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGExtendedHierarchicalProcess1719Retired,
                    new TransferSyntax("JPEG Extended, Hierarchical (Process 17 & 19) (Retired)",
                                 TransferSyntax.JPEGExtendedHierarchicalProcess1719Retired,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGFullProgressionHierarchicalProcess2426Retired,
                    new TransferSyntax("JPEG Full Progression, Hierarchical (Process 24 & 26) (Retired)",
                                 TransferSyntax.JPEGFullProgressionHierarchicalProcess2426Retired,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGFullProgressionHierarchicalProcess2527Retired,
                    new TransferSyntax("JPEG Full Progression, Hierarchical (Process 25 & 27) (Retired)",
                                 TransferSyntax.JPEGFullProgressionHierarchicalProcess2527Retired,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGFullProgressionNonHierarchicalProcess1012Retired,
                    new TransferSyntax("JPEG Full Progression, Non-Hierarchical (Process 10 & 12) (Retired)",
                                 TransferSyntax.JPEGFullProgressionNonHierarchicalProcess1012Retired,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGFullProgressionNonHierarchicalProcess1113Retired,
                    new TransferSyntax("JPEG Full Progression, Non-Hierarchical (Process 11 & 13) (Retired)",
                                 TransferSyntax.JPEGFullProgressionNonHierarchicalProcess1113Retired,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGLosslessHierarchicalProcess28Retired,
                    new TransferSyntax("JPEG Lossless, Hierarchical (Process 28) (Retired)",
                                 TransferSyntax.JPEGLosslessHierarchicalProcess28Retired,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGLosslessHierarchicalProcess29Retired,
                    new TransferSyntax("JPEG Lossless, Hierarchical (Process 29) (Retired)",
                                 TransferSyntax.JPEGLosslessHierarchicalProcess29Retired,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGLosslessNonHierarchicalProcess14,
                    new TransferSyntax("JPEG Lossless, Non-Hierarchical (Process 14)",
                                 TransferSyntax.JPEGLosslessNonHierarchicalProcess14,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGLosslessNonHierarchicalProcess15Retired,
                    new TransferSyntax("JPEG Lossless, Non-Hierarchical (Process 15) (Retired)",
                                 TransferSyntax.JPEGLosslessNonHierarchicalProcess15Retired,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1,
                    new TransferSyntax("JPEG Lossless, Non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1]): Default Transfer Syntax for Lossless JPEG Image Compression",
                                 TransferSyntax.JPEGLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGSpectralSelectionHierarchicalProcess2022Retired,
                    new TransferSyntax("JPEG Spectral Selection, Hierarchical (Process 20 & 22) (Retired)",
                                 TransferSyntax.JPEGSpectralSelectionHierarchicalProcess2022Retired,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGSpectralSelectionHierarchicalProcess2123Retired,
                    new TransferSyntax("JPEG Spectral Selection, Hierarchical (Process 21 & 23) (Retired)",
                                 TransferSyntax.JPEGSpectralSelectionHierarchicalProcess2123Retired,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGSpectralSelectionNonHierarchicalProcess68Retired,
                    new TransferSyntax("JPEG Spectral Selection, Non-Hierarchical (Process 6 & 8) (Retired)",
                                 TransferSyntax.JPEGSpectralSelectionNonHierarchicalProcess68Retired,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGSpectralSelectionNonHierarchicalProcess79Retired,
                    new TransferSyntax("JPEG Spectral Selection, Non-Hierarchical (Process 7 & 9) (Retired)",
                                 TransferSyntax.JPEGSpectralSelectionNonHierarchicalProcess79Retired,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGLSLosslessImageCompression,
                    new TransferSyntax("JPEG-LS Lossless Image Compression",
                                 TransferSyntax.JPEGLSLosslessImageCompression,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPEGLSLossyNearLosslessImageCompression,
                    new TransferSyntax("JPEG-LS Lossy (Near-Lossless) Image Compression",
                                 TransferSyntax.JPEGLSLossyNearLosslessImageCompression,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPIPReferenced,
                    new TransferSyntax("JPIP Referenced",
                                 TransferSyntax.JPIPReferenced,
                                 true, // Little Endian?
                                 false, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.JPIPReferencedDeflate,
                    new TransferSyntax("JPIP Referenced Deflate",
                                 TransferSyntax.JPIPReferencedDeflate,
                                 true, // Little Endian?
                                 false, // Encapsulated?
                                 true, // Explicit VR?
                                 true // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.MPEG2MainProfileMainLevel,
                    new TransferSyntax("MPEG2 Main Profile @ Main Level",
                                 TransferSyntax.MPEG2MainProfileMainLevel,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.RFC2557MIMEencapsulation,
                    new TransferSyntax("RFC 2557 MIME encapsulation",
                                 TransferSyntax.RFC2557MIMEencapsulation,
                                 true, // Little Endian?
                                 false, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            _transferSyntaxes.Add(TransferSyntax.RLELossless,
                    new TransferSyntax("RLE Lossless",
                                 TransferSyntax.RLELossless,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 ));

            }

            if (!_transferSyntaxes.ContainsKey(uid))
                return null;

            return _transferSyntaxes[uid];
        }
    }
}
