using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Class encapsulating a DICOM Value Representation.
    /// </summary>
    public class DicomVr
    {
        #region Private Members
        private String _name;
        private bool _isText = false;
        private bool _specificCharSet = false;
        private bool _isMultiValue = false;
        private uint _maxLength = 0;
        private bool _is16BitLength = false;
        private char _padChar = ' ';
        private int _unitSize = 1;
        #endregion

        #region Public Static Members

        /// <summary>
        /// The Application Entity VR.
        /// </summary>
        public static readonly DicomVr AEvr = new DicomVr("AE", true, false, true, 16, true, ' ', 1);
        /// <summary>
        /// The Age String VR.
        /// </summary>
        public static readonly DicomVr ASvr = new DicomVr("AS", true, false, true, 4, true, ' ', 1);
        /// <summary>
        /// The Attribute Tag VR.
        /// </summary>
        public static readonly DicomVr ATvr = new DicomVr("AT", false, false, true, 4, true, '\0', 4);
        /// <summary>
        /// The Code String VR.
        /// </summary>
        public static readonly DicomVr CSvr = new DicomVr("CS", true, false, true, 16, true, ' ', 1);
        /// <summary>
        /// The Date VR.
        /// </summary>
        public static readonly DicomVr DAvr = new DicomVr("DA", true, false, true, 8, true, ' ', 1);
        /// <summary>
        /// The Decimal String VR.
        /// </summary>
        public static readonly DicomVr DSvr = new DicomVr("DS", true, false, true, 16, true, ' ', 1);
        /// <summary>
        /// The Date Time VR.
        /// </summary>
        public static readonly DicomVr DTvr = new DicomVr("DT", true, false, true, 26, true, ' ', 1);
        /// <summary>
        /// The Floating Point Single VR.
        /// </summary>
        public static readonly DicomVr FLvr = new DicomVr("FL", false, false, true, 4, true, '\0', 4);
        /// <summary>
        /// The Floating Point Double VR.
        /// </summary>
        public static readonly DicomVr FDvr = new DicomVr("FD", false, false, true, 8, true, '\0', 8);
        /// <summary>
        /// The Integer String VR.
        /// </summary>
        public static readonly DicomVr ISvr = new DicomVr("IS", true, false, true, 12, true, ' ', 1);
        /// <summary>
        /// The Long String VR.
        /// </summary>
        public static readonly DicomVr LOvr = new DicomVr("LO", true, true, true, 64, true, ' ', 1);
        /// <summary>
        /// The Long Text VR.
        /// </summary>
        public static readonly DicomVr LTvr = new DicomVr("LT", true, true, false, 10240, true, ' ', 1);
        /// <summary>
        /// The Other Byte String VR.
        /// </summary>
        public static readonly DicomVr OBvr = new DicomVr("OB", false, false, false, 1, false, '\0', 1);
        /// <summary>
        /// The Other Float String VR.
        /// </summary>
        public static readonly DicomVr OFvr = new DicomVr("OF", false, false, false, 4, false, '\0', 4);
        /// <summary>
        /// The Other Word String VR.
        /// </summary>
        public static readonly DicomVr OWvr = new DicomVr("OW", false, false, false, 2, false, '\0', 2);
        /// <summary>
        /// The Person Name VR.
        /// </summary>
        public static readonly DicomVr PNvr = new DicomVr("PN", true, true, true, 64 * 5, true, ' ', 1);
        /// <summary>
        /// The Short String VR.
        /// </summary>
        public static readonly DicomVr SHvr = new DicomVr("SH", true, true, true, 16, true, ' ', 1);
        /// <summary>
        /// The Signed Long VR.
        /// </summary>
        public static readonly DicomVr SLvr = new DicomVr("SL", false, false, true, 4, true, '\0', 4);
        /// <summary>
        /// The Sequence of Items VR.
        /// </summary>
        public static readonly DicomVr SQvr = new DicomVr("SQ", false, false, false, 0, false, '\0', 1);
        /// <summary>
        /// The Signed Short VR.
        /// </summary>
        public static readonly DicomVr SSvr = new DicomVr("SS", false, false, true, 2, true, '\0', 2);
        /// <summary>
        /// The Short Text VR.
        /// </summary>
        public static readonly DicomVr STvr = new DicomVr("ST", true, true, false, 1024, true, ' ', 1);
        /// <summary>
        /// The Time VR.
        /// </summary>
        public static readonly DicomVr TMvr = new DicomVr("TM", true, false, true, 16, true, ' ', 1);
        /// <summary>
        /// The Unique Identifer (UID) VR.
        /// </summary>
        public static readonly DicomVr UIvr = new DicomVr("UI", true, false, true, 64, true, '\0', 1);
        /// <summary>
        /// The Unsigned Long VR.
        /// </summary>
        public static readonly DicomVr ULvr = new DicomVr("UL", false, false, true, 4, true, '\0', 4);
        /// <summary>
        /// The Unknown VR.
        /// </summary>
        public static readonly DicomVr UNvr = new DicomVr("UN", false, false, false, 0, false, '\0', 1);
        /// <summary>
        /// The Unsigned Short VR.
        /// </summary>
        public static readonly DicomVr USvr = new DicomVr("US", false, false, true, 2, true, '\0', 2);
        /// <summary>
        /// The Unlimited Text VR.
        /// </summary>
        public static readonly DicomVr UTvr = new DicomVr("UT", true, true, false, 0, false, ' ', 1);

        internal static readonly DicomVr NONE = new DicomVr("NONE", false, false, false, 1, false, '\0', 1);

        #endregion


        #region Static Methods

        public static DicomVr GetVR(String name)
        {
            if (name.Equals(AEvr.Name)) return AEvr;
            if (name.Equals(ASvr.Name)) return ASvr;
            if (name.Equals(ATvr.Name)) return ATvr;
            if (name.Equals(CSvr.Name)) return CSvr;
            if (name.Equals(DAvr.Name)) return DAvr;
            if (name.Equals(DSvr.Name)) return DSvr;
            if (name.Equals(DTvr.Name)) return DTvr;
            if (name.Equals(FLvr.Name)) return FLvr;
            if (name.Equals(FDvr.Name)) return FDvr;
            if (name.Equals(ISvr.Name)) return ISvr;
            if (name.Equals(LOvr.Name)) return LOvr;
            if (name.Equals(LTvr.Name)) return LTvr;
            if (name.Equals(OBvr.Name)) return OBvr;
            if (name.Equals(OFvr.Name)) return OFvr;
            if (name.Equals(OWvr.Name)) return OWvr;
            if (name.Equals(PNvr.Name)) return PNvr;
            if (name.Equals(SHvr.Name)) return SHvr;
            if (name.Equals(SLvr.Name)) return SLvr;
            if (name.Equals(SQvr.Name)) return SQvr;
            if (name.Equals(SSvr.Name)) return SSvr;
            if (name.Equals(STvr.Name)) return STvr;
            if (name.Equals(TMvr.Name)) return TMvr;
            if (name.Equals(UIvr.Name)) return UIvr;
            if (name.Equals(ULvr.Name)) return ULvr;
            if (name.Equals(USvr.Name)) return USvr;
            if (name.Equals(UTvr.Name)) return UTvr;

            return UNvr;
        }

        #endregion

        /// <summary>
        /// Private constructor for DicomVr.
        /// </summary>
        /// <param name="name">The two digit text name of the VR.</param>
        /// <param name="isText">Boolean telling if the VR is text based.</param>
        /// <param name="isSpecificCharacterSet">Boolean telling if the value for Specific Character Set impacts the VR.</param>
        /// <param name="isMultiValue">Boolean telling if the VR supports multiple values.</param>
        /// <param name="maxLength">The maximum length of the tag, 0 if the tag is unlimited in length (max value of 2^32).</param>
        /// <param name="is16bitLength">The VR is encoded with a 16 bit length for Explict VRs</param>
        /// <param name="padChar">The pad character used to make the VR even length in DICOM streams.</param>
        /// <param name="unitSize">For binary VRs, the size of each value for the VR.</param>
        private DicomVr(String name,
                        bool isText,
                        bool isSpecificCharacterSet,
                        bool isMultiValue,
                        uint maxLength,
                        bool is16bitLength,
                        char padChar,
                        int unitSize)
        {
            _name = name;
            _isText = isText;
            _specificCharSet = isSpecificCharacterSet;
            _isMultiValue = isMultiValue;
            _maxLength = maxLength;
            _is16BitLength = is16bitLength;
            _padChar = padChar;
            _unitSize = unitSize;
        }

        /// <summary>
        /// The two digit string representation of the VR.
        /// </summary>
        /// <returns>A Value Representation string.</returns>
        public override string ToString()
        {
            return _name;
        }

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        public static implicit operator String(DicomVr myVr)
        {
            return myVr.ToString();
        }

        #region Public Properties

        /// <summary>
        /// Is the VR text based?
        /// </summary>
        public bool IsTextVR
        {
            get { return _isText; }
        }

        /// <summary>
        /// Does the VR support multiple values?
        /// </summary>
        public bool IsMultiValue
        {
            get { return _isMultiValue; }
        }

        /// <summary>
        /// Does the value of the tag Specific Character Set impact the encoding of the VR?
        /// </summary>
        public bool SpecificCharacterSet
        {
            get { return _specificCharSet; }
        }

        /// <summary>
        /// What is the maximum length of the a tag encoded with the VR?  (A value of 0 means the maximum length is 2^32.)
        /// </summary>
        public uint MaximumLength
        {
            get { return _maxLength; }
        }

        /// <summary>
        /// The name of the VR.
        /// </summary>
        public String Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Does the VR require 16 bit length fields for Explicit VR transfer syntaxes? 
        /// </summary>
        public bool Is16BitLengthField
        {
            get { return _is16BitLength; }
        }

        /// <summary>
        /// What is the padding character for the VR?
        /// </summary>
        public char PadChar
        {
            get { return _padChar; }
        }

        /// <summary>
        /// For binary VRs, what is the size of each individual value?
        /// </summary>
        public int UnitSize
        {
            get { return _unitSize; }
        }
        #endregion

    }
}

