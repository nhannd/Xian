using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

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
    public class DicomAttributeCollection : IEnumerable<DicomAttribute>
    {
        #region Member Variables

        private SortedDictionary<uint, DicomAttribute> _attributeList = new SortedDictionary<uint, DicomAttribute>();
        private String _specificCharacterSet = "";
        private uint _startTag = 0x00000000;
        private uint _stopTag = 0xFFFFFFFF;       
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
        /// <param name="startTag"></param>
        /// <param name="stopTag"></param>
        public DicomAttributeCollection(uint startTag, uint stopTag)
        {
            _startTag = startTag;
            _stopTag = stopTag;
        }

        /// <summary>
        /// Internal constructor used when creating a copy of an DicomAttributeCollection.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="copyBinary"></param>
        internal DicomAttributeCollection(DicomAttributeCollection source, bool copyBinary)
        {
            foreach (DicomAttribute attrib in source)
            {
                if (copyBinary ||
                      (!(attrib is DicomAttributeOB)
                    && !(attrib is DicomAttributeOW)
                    && !(attrib is DicomAttributeOF)
                    && !(attrib is DicomAttributeUN)))
                {
                    this[attrib.Tag] = attrib.Copy();
                }
            }
        }

        #endregion

        #region Public Properties
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
            if (!_attributeList.ContainsKey(tag))
                return false;

            return !this[tag].IsEmpty;
        }

        /// <summary>
        /// Check if a tag is contained in an DicomAttributeCollection and has a value.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>

        public bool Contains(DicomTag tag)
        {
            if (!_attributeList.ContainsKey(tag.TagValue))
                return false;

            return !this[tag].IsEmpty;
        }

        /// <summary>
        /// Indexer to return a specific tag in the attribute collection.
        /// </summary>
        /// <remarks>
        /// When setting, if the value is null, the tag will be removed from the collection.
        /// </remarks>
        /// <param name="tag">The tag to look for.</param>
        /// <returns></returns>
        public DicomAttribute this[uint tag]
        {
            get 
            {
                DicomAttribute attr = null;

                if (!_attributeList.ContainsKey(tag))
                {
                    if ((tag < _startTag) || (tag > _stopTag))
                        throw new DicomException("Tag is out of range for collection: " + tag.ToString());

                    DicomTag dicomTag = DicomTagDictionary.GetDicomTag(tag);

                    if (dicomTag == null)
                    {
                        throw new DicomException("Invalid tag: " + tag.ToString());// TODO:  Hex formating
                    }

                    attr = dicomTag.CreateDicomAttribute();
                    attr.ParentCollection = this;
                    _attributeList[tag] = attr;
                }
                else 
                    attr = _attributeList[tag];


                return attr; 
            }
            set 
            {
                if (value == null)
                {
                    if (_attributeList.ContainsKey(tag))
                    {
                        DicomAttribute attr = _attributeList[tag];
                        attr.ParentCollection = null;
                        _attributeList.Remove(tag);
                    }
                }
                else
                {
                    if ((tag < _startTag) || (tag > _stopTag))
                        throw new DicomException("Tag is out of range for collection: " + tag.ToString());

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
        /// <param name="tag"></param>
        /// <returns></returns>
        public DicomAttribute this[DicomTag tag]
        {
            get
            {
                DicomAttribute attr = null;

                if (!_attributeList.ContainsKey(tag.TagValue))
                {
                    if ((tag.TagValue < _startTag) || (tag.TagValue > _stopTag))
                        throw new DicomException("Tag is out of range for collection: " + tag.ToString());

                    attr = tag.CreateDicomAttribute();
                    if (attr == null)
                    {
                        throw new DicomException("Invalid tag: " + tag.HexString);// TODO:  Hex formating
                    }
                    attr.ParentCollection = this;
                    _attributeList[tag.TagValue] = attr;
                }
                else
                    attr = _attributeList[tag.TagValue];

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

                    if ((tag.TagValue < _startTag) || (tag.TagValue > _stopTag))
                        throw new DicomException("Tag is out of range for collection: " + tag.ToString());

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

        public virtual DicomAttributeCollection Copy(bool copyBinary)
        {
            return new DicomAttributeCollection(this, copyBinary);
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            DicomAttributeCollection a = (DicomAttributeCollection)obj;

            IEnumerator<DicomAttribute> thisEnumerator = GetEnumerator();
            IEnumerator<DicomAttribute> compareEnumerator = a.GetEnumerator();

            for (; ; )
            {
                bool thisValidNext = thisEnumerator.MoveNext();
                bool compareValidNext = compareEnumerator.MoveNext();

                if (!thisValidNext && !compareValidNext)
                    break; // break & exit with true

                if (!thisValidNext || !compareValidNext)
                    return false;

                DicomAttribute thisAttrib = thisEnumerator.Current;
                DicomAttribute compareAttrib = compareEnumerator.Current;

                if (thisAttrib.Tag.Element == 0x0000)
                {
                    thisValidNext = thisEnumerator.MoveNext();

                    if (!thisValidNext && !compareValidNext)
                        break; // break & exit with true

                    if (!thisValidNext || !compareValidNext)
                        return false;

                    thisAttrib = thisEnumerator.Current;
                }

                if (compareAttrib.Tag.Element == 0x0000)
                {
                    compareValidNext = compareEnumerator.MoveNext();

                    if (!thisValidNext && !compareValidNext)
                        break; // break & exit with true

                    if (!thisValidNext || !compareValidNext)
                        return false;

                    compareAttrib = compareEnumerator.Current;
                }


                if (!thisAttrib.Tag.Equals(compareAttrib.Tag))
                    return false;
                if (!thisAttrib.Equals(compareAttrib))
                    return false;
            }
           
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode(); // TODO
        }

        #endregion

        #region Internal Methods


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

        public IEnumerator<DicomAttribute> GetEnumerator()
        {
            return _attributeList.Values.GetEnumerator();   
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Binding
        private object GetDefaultValue(Type vtype, DicomFieldDefault deflt)
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
                DicomLogger.LogErrorException(e,"Error in default value type! - {0}", vtype.ToString());
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
                    else if (vtype == typeof(ushort))
                    {
                        ushort value;
                        elem.TryGetUInt16(0, out value);
                        return value;
                    }
                    else if (vtype == typeof(short))
                    {
                        short value;
                        elem.TryGetInt16(0, out value);
                        return value;
                    }
                    else if (vtype == typeof(uint))
                    {
                        uint value;
                        elem.TryGetUInt32(0, out value);
                        return value;
                    }
                    else if (vtype == typeof(int))
                    {
                        int value;
                        elem.TryGetInt32(0, out value);
                        return value;
                    }
                    else if (vtype == typeof(float))
                    {
                        float value;
                        elem.TryGetFloat32(0, out value);
                        return value;
                    }
                    else if (vtype == typeof(double))
                    {
                        double value;
                        elem.TryGetFloat64(0, out value);
                        return value;
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
                        else if (vtype == typeof(DateTime))
                        {
                            DateTime dt;
                            elem.TryGetDateTime(0, out dt);
                            return dt;
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
                        if (this.Contains(dfa.Tag))
                        {
                            DicomAttribute elem = this[dfa.Tag];
                            if ((elem.StreamLength == 0 && dfa.UseDefaultForZeroLength) && dfa.DefaultValue == DicomFieldDefault.None)
                            {
                                // do nothing
                            }
                            else
                            {
                                field.SetValue(obj, LoadDicomFieldValue(elem, field.FieldType, dfa.DefaultValue, dfa.UseDefaultForZeroLength));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        DicomLogger.LogErrorException(e,"Unable to bind field");
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
                        if (this.Contains(dfa.Tag))
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
                        DicomLogger.LogErrorException(e,"Unable to bind field");
                    }
                }
            }
        }

        private void SaveDicomFieldValue(DicomTag tag, object value, bool createEmpty)
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
                            elem.Values = (object[])value;
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
                                throw new DicomDataException("Invalid binding type for Element VR!");
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
                if (Contains(tag))
                {
                    this[tag].Values = null;
                }
                else if (createEmpty)
                {
                    DicomAttribute elem = this[tag];
                }
            }
        }

        public void SaveDicomFields(object obj)
        {
            FieldInfo[] fields = obj.GetType().GetFields();
            foreach (FieldInfo field in fields)
            {
                if (field.IsDefined(typeof(DicomFieldAttribute), true))
                {
                    DicomFieldAttribute dfa = (DicomFieldAttribute)field.GetCustomAttributes(typeof(DicomFieldAttribute), true)[0];
                    object value = field.GetValue(obj);
                    SaveDicomFieldValue(dfa.Tag, value, dfa.CreateEmptyElement);
                }
            }

            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.IsDefined(typeof(DicomFieldAttribute), true))
                {
                    DicomFieldAttribute dfa = (DicomFieldAttribute)property.GetCustomAttributes(typeof(DicomFieldAttribute), true)[0];
                    object value = property.GetValue(obj, null);
                    SaveDicomFieldValue(dfa.Tag, value, dfa.CreateEmptyElement);
                }
            }
        }
        #endregion

        #region Dump
        public void Dump()
        {
            StringBuilder sb = new StringBuilder();
            Dump(sb, "", DicomDumpOptions.Default);
            Console.WriteLine(sb.ToString());
        }

        public void Dump(StringBuilder sb, String prefix, DicomDumpOptions options)
        {
            foreach (DicomAttribute item in this)
            {
                item.Dump(sb, prefix, options);
                sb.AppendLine();
            }
        }
        #endregion
    }
}
