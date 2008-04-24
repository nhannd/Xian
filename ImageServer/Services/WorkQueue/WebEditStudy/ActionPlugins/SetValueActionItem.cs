using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy.ActionPlugins
{
    /// <summary>
    /// Class for implementing "set" action in an <see cref="EditStudyCommand"/> operation.
    /// </summary>
    public class SetValueActionItem : IActionItem<EditStudyContext>
    {
        #region Private Members

        private List<DicomTag> _parentTags = null;
        private DicomTag _targetTag = null;

        private string _value = null;
        private string _failureReason = null;
        private DicomAttribute _edittedAttribute = null;
        private int _previousCalls = 0;
        #endregion

        #region Constructors
        public SetValueActionItem(uint[] tags, string value)
        {
            
            Initialize(tags, value);
        }

        
        private void Initialize(uint[] tags, string value)
        {
            Platform.CheckForNullReference(tags, "tags");

            _value = value;
            _parentTags = new List<DicomTag>();

            uint tagValue;
            for (int i = 0; i < tags.Length - 1; i++)
            {
                tagValue = tags[i];
                DicomTag tag = DicomTagDictionary.GetDicomTag(tagValue);

                if (tag==null)
                {
                    throw new Exception(String.Format("Specified tag {0} is not in the dictionary", tagValue.ToString("X8")));
                }

                _parentTags.Add(tag);
            }

            tagValue = tags[tags.Length - 1];
            _targetTag = DicomTagDictionary.GetDicomTag(tagValue);
            if (_targetTag == null)
            {
                throw new Exception(String.Format("Specified tag {0} is not in the dictionary", _targetTag));
            }


            Validate();
        }

        private void Validate()
        {
            if (_parentTags != null)
            {
                foreach (DicomTag tag in _parentTags)
                {
                    if (tag.VR != DicomVr.SQvr)
                    {
                        throw new Exception(String.Format("Expecting a SQ tag but found {0}[{1}] instead", tag, tag.VR.Name));
                    }
                }
            }

            if (_targetTag != null)
            {
                if (_targetTag.VR == DicomVr.SQvr)
                {
                    throw new Exception(String.Format("Expecting a SQ tag but {0}[{1}] instead", _targetTag, _targetTag.VR.Name));
                }
            }

            // TODO: add value validation here
        }


        #endregion

        #region Public Properties
        #endregion

        #region Public Methods
        public bool Execute(EditStudyContext context)
        {
            Platform.CheckForNullReference(context, "context");
            _previousCalls++;
            
            try
            {
                EditDicomFile(context);
                if (_previousCalls == 1 /* this is the first time*/)
                {
                    UpdateDatabase(context);
                }
                
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }

        #endregion Public Methods

        #region Private Methods

        private void UpdateDatabase(EditStudyContext context)
        {
            Platform.CheckForNullReference(context, "context");
            
            UpdatePatientDemoFields(context);
            UpdateStudyFields(context);
        }


        private void UpdatePatientDemoFields(EditStudyContext context)
        {
            Platform.CheckForNullReference(context, "context");
            
            try
            {
                // use reflection to update the field in the database
                IPatientEntityBroker broker = context.UpdateContext.GetBroker<IPatientEntityBroker>();
                Platform.CheckForNullReference(broker, "broker");

                PatientUpdateColumns columns = new PatientUpdateColumns();

                PropertyInfo[] properties = columns.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    object[] attributes = property.GetCustomAttributes(typeof(DicomFieldAttribute), true);
                    foreach (DicomFieldAttribute attr in attributes)
                    {
                        if (attr.Tag == _edittedAttribute.Tag)
                        {
                            Platform.Log(LogLevel.Debug, "Updating database field {0}", property.Name);
                            property.SetValue(columns, _value, null);

                            if (!broker.Update(context.PatientKey, columns))
                                throw new Exception("patient table update failed");
                        }
                    }
                }
            }
            catch(Exception)
            {
                FailureReason =
                    String.Format("Unable to update patient demographic fields: {0}", _edittedAttribute.Tag.Name);
                throw;
            }
        }

        private void UpdateStudyFields(EditStudyContext context)
        {
            Platform.CheckForNullReference(context, "context");
            
            try
            {
                // use reflection to update the field in the database
                IStudyEntityBroker broker = context.UpdateContext.GetBroker<IStudyEntityBroker>();

                StudyUpdateColumns columns = new StudyUpdateColumns();

                PropertyInfo[] properties = columns.GetType().GetProperties();
                bool found = false;
                foreach (PropertyInfo property in properties)
                {
                    object[] attributes = property.GetCustomAttributes(typeof(DicomFieldAttribute), true);
                    foreach (DicomFieldAttribute attr in attributes)
                    {
                        if (attr.Tag == _edittedAttribute.Tag)
                        {
                            Platform.Log(LogLevel.Debug, "Updating database field {0}", property.Name);
                            property.SetValue(columns, _value, null);

                            if (!broker.Update(context.StudyKey, columns))
                                throw new Exception("study table update failed");

                            found = true; // assume there's only one db field mapped to the tag
                            break;
                        }
                    }

                    if (found)
                        break;
                }

                // we use the study date for the study folder. If it is changed, must we also update this field
                if (_edittedAttribute.Tag.TagValue == DicomTags.StudyDate)
                {
                    IStorageFilesystemEntityBroker storageFilesystemBroker = context.UpdateContext.GetBroker<IStorageFilesystemEntityBroker>();
                    StorageFilesystemUpdateColumns parms = new StorageFilesystemUpdateColumns();
                    parms.StudyFolder = _value;
                    parms.StudyStorageKey = context.StorageLocation.GetKey();

                    StorageFilesystemSelectCriteria criteria = new StorageFilesystemSelectCriteria();
                    criteria.StudyStorageKey.EqualTo(context.StorageLocation.GetKey());
                    IList<StorageFilesystem> storageFilesystems = storageFilesystemBroker.Find(criteria);

                    foreach (StorageFilesystem fs in storageFilesystems)
                    {
                        if (!storageFilesystemBroker.Update(fs.GetKey(), parms))
                            throw new Exception("storage filesystem table update failed");
                    }

                }
            }
            catch (Exception)
            {
                FailureReason =
                    String.Format("Unable to update study fields: {0}", _edittedAttribute.Tag.Name);
                throw;
            }
        }


        private void EditDicomFile(EditStudyContext context)
        {
            Platform.CheckForNullReference(context, "context");
            
            if (_targetTag == null)
                return; // nothing to edit

            try
            {
                DicomAttributeCollection collection = context.CurrentFile.DataSet;
                if (_parentTags!=null)
                {
                    foreach (DicomTag tag in _parentTags)
                    {
                        DicomAttribute sq = collection[tag] as DicomAttributeSQ;
                        if (sq == null)
                        {
                            throw new Exception(String.Format("Invalid tag value: {0}({1}) is not a SQ VR", tag, tag.Name));
                        }
                        if (sq.IsEmpty)
                        {
                            DicomSequenceItem item = new DicomSequenceItem();
                            sq.AddSequenceItem(item);
                        }

                        DicomSequenceItem[] items = sq.Values as DicomSequenceItem[];
                        Platform.CheckForNullReference(items, "items");
                        collection = items[0];
                    }
                }

                _edittedAttribute = collection[_targetTag];

                String msg = String.Format("**** EDITING : Setting tag {0} to {1}", _edittedAttribute.Tag, _value);
                Platform.Log(LogLevel.Debug, msg);

                _edittedAttribute.SetStringValue(_value);

            }
            catch(Exception e)
            {
                FailureReason = String.Format("Unable to update the dicom file : {0}", e.Message);
                throw;
            }

        }

        #endregion

        #region IActionItem<EditStudyContext> Members


        public string FailureReason
        {
            get { return _failureReason;  }
            set { _failureReason = value; }
        }

        #endregion
    }
}
