using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.Commands
{
    /// <summary>
    /// Factory of "UpdateImages" command
    /// </summary>
    class ReconcileUpdateImageCommandFactory : IReconcileCommandFactory
    {

        #region IReconcileCommandFactory Members

        public string CommandName
        {
            get { return "UpdateImages"; }
        }

        public List<IReconcileServerCommand> Parse(XmlNode node)
        {
            UpdateImagesCommandXmlParser parser = new UpdateImagesCommandXmlParser();
            List<IReconcileServerCommand> list = new List<IReconcileServerCommand>();
            list.Add(parser.Parse(node));
            
            // implicit command
            list.Add(new ReconcileRelocateFileCommand());
            return list;
        }

        #endregion
    }

    /// <summary>
    /// Helper class to parse the Xml specifications of a <see cref="ReconcileUpdateDicomFileCommand"/>
    /// </summary>
    /// <remark>
    /// The xml specifies the <see cref="IDicomFileUpdateCommandAction"/> that is part of the command and
    /// will be applied to the file. Currently the only <see cref="SetTagAction"/> is supported. In the xml,
    /// <see cref="SetTagAction"/> actions are represented by "SetTag" nodes.
    /// 
    /// <example>
    /// The following example illustrates how to use <see cref="UpdateImagesCommandXmlParser"/> to
    /// extract the <see cref="ReconcileUpdateDicomFileCommand"/> in an xml.
    /// 
    /// <code>
    /// <ImageCommands>
    ///     <UpdateImages>
    ///         <SetTag TagPath="00100010" Value="John^Smith"/>
    ///     </UpdateImages>
    /// </ImageCommands>
    /// 
    /// XmlDocument doc = .....
    /// UpdateImagesCommandXmlSpecifications xmlSpecs = new UpdateImagesCommandXmlSpecifications();
    /// XmlNode node = doc.GetSingleNode("//UpdateImages");
    /// ReconcileUpdateDicomFileCommand command = xmlSpecs.Parse(node);
    /// </code>
    /// </example>
    /// </remark>
    public class UpdateImagesCommandXmlParser
    {
        #region Private Members

        #endregion

        #region Public Properties

        #endregion

        #region Public Properties

        /// <summary>
        /// Parses the Xml specifications
        /// </summary>
        /// <param name="containingNode"></param>
        public ReconcileUpdateDicomFileCommand Parse(XmlNode containingNode)
        {
            // TODO: Consider using plugin mechanism similar to server rules.

            DicomFileUpdateCommandActionList actionList = new DicomFileUpdateCommandActionList();
            XmlNodeList setTagNodeList = containingNode.SelectNodes("SetTag");
            foreach (XmlNode setTagNode in setTagNodeList)
            {
                string tagPath = setTagNode.Attributes["TagPath"].Value;
                string value = setTagNode.Attributes["Value"].Value;
                List<DicomTag> parents;
                DicomTag tag;
                ParseTagPath(tagPath, out parents, out tag);

                Debug.Assert(tag != null);

                SetTagAction action = new SetTagAction(new TagUpdateSpecification(parents, tag, value));
                actionList.Add(action);
            }

            return new ReconcileUpdateDicomFileCommand(actionList);
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
