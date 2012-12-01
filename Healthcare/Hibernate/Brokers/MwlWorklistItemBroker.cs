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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public class MwlQueryBuilder : QueryBuilderBase
    {
        public static class MwlHqlConstants
        {
            //joins /////////////////////////////////////////////////////////////////////////////////////////////////////
            private static readonly HqlJoin JoinVisitLocation = new HqlJoin("v.CurrentLocation", "l", HqlJoinMode.Left);

            private static readonly HqlJoin JoinModality = new HqlJoin("ps.Modality", "mod", HqlJoinMode.Left);

            private static readonly HqlJoin JoinPerformingFacility = new HqlJoin("rp.PerformingFacility", "f",
                                                                                 HqlJoinMode.Left);

            private static readonly HqlJoin JoinOrderingPractitioner = new HqlJoin("o.OrderingPractitioner", "xp",
                                                                                   HqlJoinMode.Left);

            /////////////////////////////////////////////////////////////////////////////////////////////////////


            public static readonly HqlFrom FromModalityProcedureStep =
                new HqlFrom("ModalityProcedureStep", "ps",
                            new[]
                                {
                                    HqlConstants.JoinProcedure,
                                    HqlConstants.JoinProcedureType,
                                    HqlConstants.JoinOrder,
                                    HqlConstants.JoinDiagnosticService,
                                    HqlConstants.JoinVisit,
                                    HqlConstants.JoinPatient,
                                    HqlConstants.JoinPatientProfile,
                                    JoinVisitLocation,
                                    JoinModality,
                                    JoinPerformingFacility,
                                    JoinOrderingPractitioner
                                });


            // selects /////////////////////////////////////////////////////////////////////////////////////
            private static readonly HqlSelect SelectPatientDateOfBirth = new HqlSelect("pp.DateOfBirth");

            private static readonly HqlSelect SelectPatientSex = new HqlSelect("pp.Sex");

            private static readonly HqlSelect SelectOrderingPractitionerName = new HqlSelect("xp.Name");

            private static readonly HqlSelect SelectVisitNumber = new HqlSelect("v.VisitNumber");

            private static readonly HqlSelect SelectVisitLocationPointOfCare = new HqlSelect("l.PointOfCare");

            private static readonly HqlSelect SelectProcedureTypeId = new HqlSelect("rpt.Id");

            private static readonly HqlSelect SelectPerformingFacilityCode = new HqlSelect("f.Code");

            private static readonly HqlSelect SelectModality = new HqlSelect("mod");

            private static readonly HqlSelect SelectDicomModality = new HqlSelect("mod.DicomModality");

            private static readonly HqlSelect SelectStudyInstanceUID = new HqlSelect("rp.StudyInstanceUID");

            private static readonly HqlSelect SelectProcedureNumber = new HqlSelect("rp.Number");
            /////////////////////////////////////////////////////////////////////////////////////////////////////


            static MwlHqlConstants()
            {
                HqlConstants.MapCriteriaKeyToHql.Add("Modality", "mod");

                HqlConstants.MapWorklistItemFieldToHqlSelect.Add(MwlWorklistItem.PatientDateOfBirthField,
                                                                 SelectPatientDateOfBirth);

                HqlConstants.MapWorklistItemFieldToHqlSelect.Add(MwlWorklistItem.PatientSexField, SelectPatientSex);

                HqlConstants.MapWorklistItemFieldToHqlSelect.Add(MwlWorklistItem.OrderingPractitionerNameField,
                                                                 SelectOrderingPractitionerName);

                HqlConstants.MapWorklistItemFieldToHqlSelect.Add(MwlWorklistItem.VisitNumberField, SelectVisitNumber);

                HqlConstants.MapWorklistItemFieldToHqlSelect.Add(MwlWorklistItem.VisitLocationPointOfCareField,
                                                                 SelectVisitLocationPointOfCare);


                HqlConstants.MapWorklistItemFieldToHqlSelect.Add(MwlWorklistItem.PerformingFacilityCodeField,
                                                                 SelectPerformingFacilityCode);


                HqlConstants.MapWorklistItemFieldToHqlSelect.Add(MwlWorklistItem.ProcedureTypeIdField,
                                                                 SelectProcedureTypeId);

                HqlConstants.MapWorklistItemFieldToHqlSelect.Add(MwlWorklistItem.ModalityField, SelectModality);

                HqlConstants.MapWorklistItemFieldToHqlSelect.Add(MwlWorklistItem.DicomModalityField, SelectDicomModality);

                HqlConstants.MapWorklistItemFieldToHqlSelect.Add(MwlWorklistItem.StudyInstanceUIDField, SelectStudyInstanceUID);

                HqlConstants.MapWorklistItemFieldToHqlSelect.Add(MwlWorklistItem.ProcedureNumberField, SelectProcedureNumber);
            }
        }

        public override void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args)
        {
            query.Froms.Add(MwlHqlConstants.FromModalityProcedureStep);
        }

        public override void AddConstrainPatientProfile(HqlProjectionQuery query, QueryBuilderArgs args)
        {
            query.Conditions.Add(HqlConstants.ConditionConstrainPatientProfile);
        }

        public override void AddCriteria(HqlProjectionQuery query, QueryBuilderArgs args)
        {
            base.AddCriteria(query, args);

            //only query active procedure steps
            query.Conditions.Add(HqlConstants.ConditionActiveProcedureStep);
        }
    }


    [ExtensionOf(typeof (BrokerExtensionPoint))]
    public class MwlWorklistItemBroker : Broker, IMwlWorklistItemBroker
    {
        public MwlWorklistItemBroker()
        {
            _queryBuilder = new MwlQueryBuilder();
        }

        public IList<MwlWorklistItem> GetItems(MwlWorklistItemSearchCriteria[] criteria,
                                               WorklistItemProjection projection, IMwlGetItemsContext context)
        {
            var args = new QueryBuilderArgs(_procedureStepClasses, criteria, projection, context.ResultPage);
            var tuples = ExecuteHql<object[]>(GenerateQuery(args));
            var tupleMapping = new MwlWorklistItem().GetTupleMapping(args.Projection);
            return tuples.Select(tup => CreateItem(_queryBuilder.PreProcessResult(tup, args), tupleMapping)).ToList();
        }

        public int GetCount(MwlWorklistItemSearchCriteria[] criteria)
        {
            var args = new QueryBuilderArgs(_procedureStepClasses, criteria, null, null);
            return (int) ExecuteHqlUnique<long>(GenerateQuery(args));
        }

        private readonly Type[] _procedureStepClasses = new[] {typeof (ModalityProcedureStep)};
        private readonly IQueryBuilder _queryBuilder;

        private HqlProjectionQuery GenerateQuery(QueryBuilderArgs args)
        {
            var query = new HqlProjectionQuery();
            _queryBuilder.AddRootQuery(query, args);
            _queryBuilder.AddConstrainPatientProfile(query, args);
            _queryBuilder.AddCriteria(query, args);

            if (args.CountQuery)
            {
                _queryBuilder.AddCountProjection(query, args);
            }
            else
            {
                _queryBuilder.AddItemProjection(query, args);
                _queryBuilder.AddPagingRestriction(query, args);
                _queryBuilder.AddOrdering(query, args);
            }

            return query;
        }

        private MwlWorklistItem CreateItem(object[] tuple, WorklistItem.WorklistItemFieldSetterDelegate[] mapping)
        {
            var item = new MwlWorklistItem();
            item.InitializeFromTuple(tuple, mapping);
            return item;
        }
    }
}