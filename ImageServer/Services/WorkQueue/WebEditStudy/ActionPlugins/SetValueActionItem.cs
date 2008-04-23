using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy.ActionPlugins
{
    /// <summary>
    /// Class for implementing auto-route action as specified by <see cref="IActionItem{T}"/>
    /// </summary>
    public class SetValueActionItem : IActionItem<EditStudyContext>
    {

        #region Private Members

        private List<DicomTag> _parentTags;
        private DicomTag _targetTag;
        
        private string _value;
        private string _failureReason;
        private DicomAttribute _edittedAttribute;
        #endregion

        #region Constructors
        public SetValueActionItem(uint[] tags, string value)
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

                if (tag.VR!=DicomVr.SQvr)
                {
                    throw new Exception(String.Format("Expecting a SQ tag but found {0}[{1}] instead", tag, tag.VR.Name));
                }
                _parentTags.Add(tag);
            }

            tagValue = tags[tags.Length - 1];
            _targetTag = DicomTagDictionary.GetDicomTag(tagValue);
            if (_targetTag == null)
            {
                throw new Exception(String.Format("Specified tag {0} is not in the dictionary", _targetTag));
            }

            if (_targetTag.VR == DicomVr.SQvr)
            {
                throw new Exception(String.Format("Expecting a SQ tag but {0}[{1}] instead", _targetTag, _targetTag.VR.Name));
            }
        }
        #endregion

        #region Public Properties
        #endregion

        #region Public Methods
        public bool Execute(EditStudyContext context)
        {
            bool ok = EditDicomFile(context) && UpdateDatabase(context)&& UpdateXml(context);
            return ok;
        }


        private bool UpdateDatabase(EditStudyContext context)
        {
            //TODO: avoid doing the same update on every image.

            UpdatePatientDemoFields(context);
            UpdateStudyFields(context);
            return true;
        }


        private bool UpdatePatientDemoFields(EditStudyContext context)
        {
            try
            {
                // use reflection to update the field in the database
                IPatientEntityBroker broker = context.UpdateContext.GetBroker<IPatientEntityBroker>();

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

                            return broker.Update(context.PatientKey, columns);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                FailureReason =
                    String.Format("Unable to update patient demographic fields: {0}", _edittedAttribute.Tag.Name);
                throw;
            }
            

            return true;
        }

        private bool UpdateStudyFields(EditStudyContext context)
        {
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
                                return false;

                            found = true;
                            break;
                        }
                    }

                    if (found)
                        break;
                }

                // we use the study date for the study folder. If it is changed, must we also update
                // this field
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
                        storageFilesystemBroker.Update(fs.GetKey(), parms);
                    }

                }
            }
            catch (Exception e)
            {
                FailureReason =
                    String.Format("Unable to update study fields: {0}", _edittedAttribute.Tag.Name);
                throw;
            }

            return true;
        }

        

        private bool UpdateXml(EditStudyContext context)
        {
            if (!context.NewStudyXml.AddFile(context.CurrentFile))
            {
                FailureReason = String.Format("Unable to update study header xml");
                return false;
            }
            else
                return true;

        }

        

        private bool EditDicomFile(EditStudyContext context)
        {
            if (_targetTag == null)
                return true; // nothing to edit

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
                            throw new Exception(String.Format("Invalid tag value: {0}({1}) is not a SQ VR", tag.TagValue.ToString("X8"), tag.Name));
                        }
                        if (sq.IsEmpty)
                        {
                            DicomSequenceItem item = new DicomSequenceItem();
                            sq.AddSequenceItem(item);
                        }
                        DicomSequenceItem[] items = sq.Values as DicomSequenceItem[];
                        collection = items[0];
                    }
                }

                _edittedAttribute = collection[_targetTag];

                String msg = String.Format("**** EDITING : Setting tag {0} to {1}", _edittedAttribute.Tag.Name, _value);
                Console.WriteLine(msg);

                _edittedAttribute.SetStringValue(_value);

            }
            catch(Exception e)
            {
                FailureReason = String.Format("Unable to update the dicom file : {0}", e.Message);
                throw;
            }

            return true;
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
