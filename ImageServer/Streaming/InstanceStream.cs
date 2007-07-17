using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

using ClearCanvas.ImageServer.Dicom;

namespace ClearCanvas.ImageServer.Streaming
{
    public class InstanceStream
    {
        #region Private members

        private DicomAttributeCollection _collection = null;
        private String _sopInstanceUid = null;
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

        public DicomAttributeCollection Collection
        {
            get { return _collection; }
        }

        #endregion

        #region Constructors

        public InstanceStream(DicomAttributeCollection collection)
        {
            
            _sopInstanceUid = collection[DicomTags.SOPInstanceUID];

            _collection = collection;
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
                    String vr = attributeNode.Attributes["VR"].Value;
                    String tag = attributeNode.Attributes["Tag"].Value;

                    uint tagValue = uint.Parse(tag,NumberStyles.HexNumber);

                    DicomTag theTag = DicomTagDictionary.Instance[tagValue];
                    if (theTag == null)
                        theTag = new DicomTag(tagValue,"Unknown tag",DicomVr.GetVR(vr),false,1,uint.MaxValue,false);

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
                        attribute.SetStringValue(attributeNode.InnerText);
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

            ParseCollection(_collection, instanceNode);
        }

        #endregion

		#region Internal Methods

        private XmlElement GetMomentoForCollection(XmlDocument theDocument, DicomAttributeCollection baseCollection, DicomAttributeCollection collection)
		{
			XmlElement instance = null;

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
			}

			foreach (Dicom.DicomAttribute attribute in collection)
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
				else
				{
					instanceElement.InnerText = attribute;
				}

				instance.AppendChild(instanceElement);
			}

			return instance;

		}

		internal XmlElement GetMomento(XmlDocument theDocument, InstanceStream theBaseStream)
		{
            if (theBaseStream != null)
                return GetMomentoForCollection(theDocument, theBaseStream.Collection, _collection);

            return GetMomentoForCollection(theDocument, null, _collection);
		}

		#endregion
	}
}
