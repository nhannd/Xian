using System;
using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Enumerated value to differentiate between little and big endian.
    /// </summary>
    public enum Endian
    {
        Little,
        Big
    }

    /// <summary>
    /// This class contains transfer syntax definitions.
    /// </summary>
    public class TransferSyntax
    {
        /// <summary>String representing
        /// <para>Deflated Explicit VR Little Endian</para>
        /// <para>UID: 1.2.840.10008.1.2.1.99</para>
        /// </summary>
        public static readonly String DeflatedExplicitVRLittleEndianUid = "1.2.840.10008.1.2.1.99";

        /// <summary>TransferSyntax object representing
        /// <para>Deflated Explicit VR Little Endian</para>
        /// <para>UID: 1.2.840.10008.1.2.1.99</para>
        /// </summary>
        public static readonly TransferSyntax DeflatedExplicitVRLittleEndian =
                    new TransferSyntax("Deflated Explicit VR Little Endian",
                                 TransferSyntax.DeflatedExplicitVRLittleEndianUid,
                                 true, // Little Endian?
                                 false, // Encapsulated?
                                 true, // Explicit VR?
                                 true // Deflated?
                                 );

        /// <summary>String representing
        /// <para>Explicit VR Big Endian</para>
        /// <para>UID: 1.2.840.10008.1.2.2</para>
        /// </summary>
        public static readonly String ExplicitVRBigEndianUid = "1.2.840.10008.1.2.2";

        /// <summary>TransferSyntax object representing
        /// <para>Explicit VR Big Endian</para>
        /// <para>UID: 1.2.840.10008.1.2.2</para>
        /// </summary>
        public static readonly TransferSyntax ExplicitVRBigEndian =
                    new TransferSyntax("Explicit VR Big Endian",
                                 TransferSyntax.ExplicitVRBigEndianUid,
                                 false, // Little Endian?
                                 false, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>Explicit VR Little Endian</para>
        /// <para>UID: 1.2.840.10008.1.2.1</para>
        /// </summary>
        public static readonly String ExplicitVRLittleEndianUid = "1.2.840.10008.1.2.1";

        /// <summary>TransferSyntax object representing
        /// <para>Explicit VR Little Endian</para>
        /// <para>UID: 1.2.840.10008.1.2.1</para>
        /// </summary>
        public static readonly TransferSyntax ExplicitVRLittleEndian =
                    new TransferSyntax("Explicit VR Little Endian",
                                 TransferSyntax.ExplicitVRLittleEndianUid,
                                 true, // Little Endian?
                                 false, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>Implicit VR Little Endian: Default Transfer Syntax for DICOM</para>
        /// <para>UID: 1.2.840.10008.1.2</para>
        /// </summary>
        public static readonly String ImplicitVRLittleEndianUid = "1.2.840.10008.1.2";

        /// <summary>TransferSyntax object representing
        /// <para>Implicit VR Little Endian: Default Transfer Syntax for DICOM</para>
        /// <para>UID: 1.2.840.10008.1.2</para>
        /// </summary>
        public static readonly TransferSyntax ImplicitVRLittleEndian =
                    new TransferSyntax("Implicit VR Little Endian: Default Transfer Syntax for DICOM",
                                 TransferSyntax.ImplicitVRLittleEndianUid,
                                 true, // Little Endian?
                                 false, // Encapsulated?
                                 false, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG 2000 Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.91</para>
        /// </summary>
        public static readonly String JPEG2000ImageCompressionUid = "1.2.840.10008.1.2.4.91";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG 2000 Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.91</para>
        /// </summary>
        public static readonly TransferSyntax JPEG2000ImageCompression =
                    new TransferSyntax("JPEG 2000 Image Compression",
                                 TransferSyntax.JPEG2000ImageCompressionUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG 2000 Image Compression (Lossless Only)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.90</para>
        /// </summary>
        public static readonly String JPEG2000ImageCompressionLosslessOnlyUid = "1.2.840.10008.1.2.4.90";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG 2000 Image Compression (Lossless Only)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.90</para>
        /// </summary>
        public static readonly TransferSyntax JPEG2000ImageCompressionLosslessOnly =
                    new TransferSyntax("JPEG 2000 Image Compression (Lossless Only)",
                                 TransferSyntax.JPEG2000ImageCompressionLosslessOnlyUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG 2000 Part 2 Multi-component  Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.93</para>
        /// </summary>
        public static readonly String JPEG2000Part2MulticomponentImageCompressionUid = "1.2.840.10008.1.2.4.93";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG 2000 Part 2 Multi-component  Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.93</para>
        /// </summary>
        public static readonly TransferSyntax JPEG2000Part2MulticomponentImageCompression =
                    new TransferSyntax("JPEG 2000 Part 2 Multi-component  Image Compression",
                                 TransferSyntax.JPEG2000Part2MulticomponentImageCompressionUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG 2000 Part 2 Multi-component  Image Compression (Lossless Only)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.92</para>
        /// </summary>
        public static readonly String JPEG2000Part2MulticomponentImageCompressionLosslessOnlyUid = "1.2.840.10008.1.2.4.92";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG 2000 Part 2 Multi-component  Image Compression (Lossless Only)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.92</para>
        /// </summary>
        public static readonly TransferSyntax JPEG2000Part2MulticomponentImageCompressionLosslessOnly =
                    new TransferSyntax("JPEG 2000 Part 2 Multi-component  Image Compression (Lossless Only)",
                                 TransferSyntax.JPEG2000Part2MulticomponentImageCompressionLosslessOnlyUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Baseline (Process 1): Default Transfer Syntax for Lossy JPEG 8 Bit Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.50</para>
        /// </summary>
        public static readonly String JPEGBaselineProcess1Uid = "1.2.840.10008.1.2.4.50";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Baseline (Process 1): Default Transfer Syntax for Lossy JPEG 8 Bit Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.50</para>
        /// </summary>
        public static readonly TransferSyntax JPEGBaselineProcess1 =
                    new TransferSyntax("JPEG Baseline (Process 1): Default Transfer Syntax for Lossy JPEG 8 Bit Image Compression",
                                 TransferSyntax.JPEGBaselineProcess1Uid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Extended (Process 2 & 4): Default Transfer Syntax for Lossy JPEG 12 Bit Image Compression (Process 4 only)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.51</para>
        /// </summary>
        public static readonly String JPEGExtendedProcess24Uid = "1.2.840.10008.1.2.4.51";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Extended (Process 2 & 4): Default Transfer Syntax for Lossy JPEG 12 Bit Image Compression (Process 4 only)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.51</para>
        /// </summary>
        public static readonly TransferSyntax JPEGExtendedProcess24 =
                    new TransferSyntax("JPEG Extended (Process 2 & 4): Default Transfer Syntax for Lossy JPEG 12 Bit Image Compression (Process 4 only)",
                                 TransferSyntax.JPEGExtendedProcess24Uid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Extended (Process 3 & 5) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.52</para>
        /// </summary>
        public static readonly String JPEGExtendedProcess35RetiredUid = "1.2.840.10008.1.2.4.52";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Extended (Process 3 & 5) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.52</para>
        /// </summary>
        public static readonly TransferSyntax JPEGExtendedProcess35Retired =
                    new TransferSyntax("JPEG Extended (Process 3 & 5) (Retired)",
                                 TransferSyntax.JPEGExtendedProcess35RetiredUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Extended, Hierarchical (Process 16 & 18) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.59</para>
        /// </summary>
        public static readonly String JPEGExtendedHierarchicalProcess1618RetiredUid = "1.2.840.10008.1.2.4.59";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Extended, Hierarchical (Process 16 & 18) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.59</para>
        /// </summary>
        public static readonly TransferSyntax JPEGExtendedHierarchicalProcess1618Retired =
                    new TransferSyntax("JPEG Extended, Hierarchical (Process 16 & 18) (Retired)",
                                 TransferSyntax.JPEGExtendedHierarchicalProcess1618RetiredUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Extended, Hierarchical (Process 17 & 19) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.60</para>
        /// </summary>
        public static readonly String JPEGExtendedHierarchicalProcess1719RetiredUid = "1.2.840.10008.1.2.4.60";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Extended, Hierarchical (Process 17 & 19) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.60</para>
        /// </summary>
        public static readonly TransferSyntax JPEGExtendedHierarchicalProcess1719Retired =
                    new TransferSyntax("JPEG Extended, Hierarchical (Process 17 & 19) (Retired)",
                                 TransferSyntax.JPEGExtendedHierarchicalProcess1719RetiredUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Full Progression, Hierarchical (Process 24 & 26) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.63</para>
        /// </summary>
        public static readonly String JPEGFullProgressionHierarchicalProcess2426RetiredUid = "1.2.840.10008.1.2.4.63";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Full Progression, Hierarchical (Process 24 & 26) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.63</para>
        /// </summary>
        public static readonly TransferSyntax JPEGFullProgressionHierarchicalProcess2426Retired =
                    new TransferSyntax("JPEG Full Progression, Hierarchical (Process 24 & 26) (Retired)",
                                 TransferSyntax.JPEGFullProgressionHierarchicalProcess2426RetiredUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Full Progression, Hierarchical (Process 25 & 27) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.64</para>
        /// </summary>
        public static readonly String JPEGFullProgressionHierarchicalProcess2527RetiredUid = "1.2.840.10008.1.2.4.64";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Full Progression, Hierarchical (Process 25 & 27) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.64</para>
        /// </summary>
        public static readonly TransferSyntax JPEGFullProgressionHierarchicalProcess2527Retired =
                    new TransferSyntax("JPEG Full Progression, Hierarchical (Process 25 & 27) (Retired)",
                                 TransferSyntax.JPEGFullProgressionHierarchicalProcess2527RetiredUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Full Progression, Non-Hierarchical (Process 10 & 12) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.55</para>
        /// </summary>
        public static readonly String JPEGFullProgressionNonHierarchicalProcess1012RetiredUid = "1.2.840.10008.1.2.4.55";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Full Progression, Non-Hierarchical (Process 10 & 12) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.55</para>
        /// </summary>
        public static readonly TransferSyntax JPEGFullProgressionNonHierarchicalProcess1012Retired =
                    new TransferSyntax("JPEG Full Progression, Non-Hierarchical (Process 10 & 12) (Retired)",
                                 TransferSyntax.JPEGFullProgressionNonHierarchicalProcess1012RetiredUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Full Progression, Non-Hierarchical (Process 11 & 13) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.56</para>
        /// </summary>
        public static readonly String JPEGFullProgressionNonHierarchicalProcess1113RetiredUid = "1.2.840.10008.1.2.4.56";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Full Progression, Non-Hierarchical (Process 11 & 13) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.56</para>
        /// </summary>
        public static readonly TransferSyntax JPEGFullProgressionNonHierarchicalProcess1113Retired =
                    new TransferSyntax("JPEG Full Progression, Non-Hierarchical (Process 11 & 13) (Retired)",
                                 TransferSyntax.JPEGFullProgressionNonHierarchicalProcess1113RetiredUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Lossless, Hierarchical (Process 28) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.65</para>
        /// </summary>
        public static readonly String JPEGLosslessHierarchicalProcess28RetiredUid = "1.2.840.10008.1.2.4.65";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Lossless, Hierarchical (Process 28) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.65</para>
        /// </summary>
        public static readonly TransferSyntax JPEGLosslessHierarchicalProcess28Retired =
                    new TransferSyntax("JPEG Lossless, Hierarchical (Process 28) (Retired)",
                                 TransferSyntax.JPEGLosslessHierarchicalProcess28RetiredUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Lossless, Hierarchical (Process 29) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.66</para>
        /// </summary>
        public static readonly String JPEGLosslessHierarchicalProcess29RetiredUid = "1.2.840.10008.1.2.4.66";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Lossless, Hierarchical (Process 29) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.66</para>
        /// </summary>
        public static readonly TransferSyntax JPEGLosslessHierarchicalProcess29Retired =
                    new TransferSyntax("JPEG Lossless, Hierarchical (Process 29) (Retired)",
                                 TransferSyntax.JPEGLosslessHierarchicalProcess29RetiredUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Lossless, Non-Hierarchical (Process 14)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.57</para>
        /// </summary>
        public static readonly String JPEGLosslessNonHierarchicalProcess14Uid = "1.2.840.10008.1.2.4.57";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Lossless, Non-Hierarchical (Process 14)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.57</para>
        /// </summary>
        public static readonly TransferSyntax JPEGLosslessNonHierarchicalProcess14 =
                    new TransferSyntax("JPEG Lossless, Non-Hierarchical (Process 14)",
                                 TransferSyntax.JPEGLosslessNonHierarchicalProcess14Uid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Lossless, Non-Hierarchical (Process 15) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.58</para>
        /// </summary>
        public static readonly String JPEGLosslessNonHierarchicalProcess15RetiredUid = "1.2.840.10008.1.2.4.58";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Lossless, Non-Hierarchical (Process 15) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.58</para>
        /// </summary>
        public static readonly TransferSyntax JPEGLosslessNonHierarchicalProcess15Retired =
                    new TransferSyntax("JPEG Lossless, Non-Hierarchical (Process 15) (Retired)",
                                 TransferSyntax.JPEGLosslessNonHierarchicalProcess15RetiredUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Lossless, Non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1]): Default Transfer Syntax for Lossless JPEG Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.70</para>
        /// </summary>
        public static readonly String JPEGLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1Uid = "1.2.840.10008.1.2.4.70";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Lossless, Non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1]): Default Transfer Syntax for Lossless JPEG Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.70</para>
        /// </summary>
        public static readonly TransferSyntax JPEGLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1 =
                    new TransferSyntax("JPEG Lossless, Non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1]): Default Transfer Syntax for Lossless JPEG Image Compression",
                                 TransferSyntax.JPEGLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1Uid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Spectral Selection, Hierarchical (Process 20 & 22) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.61</para>
        /// </summary>
        public static readonly String JPEGSpectralSelectionHierarchicalProcess2022RetiredUid = "1.2.840.10008.1.2.4.61";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Spectral Selection, Hierarchical (Process 20 & 22) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.61</para>
        /// </summary>
        public static readonly TransferSyntax JPEGSpectralSelectionHierarchicalProcess2022Retired =
                    new TransferSyntax("JPEG Spectral Selection, Hierarchical (Process 20 & 22) (Retired)",
                                 TransferSyntax.JPEGSpectralSelectionHierarchicalProcess2022RetiredUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Spectral Selection, Hierarchical (Process 21 & 23) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.62</para>
        /// </summary>
        public static readonly String JPEGSpectralSelectionHierarchicalProcess2123RetiredUid = "1.2.840.10008.1.2.4.62";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Spectral Selection, Hierarchical (Process 21 & 23) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.62</para>
        /// </summary>
        public static readonly TransferSyntax JPEGSpectralSelectionHierarchicalProcess2123Retired =
                    new TransferSyntax("JPEG Spectral Selection, Hierarchical (Process 21 & 23) (Retired)",
                                 TransferSyntax.JPEGSpectralSelectionHierarchicalProcess2123RetiredUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Spectral Selection, Non-Hierarchical (Process 6 & 8) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.53</para>
        /// </summary>
        public static readonly String JPEGSpectralSelectionNonHierarchicalProcess68RetiredUid = "1.2.840.10008.1.2.4.53";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Spectral Selection, Non-Hierarchical (Process 6 & 8) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.53</para>
        /// </summary>
        public static readonly TransferSyntax JPEGSpectralSelectionNonHierarchicalProcess68Retired =
                    new TransferSyntax("JPEG Spectral Selection, Non-Hierarchical (Process 6 & 8) (Retired)",
                                 TransferSyntax.JPEGSpectralSelectionNonHierarchicalProcess68RetiredUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG Spectral Selection, Non-Hierarchical (Process 7 & 9) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.54</para>
        /// </summary>
        public static readonly String JPEGSpectralSelectionNonHierarchicalProcess79RetiredUid = "1.2.840.10008.1.2.4.54";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG Spectral Selection, Non-Hierarchical (Process 7 & 9) (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.2.4.54</para>
        /// </summary>
        public static readonly TransferSyntax JPEGSpectralSelectionNonHierarchicalProcess79Retired =
                    new TransferSyntax("JPEG Spectral Selection, Non-Hierarchical (Process 7 & 9) (Retired)",
                                 TransferSyntax.JPEGSpectralSelectionNonHierarchicalProcess79RetiredUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG-LS Lossless Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.80</para>
        /// </summary>
        public static readonly String JPEGLSLosslessImageCompressionUid = "1.2.840.10008.1.2.4.80";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG-LS Lossless Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.80</para>
        /// </summary>
        public static readonly TransferSyntax JPEGLSLosslessImageCompression =
                    new TransferSyntax("JPEG-LS Lossless Image Compression",
                                 TransferSyntax.JPEGLSLosslessImageCompressionUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPEG-LS Lossy (Near-Lossless) Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.81</para>
        /// </summary>
        public static readonly String JPEGLSLossyNearLosslessImageCompressionUid = "1.2.840.10008.1.2.4.81";

        /// <summary>TransferSyntax object representing
        /// <para>JPEG-LS Lossy (Near-Lossless) Image Compression</para>
        /// <para>UID: 1.2.840.10008.1.2.4.81</para>
        /// </summary>
        public static readonly TransferSyntax JPEGLSLossyNearLosslessImageCompression =
                    new TransferSyntax("JPEG-LS Lossy (Near-Lossless) Image Compression",
                                 TransferSyntax.JPEGLSLossyNearLosslessImageCompressionUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPIP Referenced</para>
        /// <para>UID: 1.2.840.10008.1.2.4.94</para>
        /// </summary>
        public static readonly String JPIPReferencedUid = "1.2.840.10008.1.2.4.94";

        /// <summary>TransferSyntax object representing
        /// <para>JPIP Referenced</para>
        /// <para>UID: 1.2.840.10008.1.2.4.94</para>
        /// </summary>
        public static readonly TransferSyntax JPIPReferenced =
                    new TransferSyntax("JPIP Referenced",
                                 TransferSyntax.JPIPReferencedUid,
                                 true, // Little Endian?
                                 false, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>JPIP Referenced Deflate</para>
        /// <para>UID: 1.2.840.10008.1.2.4.95</para>
        /// </summary>
        public static readonly String JPIPReferencedDeflateUid = "1.2.840.10008.1.2.4.95";

        /// <summary>TransferSyntax object representing
        /// <para>JPIP Referenced Deflate</para>
        /// <para>UID: 1.2.840.10008.1.2.4.95</para>
        /// </summary>
        public static readonly TransferSyntax JPIPReferencedDeflate =
                    new TransferSyntax("JPIP Referenced Deflate",
                                 TransferSyntax.JPIPReferencedDeflateUid,
                                 true, // Little Endian?
                                 false, // Encapsulated?
                                 true, // Explicit VR?
                                 true // Deflated?
                                 );

        /// <summary>String representing
        /// <para>MPEG2 Main Profile @ Main Level</para>
        /// <para>UID: 1.2.840.10008.1.2.4.100</para>
        /// </summary>
        public static readonly String MPEG2MainProfileMainLevelUid = "1.2.840.10008.1.2.4.100";

        /// <summary>TransferSyntax object representing
        /// <para>MPEG2 Main Profile @ Main Level</para>
        /// <para>UID: 1.2.840.10008.1.2.4.100</para>
        /// </summary>
        public static readonly TransferSyntax MPEG2MainProfileMainLevel =
                    new TransferSyntax("MPEG2 Main Profile @ Main Level",
                                 TransferSyntax.MPEG2MainProfileMainLevelUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>RFC 2557 MIME encapsulation</para>
        /// <para>UID: 1.2.840.10008.1.2.6.1</para>
        /// </summary>
        public static readonly String RFC2557MIMEencapsulationUid = "1.2.840.10008.1.2.6.1";

        /// <summary>TransferSyntax object representing
        /// <para>RFC 2557 MIME encapsulation</para>
        /// <para>UID: 1.2.840.10008.1.2.6.1</para>
        /// </summary>
        public static readonly TransferSyntax RFC2557MIMEencapsulation =
                    new TransferSyntax("RFC 2557 MIME encapsulation",
                                 TransferSyntax.RFC2557MIMEencapsulationUid,
                                 true, // Little Endian?
                                 false, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        /// <summary>String representing
        /// <para>RLE Lossless</para>
        /// <para>UID: 1.2.840.10008.1.2.5</para>
        /// </summary>
        public static readonly String RLELosslessUid = "1.2.840.10008.1.2.5";

        /// <summary>TransferSyntax object representing
        /// <para>RLE Lossless</para>
        /// <para>UID: 1.2.840.10008.1.2.5</para>
        /// </summary>
        public static readonly TransferSyntax RLELossless =
                    new TransferSyntax("RLE Lossless",
                                 TransferSyntax.RLELosslessUid,
                                 true, // Little Endian?
                                 true, // Encapsulated?
                                 true, // Explicit VR?
                                 false // Deflated?
                                 );

        // Internal members
        private static Dictionary<String, TransferSyntax> _transferSyntaxes = new Dictionary<String, TransferSyntax>();
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

        ///<summary>Property representing the UID string of transfer syntax.</summary>
        public String UidString
        {
            get { return _uid; }
        }

        ///<summary>Property representing the DicomUid of the transfer syntax.</summary>
        public DicomUid DicomUid
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

        ///<summary>Property representing the Endian enumerated value for the transfer syntax.</summary>
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

                _transferSyntaxes.Add(TransferSyntax.DeflatedExplicitVRLittleEndianUid,
                                      TransferSyntax.DeflatedExplicitVRLittleEndian);

                _transferSyntaxes.Add(TransferSyntax.ExplicitVRBigEndianUid,
                                      TransferSyntax.ExplicitVRBigEndian);

                _transferSyntaxes.Add(TransferSyntax.ExplicitVRLittleEndianUid,
                                      TransferSyntax.ExplicitVRLittleEndian);

                _transferSyntaxes.Add(TransferSyntax.ImplicitVRLittleEndianUid,
                                      TransferSyntax.ImplicitVRLittleEndian);

                _transferSyntaxes.Add(TransferSyntax.JPEG2000ImageCompressionUid,
                                      TransferSyntax.JPEG2000ImageCompression);

                _transferSyntaxes.Add(TransferSyntax.JPEG2000ImageCompressionLosslessOnlyUid,
                                      TransferSyntax.JPEG2000ImageCompressionLosslessOnly);

                _transferSyntaxes.Add(TransferSyntax.JPEG2000Part2MulticomponentImageCompressionUid,
                                      TransferSyntax.JPEG2000Part2MulticomponentImageCompression);

                _transferSyntaxes.Add(TransferSyntax.JPEG2000Part2MulticomponentImageCompressionLosslessOnlyUid,
                                      TransferSyntax.JPEG2000Part2MulticomponentImageCompressionLosslessOnly);

                _transferSyntaxes.Add(TransferSyntax.JPEGBaselineProcess1Uid,
                                      TransferSyntax.JPEGBaselineProcess1);

                _transferSyntaxes.Add(TransferSyntax.JPEGExtendedProcess24Uid,
                                      TransferSyntax.JPEGExtendedProcess24);

                _transferSyntaxes.Add(TransferSyntax.JPEGExtendedProcess35RetiredUid,
                                      TransferSyntax.JPEGExtendedProcess35Retired);

                _transferSyntaxes.Add(TransferSyntax.JPEGExtendedHierarchicalProcess1618RetiredUid,
                                      TransferSyntax.JPEGExtendedHierarchicalProcess1618Retired);

                _transferSyntaxes.Add(TransferSyntax.JPEGExtendedHierarchicalProcess1719RetiredUid,
                                      TransferSyntax.JPEGExtendedHierarchicalProcess1719Retired);

                _transferSyntaxes.Add(TransferSyntax.JPEGFullProgressionHierarchicalProcess2426RetiredUid,
                                      TransferSyntax.JPEGFullProgressionHierarchicalProcess2426Retired);

                _transferSyntaxes.Add(TransferSyntax.JPEGFullProgressionHierarchicalProcess2527RetiredUid,
                                      TransferSyntax.JPEGFullProgressionHierarchicalProcess2527Retired);

                _transferSyntaxes.Add(TransferSyntax.JPEGFullProgressionNonHierarchicalProcess1012RetiredUid,
                                      TransferSyntax.JPEGFullProgressionNonHierarchicalProcess1012Retired);

                _transferSyntaxes.Add(TransferSyntax.JPEGFullProgressionNonHierarchicalProcess1113RetiredUid,
                                      TransferSyntax.JPEGFullProgressionNonHierarchicalProcess1113Retired);

                _transferSyntaxes.Add(TransferSyntax.JPEGLosslessHierarchicalProcess28RetiredUid,
                                      TransferSyntax.JPEGLosslessHierarchicalProcess28Retired);

                _transferSyntaxes.Add(TransferSyntax.JPEGLosslessHierarchicalProcess29RetiredUid,
                                      TransferSyntax.JPEGLosslessHierarchicalProcess29Retired);

                _transferSyntaxes.Add(TransferSyntax.JPEGLosslessNonHierarchicalProcess14Uid,
                                      TransferSyntax.JPEGLosslessNonHierarchicalProcess14);

                _transferSyntaxes.Add(TransferSyntax.JPEGLosslessNonHierarchicalProcess15RetiredUid,
                                      TransferSyntax.JPEGLosslessNonHierarchicalProcess15Retired);

                _transferSyntaxes.Add(TransferSyntax.JPEGLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1Uid,
                                      TransferSyntax.JPEGLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1);

                _transferSyntaxes.Add(TransferSyntax.JPEGSpectralSelectionHierarchicalProcess2022RetiredUid,
                                      TransferSyntax.JPEGSpectralSelectionHierarchicalProcess2022Retired);

                _transferSyntaxes.Add(TransferSyntax.JPEGSpectralSelectionHierarchicalProcess2123RetiredUid,
                                      TransferSyntax.JPEGSpectralSelectionHierarchicalProcess2123Retired);

                _transferSyntaxes.Add(TransferSyntax.JPEGSpectralSelectionNonHierarchicalProcess68RetiredUid,
                                      TransferSyntax.JPEGSpectralSelectionNonHierarchicalProcess68Retired);

                _transferSyntaxes.Add(TransferSyntax.JPEGSpectralSelectionNonHierarchicalProcess79RetiredUid,
                                      TransferSyntax.JPEGSpectralSelectionNonHierarchicalProcess79Retired);

                _transferSyntaxes.Add(TransferSyntax.JPEGLSLosslessImageCompressionUid,
                                      TransferSyntax.JPEGLSLosslessImageCompression);

                _transferSyntaxes.Add(TransferSyntax.JPEGLSLossyNearLosslessImageCompressionUid,
                                      TransferSyntax.JPEGLSLossyNearLosslessImageCompression);

                _transferSyntaxes.Add(TransferSyntax.JPIPReferencedUid,
                                      TransferSyntax.JPIPReferenced);

                _transferSyntaxes.Add(TransferSyntax.JPIPReferencedDeflateUid,
                                      TransferSyntax.JPIPReferencedDeflate);

                _transferSyntaxes.Add(TransferSyntax.MPEG2MainProfileMainLevelUid,
                                      TransferSyntax.MPEG2MainProfileMainLevel);

                _transferSyntaxes.Add(TransferSyntax.RFC2557MIMEencapsulationUid,
                                      TransferSyntax.RFC2557MIMEencapsulation);

                _transferSyntaxes.Add(TransferSyntax.RLELosslessUid,
                                      TransferSyntax.RLELossless);

            }

            if (!_transferSyntaxes.ContainsKey(uid))
                return null;

            return _transferSyntaxes[uid];
        }
    }
}
