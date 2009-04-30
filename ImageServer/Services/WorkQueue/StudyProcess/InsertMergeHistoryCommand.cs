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
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Data;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    
    class InsertMergeToExistingStudyHistoryCommand:ServerDatabaseCommand
    {
        private ReconcileImageContext _context;

        #region Constructors
        public InsertMergeToExistingStudyHistoryCommand(ReconcileImageContext context)
            : base("InsertMergeToExistingStudyHistoryCommand", true)
        {
            Platform.CheckForNullReference(context, "context");
            _context = context;
        }
        #endregion

        string GetUnifiedPatientName(string name1, string name2)
        {
            name1 = DicomNameUtils.Normalize(name1, DicomNameUtils.NormalizeOptions.TrimSpaces | DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents);
            name2 = DicomNameUtils.Normalize(name2, DicomNameUtils.NormalizeOptions.TrimSpaces | DicomNameUtils.NormalizeOptions.TrimEmptyEndingComponents);

            if (name1.Length!=name2.Length)
            {
                throw new ApplicationException(String.Format("Unable to unify names: {0} and {1}", name1, name2));
            }

            StringBuilder value = new StringBuilder();
            for (int i = 0; i < name1.Length; i++ )
            {
                if (Char.ToUpper(name1[i]) == Char.ToUpper(name2[i]))
                {
                    value.Append(name1[i]);
                }
                else
                {
                    if (name1[i] != '^' && name2[i]!='^')
                    {
                        throw new ApplicationException(String.Format("Unable to unify names: {0} and {1}", name1, name2));
                    }
                    else // one of them is ^
                    {    
                        if (name1[i] != ' ' && name2[i]!=' ')
                        {
                            throw new ApplicationException(String.Format("Unable to unify names: {0} and {1}", name1, name2));
                        }
                        else
                        {
                            value.Append('^');
                        }
                    }
                }
            }
            return value.ToString();
        }

        protected override void OnExecute(ClearCanvas.Enterprise.Core.IUpdateContext updateContext)
        {
            ReconcileMergeToExistingStudyDescriptor desc = new ReconcileMergeToExistingStudyDescriptor();
            desc.ExistingStudy = StudyInformation.CreateFrom(_context.CurrentStudy);
            desc.ImageSetData = _context.ImageSet;
            desc.Action = StudyReconcileAction.Merge;
            string newPatientName = GetUnifiedPatientName(desc.ExistingStudy.PatientInfo.Name, _context.ImageSet[DicomTags.PatientsName].Value);

            if (!desc.ExistingStudy.PatientInfo.Name.Equals(newPatientName))
            {
                SetTagCommand cmd = new SetTagCommand(DicomTags.PatientsName,newPatientName);
                desc.Commands = new List<BaseImageLevelUpdateCommand>();
                desc.Commands.Add(cmd);
            }
            
            desc.Automatic = true;
            desc.Description = "Patient Name Auto-Correction";

            IStudyHistoryEntityBroker broker = updateContext.GetBroker<IStudyHistoryEntityBroker>();
            StudyHistoryUpdateColumns columns = new StudyHistoryUpdateColumns();
            columns.StudyStorageKey = _context.CurrentStudyLocation.GetKey();
            columns.DestStudyStorageKey = null;
            columns.InsertTime = Platform.Time;
            columns.StudyHistoryTypeEnum = StudyHistoryTypeEnum.StudyReconciled;
            columns.StudyData = XmlUtils.SerializeAsXmlDoc(_context.ImageSet);
            XmlDocument changeDesc = XmlUtils.SerializeAsXmlDoc(desc);
            columns.ChangeDescription = changeDesc;

            StudyHistory entry = broker.Insert(columns);

            if (entry==null)
            {
                throw new ApplicationException("Unable to create study history record");
            }

            //update the context
            _context.History = entry;
            Platform.Log(LogLevel.Info, "Corrected patient name will be {0}", newPatientName);
        }
    }
}
