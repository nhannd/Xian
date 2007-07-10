using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{

    public enum UidType
    {
        TransferSyntax,
        SOPClass,
        MetaSOPClass,
        SOPInstance,
        ApplicationContextName,
        CodingScheme,
        SynchronizationFrameOfReference,
        Unknown
    }

    public class DicomUid
    {
        public readonly string UID;
        public readonly string Description;
        public readonly UidType Type;

        private DicomUid() { }

        internal DicomUid(string uid, string desc, UidType type)
        {
            UID = uid;
            Description = desc;
            Type = type;
        }

        public override string ToString()
        {
            if (Type == UidType.Unknown)
                return UID;
            return "==" + Description;
        }

        public override bool Equals(object obj)
        {
            if (obj is DicomUid)
                return ((DicomUid)obj).UID.Equals(UID);
            if (obj is String)
                return (String)obj == UID;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
