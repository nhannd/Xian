using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.CreateStudy
{
     /// <summary>
    /// "CreateStudy" xml parser.
    /// </summary>
    class CreateStudyCommandXmlParser
    {
        public IList<BaseImageLevelUpdateCommand> ParseImageLevelCommands(XmlNode createStudyNode)
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
    }

}
