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

using System;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare
{
    public class MwlWorklistItem : WorklistItem
    {
        private class EntityRefField : WorklistItemField
        {
            internal EntityRefField(WorklistItemFieldLevel level)
                : base(level)
            {
            }

            public override bool IsEntityRefField
            {
                get { return true; }
            }
        }

        public static readonly WorklistItemField PatientDateOfBirthField =
            new WorklistItemField(WorklistItemFieldLevel.Patient);

        public static readonly WorklistItemField PatientSexField = new WorklistItemField(WorklistItemFieldLevel.Patient);

        public static readonly WorklistItemField OrderingPractitionerNameField =
            new WorklistItemField(WorklistItemFieldLevel.Procedure);

        public static readonly WorklistItemField VisitNumberField =
            new WorklistItemField(WorklistItemFieldLevel.Procedure);

        public static readonly WorklistItemField VisitLocationPointOfCareField =
            new WorklistItemField(WorklistItemFieldLevel.Procedure);

        public static readonly WorklistItemField ProcedureTypeIdField =
            new WorklistItemField(WorklistItemFieldLevel.Procedure);

        public static readonly WorklistItemField PerformingFacilityCodeField =
            new WorklistItemField(WorklistItemFieldLevel.Procedure);

        public static readonly WorklistItemField ModalityField = new EntityRefField(WorklistItemFieldLevel.ProcedureStep);

        public static readonly WorklistItemField DicomModalityField =
            new WorklistItemField(WorklistItemFieldLevel.ProcedureStep);

        public static readonly WorklistItemField StudyInstanceUIDField 
              = new WorklistItemField(WorklistItemFieldLevel.Procedure);

        public static readonly WorklistItemField ProcedureNumberField
                  = new WorklistItemField(WorklistItemFieldLevel.Procedure);

        private static readonly Dictionary<WorklistItemField, WorklistItemFieldSetterDelegate> FieldSetters =
            new Dictionary<WorklistItemField, WorklistItemFieldSetterDelegate>();

        static MwlWorklistItem()
        {
            FieldSetters.Add(PatientDateOfBirthField,
                             (item, value) => ((MwlWorklistItem) item).PatientDateOfBirth = (DateTime?) value);
            FieldSetters.Add(PatientSexField, (item, value) => ((MwlWorklistItem) item).PatientSex = (Sex) value);
            FieldSetters.Add(OrderingPractitionerNameField,
                             (item, value) => ((MwlWorklistItem) item).OrderingPractitionerName = (PersonName) value);
            FieldSetters.Add(VisitNumberField,
                             (item, value) => ((MwlWorklistItem) item).VisitNumber = (VisitNumber) value);
            FieldSetters.Add(VisitLocationPointOfCareField,
                             (item, value) => ((MwlWorklistItem) item).VisitLocationPointOfCare = (string) value);
            FieldSetters.Add(ProcedureTypeIdField,
                             (item, value) => ((MwlWorklistItem) item).ProcedureTypeId = (string) value);
            FieldSetters.Add(PerformingFacilityCodeField,
                             (item, value) => ((MwlWorklistItem) item).PerformingFacilityCode = (string) value);
            FieldSetters.Add(ModalityField, (item, value) => ((MwlWorklistItem) item).ModalityRef = (EntityRef) value);
            FieldSetters.Add(DicomModalityField,
                             (item, value) => ((MwlWorklistItem) item).DicomModality = (DicomModalityEnum) value);
            FieldSetters.Add(StudyInstanceUIDField,
                             (item, value) => ((MwlWorklistItem)item).StudyInstanceUID = (string)value);

            FieldSetters.Add(ProcedureNumberField,
                 (item, value) => ((MwlWorklistItem)item).ProcedureNumber = (string)value);

            FieldSetters.Add(WorklistItemField.ProcedureStepId,
                 (item, value) => ((MwlWorklistItem)item).ProcedureStepId = (string)value);
        }


        public MwlWorklistItem()
        {
        }

        public DateTime? PatientDateOfBirth { get; private set; }
        public Sex PatientSex { get; private set; }
        public VisitNumber VisitNumber { get; private set; }
        public string VisitLocationPointOfCare { get; private set; }
        public string ProcedureTypeId { get; private set; }
        public PersonName OrderingPractitionerName { get; private set; }
        public string PerformingFacilityCode { get; private set; }
        public EntityRef ModalityRef { get; private set; }
        public DicomModalityEnum DicomModality { get; private set; }
        public string StudyInstanceUID { get; private set; }
        public string ProcedureNumber { get; private set; }
        public string ProcedureStepId { get; private set; }

        //try to get field setter from this derived class before dropping back to base field setters
        protected override WorklistItemFieldSetterDelegate GetFieldSetter(WorklistItemField field)
        {
            WorklistItemFieldSetterDelegate fieldSetter;
            return FieldSetters.TryGetValue(field, out fieldSetter) ? fieldSetter : base.GetFieldSetter(field);
        }
    }


    public static class MwlWorklistItemProjection
    {
        public static readonly WorklistItemProjection Projection;
        static MwlWorklistItemProjection()
        {
            Projection = new WorklistItemProjection(
                new[] {
					WorklistItemField.ProcedureStep,
					WorklistItemField.ProcedureStepName,
					WorklistItemField.ProcedureStepState,
					WorklistItemField.Procedure,
					WorklistItemField.Order,
					WorklistItemField.Patient,
					WorklistItemField.PatientProfile,
					WorklistItemField.Mrn,
					WorklistItemField.PatientName,
					WorklistItemField.AccessionNumber,
					WorklistItemField.ProcedureTypeName,
					WorklistItemField.ProcedurePortable,
					WorklistItemField.ProcedureLaterality,
					WorklistItemField.ProcedureStepScheduledStartTime,

                    MwlWorklistItem.OrderingPractitionerNameField,
					MwlWorklistItem.VisitNumberField,
					MwlWorklistItem.VisitLocationPointOfCareField,
					MwlWorklistItem.ProcedureTypeIdField,
                    MwlWorklistItem.PatientDateOfBirthField,
					MwlWorklistItem.PatientSexField,
					MwlWorklistItem.PerformingFacilityCodeField,
					MwlWorklistItem.ModalityField,
					MwlWorklistItem.DicomModalityField,
                    MwlWorklistItem.StudyInstanceUIDField,
                    MwlWorklistItem.ProcedureNumberField,
                    WorklistItemField.ProcedureStepId
					});
        }
    }
}