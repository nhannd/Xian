using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Offis;

namespace ClearCanvas.ImageServer.Dicom
{
    /// <summary>
    /// Abstract class representing a DICOM attribute within an attribute collection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The AbstractAttribute class is a base class that represents a DICOM attribute.  A number of abstract methods are 
    /// defined.  Derived classes exist for each of the VR types.  In addition, the <see cref="AttributeBinary"/>,
    /// <see cref="AttributeMultiValueText"/>, and <see cref="AttributeSingelValueText"/> classes contain generic
    /// implementations for binary VRs, text values that contain multiple values, and text VRs that contain a single
    /// value respectively.
    /// </para>
    /// </remarks>
    public abstract class AbstractAttribute
    {
        #region Private Members

        private DicomTag _tag;
        private long _valueCount = 0;
        private long _length = 0;
        private bool _dirty = false;
        #endregion

        #region Abstract Methods

        public abstract override string ToString();
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
        public abstract bool IsNull { get; }
        public abstract Object Values { get; set; }
        public abstract AbstractAttribute Copy();
        public abstract void SetStringValue(String stringValue);

        internal abstract AbstractAttribute Copy(bool copyBinary);
        internal abstract void FlushAttribute(OffisDcmItem item);
        #endregion

        #region Constructors

        /// <summary>
        /// Internal constructor when a <see cref="DicomTag"/> is used to identify the tag being added.
        /// </summary>
        /// <param name="tag">The DICOM tag associated with the attribute being created.</param>
        internal AbstractAttribute(DicomTag tag)
        {
            _tag = tag;
        }

        /// <summary>
        /// Internal constructor when a uint representation of the tag is used to identify the tag being added.
        /// </summary>
        /// <param name="tag">The DICOM tag associated with the attribute being created.</param>
        internal AbstractAttribute(uint tag)
        {
            _tag = DicomTagDictionary.Instance[tag];
        }

        /// <summary>
        /// Internal constructor used when copying an attribute from a pre-existing attribute instance.
        /// </summary>
        /// <param name="attrib">The attribute that is being copied.</param>
        internal AbstractAttribute(AbstractAttribute attrib)
        {
            _tag = attrib.Tag;
            _valueCount = attrib.Count;
            _dirty = true;
            _length = attrib.Length;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Retrieve <see cref="Tag"/> instance for the attribute.
        /// </summary>
        public DicomTag Tag
        {
            get { return _tag; }
        }

        public long Length
        {
            get { return _length; }
            protected set { _length = value; }
        }

        public long Count
        {
            get { return _valueCount; }
            protected set { _valueCount = value; }
        }

        #endregion

        #region Internal Properties

        internal bool Dirty
        {
            get { return _dirty; }
            set { _dirty = value; }
        }

        #endregion

        #region Internal Static Methods

        internal static AbstractAttribute NewAttribute(uint tag)
        {
            DicomTag dictionTag = DicomTagDictionary.Instance[tag];
            return NewAttribute(dictionTag);
        }

        internal static AbstractAttribute NewAttribute(DicomTag tag)
        {
            if (tag == null)
                return null;


            if (tag.VR.Equals(DicomVr.AEvr))
            {
                return new AttributeAE(tag);
            }
            else if (tag.VR.Equals(DicomVr.ASvr))
            {
                return new AttributeAS(tag);
            }
            else if (tag.VR.Equals(DicomVr.ATvr))
            {
                return new AttributeAT(tag);
            }
            else if (tag.VR.Equals(DicomVr.CSvr))
            {
                return new AttributeCS(tag);
            }
            else if (tag.VR.Equals(DicomVr.DAvr))
            {
                return new AttributeDA(tag);
            }
            else if (tag.VR.Equals(DicomVr.DSvr))
            {
                return new AttributeDS(tag);
            }
            else if (tag.VR.Equals(DicomVr.DTvr))
            {
                return new AttributeDT(tag);
            }
            else if (tag.VR.Equals(DicomVr.FLvr))
            {
                return new AttributeFL(tag);
            }
            else if (tag.VR.Equals(DicomVr.FDvr))
            {
                return new AttributeFD(tag);
            }
            else if (tag.VR.Equals(DicomVr.ISvr))
            {
                return new AttributeIS(tag);
            }
            else if (tag.VR.Equals(DicomVr.LOvr))
            {
                return new AttributeLO(tag);
            }
            else if (tag.VR.Equals(DicomVr.LTvr))
            {
                return new AttributeLT(tag);
            }
            else if (tag.VR.Equals(DicomVr.OBvr))
            {
                return new AttributeOB(tag);
            }
            else if (tag.VR.Equals(DicomVr.OFvr))
            {
                return new AttributeOF(tag);
            }
            else if (tag.VR.Equals(DicomVr.OWvr))
            {
                return new AttributeOW(tag);
            }
            else if (tag.VR.Equals(DicomVr.PNvr))
            {
                return new AttributePN(tag);
            }
            else if (tag.VR.Equals(DicomVr.SHvr))
            {
                return new AttributeSH(tag);
            }
            else if (tag.VR.Equals(DicomVr.SLvr))
            {
                return new AttributeSL(tag);
            }
            else if (tag.VR.Equals(DicomVr.SQvr))
            {
                return new AttributeSQ(tag);
            }
            else if (tag.VR.Equals(DicomVr.SSvr))
            {
                return new AttributeSS(tag);
            }
            else if (tag.VR.Equals(DicomVr.STvr))
            {
                return new AttributeST(tag);
            }
            else if (tag.VR.Equals(DicomVr.TMvr))
            {
                return new AttributeTM(tag);
            }
            else if (tag.VR.Equals(DicomVr.UIvr))
            {
                return new AttributeUI(tag);
            }
            else if (tag.VR.Equals(DicomVr.ULvr))
            {
                return new AttributeUL(tag);
            }
            else if (tag.VR.Equals(DicomVr.UNvr))
            {
                return new AttributeUN(tag);
            }
            else if (tag.VR.Equals(DicomVr.USvr))
            {
                return new AttributeUS(tag);
            }
            else if (tag.VR.Equals(DicomVr.UTvr))
            {
                return new AttributeUT(tag);
            }
            
            return null;
                    
        }
        internal static AbstractAttribute NewAttribute(uint tag, OffisDcmItem offisItem)
        {
            DicomTag dictionTag = DicomTagDictionary.Instance[tag];
            return NewAttribute(dictionTag, offisItem);
        }

        internal static AbstractAttribute NewAttribute(DicomTag tag, OffisDcmItem offisItem)
        {
            if (tag == null)
                return null;


            if (tag.VR.Equals(DicomVr.AEvr))
            {
                return new AttributeAE(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.ASvr))
            {
                return new AttributeAS(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.ATvr))
            {
                return new AttributeAT(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.CSvr))
            {
                return new AttributeCS(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.DAvr))
            {
                return new AttributeDA(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.DSvr))
            {
                return new AttributeDS(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.DTvr))
            {
                return new AttributeDT(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.FLvr))
            {
                return new AttributeFL(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.FDvr))
            {
                return new AttributeFD(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.ISvr))
            {
                return new AttributeIS(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.LOvr))
            {
                return new AttributeLO(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.LTvr))
            {
                return new AttributeLT(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.OBvr))
            {
                return new AttributeOB(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.OFvr))
            {
                return new AttributeOF(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.OWvr))
            {
                return new AttributeOW(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.PNvr))
            {
                return new AttributePN(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.SHvr))
            {
                return new AttributeSH(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.SLvr))
            {
                return new AttributeSL(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.SQvr))
            {
                return new AttributeSQ(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.SSvr))
            {
                return new AttributeSS(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.STvr))
            {
                return new AttributeST(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.TMvr))
            {
                return new AttributeTM(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.UIvr))
            {
                return new AttributeUI(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.ULvr))
            {
                return new AttributeUL(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.UNvr))
            {
                return new AttributeUN(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.USvr))
            {
                return new AttributeUS(tag, offisItem);
            }
            else if (tag.VR.Equals(DicomVr.UTvr))
            {
                return new AttributeUT(tag, offisItem);
            }

            return null;

        }
        #endregion

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        public static implicit operator String(AbstractAttribute attr)
        {
            // Uses the actual ToString implementation of the derived class.
            return attr.ToString();
        }
    }
}
