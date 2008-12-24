using System;
using System.Collections.Generic;
using System.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.MergeStudy
{
    /// <summary>
    /// "MergeStudy" xml parser.
    /// </summary>
    class MergeStudyCommandXmlParser
    {
        /// <summary>
        /// Retrieves the list of <see cref="BaseImageLevelUpdateCommand"/> specified in the xml.
        /// </summary>
        /// <param name="createStudyNode"></param>
        /// <returns></returns>
        public List<BaseImageLevelUpdateCommand> ParseImageLevelCommands(XmlNode createStudyNode)
        {
            List<BaseImageLevelUpdateCommand> _commands = new List<BaseImageLevelUpdateCommand>();

            foreach (XmlNode subNode in createStudyNode.ChildNodes)
            {
                if (! (subNode is XmlComment))
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

            return _commands;
        }

    }
}
