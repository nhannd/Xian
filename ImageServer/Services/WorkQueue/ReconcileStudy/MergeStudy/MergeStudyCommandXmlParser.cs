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
using System.Xml;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Edit;

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
