#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Data;
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

        public ReconcileCreateStudyDescriptor Parse(XmlDocument doc)
        {
            if (doc == null)
                return null;

            if (doc.DocumentElement.Name == "Reconcile")
            {
                return XmlUtils.Deserialize<ReconcileCreateStudyDescriptor>(doc.DocumentElement);
            }
            else
            {
                ReconcileCreateStudyDescriptor desc = new ReconcileCreateStudyDescriptor();
                desc.Action = StudyReconcileAction.CreateNewStudy;
                desc.Automatic = false;
                desc.Commands = ParseImageLevelCommands(doc.DocumentElement);
                desc.ExistingStudy = new StudyInformation();
                desc.ImageSetData = new ImageSetDescriptor();
                return desc;
            }
        }
    }

}
