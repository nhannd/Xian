using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    /// <summary>
    /// The AttributeCollection class models an a collection of DICOM attributes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class represents a collection of <see cref="AbstractAttribute"/> classes.  It is used by the <see cref="AbstractMessage"/> class to 
    /// represent the meta info and data set of <see cref="DicomFile"/> and <see cref="DicomMessage"/> objects.
    /// </para>
    /// <para>
    /// 
    /// </para>
    /// </remarks>
    public class AttributeCollection : IEnumerable<AbstractAttribute>
    {
        #region Member Variables

        private SortedDictionary<uint, AbstractAttribute> _attributeList = new SortedDictionary<uint, AbstractAttribute>();

        #endregion

        #region Constructors

        /// <summary>
        /// Default constuctor.
        /// </summary>
        public AttributeCollection()
        {
        }

        /// <summary>
        /// Internal constructor used when creating a copy of an AttributeCollection.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="copyBinary"></param>
        internal AttributeCollection(AttributeCollection source, bool copyBinary)
        {
            foreach (AbstractAttribute attrib in source)
            {
                if (copyBinary ||
                      (!(attrib is AttributeOB)
                    && !(attrib is AttributeOW)
                    && !(attrib is AttributeOF)
                    && !(attrib is AttributeUN)))
                {
                    this[attrib.Tag] = attrib.Copy();
                }
            }
        }

        #endregion

        #region Public Methods

        public bool Contains(uint tag)
        {
            return _attributeList.ContainsKey(tag);
        }

        public bool Contains(DicomTag tag)
        {
            return _attributeList.ContainsKey(tag.TagValue);
        }

        /// <summary>
        /// Indexer to return a specific tag in the attribute collection.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public AbstractAttribute this[uint tag]
        {
            get 
            {
                AbstractAttribute attr = null;

                if (!_attributeList.ContainsKey(tag))
                {
                    attr = AbstractAttribute.NewAttribute(tag);
             
                    if (attr == null)
                    {
                        throw new DicomException("Invalid tag: " + tag.ToString());// TODO:  Hex formating
                    }
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
                    _attributeList.Remove(tag);
                }
                else
                {
                    if (value.Tag.TagValue != tag)
                        throw new DicomException("Tag being set does not match tag in AbstractAttribute");
                    _attributeList[tag] = value;
                    
                }
            }
        }

        /// <summary>
        /// Indexer when retrieving a specific tag in the collection.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public AbstractAttribute this[DicomTag tag]
        {
            get
            {
                AbstractAttribute attr = null;

                if (!_attributeList.ContainsKey(tag.TagValue))
                {
                    attr = AbstractAttribute.NewAttribute(tag);
                    if (attr == null)
                    {
                        throw new DicomException("Invalid tag: " + tag.HexString);// TODO:  Hex formating
                    }
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
                    _attributeList.Remove(tag.TagValue);
                }
                else
                {
                    if (value.Tag.TagValue != tag.TagValue)
                        throw new DicomException("Tag being set does not match tag in AbstractAttribute");
     
                    _attributeList[tag.TagValue] = value;
                }
            }
        }

        /// <summary>
        /// Create a duplicate copy of the AttributeCollection.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method creates a copy of all of the attributes within the AttributeCollection and returns 
        /// a new copy.  Note that binary attributes with a VR of OB, OW, OF, and UN are copied.
        /// </para>
        /// </remarks>
        /// <returns>A new AttributeCollection.</returns>
        public virtual AttributeCollection Copy()
        {
            return Copy(true);
        }

        public virtual AttributeCollection Copy(bool copyBinary)
        {
            return new AttributeCollection(this, copyBinary);
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            AttributeCollection a = (AttributeCollection)obj;

            IEnumerator<AbstractAttribute> thisEnumerator = GetEnumerator();
            IEnumerator<AbstractAttribute> compareEnumerator = a.GetEnumerator();

            for (; ; )
            {
                bool thisValidNext = thisEnumerator.MoveNext();
                bool compareValidNext = compareEnumerator.MoveNext();

                if (!thisValidNext && !compareValidNext)
                    break; // break & exit with true

                if (!thisValidNext || !compareValidNext)
                    return false;

                AbstractAttribute thisAttrib = thisEnumerator.Current;
                AbstractAttribute compareAttrib = compareEnumerator.Current;

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
            foreach (AbstractAttribute item in this)
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
            foreach (AbstractAttribute item in this)
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

        public IEnumerator<AbstractAttribute> GetEnumerator()
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
                DicomLogger.LogError("Error in default value type! - {0}", vtype.ToString() + "  Exception: " + e.ToString());
                return null;
            }
        }

        private object LoadDicomFieldValue(AbstractAttribute elem, Type vtype, DicomFieldDefault deflt, bool udzl)
        {
            if (vtype.IsSubclassOf(typeof(AbstractAttribute)))
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
                            array[i] = elem.GetFloat32(i);

                        return array;
                    }
                    else if (vtype.GetElementType() == typeof(double) && (elem.Tag.VR == DicomVr.DSvr))
                    {
                        double[] array = new double[elem.Count];
                        for (int i = 0; i < array.Length; i++)
                            array[i] = elem.GetFloat64(i);

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
                        return elem.GetUInt16(0);
                    }
                    else if (vtype == typeof(short))
                    {
                        return elem.GetInt16(0);
                    }
                    else if (vtype == typeof(uint))
                    {
                        return elem.GetUInt32(0);
                    }
                    else if (vtype == typeof(int))
                    {
                        return elem.GetInt32(0);
                    }
                    else if (vtype == typeof(float))
                    {
                        return elem.GetFloat32(0);
                    }
                    else if (vtype == typeof(double))
                    {
                        return elem.GetFloat64(0);
                    }
                    if (vtype != elem.GetValueType())
                    {
                        if (vtype == typeof(DicomUid) && elem.Tag.VR == DicomVr.UIvr)
                        {
                            return (elem as AttributeUI).GetUid(0);
                        }
                        else if (vtype == typeof(TransferSyntax) && elem.Tag.VR == DicomVr.UIvr)
                        {
                            return TransferSyntax.GetTransferSyntax(elem.ToString());
                        }
                        else if (vtype == typeof(DateTime))
                        {
                            if (elem.Tag.VR == DicomVr.DAvr)
                                return DateTime.ParseExact(elem.ToString(), "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault);
                            else if (elem.Tag.VR == DicomVr.TMvr)
                                return DateTime.ParseExact(elem.ToString(), "HHmmSS.FFFFFF", CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault);
                            else if (elem.Tag.VR == DicomVr.DTvr)
                                return DateTime.ParseExact(elem.ToString(), "yyyyMMddHHmmss.FFFFFF&ZZZZ", CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault);

                            return DateTime.Parse(elem.ToString());
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
                        AbstractAttribute elem = this[dfa.Tag];
                        if ((elem == null || (elem.StreamLength == 0 && dfa.UseDefaultForZeroLength)) && dfa.DefaultValue == DicomFieldDefault.None)
                        {
                            // do nothing
                        }
                        else
                        {
                            field.SetValue(obj, LoadDicomFieldValue(elem, field.FieldType, dfa.DefaultValue, dfa.UseDefaultForZeroLength));
                        }
                    }
                    catch (Exception e)
                    {
                        DicomLogger.LogError("Unable to bind field: " + e.Message);
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
                        AbstractAttribute elem = this[dfa.Tag];
                        if ((elem == null || (elem.StreamLength == 0 && dfa.UseDefaultForZeroLength)) && dfa.DefaultValue == DicomFieldDefault.None)
                        {
                            // do nothing
                        }
                        else
                        {
                            property.SetValue(obj, LoadDicomFieldValue(elem, property.PropertyType, dfa.DefaultValue, dfa.UseDefaultForZeroLength), null);
                        }
                    }
                    catch (Exception e)
                    {
                        DicomLogger.LogError("Unable to bind field: " + e.Message);
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
                    AbstractAttribute elem = this[tag];
                    elem.AddSequenceItem((DicomSequenceItem)value);
                }
                else
                {
                    AbstractAttribute elem = this[tag];
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
                            elem.SetStringValue(ts.UID.UID);
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
                    AbstractAttribute elem = this[tag];
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
            foreach (AbstractAttribute item in this)
            {
                item.Dump(sb, prefix, DicomDumpOptions.Default);
                sb.AppendLine();
            }
        }
        #endregion
    }
}
