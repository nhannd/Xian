#region License

//MPPS Support for Clear Canvas RIS
//Copyright (C)  2012 Aaron Boxer

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Ris.Application.Services;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.MPPS
{
    public class MPPS
    {
        private const string PerformedProcedureStepStatusInProgress = "IN PROGRESS";
        private const string PerformedProcedureStepStatusCompleted = "COMPLETED";
        private const string PerformedProcedureStepStatusDiscontinued = "DISCONTINUED";
        private const string AffectedSopInstanceUIDKey = "AffectedSopInstanceUIDKey";

        public DicomMessage Create(DicomMessage message)
        {
            var responseMessage = new DicomMessage();

            //ToDo if not currently scheduled, then we need to create the procedure

            string spsId = null;
            var spsSeq =
                message.DataSet[DicomTagDictionary.GetDicomTag(DicomTags.ScheduledProcedureStepSequence).TagValue] as
                DicomAttributeSQ;
            if (spsSeq != null && spsSeq.Values != null)
            {
                var spsIdAttribute =
                    spsSeq[0][DicomTagDictionary.GetDicomTag(DicomTags.ScheduledProcedureStepId).TagValue];
                if (spsIdAttribute != null)
                    spsIdAttribute.TryGetString(0, out spsId);
            }

            if (spsId != null)
            {
                var searchCriteria = new ModalityProcedureStepSearchCriteria();
                searchCriteria.ProcedureStepId.EqualTo(spsId);

                using (var scope = new PersistenceScope(PersistenceContextType.Read))
                {
                    var mps =
                        PersistenceScope.CurrentContext.GetBroker<IModalityProcedureStepBroker>().FindOne(searchCriteria);
                    if (mps != null && mps.GetRef() != null)
                    {
                        //ignore completed or discontinued procedure step
                        if (!mps.State.Equals(ActivityStatus.CM) && !mps.State.Equals(ActivityStatus.DC))
                        {
                            if (string.IsNullOrEmpty(message.AffectedSopInstanceUid))
                                throw new Exception("MPPS: N-CREATE: type 1 field AffectedSopInstanceUid is missing");
                            var staffName =
                                message.DataSet[DicomTagDictionary.GetDicomTag(DicomTags.OperatorsName).TagValue].
                                    GetString(0, "");
                            var staff = SearchForStaff(staffName, scope.Context);
                            if (staff == null)
                                throw new Exception("MPPS N-CREATE: unrecognized staff member");

                            var startTime = message.DataSet[DicomTagDictionary.GetDicomTag(DicomTags.PerformedProcedureStepStartTime).TagValue].
                                    GetString(0, "");

                            var op = new StartModalityProcedureStepsOperation();
                            var mpps = op.Execute(new List<ModalityProcedureStep> {mps}, DateParser.Parse(startTime), staff,
                                                  new PersistentWorkflow(scope.Context));

                            mpps.ExtendedProperties.Add(AffectedSopInstanceUIDKey, message.AffectedSopInstanceUid);
                        }
                        else
                        {
                            throw new Exception("MPPS: N-CREATE called on modality procedure step that is already complete or discontinued");
                        }
                    }
                    scope.Complete();
                }
            }
            return responseMessage;
        }

        public DicomMessage Set(DicomMessage message)
        {
            var responseMessage = new DicomMessage();
            string spsId = null;
            var spsSeq =
                message.DataSet[DicomTagDictionary.GetDicomTag(DicomTags.ScheduledProcedureStepSequence).TagValue] as
                DicomAttributeSQ;
            if (spsSeq != null && spsSeq.Values != null)
            {
                var spsIdAttribute =
                    spsSeq[0][DicomTagDictionary.GetDicomTag(DicomTags.ScheduledProcedureStepId).TagValue];
                if (spsIdAttribute != null)
                    spsIdAttribute.TryGetString(0, out spsId);
            }

            if (spsId != null)
            {
                var searchCriteria = new ModalityProcedureStepSearchCriteria();
                searchCriteria.ProcedureStepId.EqualTo(spsId);

                using (var scope = new PersistenceScope(PersistenceContextType.Read))
                {
                    var mps = scope.Context.GetBroker<IModalityProcedureStepBroker>().FindOne(searchCriteria);

                    //get referenced MPPS 
                    var requestedSopInstanceUid = message.RequestedSopInstanceUid;
                    var requestedMpps = mps.PerformedSteps.Select(step => step as ModalityPerformedProcedureStep).FirstOrDefault(
                        mpps => mpps != null &&
                                  mpps.ExtendedProperties.ContainsKey(AffectedSopInstanceUIDKey) &&
                                  (mpps.ExtendedProperties[AffectedSopInstanceUIDKey]).Equals(
                                      requestedSopInstanceUid));


                    if (requestedMpps != null && requestedMpps.GetRef() != null)
                    {
                        var statusAttr =
                            message.DataSet[
                                DicomTagDictionary.GetDicomTag(DicomTags.PerformedProcedureStepStatus).TagValue];
                        var status = statusAttr.GetString(0, "").ToUpper();

                        if (status.Equals(PerformedProcedureStepStatusInProgress))
                        {
                            
                        }

                        else if (status.Equals(PerformedProcedureStepStatusDiscontinued))
                        {
                            var endTime = message.DataSet[DicomTagDictionary.GetDicomTag(DicomTags.PerformedProcedureStepEndTime).TagValue].
                            GetString(0, "");
                            var op = new DiscontinueModalityPerformedProcedureStepOperation();
                            op.Execute(requestedMpps, DateParser.Parse(endTime), new PersistentWorkflow(scope.Context));
                        }
                        else if (status.Equals(PerformedProcedureStepStatusCompleted))
                        {
                            var endTime = message.DataSet[DicomTagDictionary.GetDicomTag(DicomTags.PerformedProcedureStepEndTime).TagValue].
                            GetString(0, "");
                            var op = new CompleteModalityPerformedProcedureStepOperation();
                            op.Execute(requestedMpps, DateParser.Parse(endTime), new PersistentWorkflow(scope.Context));
                        }
                    }
                    else
                    {
                        throw new Exception("MPPS: N-SET:  unable to find referenced modality performed procedure step");
                    }
                    scope.Complete();
                }
            }
            return responseMessage;
        }

        private static Staff SearchForStaff(string staffId, IPersistenceContext context)
        {
            try
            {
                if (string.IsNullOrEmpty(staffId))
                    return null;

                var criteria = new StaffSearchCriteria();
                criteria.Id.EqualTo(staffId);

                var broker = context.GetBroker<IStaffBroker>();
                return broker.FindOne(criteria);
            }
            catch (EntityNotFoundException)
            {
                return null;
            }
        }
    }
}