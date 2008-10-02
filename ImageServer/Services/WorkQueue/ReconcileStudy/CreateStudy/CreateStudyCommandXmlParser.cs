using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.CreateStudy
{
     /// <summary>
    /// "CreateStudy" xml parser.
    /// </summary>
    class CreateStudyCommandXmlParser
    {
        public IList<IImageLevelUpdateCommand> ParseImageLevelCommands(XmlNode createStudyNode)
        {
            List<IImageLevelUpdateCommand> _commands = new List<IImageLevelUpdateCommand>();

            foreach (XmlNode subNode in createStudyNode.ChildNodes)
            {
                if (!(subNode is XmlComment))
                {
                    //TODO: use plugins?

                    if (subNode.Name == "SetTag")
                    {
                        UpdateTagCommandParser parser = new UpdateTagCommandParser();
                        _commands.Add(parser.Parse(subNode));
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
