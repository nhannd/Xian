using System;
using System.Collections.Generic;
using System.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess;
using ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy;

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
                ReconcileDescription desc =
                    XmlUtils.Deserialize<ReconcileMergeToExistingStudyDescription>(rootNode);
                
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
                            SetTagCommandCompiler compiler = new SetTagCommandCompiler();

                            _commands.Add(compiler.Compile(new XmlNodeReader(subNode)));
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

        public ReconcileMergeToExistingStudyDescription Parse(XmlDocument doc)
        {
            if (doc == null)
                return null;

            if (doc.DocumentElement.Name == "Reconcile")
            {
                return XmlUtils.Deserialize<ReconcileMergeToExistingStudyDescription>(doc.DocumentElement);
            }
            else
            {
                ReconcileMergeToExistingStudyDescription desc = new ReconcileMergeToExistingStudyDescription();
                desc.Action = ReconcileAction.Merge;
                desc.Automatic = false;
                desc.Commands = ParseImageLevelCommands(doc.DocumentElement);
                desc.ExistingStudy = new StudyInformation();
                desc.ImageSetData = new ImageSetDescriptor();
                return desc;
            }
        }
    }
}
