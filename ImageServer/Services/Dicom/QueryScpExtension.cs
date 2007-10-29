#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text.RegularExpressions;

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Model.SelectBrokers;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.ImageServer.Services.Dicom;

namespace ClearCanvas.DicomServices.ImageServer
{
    ///<summary>
    /// Plugin for handling DICOM Query Requests implementing the <see cref="IDicomScp"/> interface.
    ///</summary>
    [ExtensionOf(typeof(DicomScpExtensionPoint))]
    public class QueryScpExtension : BaseScp, IDicomScp
    {
        #region Private members
        private List<SupportedSop> _list = new List<SupportedSop>();
        #endregion

        #region Contructors
        /// <summary>
        /// Public default constructor.  Implements the Find and Move services for 
        /// Patient Root and Study Root queries.
        /// </summary>
        public QueryScpExtension()
        {
            SupportedSop sop = new SupportedSop();
            sop.SopClass = SopClass.PatientRootQueryRetrieveInformationModelFind;
            sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
            _list.Add(sop);

            sop = new SupportedSop();
            sop.SopClass = SopClass.StudyRootQueryRetrieveInformationModelFind;
            sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
            _list.Add(sop);
        }
        #endregion

        /// <summary>
        /// Load the values for the tag <see cref="DicomTags.ModalitiesInStudy"/> into a response
        /// message for a specific <see cref="Study"/>.
        /// </summary>
        /// <param name="read">The connection to use to read the values.</param>
        /// <param name="response">The message to add the value into.</param>
        /// <param name="row">The <see cref="Study"/> entity to find the values for.</param>
        public void LoadModalitiesInStudy(IReadContext read, DicomMessage response, Study row)
        {
            IQueryModalitiesInStudy select = read.GetBroker<IQueryModalitiesInStudy>();

            ModalitiesInStudyQueryParameters parms = new ModalitiesInStudyQueryParameters();

            parms.StudyKey = row.GetKey();

            IList<Series> list = select.Execute(parms);

            string value = "";
            foreach (Series series in list)
            {
                if (value.Length == 0)
                    value = series.Modality;
                else
                    value = String.Format("{0}\\{1}", value, series.Modality);
            }
            response.DataSet[DicomTags.ModalitiesInStudy].SetStringValue(value);
        }

        /// <summary>
        /// Load the values for the sequence <see cref="DicomTags.RequestAttributesSequence"/>
        /// into a response message for a specific series.
        /// </summary>
        /// <param name="read">The connection to use to read the values.</param>
        /// <param name="response">The message to add the values into.</param>
        /// <param name="row">The <see cref="Series"/> entity to load the related <see cref="RequestAttributes"/> entity for.</param>
        public void LoadRequestAttributes(IReadContext read, DicomMessage response, Series row)
        {
            IQueryRequestAttributes select = read.GetBroker<IQueryRequestAttributes>();

            RequestAttributesQueryParameters parms = new RequestAttributesQueryParameters();

            parms.SeriesKey = row.GetKey();

            IList<RequestAttributes> list = select.Execute(parms);

            if (list.Count == 0)
            {
                response.DataSet[DicomTags.RequestAttributesSequence].SetNullValue();
                return;
            }

            foreach (RequestAttributes request in list)
            {
                DicomSequenceItem item = new DicomSequenceItem();
                item[DicomTags.ScheduledProcedureStepId].SetStringValue(request.ScheduledProcedureStepId);
                item[DicomTags.RequestedProcedureId].SetStringValue(request.RequestedProcedureId);

                response.DataSet[DicomTags.RequestAttributesSequence].AddSequenceItem(item);
            }
        }

        /// <summary>
        /// Find the <see cref="ServerEntityKey"/> reference for a given Study Instance UID and Server Partition.
        /// </summary>
        /// <param name="read"></param>
        /// <param name="studyInstanceUid"></param>
        /// <returns></returns>
        public List<ServerEntityKey> LoadStudyKey(IReadContext read, string[] studyInstanceUid)
        {
            ISelectStudy find = read.GetBroker<ISelectStudy>();

            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.ServerPartitionKey.EqualTo(Partition.GetKey());
            if (studyInstanceUid.Length > 1)
                criteria.StudyInstanceUid.In(studyInstanceUid);
            else
                criteria.StudyInstanceUid.EqualTo(studyInstanceUid[0]);

            IList<Study> list = find.Find(criteria);

            List<ServerEntityKey> serverList = new List<ServerEntityKey>();

            foreach (Study row in list)
                serverList.Add(row.GetKey());

            return serverList;

        }

        /// <summary>
        /// Populate data from a <see cref="Patient"/> entity into a DICOM C-FIND-RSP message.
        /// </summary>
        /// <param name="read"></param>
        /// <param name="response"></param>
        /// <param name="tagList"></param>
        /// <param name="row"></param>
        public void PopulatePatient(IReadContext read, DicomMessage response, IList<uint> tagList, Patient row)
        {
            DicomAttributeCollection dataSet = response.DataSet;
    
            dataSet[DicomTags.RetrieveAeTitle].SetStringValue(Partition.AeTitle);
            dataSet[DicomTags.InstanceAvailability].SetStringValue("ONLINE");

            foreach (uint tag in tagList)
            {
                switch (tag)
                {
                    case DicomTags.PatientsName:
                        dataSet[DicomTags.PatientsName].SetStringValue(row.PatientName);
                        break;
                    case DicomTags.PatientId:
                        dataSet[DicomTags.PatientId].SetStringValue(row.PatientId);
                        break;
                    case DicomTags.IssuerOfPatientId:
                        dataSet[DicomTags.IssuerOfPatientId].SetStringValue(row.IssuerOfPatientId);
                        break;
                    case DicomTags.NumberOfPatientRelatedStudies:
                        dataSet[DicomTags.NumberOfPatientRelatedStudies].AppendInt32(row.NumberOfPatientRelatedStudies);
                        break;
                    case DicomTags.NumberOfPatientRelatedSeries:
                        dataSet[DicomTags.NumberOfPatientRelatedSeries].AppendInt32(row.NumberOfPatientRelatedSeries);
                        break;
                    case DicomTags.NumberOfPatientRelatedInstances:
                        dataSet[DicomTags.NumberOfPatientRelatedInstances].AppendInt32(row.NumberOfPatientRelatedInstances);
                        break;
                    case DicomTags.QueryRetrieveLevel:
                        dataSet[DicomTags.QueryRetrieveLevel].SetStringValue("PATIENT");
                        break;
                    default:
                        dataSet[tag].SetNullValue();
                        break;
                }
            }
        }

        /// <summary>
        /// Populate the data from a <see cref="Study"/> entity into a DICOM C-FIND-RSP message.
        /// </summary>
        /// <param name="read"></param>
        /// <param name="response"></param>
        /// <param name="tagList"></param>
        /// <param name="row"></param>
        public void PopulateStudy(IReadContext read, DicomMessage response, IList<uint> tagList, Study row)
        {
            DicomAttributeCollection dataSet = response.DataSet;

            dataSet[DicomTags.RetrieveAeTitle].SetStringValue(Partition.AeTitle);
            dataSet[DicomTags.InstanceAvailability].SetStringValue("ONLINE");

            foreach (uint tag in tagList)
            {
                switch (tag)
                {
                    case DicomTags.StudyInstanceUid:
                        dataSet[DicomTags.StudyInstanceUid].SetStringValue(row.StudyInstanceUid);
                        break;
                    case DicomTags.PatientsName:
                        dataSet[DicomTags.PatientsName].SetStringValue(row.PatientName);
                        break;
                    case DicomTags.PatientId:
                        dataSet[DicomTags.PatientId].SetStringValue(row.PatientId);
                        break;
                    case DicomTags.PatientsBirthDate:
                        dataSet[DicomTags.PatientsBirthDate].SetStringValue(row.PatientsBirthDate);
                        break;
                    case DicomTags.PatientsSex:
                        dataSet[DicomTags.PatientsSex].SetStringValue(row.PatientsSex);
                        break;
                    case DicomTags.StudyDate:
                        dataSet[DicomTags.StudyDate].SetStringValue(row.StudyDate);
                        break;
                    case DicomTags.StudyTime:
                        dataSet[DicomTags.StudyTime].SetStringValue(row.StudyTime);
                        break;
                    case DicomTags.AccessionNumber:
                        dataSet[DicomTags.AccessionNumber].SetStringValue(row.AccessionNumber);
                        break;
                    case DicomTags.StudyId:
                        dataSet[DicomTags.StudyId].SetStringValue(row.StudyId);
                        break;
                    case DicomTags.StudyDescription:
                        dataSet[DicomTags.StudyDescription].SetStringValue(row.StudyDescription);
                        break;
                    case DicomTags.ReferringPhysiciansName:
                        dataSet[DicomTags.ReferringPhysiciansName].SetStringValue(row.ReferringPhysiciansName);
                        break;
                    case DicomTags.NumberOfStudyRelatedSeries:
                        dataSet[DicomTags.NumberOfStudyRelatedSeries].AppendInt32(row.NumberOfStudyRelatedSeries);
                        break;
                    case DicomTags.NumberOfStudyRelatedInstances:
                        dataSet[DicomTags.NumberOfStudyRelatedInstances].AppendInt32(row.NumberOfStudyRelatedInstances);
                        break;
                    case DicomTags.ModalitiesInStudy:
                        LoadModalitiesInStudy(read, response, row);
                        break;
                    case DicomTags.QueryRetrieveLevel:
                        dataSet[DicomTags.QueryRetrieveLevel].SetStringValue("STUDY");
                        break;
                    default:
                        dataSet[tag].SetNullValue();
                        break;
                }
            }
        }

        /// <summary>
        /// Populate the data from a <see cref="Series"/> entity into a DICOM C-FIND-RSP message.
        /// </summary>
        /// <param name="read"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="tagList"></param>
        /// <param name="row"></param>
        public void PopulateSeries(IReadContext read, DicomMessage request, DicomMessage response, IList<uint> tagList, Series row)
        {
            DicomAttributeCollection dataSet = response.DataSet;

            dataSet[DicomTags.RetrieveAeTitle].SetStringValue(Partition.AeTitle);
            dataSet[DicomTags.InstanceAvailability].SetStringValue("ONLINE");

            foreach (uint tag in tagList)
            {
                switch (tag)
                {
                    case DicomTags.PatientId:
                        dataSet[DicomTags.PatientId].SetStringValue(request.DataSet[DicomTags.PatientId].ToString());
                        break;
                    case DicomTags.StudyInstanceUid:
                        dataSet[DicomTags.StudyInstanceUid].SetStringValue(request.DataSet[DicomTags.StudyInstanceUid].ToString());
                        break;
                    case DicomTags.SeriesInstanceUid:
                        dataSet[DicomTags.SeriesInstanceUid].SetStringValue(row.SeriesInstanceUid);
                        break;
                    case DicomTags.Modality:
                        dataSet[DicomTags.Modality].SetStringValue(row.Modality);
                        break;
                    case DicomTags.SeriesNumber:
                        dataSet[DicomTags.SeriesNumber].SetStringValue(row.SeriesNumber);
                        break;
                    case DicomTags.SeriesDescription:
                        dataSet[DicomTags.SeriesDescription].SetStringValue(row.SeriesDescription);
                        break;
                    case DicomTags.PerformedProcedureStepStartDate:
                        dataSet[DicomTags.PerformedProcedureStepStartDate].SetStringValue(row.PerformedProcedureStepStartDate);
                        break;
                    case DicomTags.PerformedProcedureStepStartTime:
                        dataSet[DicomTags.PerformedProcedureStepStartTime].SetStringValue(row.PerformedProcedureStepStartTime);
                        break;
                    case DicomTags.NumberOfSeriesRelatedInstances:
                        dataSet[DicomTags.NumberOfSeriesRelatedInstances].AppendInt32(row.NumberOfSeriesRelatedInstances);
                        break;
                    case DicomTags.RequestAttributesSequence:
                        LoadRequestAttributes(read, response, row);
                        break;
                    case DicomTags.QueryRetrieveLevel:
                        dataSet[DicomTags.QueryRetrieveLevel].SetStringValue("SERIES");
                        break;
                    default:
                        dataSet[tag].SetNullValue();
                        break;
                }
            }
        }

        private void PopulateInstance(DicomMessage request, DicomMessage response, List<uint> tagList, InstanceXml theInstanceStream)
        {
            DicomAttributeCollection dataSet = response.DataSet;

            dataSet[DicomTags.RetrieveAeTitle].SetStringValue(Partition.AeTitle);
            dataSet[DicomTags.InstanceAvailability].SetStringValue("ONLINE");

            DicomAttributeCollection sourceDataSet = theInstanceStream.Collection;

            foreach (uint tag in tagList)
            {
                switch (tag)
                {
                    case DicomTags.PatientId:
                        dataSet[DicomTags.PatientId].SetStringValue(request.DataSet[DicomTags.PatientId].ToString());
                        break;
                    case DicomTags.StudyInstanceUid:
                        dataSet[DicomTags.StudyInstanceUid].SetStringValue(request.DataSet[DicomTags.StudyInstanceUid].ToString());
                        break;
                    case DicomTags.SeriesInstanceUid:
                        dataSet[DicomTags.SeriesInstanceUid].SetStringValue(request.DataSet[DicomTags.SeriesInstanceUid].ToString());
                        break;
                    case DicomTags.QueryRetrieveLevel:
                        dataSet[DicomTags.QueryRetrieveLevel].SetStringValue("IMAGE");
                        break;
                    default:
                        if (sourceDataSet.Contains(tag))
                            dataSet[tag] = sourceDataSet[tag].Copy();
                        else
                            dataSet[tag].SetNullValue();
                        break;
                }
            }
        }

        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for a <see cref="ServerEntityKey"/> reference.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="vals"></param>
        private static void SetKeyCondition(ISearchCondition<ServerEntityKey> cond, ServerEntityKey[] vals)
        {
            if (vals == null || vals.Length == 0)
                return;

            if (vals.Length == 1)
                cond.EqualTo(vals[0]);
            else
                cond.In(vals);
        }

        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for an array of matching string values.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="vals"></param>
        private static void SetStringArrayCondition(ISearchCondition<string> cond, string[] vals)
        {
            if (vals == null || vals.Length == 0)
                return;

            if (vals.Length == 1)
                cond.EqualTo(vals[0]);
            else
                cond.In(vals);
        }

        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for a DICOM range matching string value.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="val"></param>
        private static void SetRangeCondition(ISearchCondition<string> cond, string val)
        {
            if (val.Length == 0)
                return;

            if (val.Contains("-"))
            {
                string[] vals = val.Split(new char[] { '-' });
                if (val.IndexOf('-')==0)
                    cond.LessThanOrEqualTo(vals[1]);
                else if (val.IndexOf('-')==val.Length-1)
                    cond.MoreThanOrEqualTo(vals[0]);
                else 
                    cond.Between(vals[0],vals[1]);
            }
            else
                cond.EqualTo(val);
        }

        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for DICOM string based (wildcard matching) value.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="val"></param>
        private static void SetStringCondition(ISearchCondition<string> cond, string val)
        {
            if (val.Length == 0)
                return;

            if (val.Contains("*") || val.Contains("?"))
            {
                String value = val.Replace('*', '%');
                value = value.Replace('?', '_');
                cond.Like(value);
            }
            else 
                cond.EqualTo(val);
        }

        /// <summary>
        /// Method for processing Patient level queries.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="association"></param>
        /// <param name="presentationID"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool OnReceivePatientQuery(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
            List<uint> tagList = new List<uint>();

            IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();

            ISelectPatient find = read.GetBroker<ISelectPatient>();

            PatientSelectCriteria criteria = new PatientSelectCriteria();
            criteria.ServerPartitionKey.EqualTo(Partition.GetKey());

            DicomAttributeCollection data = message.DataSet;
            foreach (DicomAttribute attrib in message.DataSet)
            {
                tagList.Add(attrib.Tag.TagValue);
                if (!attrib.IsNull)
                    switch (attrib.Tag.TagValue)
                    {
                        case DicomTags.PatientsName:
                            SetStringCondition(criteria.PatientName, data[DicomTags.PatientsName].GetString(0, ""));
                            break;
                        case DicomTags.PatientId:
                            SetStringCondition(criteria.PatientId, data[DicomTags.PatientId].GetString(0, ""));
                            break;
                        case DicomTags.IssuerOfPatientId:
                            SetStringCondition(criteria.IssuerOfPatientId, data[DicomTags.IssuerOfPatientId].GetString(0, ""));
                            break;
                        default:
                            break;
                    }
            }


            IReadContext subRead = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();

            try
            {
                find.Find(criteria, delegate(Patient row)
                                     {
                                         DicomMessage response = new DicomMessage();
                                         PopulatePatient(subRead, response, tagList, row);
                                         server.SendCFindResponse(presentationID, message.MessageId, response,
                                                                  DicomStatuses.Pending);
                                     });
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when processing FIND request.");
                DicomMessage errorResponse = new DicomMessage();
                server.SendCFindResponse(presentationID, message.MessageId, errorResponse,
                                         DicomStatuses.ProcessingFailure);
                subRead.Dispose();
                read.Dispose();
                return true;
            }

            read.Dispose();
            subRead.Dispose();

            DicomMessage finalResponse = new DicomMessage();
            server.SendCFindResponse(presentationID, message.MessageId, finalResponse, DicomStatuses.Success);

            return true;
        }

        /// <summary>
        /// Method for processing Study level queries.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="association"></param>
        /// <param name="presentationID"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool OnReceiveStudyLevelQuery(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
            List<uint> tagList = new List<uint>();

            IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();

            ISelectStudy find = read.GetBroker<ISelectStudy>();

            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.ServerPartitionKey.EqualTo(Partition.GetKey());

            DicomAttributeCollection data = message.DataSet;
            foreach (DicomAttribute attrib in message.DataSet)
            {
                tagList.Add(attrib.Tag.TagValue);
                if (!attrib.IsNull)
                switch (attrib.Tag.TagValue)
                {
                    case DicomTags.StudyInstanceUid:
                        SetStringArrayCondition(criteria.StudyInstanceUid, (string[])data[DicomTags.StudyInstanceUid].Values);
                        break;
                    case DicomTags.PatientsName:
                        SetStringCondition(criteria.PatientName, data[DicomTags.PatientsName].GetString(0, ""));
                        break;
                    case DicomTags.PatientId:
                        SetStringCondition(criteria.PatientId, data[DicomTags.PatientId].GetString(0, ""));
                        break;
                    case DicomTags.PatientsBirthDate:
                        SetRangeCondition(criteria.PatientsBirthDate, data[DicomTags.PatientsBirthDate].GetString(0, ""));
                        break;
                    case DicomTags.PatientsSex:
                        SetStringCondition(criteria.PatientsSex, data[DicomTags.PatientsSex].GetString(0, ""));
                        break;
                    case DicomTags.StudyDate:
                        SetRangeCondition(criteria.StudyDate, data[DicomTags.StudyDate].GetString(0, ""));
                        break;
                    case DicomTags.StudyTime:
                        SetRangeCondition(criteria.StudyTime, data[DicomTags.StudyTime].GetString(0, ""));
                        break;
                    case DicomTags.AccessionNumber:
                        SetStringCondition(criteria.AccessionNumber, data[DicomTags.AccessionNumber].GetString(0, ""));
                        break;
                    case DicomTags.StudyId:
                        SetStringCondition(criteria.StudyId, data[DicomTags.StudyId].GetString(0, ""));
                        break;
                    case DicomTags.StudyDescription:
                        SetStringCondition(criteria.StudyDescription, data[DicomTags.StudyDescription].GetString(0, ""));
                        break;
                    case DicomTags.ReferringPhysiciansName:
                        SetStringCondition(criteria.ReferringPhysiciansName, data[DicomTags.ReferringPhysiciansName].GetString(0, ""));
                        break;
                    case DicomTags.ModalitiesInStudy:// todo
                        // Specify a subselect on Modality in series
                        SeriesSelectCriteria seriesSelect = new SeriesSelectCriteria();
                        SetStringArrayCondition(seriesSelect.Modality,(string[])data[DicomTags.ModalitiesInStudy].Values);
                        criteria.SeriesRelatedEntityCondition.Exists(seriesSelect);
                        break;
                    default:
                        break;
                }
            }

            // Open another read context, in case additional queries are required.
            IReadContext subRead = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();

            try
            {
                find.Find(criteria, delegate(Study row)
                                     {
                                         DicomMessage response = new DicomMessage();
                                         PopulateStudy(subRead, response, tagList, row);
                                         server.SendCFindResponse(presentationID, message.MessageId, response,
                                                                  DicomStatuses.Pending);
                                     });
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when processing FIND request.");
                DicomMessage errorResponse = new DicomMessage();
                server.SendCFindResponse(presentationID, message.MessageId, errorResponse,
                                         DicomStatuses.ProcessingFailure);
                subRead.Dispose();
                read.Dispose();
                return true;
            }

            read.Dispose();
            subRead.Dispose();

            DicomMessage finalResponse = new DicomMessage();
            server.SendCFindResponse(presentationID, message.MessageId, finalResponse, DicomStatuses.Success);
                             
            return true;
        }

        /// <summary>
        /// Method for processing Series level queries.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="association"></param>
        /// <param name="presentationID"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool OnReceiveSeriesLevelQuery(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
            List<uint> tagList = new List<uint>();

            //Read context for the query.
            IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();

            ISelectSeries find = read.GetBroker<ISelectSeries>();

            SeriesSelectCriteria criteria = new SeriesSelectCriteria();
            criteria.ServerPartitionKey.EqualTo(Partition.GetKey());

            DicomAttributeCollection data = message.DataSet;
            foreach (DicomAttribute attrib in message.DataSet)
            {
                tagList.Add(attrib.Tag.TagValue);
                if (!attrib.IsNull)
                    switch (attrib.Tag.TagValue)
                    {
                        case DicomTags.StudyInstanceUid: 
                            List<ServerEntityKey> list =
                                LoadStudyKey(read, (string[]) data[DicomTags.StudyInstanceUid].Values);
                            SetKeyCondition(criteria.StudyKey,list.ToArray());
                            break;
                        case DicomTags.SeriesInstanceUid:
                            SetStringArrayCondition(criteria.SeriesInstanceUid, (string[])data[DicomTags.SeriesInstanceUid].Values);
                            break;
                        case DicomTags.Modality:
                            SetStringCondition(criteria.Modality, data[DicomTags.Modality].GetString(0, ""));
                            break;
                        case DicomTags.SeriesNumber:
                            SetStringCondition(criteria.SeriesNumber, data[DicomTags.SeriesNumber].GetString(0, ""));
                            break;
                        case DicomTags.SeriesDescription:
                            SetStringCondition(criteria.SeriesDescription, data[DicomTags.SeriesDescription].GetString(0, ""));
                            break;
                        case DicomTags.PerformedProcedureStepStartDate:
                            SetRangeCondition(criteria.PerformedProcedureStepStartDate, data[DicomTags.PerformedProcedureStepStartDate].GetString(0, ""));
                            break;
                        case DicomTags.PerformedProcedureStepStartTime:
                            SetRangeCondition(criteria.PerformedProcedureStepStartTime, data[DicomTags.PerformedProcedureStepStartTime].GetString(0, ""));
                            break;
                        case DicomTags.RequestAttributesSequence:// todo
                            break;
                        default:
                            break;
                    }
            }

            // Open a second read context, in case other queries are required.
            IReadContext subRead = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();

            try
            {
                find.Find(criteria, delegate(Series row)
                                     {
                                         DicomMessage response = new DicomMessage();
                                         PopulateSeries(subRead, message, response, tagList, row);
                                         server.SendCFindResponse(presentationID, message.MessageId, response,
                                                                  DicomStatuses.Pending);
                                     });
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when processing FIND request.");
                DicomMessage errorResponse = new DicomMessage();
                server.SendCFindResponse(presentationID, message.MessageId, errorResponse,
                                         DicomStatuses.ProcessingFailure);
                subRead.Dispose();
                read.Dispose();
                return true;
            }

            read.Dispose();
            subRead.Dispose();

            DicomMessage finalResponse = new DicomMessage();
            server.SendCFindResponse(presentationID, message.MessageId, finalResponse, DicomStatuses.Success);

            return true;
        }

        private bool CompareInstanceMatch(DicomMessage queryMessage, List<uint> matchTagList, InstanceXml instanceStream)
        {
            foreach (uint tag in matchTagList)
            {
                if (!instanceStream.Collection.Contains(tag))
                    continue;

                DicomAttribute sourceAttrib = queryMessage.DataSet[tag];
                DicomAttribute matchAttrib = instanceStream.Collection[tag];

                if (sourceAttrib.Tag.VR.Equals(DicomVr.SQvr))
                    continue; // TODO

                if (sourceAttrib.IsNull)
                    continue;

                string sourceString = sourceAttrib.ToString();
                if (sourceString.Contains("*") || sourceString.Contains("?"))
                {
                    sourceString = sourceString.Replace("*", "[\x21-\x7E]");
                    sourceString = sourceString.Replace("?", ".");
                    if (!Regex.IsMatch(matchAttrib.ToString(), sourceString))
                        return false;                    
                }
                else
                    if (!sourceAttrib.Equals(matchAttrib))
                        return false;
            }

            return true;    
        }

        /// <summary>
        /// Method for processing Image level queries.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="association"></param>
        /// <param name="presentationID"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool OnReceiveImageLevelQuery(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
            List<uint> tagList = new List<uint>();
            List<uint> matchingTagList = new List<uint>();

            DicomAttributeCollection data = message.DataSet;
            string studyInstanceUid = data[DicomTags.StudyInstanceUid].GetString(0, String.Empty);
            string seriesInstanceUid = data[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);

            StudyStorageLocation location;
            if (false == GetStudyStorageLocation(studyInstanceUid,out location))
            {
                
            }

            StudyXml theStream = LoadStudyStream(location);

            SeriesXml theSeriesStream = theStream[seriesInstanceUid];

            foreach (DicomAttribute attrib in message.DataSet)
            {
                tagList.Add(attrib.Tag.TagValue);
                if (!attrib.IsNull)
                    matchingTagList.Add(attrib.Tag.TagValue);
            }

            foreach (InstanceXml theInstanceStream in theSeriesStream)
            {
                if (CompareInstanceMatch(message,matchingTagList, theInstanceStream))
                {
                    DicomMessage response = new DicomMessage();
                    PopulateInstance(message, response, tagList, theInstanceStream);
                    server.SendCFindResponse(presentationID, message.MessageId, response,
                                             DicomStatuses.Pending);
                }

            }
           
            DicomMessage finalResponse = new DicomMessage();
            server.SendCFindResponse(presentationID, message.MessageId, finalResponse, DicomStatuses.Success);

            return true;
        }

        #region IDicomScp Members

        /// <summary>
        /// Extension method called when a new DICOM Request message has been called that the 
        /// extension will process.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="association"></param>
        /// <param name="presentationID"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool OnReceiveRequest(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
            String level = message.DataSet[DicomTags.QueryRetrieveLevel].GetString(0, "");
           
            if (message.AffectedSopClassUid.Equals(SopClass.StudyRootQueryRetrieveInformationModelFindUid))
            {
                if (level.Equals("STUDY"))
                {
                    return OnReceiveStudyLevelQuery(server, association, presentationID, message);
                }
                else if (level.Equals("SERIES"))
                {
                    return OnReceiveSeriesLevelQuery(server, association, presentationID, message);
                }
                else if (level.Equals("IMAGE"))
                {
                    return OnReceiveImageLevelQuery(server, association, presentationID, message);
                }
                else
                {
                    Platform.Log(LogLevel.Error, "Unexpected Study Root Query/Retrieve level: {0}", level);

                    server.SendCFindResponse(presentationID, message.MessageId, new DicomMessage(),
                                             DicomStatuses.QueryRetrieveIdentifierDoesNotMatchSOPClass);
                    return true;
                }
            }
            else if (message.AffectedSopClassUid.Equals(SopClass.PatientRootQueryRetrieveInformationModelFindUid))
            {
                if (level.Equals("PATIENT"))
                {
                    return OnReceivePatientQuery(server, association, presentationID, message);
                }
                else if (level.Equals("STUDY"))
                {
                    return OnReceiveStudyLevelQuery(server, association, presentationID, message);
                }
                else if (level.Equals("SERIES"))
                {
                    return OnReceiveSeriesLevelQuery(server, association, presentationID, message);
                }
                else if (level.Equals("IMAGE"))
                {
                    return OnReceiveImageLevelQuery(server, association, presentationID, message);
                }
                else
                {
                    Platform.Log(LogLevel.Error, "Unexpected Patient Root Query/Retrieve level: {0}", level);

                    server.SendCFindResponse(presentationID, message.MessageId, new DicomMessage(),
                                             DicomStatuses.QueryRetrieveIdentifierDoesNotMatchSOPClass);
                    return true;
                }
            }

            // Not supported message type, send a failure status.
            server.SendCFindResponse(presentationID, message.MessageId, new DicomMessage(),
                                         DicomStatuses.QueryRetrieveIdentifierDoesNotMatchSOPClass);           
            return true;
        }

        /// <summary>
        /// Extension method called to get a list of the SOP Classes and transfer syntaxes supported by the extension.
        /// </summary>
        /// <returns></returns>
        public override IList<SupportedSop> GetSupportedSopClasses()
        {
            return _list;
        }

        #endregion
    }
}
