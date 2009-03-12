using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess;
using ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.CreateStudy
{
     /// <summary>
    /// "CreateStudy" xml parser.
    /// </summary>
    public class CreateStudyCommandXmlParser
    {
        private List<BaseImageLevelUpdateCommand> ParseImageLevelCommands(XmlNode createStudyNode)
        {
            List<BaseImageLevelUpdateCommand> _commands = new List<BaseImageLevelUpdateCommand>();

            foreach (XmlNode subNode in createStudyNode.ChildNodes)
            {
                if (!(subNode is XmlComment))
                {
                    //TODO: use plugins?

                    if (subNode.Name == "SetTag")
                    {
                        SetTagCommandCompiler compiler = new SetTagCommandCompiler();
                        _commands.Add(compiler.Compile(new XmlNodeReader(subNode)));
                    }
                    else
                    {
                        throw new NotSupportedException(subNode.Name);
                    }
                }
            }

            return _commands;
        }

        public ReconcileCreateStudyDescription Parse(XmlDocument doc)
        {
            if (doc == null)
                return null;

            if (doc.DocumentElement.Name == "Reconcile")
            {
                return XmlUtils.Deserialize<ReconcileCreateStudyDescription>(doc.DocumentElement);
            }
            else
            {
                ReconcileCreateStudyDescription desc = new ReconcileCreateStudyDescription();
                desc.Action = ReconcileAction.CreateNewStudy;
                desc.Automatic = false;
                desc.Commands = ParseImageLevelCommands(doc.DocumentElement);
                desc.ExistingStudy = new StudyInformation();
                desc.ImageSetData = new ImageSetDescriptor();
                return desc;
            }
        }
    }

}
