using System;
using System.Collections.Generic;
using System.Globalization;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using System.Xml;
using ClearCanvas.Dicom;
using System.Diagnostics;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    /// <summary>
    /// Xml specification of an "UpdateImages" command used in reconciliation.
    /// </summary>
    public class UpdateImagesCommandXmlSpecification
    {
        #region Private Members
        private DicomFileUpdateCommandActionList _actionList;
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a list of actions specified in the specifications.
        /// </summary>
        /// 
        public DicomFileUpdateCommandActionList ActionList
        {
            get { return _actionList; }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Parses the Xml specifications
        /// </summary>
        /// <param name="containingNode"></param>
        public void Parse(XmlNode containingNode)
        {
            // TODO: Consider using plugin mechanism similar to server rules.

            _actionList = new DicomFileUpdateCommandActionList();
            XmlNodeList setTagNodeList = containingNode.SelectNodes("SetTag");
            foreach (XmlNode setTagNode in setTagNodeList)
            {
                string tagPath = setTagNode.Attributes["TagPath"].Value;
                string value = setTagNode.Attributes["Value"].Value;
                List<DicomTag> parents;
                DicomTag tag;
                ParseTagPath(tagPath, out parents, out tag);

                Debug.Assert(tag != null);

                SetTagAction action = new SetTagAction();
                action.Specifications = new UpdateSpecification(parents, tag, value);
                _actionList.Add(action);
            }
        }

        #endregion

        #region Private Methods

        private static void ParseTagPath(string tagPath, out List<DicomTag> parentTags, out DicomTag tag)
        {
            Platform.CheckForNullReference(tagPath, "tagPath");

            parentTags = null;
            tag = null;

            string[] tagPathComponents = tagPath.Split(',');
            if (tagPathComponents != null)
            {
                uint tagValue;
                if (tagPathComponents.Length > 1)
                {
                    parentTags = new List<DicomTag>();

                    for (int i = 0; i < tagPathComponents.Length - 1; i++)
                    {
                        tagValue = uint.Parse(tagPathComponents[i], NumberStyles.HexNumber);
                        DicomTag parent = DicomTagDictionary.GetDicomTag(tagValue);
                        if (parent == null)
                            throw new Exception(String.Format("Specified tag {0} is not in the dictionary", parent));
                        parentTags.Add(parent);
                    }
                }

                tagValue = uint.Parse(tagPathComponents[tagPathComponents.Length - 1], NumberStyles.HexNumber);
                tag = DicomTagDictionary.GetDicomTag(tagValue);
                if (tag == null)
                    throw new Exception(String.Format("Specified tag {0} is not in the dictionary", tag));

            }
        }

        #endregion

    }
}
