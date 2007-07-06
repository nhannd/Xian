using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.ImageServer.Dicom.IO;

namespace ClearCanvas.ImageServer.Dicom
{
    #region AttributeMultiValueText
    public abstract class AttributeMultiValueText : AbstractAttribute
    {
        #region Private Members

        protected String[] _values = null;

        #endregion

        #region Constructors

        internal AttributeMultiValueText(uint tag) 
            : base(tag)
        {
            
        }

        internal AttributeMultiValueText(DicomTag tag)
            : base(tag)
        {
            
        }

        internal AttributeMultiValueText(DicomTag tag, ByteBuffer item)
            : base(tag)
        {
            String valueArray = item.GetString();

            // store the length before removing pad chars
            StreamLength = (uint)valueArray.Length;

            // Saw some Osirix images that had padding on SH attributes with a null character, just
            // pull them out here.
            valueArray = valueArray.Trim(new char[] { tag.VR.PadChar, '\0' });

            _values = valueArray.Split(new char[] { '\\' });

            Count = (long)_values.Length;

            StreamLength = (uint)valueArray.Length;
        }

        internal AttributeMultiValueText(AttributeMultiValueText attrib)
            : base(attrib)
        {
            String[] values = (String[])attrib.Values;

            _values = new String[values.Length];

            for (int i = 0; i < values.Length; i++)
                _values[i] = String.Copy(values[i]);
        }

        #endregion

        #region Operators

        public String this[int val]
        {
            get
            {
                return _values[val];
            }
        }

        #endregion

        #region Abstract Method Implementation

        public override string ToString()
        {
            if (_values == null)
                return "";

            String value = null;

            foreach (String val in _values)
            {
                if (value == null)
                    value = val;
                else 
                    value += "\\" + val;
            }

            if (value == null) return "";
             
            return value;
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            AttributeMultiValueText a = (AttributeMultiValueText)obj;

            // check if both values are null
            if (this.Count == 1 && this.StreamLength == 0 && a.Count == 1 && a.StreamLength == 0)
                return true;

            return ToString().Equals(a.ToString());
        }

        public override int GetHashCode()
        {
            return _values.GetHashCode();
        }

        public override bool IsNull
        {
            get
            {
                if ((Count == 1) && (_values!=null) && (_values.Length == 0))
                    return true;
                return false;
            }
        }

        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is string[])
                {
                    _values = (string[])value;
                    Count = _values.Length;
                    StreamLength = (uint)ToString().Length;
                }
                else if (value is string)
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
                _values = new String[0];
                return;
            }

            _values = stringValue.Split(new char[] { '\\' });

            Count = (long)_values.Length;

            StreamLength = (uint)stringValue.Length;
        }

        public abstract override AbstractAttribute Copy();
        internal abstract override AbstractAttribute Copy(bool copyBinary);

        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax)
        {
            ByteBuffer bb = new ByteBuffer(syntax.Endian);

            bb.SetString(ToString(), (byte)' ');

            return bb;
        }

        #endregion

    }
    #endregion

    #region AttributeAE
    public class AttributeAE : AttributeMultiValueText
    {
        #region Constructors

        public AttributeAE(uint tag)
            : base(tag)
        {

        }

        public AttributeAE(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.AEvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeAE(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeAE(AttributeAE attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Abstract Method Implementation

        public override AbstractAttribute Copy()
        {
            return new AttributeAE(this);
        }
        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeAE(this);
        }

        #endregion

    }
    #endregion

    #region AttributeAS
    public class AttributeAS : AttributeMultiValueText
    {
        #region Constructors

        public AttributeAS(uint tag)
            : base(tag)
        {

        }

        public AttributeAS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.ASvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeAS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeAS(AttributeAS attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeAS(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeAS(this);
        }

    }
    #endregion

    #region AttributeCS
    public class AttributeCS : AttributeMultiValueText
    {
        #region Constructors

        public AttributeCS(uint tag)
            : base(tag)
        {

        }

        public AttributeCS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.CSvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeCS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        internal AttributeCS(AttributeCS attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeCS(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeCS(this);
        }

    }
    #endregion

    #region AttributeDA
    public class AttributeDA : AttributeMultiValueText
    {
        #region Constructors

        public AttributeDA(uint tag)
            : base(tag)
        {
        }

        public AttributeDA(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.DAvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeDA(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeDA(AttributeDA attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeDA(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeDA(this);
        }

    }
    #endregion

    #region AttributeDS
    public class AttributeDS : AttributeMultiValueText
    {
        #region Constructors

        public AttributeDS(uint tag)
            : base(tag)
        {

        }

        public AttributeDS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.DSvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeDS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeDS(AttributeDS attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeDS(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeDS(this);
        }

    }
    #endregion

    #region AttributeDT
    public class AttributeDT : AttributeMultiValueText
    {
        #region Constructors

        public AttributeDT(uint tag)
            : base(tag)
        {

        }

        public AttributeDT(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.DTvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeDT(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeDT(AttributeDT attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeDT(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeDT(this);
        }

    }
    #endregion

    #region AttributeIS
    public class AttributeIS : AttributeMultiValueText
    {
        #region Constructors

        public AttributeIS(uint tag)
            : base(tag)
        {

        }

        public AttributeIS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.ISvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeIS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeIS(AttributeIS attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeIS(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeIS(this);
        }

    }
    #endregion

    #region AttributeLO
        public class AttributeLO : AttributeMultiValueText
    {
        #region Constructors

        public AttributeLO(uint tag) 
            : base(tag)
        {
            
        }

        public AttributeLO(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.LOvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeLO(AttributeLO attrib)
            : base(attrib)
        {
        }

        internal AttributeLO(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        #endregion 

        public override AbstractAttribute Copy()
        {
            return new AttributeLO(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeLO(this);
        }

    }
    #endregion

    #region AttributePN
    public class AttributePN : AttributeMultiValueText
    {
        #region Constructors

        public AttributePN(uint tag)
            : base(tag)
        {

        }

        public AttributePN(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.PNvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributePN(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        internal AttributePN(AttributePN attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributePN(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributePN(this);
        }

    }
    #endregion

    #region AttributeSH
    public class AttributeSH : AttributeMultiValueText
    {
        #region Constructors

        public AttributeSH(uint tag)
            : base(tag)
        {

        }

        public AttributeSH(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.SHvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeSH(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        internal AttributeSH(AttributeSH attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeSH(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeSH(this);
        }

    }
    #endregion

    #region AttributeTM
    public class AttributeTM : AttributeMultiValueText
    {
        #region Constructors

        public AttributeTM(uint tag)
            : base(tag)
        {

        }

        public AttributeTM(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.TMvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeTM(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeTM(AttributeTM attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeTM(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeTM(this);
        }
    }
    #endregion

    #region AttributeUI
    public class AttributeUI : AttributeMultiValueText
    {
        #region Constructors

        public AttributeUI(uint tag)
            : base(tag)
        {

        }

        public AttributeUI(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.UIvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeUI(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeUI(AttributeUI attrib)
            : base(attrib)
        {
        }

        #endregion

        public override DicomUid GetUid(int i)
        {
            SopClass sop = SopClass.GetSopClass(base._values[i]);
            if (sop != null)
                return new DicomUid(sop.Uid, sop.Name, UidType.SOPClass);

            TransferSyntax ts = TransferSyntax.GetTransferSyntax(base._values[i]);
            if (ts != null)
                return new DicomUid(ts.UID.UID, ts.Name, UidType.TransferSyntax);

            return new DicomUid(base._values[i], base._values[i], UidType.Unknown);
        }


        public override AbstractAttribute Copy()
        {
            return new AttributeUI(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeUI(this);
        }


        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax)
        {
            ByteBuffer bb = new ByteBuffer(syntax.Endian);

            bb.SetString(ToString(), (byte)0x00);

            return bb;
        }

    }
    #endregion
}
