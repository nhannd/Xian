using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Dicom
{
    /// <summary>
    /// The DicomTag class contains all DICOM information for a specific tag.
    /// </summary>
    /// <remarks>
    /// <para>The DicomTag class is used as described in the Flyweight pattern.  A single instance should only be allocated
    /// for each DICOM tag, and that instance will be shared in any <see cref="AttributeCollection"/> 
    /// that references the specific tag.</para>
    /// <para>Note, however, that non standard DICOM tags (or tags not in stored in the <see cref="DicomTagDictionary>"/>
    /// will have a specific instance allocated to store their information when they are encountered by the assembly.</para>
    /// </remarks>
    public class DicomTag
    {
        #region Private Members
        private uint _tag;
        private string _name;
        private DicomVr _vr;
        private uint _vmLow;
        private uint _vmHigh;
        private bool _isRetired;
        private bool _multiVrTag;
        #endregion

        #region Constructors
        /// <summary>
        /// Primary constructor for dictionary tags
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="name"></param>
        /// <param name="vr"></param>
        /// <param name="isMultiVrTag"></param>
        /// <param name="vmLow"></param>
        /// <param name="vmHigh"></param>
        /// <param name="isRetired"></param>
        public DicomTag(uint tag, String name, DicomVr vr, bool isMultiVrTag, uint vmLow, uint vmHigh, bool isRetired)
        {
            _tag = tag;
            _name = name;
            _vr = vr;
            _multiVrTag = isMultiVrTag;
            _vmLow = vmLow;
            _vmHigh = vmHigh;
            _isRetired = isRetired;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Group Number of the tag as a 16-bit unsigned integer.
        /// </summary>
        public ushort Group
        {
            get { return ((ushort)((_tag & 0xffff0000) >> 16)); }
        }

        /// <summary>
        /// Gets the Element Number of the tag as a 16-bit unsigned integer.
        /// </summary>
        public ushort Element
        {
            get { return ((ushort)(_tag & 0x0000ffff)); }
        }

        /// <summary>
        /// Gets a text description of the tag.
        /// </summary>
        public String Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets a boolean telling if the tag is retired.
        /// </summary>
        public bool Retired
        {
            get { return _isRetired; }
        }

        /// <summary>
        /// Gets a boolean telling if the tag supports multiple VRs.
        /// </summary>
        public bool MultiVR
        {
            get { return _multiVrTag; }
        }

        /// <summary>
        /// Returns a <see cref="DicomVr"/> object representing the VR of the tag.
        /// </summary>
        public DicomVr VR
        {
            get { return _vr; }
        }

        /// <summary>
        /// Returns a uint DICOM Tag value for the object.
        /// </summary>
        public uint TagValue
        {
            get { return _tag; }
        }

		/// <summary>
		/// Returns a string with the tag value in Hex
		/// </summary>
		public String HexString
		{
			get
			{
				return _tag.ToString("X8");
			}
		}

        #endregion

        /// <summary>
        /// Provides a hash code that's more natural by using the
        /// Group and Element number of the tag.
        /// </summary>
        /// <returns>The Group and Element number as a 32-bit integer.</returns>
        public override int GetHashCode()
        {
            return ((int)_tag);
        }

        /// <summary>
        /// Provides a human-readable representation of the tag.
        /// </summary>
        /// <returns>The string representation of the Group and Element.</returns>
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.AppendFormat("({0:x4},{1:x4}) ", Group, Element);
            buffer.Append(_name);

            return buffer.ToString();
        }

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        public static implicit operator String(DicomTag myTag)
        {
            return myTag.ToString();
        }


        /// <summary>
        /// This override allows the comparison of two DicomTag objects
        /// for semantic equivalence. 
        /// </summary>
        /// <param name="obj">The other DicomTag object to compare this object to.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            DicomTag otherTag = obj as DicomTag;
            if (null == otherTag)
                return false;

            return (otherTag.GetHashCode() == this.GetHashCode());
        }
    }
}
