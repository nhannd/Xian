#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

#region mDCM License
// mDCM: A C# DICOM library
//
// Copyright (c) 2008  Colby Dillion
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Author:
//    Colby Dillion (colby.dillion@gmail.com)
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// The DicomAttributeCollection class models an a collection of DICOM attributes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class represents a collection of <see cref="DicomAttribute"/> classes.  It is used by the <see cref="DicomMessageBase"/> class to 
    /// represent the meta info and data set of <see cref="DicomFile"/> and <see cref="DicomMessage"/> objects.
    /// </para>
    /// <para>
    /// 
    /// </para>
    /// </remarks>
	public class DicomAttributeCollection : IEnumerable<DicomAttribute>, IDicomAttributeProvider
    {
        #region Member Variables
        private readonly SortedDictionary<uint, DicomAttribute> _attributeList = new SortedDictionary<uint, DicomAttribute>();
        private String _specificCharacterSet = String.Empty;
        private readonly uint _startTag = 0x00000000;
        private readonly uint _endTag = 0xFFFFFFFF;

        private bool _validateVrLengths = DicomSettings.Default.ValidateVrLengths;
		private bool _validateVrValues = DicomSettings.Default.ValidateVrValues;

		#endregion

        #region Constructors

        /// <summary>
        /// Default constuctor.
        /// </summary>
        public DicomAttributeCollection()
        {
        }

        /// <summary>
        /// Contructor that sets the range of tags in use for the collection.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This constructor is used to set a range of valid tags for the collection.
        /// All tags must be greater than or equal to <paramref name="startTag"/>
        /// ad less than or equal to <paramref name="endTag"/>.  
        /// </para>
        /// <para>
        /// The <see cref="DicomMessage"/> and <see cref="DicomFile"/> classes use 
        /// this form of the constructor when creating the DataSet and MetaInfo 
        /// <see cref="DicomAttributeCollection"/> instances.</para>
        /// </remarks>
        /// <param name="startTag">The valid start tag for attributes in the collection.</param>
        /// <param name="endTag">The value stop tag for attributes in the collection.</param>
        public DicomAttributeCollection(uint startTag, uint endTag)
        {
            _startTag = startTag;
            _endTag = endTag;
        }

        /// <summary>
        /// Internal constructor used when creating a copy of an DicomAttributeCollection.
        /// </summary>
        /// <param name="source">The source collection to copy attributes from.</param>
        /// <param name="copyBinary"></param>
        internal DicomAttributeCollection(DicomAttributeCollection source, bool copyBinary)
        {
        	_startTag = source.StartTagValue;
        	_endTag = source.EndTagValue;
        	_specificCharacterSet = source.SpecificCharacterSet;

            foreach (DicomAttribute attrib in source)
            {
                if (copyBinary ||
                      (!(attrib is DicomAttributeOB)
                    && !(attrib is DicomAttributeOW)
                    && !(attrib is DicomAttributeOF)
                    && !(attrib is DicomFragmentSequence)
                    && !(attrib is DicomAttributeUN)))
                {
                    this[attrib.Tag] = attrib.Copy(copyBinary);
                }
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The specific character set string associated with the collection.
        /// </summary>
        /// <remarks>An empty string is returned if the specific character set
        /// tag is not set for the collection.</remarks>
        public String SpecificCharacterSet
        {
            get { return _specificCharacterSet; }
            set 
            { 
                _specificCharacterSet = value;

                // This line forces the value to be placed in sequences when we don't want it to be, because of how the parser is set
                //this[DicomTags.SpecificCharacterSet].SetStringValue(_specificCharacterSet);
            }
        }

        /// <summary>
        /// The number of attributes in the collection.
        /// </summary>
        public int Count
        { 
            get { return _attributeList.Count; } 
        }

        /// <summary>
        /// The first valid tag for attributes in the collection.
        /// </summary>
        public uint StartTagValue
        {
            get { return _startTag; }
        }

        /// <summary>
        /// The last valid tag for attributes in the collection.
        /// </summary>
        public uint EndTagValue
        {
            get { return _endTag; }
        }

        /// <summary>
        /// Gets the dump string (useful for seeing the dump output in the debugger's local variables window).
        /// </summary>
        /// <value>The dump string.</value>
        public string DumpString
        {
            get { return Dump(String.Empty, DicomDumpOptions.None); }
        }

		public bool ValidateVrLengths
		{
			get { return _validateVrLengths; }
			set { _validateVrLengths = value; }
		}
		
		public bool ValidateVrValues
    	{
			get { return _validateVrValues; }
			set { _validateVrValues = value; }
    	}

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines if an attribute collection is empty.
        /// </summary>
        /// <returns>true if empty (no tags have a value), false otherwise.</returns>
        public bool IsEmpty()
        {
            foreach (DicomAttribute attr in this)
            {
                if (attr.Count > 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Check if a tag is contained in an DicomAttributeCollection and has a value.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool Contains(uint tag)
        {
			DicomAttribute attrib;
			return TryGetAttribute(tag, out attrib);
		}

        /// <summary>
        /// Check if a tag is contained in an DicomAttributeCollection and has a value.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool Contains(DicomTag tag)
        {
        	DicomAttribute attrib;
        	return TryGetAttribute(tag, out attrib);
        }

		/// <summary>
		/// Combines the functionality of the <see cref="Contains(uint)"/> method with the indexer.  Returns the attribute if it exists within the collection.
		/// </summary>
		/// <param name="tag">The tag to get.</param>
		/// <param name="attrib">The output attribute.  Null if the attribute doesn't exist in the collection.  Will be set if the attribute exists, but is empty.</param>
		/// <returns>true if the attribute exists and is not empty.</returns>
		public bool TryGetAttribute(uint tag, out DicomAttribute attrib)
		{
			if (!_attributeList.TryGetValue(tag, out attrib))
			{
				return false;
			}

			return !attrib.IsEmpty;
		}

		/// <summary>
		/// Combines the functionality of the <see cref="Contains(uint)"/> method with the indexer.  Returns the attribute if it exists within the collection.
		/// </summary>
		/// <param name="tag">The tag to get.</param>
		/// <param name="attrib">The output attribute.  Null if the attribute doesn't exist in the collection.  Will be set if the attribute exists, but is empty.</param>
		/// <returns>true if the attribute exists and is not empty.</returns>
		public bool TryGetAttribute(DicomTag tag, out DicomAttribute attrib)
		{
			if (!_attributeList.TryGetValue(tag.TagValue, out attrib))
			{
				return false;
			}

			return !attrib.IsEmpty;
		}

        /// <summary>
        /// Get a <see cref="DicomAttribute"/> for a specific DICOM tag.
        /// </summary>
        /// <remarks>
        /// <para>If <paramref name="tag"/> does not exist in the collection, a new <see cref="DicomAttribute"/>
        /// instance is created, however, it is not added to the collection.</para>
        /// <para>A check is done to be sure that <paramref name="tag"/> is a valid DICOM tag in the 
        /// <see cref="DicomTagDictionary"/>.  If it is not a valid tag, a <see cref="DicomException"/>
        /// is thrown.</para>
        /// </remarks>
        /// <param name="tag">The DICOM tag.</param>
        /// <returns>A <see cref="DicomAttribute"/> instance.</returns>
        public DicomAttribute GetAttribute(uint tag)
        {
            DicomAttribute attr;
			if (!_attributeList.TryGetValue(tag, out attr))
			{
                if ((tag < _startTag) || (tag > _endTag))
                    throw new DicomException("Tag is out of range for collection: " + tag.ToString("X8"));

                DicomTag dicomTag = DicomTagDictionary.GetDicomTag(tag);

                if (dicomTag == null)
                {
                    throw new DicomException("Invalid tag: " + tag.ToString("X8"));
                }

				attr = dicomTag.CreateDicomAttribute();
            }

            return attr;
        }

        /// <summary>
        /// Get a <see cref="DicomAttribute"/> for a specific DICOM tag.
        /// </summary>
        /// <remarks>
        /// <para>If <paramref name="tag"/> does not exist in the collection, a new <see cref="DicomAtribute"/>
        /// instance is created, however, it is not added to the collection.</para>
        /// <para>A check is done to be sure that <paramref name="tag"/> is a valid DICOM tag in the 
        /// <see cref="DicomTagDictionary"/>.  If it is not a valid tag, a <see cref="DicomException"/>
        /// is thrown.</para>
        /// </remarks>
        /// <param name="tag">The DICOM tag.</param>
        /// <returns>A <see cref="DicomAttribute"/> instance.</returns>
        public DicomAttribute GetAttribute(DicomTag tag)
        {
            DicomAttribute attr;

            if (tag == null)
                throw new NullReferenceException("Null DicomTa parameter");

			if (!_attributeList.TryGetValue(tag.TagValue, out attr))
			{
				if ((tag.TagValue < _startTag) || (tag.TagValue > _endTag))
					throw new DicomException("Tag is out of range for collection: " + tag);

				attr = tag.CreateDicomAttribute();

				if (attr == null)
					throw new DicomException("Invalid tag: " + tag.HexString);
			}

        	return attr;
        }

        /// <summary>
        /// Indexer to return a specific tag in the attribute collection.
        /// </summary>
        /// <remarks>
        /// <para>When setting, if the value is null, the tag will be removed from the collection.</para>
        /// <para>If the tag does not exist within the collection, a new <see cref="DicomAttribute"/>
        /// derived instance will be created and returned by this indexer.</para>
        /// </remarks>
        /// <param name="tag">The tag to look for.</param>
        /// <returns></returns>
        public DicomAttribute this[uint tag]
        {
            get 
            {
                DicomAttribute attr;

				if (!_attributeList.TryGetValue(tag, out attr))
				{
					if ((tag < _startTag) || (tag > _endTag))
						throw new DicomException("Tag is out of range for collection: " + tag);

					DicomTag dicomTag = DicomTagDictionary.GetDicomTag(tag);

					if (dicomTag == null)
					{
						throw new DicomException("Invalid tag: " + tag.ToString("X8"));
					}
					attr = dicomTag.CreateDicomAttribute();
					attr.ParentCollection = this;
					_attributeList[tag] = attr;
				}

            	return attr; 
            }
            set 
            {
                if (value == null)
                {
                	DicomAttribute attr;
                    if (_attributeList.TryGetValue(tag, out attr))
                    {
                        attr.ParentCollection = null;
                        _attributeList.Remove(tag);
                    }
                }
                else
                {
                    if ((tag < _startTag) || (tag > _endTag))
                        throw new DicomException("Tag is out of range for collection: " + tag);

                    if (value.Tag.TagValue != tag)
                        throw new DicomException("Tag being set does not match tag in DicomAttribute");

                    _attributeList[tag] = value;
                    value.ParentCollection = this;                    
                }
            }
        }

        /// <summary>
        /// Indexer when retrieving a specific tag in the collection.
        /// </summary>
        /// <remarks>
        /// <para>When setting, if the value is null, the tag will be removed from the collection.</para>
        /// <para>If the tag does not exist within the collection, a new <see cref="DicomAttribute"/>
        /// derived instance will be created and returned by this indexer.</para>
        /// </remarks>
        /// <param name="tag"></param>
        /// <returns></returns>
        public DicomAttribute this[DicomTag tag]
        {
            get
            {
                DicomAttribute attr;

				if (!_attributeList.TryGetValue(tag.TagValue, out attr))
				{
					if ((tag.TagValue < _startTag) || (tag.TagValue > _endTag))
						throw new DicomException("Tag is out of range for collection: " + tag);

					attr = tag.CreateDicomAttribute();

					if (attr == null)
					{
						throw new DicomException("Invalid tag: " + tag.HexString);
					}
					attr.ParentCollection = this;
					_attributeList[tag.TagValue] = attr;
				}

            	return attr;
            }
            set
            {
                if (value == null)
                {
                    if (_attributeList.ContainsKey(tag.TagValue))
                    {
                        DicomAttribute attr = _attributeList[tag.TagValue];
                        attr.ParentCollection = null;
                        _attributeList.Remove(tag.TagValue);
                    }
                }
                else
                {
                    if (value.Tag.TagValue != tag.TagValue)
                        throw new DicomException("Tag being set does not match tag in DicomAttribute");

                    if ((tag.TagValue < _startTag) || (tag.TagValue > _endTag))
                        throw new DicomException("Tag is out of range for collection: " + tag);

                    _attributeList[tag.TagValue] = value;
                    value.ParentCollection = this;
                }
            }
        }

        /// <summary>
        /// Create a duplicate copy of the DicomAttributeCollection.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method creates a copy of all of the attributes within the DicomAttributeCollection and returns 
        /// a new copy.  Note that binary attributes with a VR of OB, OW, OF, and UN are copied.
        /// </para>
        /// </remarks>
        /// <returns>A new DicomAttributeCollection.</returns>
        public virtual DicomAttributeCollection Copy()
        {
            return Copy(true);
        }

        /// <summary>
        /// Create a duplicate copy of the DicomAttributeCollection.
        /// </summary>
        /// <remarks>This method will not copy <see cref="DicomAttributeOB"/>,
        /// <see cref="DicomAttributeOW"/> and <see cref="DicomAttributeOF"/>
        /// instances if the <paramref name="copyBinary"/> parameter is set
        /// to false.</remarks>
        /// <param name="copyBinary">Flag to set if binary VR attributes will be copied.</param>
        /// <returns>a new DicomAttributeCollection.</returns>
        public virtual DicomAttributeCollection Copy(bool copyBinary)
        {
            return new DicomAttributeCollection(this, copyBinary);
        }

        /// <summary>
        /// Check if the contents of the DicomAttributeCollection is identical to another DicomAttributeCollection instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method compares the contents of two attribute collections to see if they are equal.  The method
        /// will step through each of the tags within the collection, and compare them to see if they are equal.  The
        /// method will also recurse into sequence attributes to be sure they are equal.</para>
        /// </remarks>
        /// <param name="obj">The objec to compare to.</param>
        /// <returns>true if the collections are equal.</returns>
        public override bool Equals(object obj)
        {
            string failureReason;
            return Equals(obj, out failureReason);
        }

        /// <summary>
        /// Check if the contents of the DicomAttributeCollection is identical to another DicomAttributeCollection instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method compares the contents of two attribute collections to see if they are equal.  The method
        /// will step through each of the tags within the collection, and compare them to see if they are equal.  The
        /// method will also recurse into sequence attributes to be sure they are equal.</para>
        /// </remarks>
        /// <param name="obj">The objec to compare to.</param>
        /// <param name="comparisonFailure">An output string describing why the objects are not equal.</param>
        /// <returns>true if the collections are equal.</returns>
        public bool Equals(object obj, out string comparisonFailure)
        {
            DicomAttributeCollection a = obj as DicomAttributeCollection;
            if (a == null)
            {
                comparisonFailure = String.Format("Comparison object is invalid type: {0}", obj.GetType());
                return false;
            }

            IEnumerator<DicomAttribute> thisEnumerator = GetEnumerator();
            IEnumerator<DicomAttribute> compareEnumerator = a.GetEnumerator();

            for (; ; )
            {
                bool thisValidNext = thisEnumerator.MoveNext();
                bool compareValidNext = compareEnumerator.MoveNext();

                // Skip empty attributes
                while (thisValidNext && thisEnumerator.Current.IsEmpty)
                    thisValidNext = thisEnumerator.MoveNext();
                while (compareValidNext && compareEnumerator.Current.IsEmpty)
                    compareValidNext = compareEnumerator.MoveNext();

                if (!thisValidNext && !compareValidNext)
                    break; // break & exit with true

                if (!thisValidNext || !compareValidNext)
                {
                    comparisonFailure = String.Format("Invalid last tag in attribute collection");
                    return false;
                }
                DicomAttribute thisAttrib = thisEnumerator.Current;
                DicomAttribute compareAttrib = compareEnumerator.Current;

                if (thisAttrib.Tag.Element == 0x0000)
                {
                    thisValidNext = thisEnumerator.MoveNext();

                    if (!thisValidNext)
                    {
                        comparisonFailure = String.Format("Invalid last tag in attribute collection");
                        return false;
                    }
                    thisAttrib = thisEnumerator.Current;
                }

                if (compareAttrib.Tag.Element == 0x0000)
                {
                    compareValidNext = compareEnumerator.MoveNext();

                    if (!compareValidNext)
                    {
                        comparisonFailure = String.Format("Invalid last tag in attribute collection");
                        return false;
                    }
                    compareAttrib = compareEnumerator.Current;
                }


                if (!thisAttrib.Tag.Equals(compareAttrib.Tag))
                {
                    comparisonFailure =
                        String.Format(
                            "Source tag {0} and comparison message tag {1} not the same, possible missing tag.",
                            thisAttrib.Tag, compareAttrib.Tag);
                    return false;
                }
                if (!thisAttrib.Equals(compareAttrib))
                {
                    if (thisAttrib.StreamLength < 64 && compareAttrib.StreamLength < 64)
                        comparisonFailure =
                            String.Format("Tag {0} values not equal, Base value: '{1}', Comparison value: '{2}'",
                                          thisAttrib.Tag,
                                          thisAttrib, compareAttrib);
                    else
                        comparisonFailure = String.Format("Tag {0} values not equal in message", thisAttrib.Tag);
                    return false;
                }
            }

            comparisonFailure = "DicomAttributeCollections are equal!";
            return true;
        }

        /// <summary>
        /// Override to get a hash code to represent the object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode(); // TODO
        }

        #endregion

        #region Internal Methods
        /// <summary>
        /// Used to calculate group lengths.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="syntax"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal uint CalculateGroupWriteLength(ushort group, TransferSyntax syntax, DicomWriteOptions options)
        {
            uint length = 0;
            foreach (DicomAttribute item in this)
            {
                if (item.Tag.Group < group || item.Tag.Element == 0x0000)
                    continue;
                if (item.Tag.Group > group)
                    return length;
                length += item.CalculateWriteLength(syntax, options);
            }
            return length;
        }

        /// <summary>
        /// Used to calculate the write length of the collection.
        /// </summary>
        /// <param name="syntax"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal uint CalculateWriteLength(TransferSyntax syntax, DicomWriteOptions options)
        {
            uint length = 0;
            ushort group = 0xffff;
            foreach (DicomAttribute item in this)
            {
                if (item.Tag.Element == 0x0000)
                    continue;
                if (item.Tag.Group != group)
                {
                    group = item.Tag.Group;
                    if (Flags.IsSet(options, DicomWriteOptions.CalculateGroupLengths))
                    {
                        if (syntax.ExplicitVr)
                            length += 4 + 2 + 2 + 4;
                        else
                            length += 4 + 4 + 4;
                    }
                }
                length += item.CalculateWriteLength(syntax, options);
            }
            return length;
        }
        #endregion

        #region IEnumerable Implementation

        /// <summary>
        /// Method for implementing the <see cref="IEnumerable"/> interface.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DicomAttribute> GetEnumerator()
        {
            return _attributeList.Values.GetEnumerator();   
        }

        /// <summary>
        /// Method for implementing the <see cref="IEnumerable"/> interface.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Binding
        /// <summary>
        /// Internal method for getting a default value for an attribute.
        /// </summary>
        /// <param name="vtype"></param>
        /// <param name="deflt"></param>
        /// <returns></returns>
        private static object GetDefaultValue(Type vtype, DicomFieldDefault deflt)
        {
            try
            {
                if (deflt == DicomFieldDefault.Null || deflt == DicomFieldDefault.None)
                    return null;
                if (deflt == DicomFieldDefault.DBNull)
                    return DBNull.Value;
                if (deflt == DicomFieldDefault.Default && vtype != typeof(string))
                    return Activator.CreateInstance(vtype);
                if (vtype == typeof(string))
                {
                    if (deflt == DicomFieldDefault.StringEmpty || deflt == DicomFieldDefault.Default)
                        return String.Empty;
                }
                else if (vtype == typeof(DateTime))
                {
                    if (deflt == DicomFieldDefault.DateTimeNow)
                        return DateTime.Now;
                    if (deflt == DicomFieldDefault.MinValue)
                        return DateTime.MinValue;
                    if (deflt == DicomFieldDefault.MaxValue)
                        return DateTime.MaxValue;
                }
                else if (vtype.IsSubclassOf(typeof(ValueType)))
                {
                    if (deflt == DicomFieldDefault.MinValue)
                    {
                        PropertyInfo pi = vtype.GetProperty("MinValue", BindingFlags.Static);
                        if (pi != null) return pi.GetValue(null, null);
                    }
                    if (deflt == DicomFieldDefault.MaxValue)
                    {
                        PropertyInfo pi = vtype.GetProperty("MaxValue", BindingFlags.Static);
                        if (pi != null) return pi.GetValue(null, null);
                    }
                    return Activator.CreateInstance(vtype);
                }
                return null;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Error in default value type! - {0}", vtype.ToString());
                return null;
            }
        }

        private object LoadDicomFieldValue(DicomAttribute elem, Type vtype, DicomFieldDefault deflt, bool udzl)
        {
            if (vtype.IsSubclassOf(typeof(DicomAttribute)))
            {
                if (elem != null && vtype != elem.GetType())
                    throw new DicomDataException("Invalid binding type for Element VR!");
                return elem;
            }
            else if (vtype.IsArray)
            {
                if (elem != null)
                {
                    if (vtype.GetElementType() == typeof(float) && (elem.Tag.VR == DicomVr.DSvr))
                    {
                        float[] array = new float[elem.Count];
                        for (int i = 0; i < array.Length; i++)
                        {
                             elem.TryGetFloat32(i, out array[i]);
                        }
                        return array;
                    }
                    else if (vtype.GetElementType() == typeof(double) && (elem.Tag.VR == DicomVr.DSvr))
                    {
                        double[] array = new double[elem.Count];
                        for (int i = 0; i < array.Length; i++)
                            elem.TryGetFloat64(i, out array[i]);

                        return array;
                    }
                    
                    if (vtype.GetElementType() != elem.GetValueType())
                        throw new DicomDataException("Invalid binding type for Element VR!");
                    //if (elem.GetValueType() == typeof(DateTime))
                    //    return (elem as AbstractAttribute).GetDateTimes();
                    else
                        return elem.Values;
                }
                else
                {
                    if (deflt == DicomFieldDefault.EmptyArray)
                        return Array.CreateInstance(vtype, 0);
                    else
                        return null;
                }
            }
            else
            {
                if (elem != null)
                {
                    if (elem.StreamLength == 0 && udzl)
                    {
                        return GetDefaultValue(vtype, deflt);
                    }
					if (vtype == typeof(string))
                    {
                        return elem.ToString();
                    }

					Type nullableType;
					if (null != (nullableType = Nullable.GetUnderlyingType(vtype)) || vtype.IsValueType)
					{
						bool isNullable = nullableType != null;
						Type valueType = nullableType ?? vtype;

						if (valueType == typeof (ushort))
						{
							ushort value;
							if (!elem.TryGetUInt16(0, out value) && isNullable)
								return null;
							return value;
						}
						else if (valueType == typeof (short))
						{
							short value;
							if (!elem.TryGetInt16(0, out value) && isNullable)
								return null;
							return value;
						}
						else if (valueType == typeof (uint))
						{
							uint value;
							if (!elem.TryGetUInt32(0, out value) && isNullable)
								return null;
							return value;
						}
						else if (valueType == typeof (int))
						{
							int value;
							if (!elem.TryGetInt32(0, out value) && isNullable)
								return null;
							return value;
						}
						else if (valueType == typeof(UInt64))
						{
							UInt64 value;
							if (!elem.TryGetUInt64(0, out value) && isNullable)
								return null;
							return value;
						}
						else if (valueType == typeof(Int64))
						{
							Int64 value;
							if (!elem.TryGetInt64(0, out value) && isNullable)
								return null;
							return value;
						}
						else if (valueType == typeof(float))
						{
							float value;
							if (!elem.TryGetFloat32(0, out value) && isNullable)
								return null;

							return value;
						}
						else if (valueType == typeof (double))
						{
							double value;
							if (!elem.TryGetFloat64(0, out value) && isNullable)
								return null;
							return value;
						}
						else if (valueType == typeof(DateTime))
						{
							DateTime value;
							if (!elem.TryGetDateTime(0, out value) && isNullable)
								return null;
							return value;
						}
					}
                	
					if (vtype != elem.GetValueType())
                    {
                        if (vtype == typeof(DicomUid) && elem.Tag.VR == DicomVr.UIvr)
                        {
                            DicomUid uid;
                            elem.TryGetUid(0, out uid);
                            return uid;
                        }
                        else if (vtype == typeof(TransferSyntax) && elem.Tag.VR == DicomVr.UIvr)
                        {
                            return TransferSyntax.GetTransferSyntax(elem.ToString());
                        }
                        //else if (vtype == typeof(DcmDateRange) && elem.GetType().IsSubclassOf(typeof(AttributeMultiValueText)))
                        //{
                        //    return (elem as AbstractAttribute).GetDateTimeRange();
                        // }
                        else if (vtype == typeof(object))
                        {
                            return elem.Values;
                        }
                        else
                            throw new DicomDataException("Invalid binding type for Element VR!");
                    }
                    else
                    {
                        return elem.Values;
                    }
                }
                else
                {
                    return GetDefaultValue(vtype, deflt);
                }
            }
        }

        /// <summary>
        /// Load the contents of attributes in the collection into a structure or class.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method will use reflection to look at the contents of the object specified by
        /// <paramref name="obj"/> and copy the values of attributes within this collection to
        /// fields in the object with the <see cref="DicomFieldAttribute"/> attribute set for
        /// them.
        /// </para>
        /// </remarks>
        /// <param name="obj"></param>
        /// <seealso cref="DicomFieldAttribute"/>
        public void LoadDicomFields(object obj)
        {
            FieldInfo[] fields = obj.GetType().GetFields();
            foreach (FieldInfo field in fields)
            {
                if (field.IsDefined(typeof(DicomFieldAttribute), true))
                {
                    try
                    {
                        DicomFieldAttribute dfa = (DicomFieldAttribute)field.GetCustomAttributes(typeof(DicomFieldAttribute), true)[0];
                        if ((dfa.Tag.TagValue >= StartTagValue) && (dfa.Tag.TagValue <= this.EndTagValue))
                        {
                            if (Contains(dfa.Tag))
                            {
                                DicomAttribute elem = this[dfa.Tag];
                                if ((elem.StreamLength == 0 && dfa.UseDefaultForZeroLength) &&
                                    dfa.DefaultValue == DicomFieldDefault.None)
                                {
                                    // do nothing
                                }
                                else
                                {
                                    field.SetValue(obj,
                                                   LoadDicomFieldValue(elem, field.FieldType, dfa.DefaultValue,
                                                                       dfa.UseDefaultForZeroLength));
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e,"Unable to bind field");
                    }
                }
            }

            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.IsDefined(typeof(DicomFieldAttribute), true))
                {
                    try
                    {
                        DicomFieldAttribute dfa = (DicomFieldAttribute)property.GetCustomAttributes(typeof(DicomFieldAttribute), true)[0];
                        if (Contains(dfa.Tag))
                        {
                            DicomAttribute elem = this[dfa.Tag];
                            if ((elem.StreamLength == 0 && dfa.UseDefaultForZeroLength) && dfa.DefaultValue == DicomFieldDefault.None)
                            {
                                // do nothing
                            }
                            else
                            {
                                property.SetValue(obj, LoadDicomFieldValue(elem, property.PropertyType, dfa.DefaultValue, dfa.UseDefaultForZeroLength), null);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e,"Unable to bind field");
                    }
                }
            }
        }

        private void SaveDicomFieldValue(DicomTag tag, object value, bool createEmpty, bool setNullIfEmpty)
        {
            if (value != null && value != DBNull.Value)
            {
                Type vtype = value.GetType();
                if (vtype.IsSubclassOf(typeof(DicomSequenceItem)))
                {
                    DicomAttribute elem = this[tag];
                    elem.AddSequenceItem((DicomSequenceItem)value);
                }
                else
                {
                    DicomAttribute elem = this[tag];
                    if (vtype.IsArray)
                    {
                        if (vtype.GetElementType() != elem.GetValueType())
                            throw new DicomDataException("Invalid binding type for Element VR!");
//                        if (elem.GetValueType() == typeof(DateTime))
  //                          (elem as AbstractAttribute).SetDateTimes((DateTime[])value);
    //                    else
                            elem.Values = value;
                    }
                    else
                    {
                    	if (elem.Tag.VR == DicomVr.UIvr && vtype == typeof(DicomUid))
                        {
                            DicomUid ui = (DicomUid)value;
                            elem.SetStringValue(ui.UID);
                        }
                        else if (elem.Tag.VR == DicomVr.UIvr && vtype == typeof(TransferSyntax))
                        {
                            TransferSyntax ts = (TransferSyntax)value;
                            elem.SetStringValue(ts.DicomUid.UID);
                        }
                      //  else if (vtype == typeof(DcmDateRange) && elem.GetType().IsSubclassOf(typeof(AbstractAttribute)))
                      //  {
                      //      DcmDateRange dr = (DcmDateRange)value;
                      //      (elem as AbstractAttribute).SetDateTimeRange(dr);
                      //  }
                        else if (vtype != elem.GetValueType())
                        {
							if (vtype == typeof(string))
							{
								elem.SetStringValue((string)value);
							}
							else
							{
								Type nullableType;
								if (null != (nullableType = Nullable.GetUnderlyingType(vtype)) || vtype.IsValueType)
								{
									Type valueType = nullableType ?? vtype;

									if (valueType == typeof (UInt16))
										elem.SetUInt16(0, (UInt16) value);
									else if (valueType == typeof(Int16))
										elem.SetInt16(0, (Int16) value);
									else if (valueType == typeof(UInt32))
										elem.SetUInt32(0, (UInt32) value);
									else if (valueType == typeof(Int32))
										elem.SetInt32(0, (Int32) value);
									else if (valueType == typeof(Int64))
										elem.SetInt64(0, (Int64)value);
									else if (valueType == typeof(UInt64))
										elem.SetUInt64(0, (UInt64)value);
									else if (valueType == typeof(float))
										elem.SetFloat32(0, (float)value);
									else if (valueType == typeof(double))
										elem.SetFloat64(0, (double)value);
									else if (valueType == typeof(DateTime))
										elem.SetDateTime(0, (DateTime)value);
									else
										throw new DicomDataException("Invalid binding type for Element VR!");
								}
								else
									throw new DicomDataException("Invalid binding type for Element VR!");
							}
                        }
                        else
                        {
                            elem.Values = value;
                        }
                    }
                }
            }
            else
            {
				if (createEmpty)
                {
                    // force the element creation
                    DicomAttribute attr = this[tag];
					if (setNullIfEmpty)
						attr.SetNullValue();
                }
                else if (Contains(tag))
                {
                    this[tag].Values = null;
                }
            }
        }

        /// <summary>
        /// This method will copy attributes from the input object into the collection.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="obj">The object to copy values out of into the collection.</param>
        /// <seealso cref="DicomFieldAttribute"/>
        public void SaveDicomFields(object obj)
        {
            FieldInfo[] fields = obj.GetType().GetFields();
            foreach (FieldInfo field in fields)
            {
                if (field.IsDefined(typeof(DicomFieldAttribute), true))
                {
                    DicomFieldAttribute dfa = (DicomFieldAttribute)field.GetCustomAttributes(typeof(DicomFieldAttribute), true)[0];
                    object value = field.GetValue(obj);
                    SaveDicomFieldValue(dfa.Tag, value, dfa.CreateEmptyElement, dfa.SetNullValueIfEmpty);
                }
            }

            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.IsDefined(typeof(DicomFieldAttribute), true))
                {
                    DicomFieldAttribute dfa = (DicomFieldAttribute)property.GetCustomAttributes(typeof(DicomFieldAttribute), true)[0];
                    object value = property.GetValue(obj, null);
					SaveDicomFieldValue(dfa.Tag, value, dfa.CreateEmptyElement, dfa.SetNullValueIfEmpty);
                }
            }
        }
        #endregion

        #region Dump
        /// <summary>
        /// Method to dump the contents of the collection to a <see>StringBuilder</see> instance.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="prefix"></param>
        /// <param name="options"></param>
        public void Dump(StringBuilder sb, String prefix, DicomDumpOptions options)
        {
            if (sb == null) throw new ArgumentNullException("sb");
            foreach (DicomAttribute item in this)
            {
                item.Dump(sb, prefix, options);
                sb.AppendLine();
            }
        }

        /// <summary>
        /// Method to dump the contents of a collection to a string.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public string Dump(string prefix, DicomDumpOptions options)
        {
            StringBuilder sb = new StringBuilder();
            Dump(sb, prefix, options);
            return sb.ToString();
        }

        /// <summary>
        /// Method to dump the contents of a collection to a string.
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public string Dump(string prefix)
        {
            return Dump(prefix, DicomDumpOptions.Default);
        }

        /// <summary>
        /// Method to dump the contents of a collection to a string.
        /// </summary>
        /// <returns></returns>
        public string Dump()
        {
            return Dump(String.Empty, DicomDumpOptions.Default);
        }
        #endregion
    }
}
