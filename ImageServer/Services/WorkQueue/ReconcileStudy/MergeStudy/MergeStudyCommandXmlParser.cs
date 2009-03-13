using System;
using System.Collections.Generic;
using System.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Data;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.MergeStudy
{
    /// <summary>
    /// "MergeStudy" xml parser.
    /// </summary>
    public class MergeStudyCommandXmlParser
    {
        /// <summary>
        /// Retrieves the list of <see cref="BaseImageLevelUpdateCommand"/> specified in the xml.
        /// </summary>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        private List<BaseImageLevelUpdateCommand> ParseImageLevelCommands(XmlNode rootNode)
        {
            List<BaseImageLevelUpdateCommand> _commands = new List<BaseImageLevelUpdateCommand>();

            if (rootNode.Name == "ReconcileMergeToExistingStudy")
            {
                ReconcileMergeToExistingStudyDescriptor desc =
                    XmlUtils.Deserialize<ReconcileMergeToExistingStudyDescriptor>(rootNode);
                
                _commands = desc.Commands;
            }
            else if (rootNode.Name == "MergeStudy")
            {
                // old format
                foreach (XmlNode subNode in rootNode.ChildNodes)
                {
                    if (!(subNode is XmlComment))
                    {
                        //TODO: Use plugin?
                        if (subNode.Name == "SetTag")
                        {
                            SetTagCommand command = XmlUtils.Deserialize<SetTagCommand>(subNode);
                            _commands.Add(command);
                        }
                        else
                        {
                            throw new NotSupportedException(String.Format("Unsupported operator {0}", subNode.Name));
                        }
                    }
                }
            }
            else
            {
                throw new NotSupportedException(String.Format("Merge command: {0}", rootNode.Name));
            }
            
            return _commands;
        }

        public ReconcileMergeToExistingStudyDescriptor Parse(XmlDocument doc)
        {
            if (doc == null)
                return null;

            if (doc.DocumentElement.Name == "Reconcile")
            {
                return XmlUtils.Deserialize<ReconcileMergeToExistingStudyDescriptor>(doc.DocumentElement);
            }
            else
            {
                ReconcileMergeToExistingStudyDescriptor desc = new ReconcileMergeToExistingStudyDescriptor();
                desc.Action = StudyReconcileAction.Merge;
                desc.Automatic = false;
                desc.Commands = ParseImageLevelCommands(doc.DocumentElement);
                desc.ExistingStudy = new StudyInformation();
                desc.ImageSetData = new ImageSetDescriptor();
                return desc;
            }
        }
    }
}
