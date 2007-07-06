using System;
using System.Collections;
using System.Collections.Generic;
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
