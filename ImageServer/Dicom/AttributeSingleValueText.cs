using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.ImageServer.Dicom.IO;

namespace ClearCanvas.ImageServer.Dicom
{
    #region AttributeSingleValueText
    public abstract class AttributeSingleValueText : AbstractAttribute
    {
        private String _value = null;

        #region Constructors

        internal AttributeSingleValueText(uint tag) 
            : base(tag)
        {
            
        }

        internal AttributeSingleValueText(DicomTag tag)
            : base(tag)
        {
            
        }

        internal AttributeSingleValueText(DicomTag tag, ByteBuffer item)
            : base(tag)
        {
            _value = item.GetString();
            
            // Saw some Osirix images that had padding on SH attributes with a null character, just
            // pull them out here.
            _value = _value.Trim(new char[] { tag.VR.PadChar, '\0' });

            Count = 1;
            StreamLength = (uint)_value.Length;
        }

        internal AttributeSingleValueText(AttributeSingleValueText attrib)
            : base(attrib)
        {
            String value = attrib;

            _value = String.Copy(value);

        }

        #endregion


        #region Abstract Method Implementation

        public override string ToString()
        {
            if (_value == null)
                return "";

            return _value;
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            AbstractAttribute a = (AbstractAttribute)obj;
            string str = (string)a.Values;

            return _value.Equals(str);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override bool IsNull
        {
            get
            {
                if ((_value != null) && (_value.Length == 0))
                    return true;
                return false;
            }
        }

        public override Object Values
        {
            get { return _value; }
            set
            {
                if (value is String)
                {
                    SetStringValue((string)value);
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override void SetStringValue(String stringValue)
        {
            if (stringValue == null || stringValue.Length == 0)
            {
                Count = 1;
                StreamLength = 0;
                _value = "";
                return;
            }

            _value = stringValue;

            Count = 1;
            StreamLength = (uint)_value.Length;
        }

        public abstract override AbstractAttribute Copy();
        internal abstract override AbstractAttribute Copy(bool copyBinary);

        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax)
        {
            ByteBuffer bb = new ByteBuffer(syntax.Endian);

            bb.SetString(_value, (byte)' ');
            return bb;
        }

        #endregion
    }
    #endregion

    #region AttributeLT
    public class AttributeLT : AttributeSingleValueText
    {
        #region Constructors

        public AttributeLT(uint tag)
            : base(tag)
        {

        }

        public AttributeLT(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.LTvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeLT(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeLT(AttributeLT attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeLT(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeLT(this);
        }

    }
    #endregion

    #region AttributeST
    public class AttributeST : AttributeSingleValueText
    {
        #region Constructors

        public AttributeST(uint tag)
            : base(tag)
        {

        }

        public AttributeST(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.STvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeST(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        internal AttributeST(AttributeST attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeST(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeST(this);
        }

    }
    #endregion

    #region AttributeUT
    public class AttributeUT : AttributeSingleValueText
    {
        #region Constructors

        public AttributeUT(uint tag)
            : base(tag)
        {

        }

        public AttributeUT(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.UTvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeUT(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeUT(AttributeUT attrib)
            : base(attrib)
        {

        }

        #endregion


        public override AbstractAttribute Copy()
        {
            return new AttributeUT(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeUT(this);
        }

    }
    #endregion
}
