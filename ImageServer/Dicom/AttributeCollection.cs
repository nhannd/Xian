using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Offis;

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
        private OffisDcmItem _offisDataset = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constuctor.
        /// </summary>
        public AttributeCollection()
        {
        }

        /// <summary>
        /// Internal constructor used when marshalling data from the Offis tool kit.
        /// </summary>
        /// <param name="theSet"></param>
        /// <param name="fileFormat"></param>
        internal AttributeCollection(DcmItem theSet, DcmFileFormat fileFormat)
        {
            _offisDataset = new OffisDcmItem(theSet, fileFormat);
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

                //TODO -- change to check if Key Exists

                if (!_attributeList.ContainsKey(tag))
                {
                    if (_offisDataset == null)
                        attr = AbstractAttribute.NewAttribute(tag);
                    else
                        attr = AbstractAttribute.NewAttribute(tag, _offisDataset);
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

                    if (this._offisDataset != null)
                    {
                        DcmTagKey offisTag = new DcmTagKey((ushort)((tag& 0xffff0000) >> 16), (ushort) (tag & 0x0000ffff));

                        this._offisDataset.Item.findAndDeleteElement(offisTag);
                    }
                }
                else
                {
                    value.Dirty = true;
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
                    if (_offisDataset == null)
                        attr = AbstractAttribute.NewAttribute(tag);
                    else
                        attr = AbstractAttribute.NewAttribute(tag, _offisDataset);
                    if (attr == null)
                    {
                        throw new DicomException("Invalid tag: " + tag.ToString());// TODO:  Hex formating
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

                    if (this._offisDataset != null)
                    {
                        DcmTagKey offisTag = new DcmTagKey(tag.Group, tag.Element);

                        this._offisDataset.Item.findAndDeleteElement(offisTag);
                    }
                }
                else
                {
                    value.Dirty = true;
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

        /// <summary>
        /// Method to traverse through all the tags in the Offis message and force them to be loaded.
        /// </summary>
        internal void LoadAttributes()
        {
            if (_offisDataset == null)
                return;

            string tagName;
            ushort group;
            ushort element;
            DcmVR vr = null;
            string vrName;

            DcmTag tag = null;
            DcmObject container = _offisDataset.Item;
            DcmObject obj = _offisDataset.Item.nextInContainer(null);
            
            /*
             * We first traverse through the list of tags & put the Tag objects into our own list,
             * then we go and actually extract each of the attributes.  This is because when we
             * import some of the attributes in the message, we delete them out of the source
             * message (specifically for pixel data), which causes the nextInContainer call to 
             * terminate
             */
            List<DicomTag> tagList = new List<DicomTag>();
            while (obj != null)
            {
                tag = obj.getTag();
                group = tag.getGroup();
                element = tag.getElement();
                vr = tag.getVR();
                vrName = vr.getVRName();

                DicomTag ccTag = DicomTagDictionary.Instance[group, element];

                if (ccTag == null)
                {
                    tagName = tag.getTagName();
                    ccTag = new DicomTag((uint)group << 16 | (uint)element, tagName, DicomVr.GetVR(vrName), true, 1, uint.MaxValue, false);
                }
                else
                {
                    if (!ccTag.VR.Name.Equals(vrName))
                    {
                        // TODO:  A prime tag here are group length tags, they are currently not in the dictionary.

                        //if (!ccTag.MultiVR)
                            //TODO: Log something

                        // TODO:  Need some better logic here
                        ccTag = new DicomTag((uint)group << 16 | (uint)element, ccTag.Name, DicomVr.GetVR(vrName), true, 1, uint.MaxValue, false);
                     
                    }
                }

                tagList.Add(ccTag);


                obj = container.nextInContainer(obj);
            }

            foreach( DicomTag ccTag in tagList)
            {
                // This forces a load from Offis
                AbstractAttribute attr = this[ccTag];
            }

        }

        internal void FlushDirtyAttributes()
        {
            foreach (AbstractAttribute attr in this)
            {
                if (attr.Dirty)
                {
                    attr.FlushAttribute(_offisDataset);
                }
            }
        }

        #endregion

        #region Internal Properties

        internal OffisDcmItem OffisDataset
        {
            get { return _offisDataset; }
            set { _offisDataset = value; }
        }

        #endregion

        #region IEnumerable Implementation

        public IEnumerator<AbstractAttribute> GetEnumerator()
        {
            LoadAttributes();

            return _attributeList.Values.GetEnumerator();   
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
