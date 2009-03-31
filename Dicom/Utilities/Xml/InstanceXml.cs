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
using System.Collections.ObjectModel;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.Utilities.Xml
{
	#region InstanceXml DicomAttribute stuff

	//TODO: this is not ideal, but is the most straightforward given the current study xml design.  Later,
	//we should refactor for a cleaner API.
	public interface IInstanceXmlDicomAttributeCollection : IDicomAttributeProvider, IEnumerable<DicomAttribute>
	{
		ReadOnlyCollection<uint> ExcludedTags { get; }

		bool HasExcludedTags(bool recursive);
	}

	internal interface IPrivateInstanceXmlDicomAttributeCollection : IInstanceXmlDicomAttributeCollection
	{
		ExcludedTagsHelper ExcludedTagsHelper { get; }
	}

	internal class ExcludedTagsHelper
	{
		private readonly IInstanceXmlDicomAttributeCollection _parent;

		private readonly ReadOnlyCollection<uint> _readOnlyExcludedTags;
		private readonly List<uint> _excludedTags;

		public ExcludedTagsHelper(IInstanceXmlDicomAttributeCollection parent)
		{
			_parent = parent;
			_excludedTags = new List<uint>();
			_readOnlyExcludedTags = new ReadOnlyCollection<uint>(_excludedTags);
		}

		public ReadOnlyCollection<uint> ExcludedTags
		{
			get { return _readOnlyExcludedTags; }
		}

		public void Add(uint tag)
		{
			if (!_excludedTags.Contains(tag))
				_excludedTags.Add(tag);
		}

		public bool HasExcludedTags(bool recursive)
		{
			if (ExcludedTags.Count > 0)
				return true;

			if (recursive)
			{
				foreach (DicomAttribute attribute in _parent)
				{
					if (attribute is DicomAttributeSQ)
					{
						DicomSequenceItem[] items = attribute.Values as DicomSequenceItem[];
						if (items != null)
						{
							foreach (DicomSequenceItem item in items)
							{
								if (item is InstanceXmlDicomSequenceItem)
								{
									if (((InstanceXmlDicomSequenceItem)item).HasExcludedTags(recursive))
										return true;
								}
							}
						}
					}
				}
			}

			return false;
		}
	}

	public class InstanceXmlDicomAttributeCollection : DicomAttributeCollection, IPrivateInstanceXmlDicomAttributeCollection
	{
		private readonly ExcludedTagsHelper _excludedTagsHelper;

		internal InstanceXmlDicomAttributeCollection()
		{
			_excludedTagsHelper = new ExcludedTagsHelper(this);
		}

		#region IInstanceXmlDicomAttributeCollection Members

		public ReadOnlyCollection<uint> ExcludedTags
		{
			get { return _excludedTagsHelper.ExcludedTags; }
		}

		public bool HasExcludedTags(bool recursive)
		{
			return _excludedTagsHelper.HasExcludedTags(recursive);
		}

		#endregion

		#region IInternalInstanceXmlDicomAttributeCollection Members

		ExcludedTagsHelper IPrivateInstanceXmlDicomAttributeCollection.ExcludedTagsHelper
		{
			get { return _excludedTagsHelper; }
		}

		#endregion
	}

	public class InstanceXmlDicomSequenceItem : DicomSequenceItem, IPrivateInstanceXmlDicomAttributeCollection
	{
		private readonly ExcludedTagsHelper _excludedTagsHelper;

		internal InstanceXmlDicomSequenceItem()
		{
			_excludedTagsHelper = new ExcludedTagsHelper(this);
		}

		#region IInstanceXmlDicomAttributeCollection Members

		public ReadOnlyCollection<uint> ExcludedTags
		{
			get { return _excludedTagsHelper.ExcludedTags; }
		}

		public bool HasExcludedTags(bool recursive)
		{
			return _excludedTagsHelper.HasExcludedTags(recursive);
		}

		#endregion

		#region IInternalInstanceXmlDicomAttributeCollection Members

		ExcludedTagsHelper IPrivateInstanceXmlDicomAttributeCollection.ExcludedTagsHelper
		{
			get { return _excludedTagsHelper; }
		}

		#endregion
	}

	#endregion

	/// <summary>
    /// Class for representing a SOP Instance as XML.
    /// </summary>
    /// <remarks>
    /// This class may change in a future release.
    /// </remarks>
    public class InstanceXml
	{
		#region Private members

		private readonly String _sopInstanceUid = null;
        private readonly SopClass _sopClass = null;
        private readonly TransferSyntax _transferSyntax = null;
    	private string _sourceFileName = null;
    	private long _fileSize = 0;

		private BaseInstanceXml _baseInstance = null;
		private XmlElement _cachedElement = null;

		private DicomAttributeCollection _collection = null;

		private IEnumerator<DicomAttribute> _baseCollectionEnumerator = null;
		private IEnumerator _instanceXmlEnumerator = null;

        #endregion

        #region Public Properties

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

    	public long FileSize
    	{
			get { return _fileSize; }
			set { _fileSize = value; }
    	}

		public TransferSyntax TransferSyntax
        {
            get { return _transferSyntax; }
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
				ParseTo(0xffffffff);
            	return _collection;
            }
        }
		
    	public DicomAttribute this[DicomTag tag]
    	{
    		get
    		{
    			return this[tag.TagValue];
    		}	
    	}

		public DicomAttribute this[uint tag]
		{
			get
			{
				ParseTo(tag);
				return _collection[tag];
			}
		}

		#endregion

        #region Constructors

		public InstanceXml(DicomAttributeCollection collection, SopClass sopClass, TransferSyntax syntax)
        {
            _sopInstanceUid = collection[DicomTags.SopInstanceUid];

            _collection = collection;
            _sopClass = sopClass;
            _transferSyntax = syntax;
        }

		public InstanceXml(XmlNode instanceNode, DicomAttributeCollection baseCollection)
        {
			_collection = new InstanceXmlDicomAttributeCollection();

			if (baseCollection != null)
			{
				_baseCollectionEnumerator = baseCollection.GetEnumerator();
				if (!_baseCollectionEnumerator.MoveNext())
					_baseCollectionEnumerator = null;
			}

        	if (!instanceNode.HasChildNodes)
                return;

        	_instanceXmlEnumerator = instanceNode.ChildNodes.GetEnumerator();
			if (!_instanceXmlEnumerator.MoveNext())
				_instanceXmlEnumerator = null;

            if (instanceNode.Attributes["UID"] != null)
            {
                _sopInstanceUid = instanceNode.Attributes["UID"].Value;
            }

            if (instanceNode.Attributes["SopClassUID"] != null)
            {
                _sopClass = SopClass.GetSopClass(instanceNode.Attributes["SopClassUID"].Value);
            }

            if (instanceNode.Attributes["TransferSyntaxUID"] != null)
            {
                _transferSyntax = TransferSyntax.GetTransferSyntax(instanceNode.Attributes["TransferSyntaxUID"].Value);
            }
            else
            {
            	_transferSyntax = TransferSyntax.ExplicitVrLittleEndian;
            }

			if (instanceNode.Attributes["SourceFileName"] != null)
			{
				_sourceFileName = instanceNode.Attributes["SourceFileName"].Value;
			}

			if (instanceNode.Attributes["FileSize"] != null)
			{
				long.TryParse(instanceNode.Attributes["FileSize"].Value, out _fileSize);
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

    	#region Private Static Methods

        private static bool AttributeShouldBeIncluded(DicomAttribute attribute, StudyXmlOutputSettings settings)
        {
            if (settings == null)
                return true;

            if (attribute is DicomAttributeSQ)
            {
                if (attribute.Tag.IsPrivate)
                    return settings.IncludePrivateValues;
                else
                    return true;
            }


            // private tag
            if (attribute.Tag.IsPrivate)
            {
                if (settings.IncludePrivateValues == false)
                    return false;
            }

            // check type
            if (attribute is DicomAttributeUN)
            {
                if (settings.IncludeUnknownTags == false)
                    return false;
            }

			// check the size
            ulong length = attribute.StreamLength;
            return (length <= settings.MaxTagLength);
        }

		private void ParseTo(uint tag)
		{
			while (_baseCollectionEnumerator != null && _baseCollectionEnumerator.Current.Tag.TagValue <= tag)
			{
				_collection[_baseCollectionEnumerator.Current.Tag] = _baseCollectionEnumerator.Current.Copy();
				if (!_baseCollectionEnumerator.MoveNext())
					_baseCollectionEnumerator = null;
			}

			while (_instanceXmlEnumerator != null)
			{
				XmlNode node = (XmlNode)_instanceXmlEnumerator.Current;
				String tagString = node.Attributes["Tag"].Value;
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

		private static void ParseCollection(DicomAttributeCollection theCollection, XmlNode theNode)
        {
            XmlNode attributeNode = theNode.FirstChild;
            while (attributeNode != null)
            {
				ParseAttribute(theCollection, attributeNode);
                attributeNode = attributeNode.NextSibling;
            }
        }

		private static void ParseAttribute(DicomAttributeCollection theCollection, XmlNode attributeNode)
		{
			if (attributeNode.Name.Equals("Attribute"))
			{
				DicomTag theTag = GetTagFromAttributeNode(attributeNode);

				DicomAttribute attribute = theCollection[theTag];
				if (attribute is DicomAttributeSQ)
				{
					DicomAttributeSQ attribSQ = (DicomAttributeSQ)attribute;
					//set the null value in case there are no child nodes.
					attribute.SetNullValue();

					if (attributeNode.HasChildNodes)
					{
						XmlNode itemNode = attributeNode.FirstChild;

						while (itemNode != null)
						{
							DicomSequenceItem theItem = new InstanceXmlDicomSequenceItem();

							ParseCollection(theItem, itemNode);

							attribSQ.AddSequenceItem(theItem);

							itemNode = itemNode.NextSibling;
						}
					}
				}
				else
				{
					// Cleanup the common XML character replacements
					string tempString = attributeNode.InnerText;
					tempString = tempString.Replace("&lt;", "<").
						Replace("&gt;", ">").
						Replace("&quot;", "\"").
						Replace("&apos;", "'").
						Replace("&amp;", "&");
					try
					{
						attribute.SetStringValue(tempString);
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e,
									 "Unexpected exception with tag {0} setting value '{1}'.  Ignoring tag value.",
									 attribute.Tag,
									 tempString);
					}
				}
			}
			else if (attributeNode.Name.Equals("EmptyAttribute"))
			{
				//Means that the tag should not be in this collection, but is in the base.  So, we remove it.
				DicomTag theTag = GetTagFromAttributeNode(attributeNode);
				theCollection[theTag] = null;
			}
			else if (attributeNode.Name.Equals("ExcludedAttribute"))
			{
				DicomTag theTag = GetTagFromAttributeNode(attributeNode);
				((IPrivateInstanceXmlDicomAttributeCollection)theCollection).ExcludedTagsHelper.Add(theTag.TagValue);
			}
		}

    	#endregion

        #region Private Methods
		
		private void SwitchToCachedXml()
		{
			// Give to the garbage collector the memory associated with the collection
			_collection = new InstanceXmlDicomAttributeCollection();

			if (_baseInstance != null)
			{
				_baseCollectionEnumerator = _baseInstance.Collection.GetEnumerator();
				if (!_baseCollectionEnumerator.MoveNext())
					_baseCollectionEnumerator = null;
			}

			if (!_cachedElement.HasChildNodes)
				return;

			_instanceXmlEnumerator = _cachedElement.ChildNodes.GetEnumerator();
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

			IEnumerator<DicomAttribute> baseIterator = null;
        	bool validIterator = false;
			if (baseCollection != null)
			{
				baseIterator = baseCollection.GetEnumerator();
				validIterator = baseIterator.MoveNext();
			}

			foreach (DicomAttribute attribute in collection)
            {
				bool isInBase = false;
				if (baseIterator != null && validIterator)
                {
					while (baseIterator.Current.Tag < attribute.Tag && validIterator)
					{
						XmlElement emptyAttributeElement = CreateDicomAttributeElement(theDocument, baseIterator.Current, "EmptyAttribute");
						instance.AppendChild(emptyAttributeElement);

						validIterator = baseIterator.MoveNext();
					}
					if (validIterator)
					{
						if (baseIterator.Current.Tag == attribute.Tag)
						{
							//The attribute exists in the base and it is not empty.
							isInBase = true;

							if (!(attribute is DicomAttributeOB)
								&& !(attribute is DicomAttributeOW)
								&& !(attribute is DicomAttributeOF)
								&& !(attribute is DicomFragmentSequence))
							{
								if (attribute.Equals(baseIterator.Current))
								{
									validIterator = baseIterator.MoveNext();
									continue; // Skip the attribute, its the same as in the base!
								}
							}
							// Move to the next attribute for the next time through the loop
							validIterator = baseIterator.MoveNext();
						}
					}
                }

				//Only store an empty attribute when:
				// - there is a base collection
				// - the attribute is in the base and it is not empty.
				if (attribute.IsEmpty && (baseCollection == null || !isInBase))
						continue;

				if (!AttributeShouldBeIncluded(attribute, settings))
				{
					XmlElement excludedAttributeElement = CreateDicomAttributeElement(theDocument, attribute, "ExcludedAttribute");
					instance.AppendChild(excludedAttributeElement);
					continue;
				}

				if (attribute.IsEmpty)
				{
					XmlElement emptyAttributeElement = CreateDicomAttributeElement(theDocument, attribute, "EmptyAttribute");
					instance.AppendChild(emptyAttributeElement);
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
                    byte[] val = (byte[]) attribute.Values;

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
                    float[] val = (float[]) attribute.Values;
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
                    // Do the regular expression to block out invalid XML characters in the string.
                    string text = SecurityElement.Escape(attribute);
                    if (text.Length == 0 || !Regex.IsMatch(text, "[^\x9\xA\xD\x20-\xD7FF\xE000-\xFFFD\x10000-\x10FFFF]"))
                        instanceElement.InnerText = text;
                    // else
                    //     Platform.Log(LogLevel.Error, "Invalid encoding: {0}", text);
                }

                instance.AppendChild(instanceElement);
            }

			if (collection is IInstanceXmlDicomAttributeCollection)
			{
				foreach (uint excludedTag in ((IInstanceXmlDicomAttributeCollection)collection).ExcludedTags)
					instance.AppendChild(CreateDicomAttributeElement(theDocument, excludedTag, "ExcludedAttribute"));
			}

            return instance;
        }

		private static DicomTag GetTagFromAttributeNode(XmlNode attributeNode)
		{
			String tag = attributeNode.Attributes["Tag"].Value;

			uint tagValue;
			DicomTag theTag;
			DicomVr xmlVr;
			if (tag.StartsWith("$"))
			{
				theTag = DicomTagDictionary.GetDicomTag(tag.Substring(1));
			}
			else
			{
				tagValue = uint.Parse(tag, NumberStyles.HexNumber);
				theTag = DicomTagDictionary.GetDicomTag(tagValue);
				xmlVr = DicomVr.GetVR(attributeNode.Attributes["VR"].Value);
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

		private static XmlElement CreateDicomAttributeElement(XmlDocument document, uint dicomTag, string name)
		{
			return CreateDicomAttributeElement(document, DicomTagDictionary.GetDicomTag(dicomTag), name);
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
		
		#endregion
	}	
}
