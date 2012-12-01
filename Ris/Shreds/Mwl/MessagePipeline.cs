#region License

//MWL Support for Clear Canvas RIS
//Copyright (C)  2012 Archibald Archibaldovitch

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

using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Shreds.Mwl
{
    public interface IConvertMessageContext
    {
        DicomMessage Message { get; }
    }

    public interface IConvertResultContext : IConvertMessageContext
    {
        MwlWorklistItem Item { get; }
    }

    public class MessagePipeline
    {
        static MessagePipeline()
        {
            StaticConverters =
                new List<IAttributeConverter>
                    {
                        new SequenceConverter(DicomTags.ScheduledProcedureStepSequence, DicomDataElementType.Type1),
                        new ScheduledProcedureStepStartDateConverter(),
                        new SequenceConverter(DicomTags.ScheduledProcedureStepSequence,
                                              DicomTags.ScheduledProcedureStepStartTime, DicomDataElementType.Type1,
                                              item =>
                                              item.Time == null
                                                  ? null
                                                  : item.Time.Value.ToString(AttributeConverter.DicomTimeFormat)),
                        new ScheduledProcedureStepModalityConverter(),
                        new ScheduledProcedureStepDescriptionConverter(),
                        new SequenceConverter(DicomTags.ScheduledProcedureStepSequence,
                                              DicomTags.ScheduledProcedureStepId, DicomDataElementType.Type1, 16,
                                              item => item.ProcedureStepId),
                        new SequenceConverter(DicomTags.ScheduledProcedureStepSequence,
                                              DicomTags.ScheduledPerformingPhysiciansName, DicomDataElementType.Type2),
                        new SequenceConverter(DicomTags.ScheduledProcedureStepSequence, DicomTags.ScheduledStationName,
                                              DicomDataElementType.Type2),
                        new SequenceConverter(DicomTags.ScheduledProcedureStepSequence,
                                              DicomTags.ScheduledProcedureStepLocation, DicomDataElementType.Type2),
                        new AttributeConverter(DicomTags.RequestedProcedureId, DicomDataElementType.Type1, 16,
                                               item => item.ProcedureNumber),
                        new RequestedProcedureDescriptionConverter(),
                        new AttributeConverter(DicomTags.StudyInstanceUid, DicomDataElementType.Type1, 64,
                                               item =>item.StudyInstanceUID),
                        new SequenceConverter(DicomTags.ReferencedStudySequence, DicomDataElementType.Type2),
                        new AttributeConverter(DicomTags.RequestedProcedurePriority, DicomDataElementType.Type2),
                        new AttributeConverter(DicomTags.PatientTransportArrangements, DicomDataElementType.Type2),
                        new AttributeConverter(DicomTags.AccessionNumber, DicomDataElementType.Type2,
                                               item => item.AccessionNumber),
                        new AttributeConverter(DicomTags.RequestingPhysician, DicomDataElementType.Type2),
                        new AttributeConverter(
                            DicomTags.ReferringPhysiciansName, DicomDataElementType.Type2,
                            item => AttributeConverter.GeneratetDicomPersonName(item.OrderingPractitionerName)),
                        new AttributeConverter(
                            DicomTags.AdmissionId, DicomDataElementType.Type2,
                            item => item.VisitNumber == null ? null : item.VisitNumber.Id),
                        new AttributeConverter(DicomTags.CurrentPatientLocation, DicomDataElementType.Type2,
                                               item => item.VisitLocationPointOfCare),
                        new SequenceConverter(DicomTags.ReferencedPatientSequence, DicomDataElementType.Type2),
                        new PatientsNameConverter(),
                        new PatientIdConverter(),
                        new AttributeConverter(DicomTags.PatientsBirthDate, DicomDataElementType.Type2,
                                               item =>
                                               item.PatientDateOfBirth == null
                                                   ? null
                                                   : DateParser.ToDicomString(item.PatientDateOfBirth.Value)
                            ),
                        new AttributeConverter(DicomTags.PatientsSex, DicomDataElementType.Type2,
                                               item => item.PatientSex.ToString()),
                        new AttributeConverter(DicomTags.PatientsWeight, DicomDataElementType.Type2),
                        new AttributeConverter(DicomTags.ConfidentialityConstraintOnPatientDataDescription,
                                               DicomDataElementType.Type2),
                        new AttributeConverter(DicomTags.PatientState, DicomDataElementType.Type2),
                        new AttributeConverter(DicomTags.PregnancyStatus, DicomDataElementType.Type1),
                        new AttributeConverter(DicomTags.MedicalAlerts, DicomDataElementType.Type2),
                        new AttributeConverter(DicomTags.Allergies, DicomDataElementType.Type2),
                        new AttributeConverter(DicomTags.SpecialNeeds, DicomDataElementType.Type2),
                        new AttributeConverter(DicomTags.FillerOrderNumberImagingServiceRequest,
                                               DicomDataElementType.Type3,
                                               item => item.AccessionNumber),
                        new AttributeConverter(DicomTags.InstitutionName, DicomDataElementType.Type3,
                                               item => item.PerformingFacilityCode)
                    };
        }


        public IList<MwlWorklistItemSearchCriteria> ConvertMessage(IList<MwlWorklistItemSearchCriteria> criteria,
                                                                   IConvertMessageContext context)
        {
            _scheduledStationAeTitleConverter = new SequenceConverter(DicomTags.ScheduledStationAeTitle,
                                                                      DicomDataElementType.Type1, 16,
                                                                      item =>
                                                                      GetScheduledStationAeTitle(context.Message));

            var returnCriteria = new List<MwlWorklistItemSearchCriteria>(criteria);
            if (returnCriteria.Count == 0)
                returnCriteria.Add(new MwlWorklistItemSearchCriteria());

            foreach (var c in returnCriteria)
            {
                foreach (var handler in Converters)
                {
                    handler.GenerateSearchCriteria(handler.Extract(context.Message).GetString(0, ""), c);
                }
            }

            return returnCriteria;
        }

        public virtual DicomMessage ConvertResult(DicomMessage message, IConvertResultContext context)
        {
            var result = message;
            foreach (var handler in Converters)
            {
                handler.GenerateResponseMessage(context.Item, result);
            }

            return result;
        }

        private static readonly List<IAttributeConverter> StaticConverters;


        private SequenceConverter _scheduledStationAeTitleConverter;

        protected List<IAttributeConverter> Converters
        {
            get
            {
                return new List<IAttributeConverter>(StaticConverters)
                           {
                               _scheduledStationAeTitleConverter
                           };
            }
        }

        private string GetScheduledStationAeTitle(DicomMessageBase message)
        {
            DicomAttribute matchedAttribute;
            GetAttribute(message.DataSet, DicomTags.ScheduledStationAeTitle, out matchedAttribute);
            return matchedAttribute != null ? matchedAttribute.GetString(0, "") : null;
        }


        private bool GetAttribute(IEnumerable<DicomAttribute> attributes, uint desiredTag,
                                  out DicomAttribute matchedAttribute)
        {
            foreach (var attr in attributes)
            {
                if (attr.IsEmpty)
                    continue;

                if (attr.Tag.TagValue == desiredTag)
                {
                    matchedAttribute = attr;
                    return true;
                }

                var sqAttr = attr as DicomAttributeSQ;
                if (sqAttr != null)
                {
                    for (var i = 0; i < sqAttr.Count; i++)
                    {
                        var sqItem = sqAttr[i];
                        if (sqItem == null || sqItem.IsEmpty())
                            continue;

                        if (GetAttribute(sqItem, desiredTag, out matchedAttribute))
                            return true;
                    }
                }
            }
            matchedAttribute = null;
            return false;
        }
    }
}