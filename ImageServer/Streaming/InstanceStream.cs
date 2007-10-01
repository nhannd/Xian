using System;
using System.Globalization;
using System.Security;
using System.Text;
using System.Xml;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Streaming
{
    public class InstanceStream
    {
        #region Private members

        private DicomAttributeCollection _collection = null;
        private String _sopInstanceUid = null;
        private SopClass _sopClass = null;
        private TransferSyntax _transferSyntax = null;
        #endregion

        #region Public Properties

        public String SopInstanceUid
        {
            get
            {
                if (_sopInstanceUid == null)
                    return "";
                return _sopInstanceUid;
            }
        }

        public SopClass SopClass
        {
            get { return _sopClass; }
        }

        public TransferSyntax TransferSyntax
        {
            get { return _transferSyntax; }
        }

        public DicomAttributeCollection Collection
        {
            get { return _collection; }
        }

        #endregion

        #region Constructors

        public InstanceStream(DicomAttributeCollection collection, SopClass sopClass, TransferSyntax syntax)
        {
            
            _sopInstanceUid = collection[DicomTags.SopInstanceUid];

            _collection = collection;
            _sopClass = sopClass;
            _transferSyntax = syntax;
        }

        public InstanceStream()
        {
        }

        private void ParseCollection(DicomAttributeCollection theCollection, XmlNode theNode)
        {
            XmlNode attributeNode = theNode.FirstChild;
            while (attributeNode != null)
            {
                if (attributeNode.Name.Equals("Attribute"))
                {
                    DicomVr xmlVr = DicomVr.GetVR(attributeNode.Attributes["VR"].Value);
                    String tag = attributeNode.Attributes["Tag"].Value;

                    uint tagValue = uint.Parse(tag,NumberStyles.HexNumber);

                    DicomTag theTag = DicomTagDictionary.GetDicomTag(tagValue);
                    if (theTag == null)
                        theTag = new DicomTag(tagValue,"Unknown tag",xmlVr,false,1,uint.MaxValue,false);

                    if (!theTag.VR.Equals(xmlVr))
                    {
                        theTag = new DicomTag(tagValue, theTag.Name, xmlVr, theTag.MultiVR, theTag.VMLow, theTag.VMHigh, theTag.Retired);
                    }
                    DicomAttribute attribute = theCollection[theTag];

                    if (attribute is DicomAttributeSQ)
                    {
                        DicomAttributeSQ attribSQ = (DicomAttributeSQ)attribute;

                        if (attributeNode.HasChildNodes)
                        {
                            XmlNode itemNode = attributeNode.FirstChild;

                            while (itemNode != null)
                            {
                                DicomSequenceItem theItem = new DicomSequenceItem();

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

                        attribute.SetStringValue(tempString);
                    }
                }
                attributeNode = attributeNode.NextSibling;
            }
        }

        internal InstanceStream(XmlNode instanceNode,DicomAttributeCollection baseCollection)
        {
            if (baseCollection == null)
                _collection = new DicomAttributeCollection();
            else
                _collection = baseCollection.Copy();

            if (!instanceNode.HasChildNodes)
                return;
            
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
                _transferSyntax = TransferSyntax.ExplicitVrLittleEndian;

            ParseCollection(_collection, instanceNode);

            // This should never happen
            if (_sopClass == null)
            {
                _sopClass = SopClass.GetSopClass(_collection[DicomTags.SopClassUid].GetString(0, String.Empty));
            }
        }

        #endregion

		#region Internal Methods

        private XmlElement GetMomentoForCollection(XmlDocument theDocument, DicomAttributeCollection baseCollection, DicomAttributeCollection collection)
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

                if (_transferSyntax != null)
                {
                    XmlAttribute transferSyntaxAttribute = theDocument.CreateAttribute("TransferSyntaxUID");
                    transferSyntaxAttribute.Value = _transferSyntax.UidString;
                    instance.Attributes.Append(transferSyntaxAttribute);
                }
			}

			foreach (DicomAttribute attribute in collection)
			{
                if (baseCollection!=null)
                {
                    DicomAttribute attribBase = baseCollection[attribute.Tag];
                    if (attribBase!=null)
                    {
                        if (!(attribute is DicomAttributeOB)
                         && !(attribute is DicomAttributeOW)
                         && !(attribute is DicomAttributeOF))
                        {
                            if (attribute.Equals(attribBase))
                                continue; // SKip the attribute, its the same as in the base!
                        }
                        
                    }
                }
				XmlElement instanceElement = theDocument.CreateElement("Attribute");

				XmlAttribute tag = theDocument.CreateAttribute("Tag");
				tag.Value = attribute.Tag.HexString;

				XmlAttribute vr = theDocument.CreateAttribute("VR");
				vr.Value = attribute.Tag.VR.ToString();

				instanceElement.Attributes.Append(tag);
				instanceElement.Attributes.Append(vr);

				if (attribute is DicomAttributeSQ)
				{
					DicomSequenceItem[] items = (DicomSequenceItem[])attribute.Values;
					foreach (DicomSequenceItem item in items)
					{
						XmlElement itemElement = GetMomentoForCollection(theDocument, null, item);

						instanceElement.AppendChild(itemElement);
					}
				}
                else if (attribute is DicomAttributeOW)
                {
                    ushort[] val = (ushort[])attribute.Values;

                    StringBuilder str = null;
                    foreach (ushort i in val)
                    {
                        if (str == null)
                            str = new StringBuilder(i.ToString());
                        else
                            str.AppendFormat("\\{0}", i.ToString());
                    }
                    if (str!= null)
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
                            str.AppendFormat("\\{0}", i.ToString());
                    }
                    if (str != null)
                        instanceElement.InnerText = str.ToString();
                }
                else
                {
                    instanceElement.InnerText = SecurityElement.Escape(attribute);
                }

				instance.AppendChild(instanceElement);
			}

			return instance;

		}

        private XmlElement _cachedElement = null;

		internal XmlElement GetMomento(XmlDocument theDocument, InstanceStream theBaseStream)
		{
            if (_cachedElement != null)
            {
         //       return _cachedElement;
            }

            if (theBaseStream != null)
            {
                _cachedElement = GetMomentoForCollection(theDocument, theBaseStream.Collection, _collection);
                return _cachedElement;
            }

		    _cachedElement = GetMomentoForCollection(theDocument, null, _collection);
		    return _cachedElement;
		}

		#endregion
	}
}
