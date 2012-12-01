#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Xml.Nodes;

namespace ClearCanvas.Dicom.Utilities.Xml
{
    public class SourceImageInfo
    {
        public string SopInstanceUid { get; set; }
    }

	/// <summary>
	/// Class for representing a SOP Instance as XML.
	/// </summary>
	/// <remarks>
	/// This class may change in a future release.
	/// </remarks>
    public class InstanceXml : IDicomAttributeCollectionProvider
	{
		#region Private members

		private readonly String _sopInstanceUid;
		private readonly SopClass _sopClass;
		private readonly TransferSyntax _transferSyntax;
        private string _sourceAETitle;
        private string _sourceFileName;
		private long _fileSize;

		private BaseInstanceXml _baseInstance;
		private XmlElement _cachedElement;

        private InstanceXmlDicomAttributeCollection _collection;
	    private IDicomAttributeCollectionProvider _baseCollectionGetter;

		private IEnumerator<DicomAttribute> _baseCollectionEnumerator;
		private IEnumerator<INode> _instanceXmlEnumerator;

	    private IList<SourceImageInfo> _sourceImageInfoList;
	    private bool _sourceImageInfoListLoaded;

	    private readonly SeriesXml _seriesXml;

	    public IList<SourceImageInfo> SourceImageInfoList
	    {
	        get
	        {
                if (!_sourceImageInfoListLoaded)
                {
                    _sourceImageInfoList = LoadSourceImageInfo(Collection);
                    _sourceImageInfoListLoaded = true;
                }
	            return _sourceImageInfoList;
	        } 
            private set
            {
                _sourceImageInfoList = value;
            }
	    }

		#endregion

		#region Public Properties

        public bool IsLoaded { get; private set; }

        public String SeriesInstanceUid
        {
            get
            {
                return _seriesXml == null ? "" : _seriesXml.SeriesInstanceUid;
            }
        }
        public String StudyInstanceUid
        {
            get
            {
                return _seriesXml == null ? "" : _seriesXml.StudyInstanceUid;
            }
        }

		public String SopInstanceUid
		{
			get
			{
				return _sopInstanceUid ?? "";
			}
		}

		public SopClass SopClass
		{
			get { return _sopClass; }
		}

		public string SourceFileName
		{
			get { return _sourceFileName; }
			internal set
			{
				if (value != null && Path.IsPathRooted(value))
					value = Path.GetFullPath(value);

				_sourceFileName = value;
			}
		}

	    public string SourceAETitle
	    {
            get { return _sourceAETitle; }
            set { _sourceAETitle = value; }
	    }

	    public long FileSize
		{
			get { return _fileSize; }
			set { _fileSize = value; }
		}

		public TransferSyntax TransferSyntax
		{
			get { return _transferSyntax; }
		}

        private TResult Execute<TResult>(Func<TResult> command)
        {
            if (_seriesXml == null)
            {
                return command();
            }
            using (_seriesXml.Lock())
            {
                return command();
            }
        }
        private void Execute(Action command)
        {
            if (_seriesXml == null)
            {
                command();
            }
            else using (_seriesXml.Lock())
            {
                command();
            }
        }
	    public void Load(INode instanceNode, IDicomAttributeCollectionProvider baseCollectionGetter)
        {
            Action command = () =>
                                 {
                                     if (IsLoaded)
                                         return;

                                     _collection = new InstanceXmlDicomAttributeCollection { ValidateVrValues = false, ValidateVrLengths = false };
                                     _baseCollectionGetter = baseCollectionGetter;

                                     if (instanceNode != null && instanceNode.HasChildNodes)
                                     {
                                         _instanceXmlEnumerator = instanceNode.GetChildEnumerator();
                                         if (!_instanceXmlEnumerator.MoveNext())
                                             _instanceXmlEnumerator = null;
                                     }
                                     IsLoaded = true;
                                 };
            Execute(command);
          


        }
        public void Unload()
        {
            Action command = () =>
                                 {
                                     _collection = null;
                                     _baseCollectionEnumerator = null;
                                     _instanceXmlEnumerator = null;
                                     IsLoaded = false;
                                 };
            Execute(command);

        }


		/// <summary>
		/// Gets the underlying data as a <see cref="DicomAttributeCollection"/>.
		/// </summary>
		/// <remarks>
		/// When parsed from xml, the return type is <see cref="InstanceXmlDicomAttributeCollection"/>, otherwise
		/// it is the source <see cref="DicomAttributeCollection"/>.
		/// </remarks>
		public DicomAttributeCollection Collection
		{
			get
			{
                return Execute(() =>
                {
                    ParseTo(0xffffffff);
                    return _collection;
                });

			}
		}

	    private void LazyCollectionInit()
        {
            //load series
            if (!IsLoaded && _seriesXml != null)
            {
                _seriesXml.Load();
            }

            if (_baseCollectionGetter == null)
                return;
            var baseCollection = _baseCollectionGetter.Collection;
            AddExcludedTagsFromBase(baseCollection);

            _baseCollectionEnumerator = baseCollection.GetEnumerator();
            if (!_baseCollectionEnumerator.MoveNext())
                _baseCollectionEnumerator = null;
            _baseCollectionGetter = null;
        }
		public DicomAttribute this[DicomTag tag]
		{
			get
			{
                return Execute(() =>
			                                       {
			                                           ParseTo(tag.TagValue);
			                                           return _collection[tag.TagValue];
			                                       });
			}
		}

		public DicomAttribute this[uint tag]
		{
			get
			{
                return Execute(() =>
			                                       {
			                                           ParseTo(tag);
			                                           return _collection[tag];
			                                       });
			}
		}

		public bool IsTagExcluded(uint tag)
		{
            return Execute(() =>
		                             {
		                                 if (!(_collection is IInstanceXmlDicomAttributeCollection))
		                                 {
		                                     return false;
		                                 }
		                                 ParseTo(tag);
		                                 return ((IInstanceXmlDicomAttributeCollection) _collection).IsTagExcluded(tag);
		                             });
		}

		#endregion

		#region Constructors

        public InstanceXml(InstanceXmlDicomAttributeCollection collection, SopClass sopClass, TransferSyntax syntax)
		{
			_sopInstanceUid = collection[DicomTags.SopInstanceUid];

			_collection = collection;
			_sopClass = sopClass;
			_transferSyntax = syntax;

			_collection.ValidateVrValues = false;
			_collection.ValidateVrLengths = false;
		}

        private static IList<SourceImageInfo> LoadSourceImageInfo(DicomAttributeCollection collection)
	    {
	        if (collection.Contains(DicomTags.SourceImageSequence))
	        {
	            DicomAttributeSQ sq = collection[DicomTags.SourceImageSequence] as DicomAttributeSQ;
	            IList<SourceImageInfo> list = new List<SourceImageInfo>();
	            foreach(DicomSequenceItem item in sq.Values as DicomSequenceItem[])
	            {
                    list.Add(new SourceImageInfo()
	                                            {SopInstanceUid = item[DicomTags.ReferencedSopInstanceUid].ToString()});

	            }
                return list;
	        }

            return null;
	    }

        public InstanceXml(SeriesXml seriesXml, XmlNode instanceNode, IDicomAttributeCollectionProvider baseCollectionGetter) : 
                                                              this(seriesXml, new XmlNodeWrapper(instanceNode),baseCollectionGetter )
        {
            
        }

	    public InstanceXml(SeriesXml seriesXml, INode instanceNode, IDicomAttributeCollectionProvider baseCollectionGetter)
	    {
	        _seriesXml = seriesXml;
		    Load(instanceNode, baseCollectionGetter);

			if (instanceNode.HasAttribute("UID"))
			{
				_sopInstanceUid = instanceNode.Attribute("UID");
			}

            if (instanceNode.HasAttribute("SourceAETitle"))
            {
                _sourceAETitle = XmlUnescapeString(instanceNode.Attribute("SourceAETitle"));
            }

            if (instanceNode.HasAttribute("SopClassUID") )
			{
				_sopClass = SopClass.GetSopClass(instanceNode.Attribute("SopClassUID"));
			}

            _transferSyntax = instanceNode.HasAttribute("TransferSyntaxUID") 
				? TransferSyntax.GetTransferSyntax(instanceNode.Attribute("TransferSyntaxUID")) 
				: TransferSyntax.ExplicitVrLittleEndian;

            if (instanceNode.HasAttribute("SourceFileName"))
			{
				_sourceFileName = instanceNode.Attribute("SourceFileName");
			}

            if (instanceNode.HasAttribute("FileSize"))
			{
				long.TryParse(instanceNode.Attribute("FileSize"), out _fileSize);
			}

			// This should never happen
			if (_sopClass == null)
			{
				_sopClass = SopClass.GetSopClass(Collection[DicomTags.SopClassUid].GetString(0, String.Empty));
			}

		}


		#endregion

		#region Internal Methods

		internal XmlElement GetMemento(XmlDocument theDocument, StudyXmlOutputSettings settings)
		{
			if (_cachedElement != null)
			{
				return _cachedElement;
			}

			if (_baseInstance != null)
			{
				_cachedElement = GetMementoForCollection(theDocument, _baseInstance.Collection, Collection, settings);

				// Only keep around the cached xml data, free the collection to reduce memory usage
				SwitchToCachedXml();

				return _cachedElement;
			}

			_cachedElement = GetMementoForCollection(theDocument, null, Collection, settings);
			return _cachedElement;
		}

		internal void SetBaseInstance(BaseInstanceXml baseInstance)
		{
			if (_baseInstance != baseInstance)
			{
				// force the element to be regenerated when GetMemento() is called
				_cachedElement = null;
			}

			_baseInstance = baseInstance;
		}

		#endregion


		#region Private Methods

		private void AddExcludedTagsFromBase(DicomAttributeCollection baseCollection)
		{
			if (baseCollection is IPrivateInstanceXmlDicomAttributeCollection)
			{
				if (_collection is IPrivateInstanceXmlDicomAttributeCollection)
				{
					((IPrivateInstanceXmlDicomAttributeCollection)_collection).ExcludedTagsHelper.Add(
						((IPrivateInstanceXmlDicomAttributeCollection)baseCollection).ExcludedTags);
				}
			}
		}

		private static StudyXmlTagInclusion AttributeShouldBeIncluded(DicomAttribute attribute, StudyXmlOutputSettings settings)
		{
			if (settings == null)
				return StudyXmlTagInclusion.IncludeTagValue;

			if (attribute is DicomAttributeSQ)
			{
				if (attribute.Tag.IsPrivate)
					return settings.IncludePrivateValues;
				return StudyXmlTagInclusion.IncludeTagValue;
			}


			// private tag
			if (attribute.Tag.IsPrivate)
			{
				if (settings.IncludePrivateValues != StudyXmlTagInclusion.IncludeTagValue)
					return settings.IncludePrivateValues;
			}

			// check type
			if (attribute is DicomAttributeUN)
			{
				if (settings.IncludeUnknownTags != StudyXmlTagInclusion.IncludeTagValue)
					return settings.IncludeUnknownTags;
			}

			// This check isn't needed, but it bypasses the StreamLength calculation if its not needed
			if (settings.IncludeLargeTags == StudyXmlTagInclusion.IncludeTagValue)
				return settings.IncludeLargeTags;

			// check the size
			ulong length = attribute.StreamLength;
			if (length <= settings.MaxTagLength)
				return StudyXmlTagInclusion.IncludeTagValue;

			// Move here, such that we first check if the tag should be excluded
			if ((attribute is DicomAttributeOB)
			 || (attribute is DicomAttributeOW)
			 || (attribute is DicomAttributeOF)
			 || (attribute is DicomFragmentSequence))
				return StudyXmlTagInclusion.IncludeTagExclusion;

			return settings.IncludeLargeTags;
		}

		private void ParseTo(uint tag)
		{
            LazyCollectionInit();

            //add all base attributes up to "tag" to _collection
			while (_baseCollectionEnumerator != null && _baseCollectionEnumerator.Current.Tag.TagValue <= tag)
			{
				_collection[_baseCollectionEnumerator.Current.Tag] = _baseCollectionEnumerator.Current.Copy();
				if (!_baseCollectionEnumerator.MoveNext())
					_baseCollectionEnumerator = null;
			}
            //add all attributes  up to "tag" to collection
			while (_instanceXmlEnumerator != null)
			{
				var node = _instanceXmlEnumerator.Current;
				String tagString = node.Attribute("Tag");
				uint tagValue;
				if (tagString.StartsWith("$"))
				{
					DicomTag dicomTag = DicomTagDictionary.GetDicomTag(tagString.Substring(1));
					if (dicomTag == null) throw new DicomDataException("Invalid tag name when parsing XML: " + tagString);
					tagValue = dicomTag.TagValue;
				}
				else
				{
					tagValue = uint.Parse(tagString, NumberStyles.HexNumber);
				}

				if (tagValue <= tag)
				{
					ParseAttribute(_collection, node);
					if (!_instanceXmlEnumerator.MoveNext())
						_instanceXmlEnumerator = null;
				}
				else break;
			}
		}

        private static bool ParseCollection(DicomAttributeCollection theCollection, INode theNode)
        {
            if (!theNode.HasChildNodes)
            {
                return false;
            }
            bool rc = false;
            var enumerator = theNode.GetChildEnumerator();
            while (enumerator.MoveNext() && enumerator.Current != null)
            {
                rc = true;
                ParseAttribute(theCollection, enumerator.Current);
            }
            return rc;
        }

        private static void ParseAttribute(DicomAttributeCollection theCollection, INode attributeNode)
        {
            if (attributeNode.Name.Equals("Attribute"))
            {
                var theTag = GetTagFromAttributeNode(attributeNode);

                var attribute = theCollection[theTag];
                if (attribute is DicomAttributeSQ)
                {
                    var attribSQ = (DicomAttributeSQ)attribute;
                    //set the null value in case there are no child nodes.
                    attribute.SetNullValue();

                    if (attributeNode.HasChildNodes)
                    {
                        var enumerator = attributeNode.GetChildEnumerator();
                        while (enumerator.MoveNext() && enumerator.Current != null)
                        {
                            DicomSequenceItem theItem = new InstanceXmlDicomSequenceItem();

                            if (!ParseCollection(theItem, enumerator.Current))
                            {
                                Platform.Log(LogLevel.Warn, "failed to parse collection for sequence tag {0}", theTag);
                            }
                            attribSQ.AddSequenceItem(theItem);


                        }
                    }
                }
                else
                {
                    try
                    {
                        attribute.SetStringValue(attributeNode.Content);
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e,
                                     "Unexpected exception with tag {0} setting value '{1}'.  Ignoring tag value.",
                                     attribute.Tag,
                                     attributeNode.Content);
                    }

                    ((IPrivateInstanceXmlDicomAttributeCollection)theCollection).ExcludedTagsHelper.Remove(theTag);
                }
            }
            else if (attributeNode.Name.Equals("EmptyAttribute"))
            {
                //Means that the tag should not be in this collection, but is in the base.  So, we remove it.
                DicomTag theTag = GetTagFromAttributeNode(attributeNode);

                //NOTE: we want these attributes to be in the collection and empty so that
                //the base collection doesn't need to be modified in order to insert
                //excluded tags in the correct order.
                theCollection[theTag].SetEmptyValue();

                ((IPrivateInstanceXmlDicomAttributeCollection)theCollection).ExcludedTagsHelper.Remove(theTag);
            }
            else if (attributeNode.Name.Equals("ExcludedAttribute"))
            {
                DicomTag theTag = GetTagFromAttributeNode(attributeNode);

                //NOTE: we want these attributes to be in the collection and empty so that
                //the base collection doesn't need to be modified in order to insert
                //excluded tags in the correct order.
                theCollection[theTag].SetEmptyValue();

                ((IPrivateInstanceXmlDicomAttributeCollection)theCollection).ExcludedTagsHelper.Add(theTag);
            }
        }


		private void SwitchToCachedXml()
		{
			// Give to the garbage collector the memory associated with the collection
			_collection = new InstanceXmlDicomAttributeCollection();
			_collection.ValidateVrValues = false;
			_collection.ValidateVrLengths = false;

			if (_baseInstance != null)
			{
				AddExcludedTagsFromBase(_baseInstance.Collection);

				_baseCollectionEnumerator = _baseInstance.Collection.GetEnumerator();
				if (!_baseCollectionEnumerator.MoveNext())
					_baseCollectionEnumerator = null;
			}

			if (!_cachedElement.HasChildNodes)
				return;

            _instanceXmlEnumerator = new XmlChildNodeEnumerator(_cachedElement);
			if (!_instanceXmlEnumerator.MoveNext())
				_instanceXmlEnumerator = null;
		}

		private XmlElement GetMementoForCollection(XmlDocument theDocument, DicomAttributeCollection baseCollection,
												   DicomAttributeCollection collection, StudyXmlOutputSettings settings)
		{
			XmlElement instance;

			if (collection is DicomSequenceItem)
			{
				instance = theDocument.CreateElement("Item");
			}
			else
			{
				instance = theDocument.CreateElement("Instance");

				XmlAttribute sopInstanceUid = theDocument.CreateAttribute("UID");
				sopInstanceUid.Value = _sopInstanceUid;
				instance.Attributes.Append(sopInstanceUid);

				if (_sopClass != null)
				{
					XmlAttribute sopClassAttribute = theDocument.CreateAttribute("SopClassUID");
					sopClassAttribute.Value = _sopClass.Uid;
					instance.Attributes.Append(sopClassAttribute);
				}

				if (_transferSyntax != null && !(this is BaseInstanceXml))
				{
					XmlAttribute transferSyntaxAttribute = theDocument.CreateAttribute("TransferSyntaxUID");
					transferSyntaxAttribute.Value = _transferSyntax.UidString;
					instance.Attributes.Append(transferSyntaxAttribute);
				}

                if (_sourceAETitle != null)
                {
                    XmlAttribute sourceAEAttribute = theDocument.CreateAttribute("SourceAETitle");
                    sourceAEAttribute.Value = XmlEscapeString(_sourceAETitle);
                    instance.Attributes.Append(sourceAEAttribute);
                }
                
                if (_sourceFileName != null && settings.IncludeSourceFileName)
				{
					XmlAttribute sourceFileNameAttribute = theDocument.CreateAttribute("SourceFileName");
					string fileName = SecurityElement.Escape(_sourceFileName);
					sourceFileNameAttribute.Value = fileName;
					instance.Attributes.Append(sourceFileNameAttribute);
				}

				if (_fileSize != 0)
				{
					XmlAttribute fileSize = theDocument.CreateAttribute("FileSize");
					fileSize.Value = _fileSize.ToString();
					instance.Attributes.Append(fileSize);
				}
			}

			IPrivateInstanceXmlDicomAttributeCollection thisCollection = null;
			if (collection is IPrivateInstanceXmlDicomAttributeCollection)
			{
				thisCollection = (IPrivateInstanceXmlDicomAttributeCollection)collection;
				foreach (DicomTag tag in thisCollection.ExcludedTags)
				{
					//Artificially seed the collection with empty attributes from this
					//instance and the base instance so we can add them in the right order.
					//Note in the case of the base instance, this will never alter
					//the collection because the empty attribute is already there (see ParseAttribute).
					DicomAttribute attribute = collection[tag];
				}
			}

			IEnumerator<DicomAttribute> baseIterator = null;
			IPrivateInstanceXmlDicomAttributeCollection privateBaseCollection = null;
			if (baseCollection != null)
			{
				privateBaseCollection = baseCollection as IPrivateInstanceXmlDicomAttributeCollection;
				if (privateBaseCollection != null)
				{
					foreach (DicomTag tag in privateBaseCollection.ExcludedTags)
					{
						//Artificially seed the collection with empty attributes from this
						//instance and the base instance so we can add them in the right order.
						//Note in the case of the base instance, this will never alter
						//the collection because the empty attribute is already there (see ParseAttribute).
						DicomAttribute attribute = collection[tag];
					}
				}

				baseIterator = baseCollection.GetEnumerator();
				if (!baseIterator.MoveNext())
					baseIterator = null;
			}

			List<DicomTag> newlyExcludedTags = new List<DicomTag>();

			foreach (DicomAttribute attribute in collection)
			{
				bool isExcludedFromThisCollection = thisCollection != null && 
					thisCollection.IsTagExcluded(attribute.Tag.TagValue);

				bool isExcludedFromBase = privateBaseCollection != null && 
					privateBaseCollection.ExcludedTagsHelper.IsTagExcluded(attribute.Tag.TagValue);

				bool isInBase = isExcludedFromBase;
				bool isSameAsInBase = isExcludedFromThisCollection && isExcludedFromBase;

				if (baseIterator != null)
				{
					while (baseIterator != null && baseIterator.Current.Tag < attribute.Tag)
					{
						if (!baseIterator.Current.IsEmpty)
						{
							XmlElement emptyAttributeElement = CreateDicomAttributeElement(theDocument, baseIterator.Current, "EmptyAttribute");
							instance.AppendChild(emptyAttributeElement);
						}

						if (!baseIterator.MoveNext())
							baseIterator = null;
					}

					if (baseIterator != null)
					{
						if (baseIterator.Current.Tag == attribute.Tag)
						{
							isInBase = !baseIterator.Current.IsEmpty || isExcludedFromBase;

							bool isEmpty = attribute.IsEmpty && !isExcludedFromThisCollection;
							bool isEmptyInBase = baseIterator.Current.IsEmpty && !isExcludedFromBase;

							isSameAsInBase = (isExcludedFromThisCollection && isExcludedFromBase)
							                 || (isEmpty && isEmptyInBase);

							if (!baseIterator.Current.IsEmpty && !isExcludedFromBase)
							{
								if (!(attribute is DicomAttributeOB)
								    && !(attribute is DicomAttributeOW)
								    && !(attribute is DicomAttributeOF)
								    && !(attribute is DicomFragmentSequence))
								{
									if (attribute.Equals(baseIterator.Current))
										isSameAsInBase = true;
								}
							}

							// Move to the next attribute for the next time through the loop
							if (!baseIterator.MoveNext())
								baseIterator = null;
						}
					}
				}
				
				//by this point, equality has been covered for both attributes with values, empty attributes and excluded attributes.
				if (isSameAsInBase)
					continue;

				if (isExcludedFromThisCollection)
				{
					XmlElement excludedAttributeElement = CreateDicomAttributeElement(theDocument, attribute, "ExcludedAttribute");
					instance.AppendChild(excludedAttributeElement);
					continue;
				}

				if (attribute.IsEmpty)
				{
					//Only store an empty attribute if it is in the base (either non-empty or excluded).
					if (isInBase)
					{
						XmlElement emptyAttributeElement = CreateDicomAttributeElement(theDocument, attribute, "EmptyAttribute");
						instance.AppendChild(emptyAttributeElement);
					}
					continue;
				}

				StudyXmlTagInclusion inclusion = AttributeShouldBeIncluded(attribute, settings);
				if (inclusion == StudyXmlTagInclusion.IncludeTagExclusion)
				{
					newlyExcludedTags.Add(attribute.Tag);
					if (!isExcludedFromBase)
					{
						XmlElement excludedAttributeElement = CreateDicomAttributeElement(theDocument, attribute, "ExcludedAttribute");
						instance.AppendChild(excludedAttributeElement);
					}
					continue;
				}
				if (inclusion == StudyXmlTagInclusion.IgnoreTag)
				{
					continue;
				}

				XmlElement instanceElement = CreateDicomAttributeElement(theDocument, attribute, "Attribute");
				if (attribute is DicomAttributeSQ)
				{
					DicomSequenceItem[] items = (DicomSequenceItem[])attribute.Values;
					foreach (DicomSequenceItem item in items)
					{
						XmlElement itemElement = GetMementoForCollection(theDocument, null, item, settings);

						instanceElement.AppendChild(itemElement);
					}
				}
				else if (attribute is DicomAttributeOW || attribute is DicomAttributeUN)
				{
					byte[] val = (byte[])attribute.Values;

					StringBuilder str = null;
					foreach (byte i in val)
					{
						if (str == null)
							str = new StringBuilder(i.ToString());
						else
							str.AppendFormat("\\{0}", i);
					}
					if (str != null)
						instanceElement.InnerText = str.ToString();
				}
				else if (attribute is DicomAttributeOF)
				{
					float[] val = (float[])attribute.Values;
					StringBuilder str = null;
					foreach (float i in val)
					{
						if (str == null)
							str = new StringBuilder(i.ToString());
						else
							str.AppendFormat("\\{0}", i);
					}
					if (str != null)
						instanceElement.InnerText = str.ToString();
				}
				else
				{
					instanceElement.InnerText = XmlEscapeString(attribute);
				}

				instance.AppendChild(instanceElement);
			}

			//fill in empty attributes past the end of this collection
			while(baseIterator != null)
			{
				if (!baseIterator.Current.IsEmpty)
				{
					XmlElement emptyAttributeElement = CreateDicomAttributeElement(theDocument, baseIterator.Current, "EmptyAttribute");
					instance.AppendChild(emptyAttributeElement);
				}

				if (!baseIterator.MoveNext())
					baseIterator = null;
			}

			if (thisCollection != null)
			{
				//when this is the base collection, 'thisCollection' will never be null.
				//when this is not the base collection, we switch to 'cached xml' right after this, anyway,
				//so the fact that this won't occur is ok.
				foreach (DicomTag tag in newlyExcludedTags)
				{
					collection[tag].SetEmptyValue();
					thisCollection.ExcludedTagsHelper.Add(tag);
				}
			}

			return instance;
		}

		private static DicomTag GetTagFromAttributeNode(INode attributeNode)
		{
			String tag = attributeNode.Attribute("Tag");

			DicomTag theTag;
			if (tag.StartsWith("$"))
			{
				theTag = DicomTagDictionary.GetDicomTag(tag.Substring(1));
			}
			else
			{
				uint tagValue = uint.Parse(tag, NumberStyles.HexNumber);
				theTag = DicomTagDictionary.GetDicomTag(tagValue);
				DicomVr xmlVr = DicomVr.GetVR(attributeNode.Attribute("VR"));
				if (theTag == null)
					theTag = new DicomTag(tagValue, "Unknown tag", "UnknownTag", xmlVr, false, 1, uint.MaxValue, false);

				if (!theTag.VR.Equals(xmlVr))
				{
					theTag = new DicomTag(tagValue, theTag.Name, theTag.VariableName, xmlVr, theTag.MultiVR, theTag.VMLow,
										  theTag.VMHigh, theTag.Retired);
				}
			}
			return theTag;
		}

		private static XmlElement CreateDicomAttributeElement(XmlDocument document, DicomAttribute attribute, string name)
		{
			return CreateDicomAttributeElement(document, attribute.Tag, name);
		}

		private static XmlElement CreateDicomAttributeElement(XmlDocument document, DicomTag dicomTag, string name)
		{
			XmlElement dicomAttributeElement = document.CreateElement(name);

			XmlAttribute tag = document.CreateAttribute("Tag");
			tag.Value = dicomTag.HexString;

			XmlAttribute vr = document.CreateAttribute("VR");
			vr.Value = dicomTag.VR.ToString();

			dicomAttributeElement.Attributes.Append(tag);
			dicomAttributeElement.Attributes.Append(vr);

			return dicomAttributeElement;
		}

		private static string XmlEscapeString(string input)
		{
			string result = input ?? string.Empty;

			result = SecurityElement.Escape(result);

			// Do the regular expression to escape out other invalid XML characters in the string not caught by the above.
			// NOTE: the \x sequences you see below are C# escapes, not Regex escapes
			result = Regex.Replace(result, "[^\x9\xA\xD\x20-\xFFFD]", m => string.Format("&#x{0:X};", (int) m.Value[0]));

			return result;
		}

		private static string XmlUnescapeString(string input)
		{
			string result = input ?? string.Empty;

			// unescape any value-encoded XML entities
			result = Regex.Replace(result, "&#[Xx]([0-9A-Fa-f]+);", m => ((char) int.Parse(m.Groups[1].Value, NumberStyles.AllowHexSpecifier)).ToString());
			result = Regex.Replace(result, "&#([0-9]+);", m => ((char) int.Parse(m.Groups[1].Value)).ToString());

			// unescape any entities encoded by SecurityElement.Escape (only <>'"&)
			result = result.Replace("&lt;", "<").
				Replace("&gt;", ">").
				Replace("&quot;", "\"").
				Replace("&apos;", "'").
				Replace("&amp;", "&");

			return result;
		}

#if UNIT_TESTS
		internal static string TestXmlEscapeString(string input)
		{
			return XmlEscapeString(input);
		}

		internal static string TestXmlUnescapeString(string input)
		{
			return XmlUnescapeString(input);
		}
#endif

		#endregion
	}
}
