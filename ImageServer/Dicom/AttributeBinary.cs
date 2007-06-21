using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    public abstract class AttributeBinary : AbstractAttribute
    {
        protected byte[] _values;

        internal AttributeBinary(uint tag) 
            : base(tag)
        {
            
        }

        internal AttributeBinary(DicomTag tag)
            : base(tag)
        {
            
        }

        internal AttributeBinary(DicomTag tag, OffisDcmItem item)
            : base(tag)
        {
        }

        internal AttributeBinary(AttributeBinary attrib)
            : base(attrib)
        {
        }


        #region Abstract Methods

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            AttributeBinary a = (AttributeBinary)obj;
            byte[] array = (byte[])a.Values;

            if (Count != a.Count)
                return false;
            if (Count == 0 && a.Count == 0)
                return true;
            if (array.Length != _values.Length)
                return false;

            for (int i = 0; i < a.Count; i++)
                if (!array[i].Equals(_values[i]))
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            return _values.GetHashCode();
        }

        public override void SetStringValue(String stringValue)
        {
            throw new DicomException("Function all incompativle with VR type");
        }

        public abstract override string ToString();
        public abstract override bool IsNull { get; }
        public abstract override Object Values { get; }
        public abstract override AbstractAttribute Copy();
        internal abstract override AbstractAttribute Copy(bool copyBinary);
        internal abstract override void FlushAttribute(OffisDcmItem item);

        #endregion
    }
}
