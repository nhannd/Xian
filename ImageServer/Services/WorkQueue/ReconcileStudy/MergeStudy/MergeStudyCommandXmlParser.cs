using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.CreateStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy.MergeStudy
{
    class MergeStudyCommandXmlParser
    {
        public IList<IImageLevelUpdateCommand> ParseImageLevelCommands(XmlNode createStudyNode)
        {
            List<IImageLevelUpdateCommand> _commands = new List<IImageLevelUpdateCommand>();

            foreach (XmlNode subNode in createStudyNode.ChildNodes)
            {
                if (! (subNode is XmlComment))
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
