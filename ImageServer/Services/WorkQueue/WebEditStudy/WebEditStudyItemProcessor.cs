#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using System.Diagnostics;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    /// <summary>
    /// 'WebEditStudy' processor.
    /// </summary>
    public class WebEditStudyItemProcessor : BaseItemProcessor
    {
        #region Private Members
        private ServerPartition _partition;
        private Study _study;
        private Patient _patient;
        private string _tempPath;
        #endregion Private Members

        #region Public Properties

        /// <summary>
        ///  The <see cref="Study"/> associated  with the 'WebEditStudy' work queue item 
        /// </summary>
        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        /// <summary>
        ///  The <see cref="Patient"/> associated  with the 'WebEditStudy' work queue item 
        /// </summary>
        public Patient Patient
        {
            get { return _patient; }
            set { _patient = value; }
        }

        /// <summary>
        ///  The <see cref="ServerPartition"/> associated  with the 'WebEditStudy' work queue item 
        /// </summary>
        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        #endregion Public Properties

        #region Overridden Protected Method

        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item.Data, "item.Data");
            ServerCommandProcessor processor = new ServerCommandProcessor("Process EditStudy");
            bool successful = false;
            try
            {
                LoadStorageLocation(item);

                LoadEntities(item);

                if (Study != null && Patient!=null)
                {
                    EditStudyContext ctx = new EditStudyContext();
                    ctx.WorkQueueItem = item;
                    ctx.StudyKey = Study.GetKey();
                    ctx.PatientKey = Patient.GetKey();
                    ctx.Partition = Partition;
                    ctx.StorageLocation = StorageLocation;
                    ctx.NewStudyXml = new DicomServices.Xml.StudyXml();

                    // output folder = temp\ImageServer\EditStudy\.....
                    string destFolder = GetTempPath();
                    destFolder = Path.Combine(destFolder, "ImageServer");
                    destFolder = Path.Combine(destFolder, "EditStudy");
                    destFolder = Path.Combine(destFolder, Path.GetRandomFileName());
                    ctx.DestinationFolder = destFolder;


                    string itemSesc = String.Format("WebEditStudy for {0} study={1} UID={2}", Patient.Name, Study.StudyDescription, Study.StudyInstanceUid);
                    EditStudyCommand editCommand = new EditStudyCommand(itemSesc, item.Data.DocumentElement);
                    editCommand.Context = ctx;

                    processor.AddCommand(editCommand);

                    successful = processor.Execute();
                }
                
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e,"Unexpected exception occured while processing the WebEditStudy work queue item");
                item.FailureDescription = e.Message;
            }
            finally
            {
				if (successful)
					PostProcessing(item,false,true); // Complete
				else
					PostProcessingFailure(item, false);
            }
        }

        
        #endregion Public Methods

        #region Private Methods

        private string GetTempPath()
        {
            if (String.IsNullOrEmpty(_tempPath))
            {
                string root = Path.GetPathRoot(Path.GetTempPath());
                _tempPath = Path.Combine(root, "temp");    
            }
            if (!Directory.Exists(_tempPath))
                Directory.CreateDirectory(_tempPath);
            
            return _tempPath;
        }

        private void LoadEntities(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");

            string studyInstanceUid = StorageLocation.StudyInstanceUid;
            ServerEntityKey partitionKey = item.ServerPartitionKey;

            IServerPartitionEntityBroker serverPartitionBroker = ReadContext.GetBroker<IServerPartitionEntityBroker>();
            _partition = serverPartitionBroker.Load(partitionKey);

            IStudyEntityBroker studyBroker = ReadContext.GetBroker<IStudyEntityBroker>();
            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.ServerPartitionKey.EqualTo(partitionKey);
            criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
            IList<Study> studies = studyBroker.Find(criteria);

            if (studies == null || studies.Count == 0)
            {
                // no study found. One possiblity this could happen is EditStudy entry was scheduled to happend after another StudyDelete.
                Platform.Log(LogLevel.Error, "No study entity found for work queue item {0}", item.GetKey().Key);
                return;
            }

            _study = studies[0];
            IPatientEntityBroker patientBroker = ReadContext.GetBroker<IPatientEntityBroker>();
            _patient = patientBroker.Load(_study.PatientKey);

            Debug.Assert(Partition != null);
            Debug.Assert(Patient != null);
            Debug.Assert(Study != null);
        }

        #endregion

    }
}
