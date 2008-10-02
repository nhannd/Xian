using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    class WebEditStudyCommandXmlParser
    {
        public IList<IImageLevelUpdateCommand> ParseImageLevelCommands(XmlNode editNode)
        {
            List<IImageLevelUpdateCommand> _commands = new List<IImageLevelUpdateCommand>();

            foreach (XmlNode subNode in editNode.ChildNodes)
            {
                if (!(subNode is XmlComment))
                {
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
