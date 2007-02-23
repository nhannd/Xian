using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Registration
{
    [ExtensionPoint]
    public class WorklistExtensionPoint : ExtensionPoint<IWorklist>
    {
    }

    public class Worklists
    {
        public abstract class RegistrationWorklist : Worklist
        {
            public override IList GetWorklist(IPersistenceContext context, SearchCriteria additionalCriteria)
            {
                return (IList)Worklists.ConvertQueryResultsToWorkLists(this.GetType().ToString(), GetQueryResultsHelper(context, additionalCriteria as PatientProfileSearchCriteria));
            }

            public override IList GetQueryResultForWorklistItem(IPersistenceContext context, IWorklistItem item)
            {
                return (IList)GetQueryResultsHelper(context, new PatientProfileSearchCriteria((item as WorklistItem).PatientProfile));
            }

            public virtual IList<WorklistQueryResult> GetQueryResultsHelper(IPersistenceContext context, PatientProfileSearchCriteria profileCriteria)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return broker.GetWorklist(SearchCriteria, profileCriteria);
            }        
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Scheduled : RegistrationWorklist
        {
            public Scheduled() 
                : base()
            {
                this.SearchCriteria = new ModalityProcedureStepSearchCriteria();
                //this.SearchCriteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
                ((ModalityProcedureStepSearchCriteria)SearchCriteria).State.EqualTo(ActivityStatus.SC);
            }

            public override IList GetWorklist(IPersistenceContext context, SearchCriteria additionalCriteria)
            {
                CheckIn checkInWorklist = new CheckIn();
                PatientProfileSearchCriteria profileCriteria = additionalCriteria as PatientProfileSearchCriteria;

                return (IList) Worklists.ConvertQueryResultsToWorkLists(this.GetType().ToString(), Worklists.FilterOutCheckInResults(
                    GetQueryResultsHelper(context, profileCriteria),
                    checkInWorklist.GetQueryResultsHelper(context, profileCriteria)));
            }

            public override IList GetQueryResultForWorklistItem(IPersistenceContext context, IWorklistItem item)
            {
                CheckIn checkInWorklist = new CheckIn();
                PatientProfileSearchCriteria profileCriteria = new PatientProfileSearchCriteria((item as WorklistItem).PatientProfile);

                return (IList) Worklists.FilterOutCheckInResults(
                    GetQueryResultsHelper(context, profileCriteria), 
                    checkInWorklist.GetQueryResultsHelper(context, profileCriteria));
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class CheckIn : RegistrationWorklist
        {
            public CheckIn() 
                : base()
            {
                this.SearchCriteria = new CheckInProcedureStepSearchCriteria();
                //(this.SearchCriteria as CheckInProcedureStepSearchCriteria).Procedure.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class InProgress : RegistrationWorklist
        {
            public InProgress() 
                : base()
            {
                this.SearchCriteria = new ModalityProcedureStepSearchCriteria();
                //SearchCriteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
                ((ModalityProcedureStepSearchCriteria)SearchCriteria).State.EqualTo(ActivityStatus.IP);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Completed : RegistrationWorklist
        {
            public Completed() 
                : base()
            {
                this.SearchCriteria = new ModalityProcedureStepSearchCriteria();
                //SearchCriteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
                ((ModalityProcedureStepSearchCriteria)SearchCriteria).State.EqualTo(ActivityStatus.CM);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Cancelled : RegistrationWorklist
        {
            public Cancelled() 
                : base()
            {
                this.SearchCriteria = new ModalityProcedureStepSearchCriteria();
                //SearchCriteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
                ((ModalityProcedureStepSearchCriteria)SearchCriteria).State.EqualTo(ActivityStatus.DC);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Search : RegistrationWorklist
        {
            public Search() 
                : base()
            {
                this.SearchCriteria = new PatientProfileSearchCriteria();
            }

            public override IList GetQueryResultForWorklistItem(IPersistenceContext context, IWorklistItem item)
            {
                Scheduled scheduledWorklist = new Scheduled();
                return scheduledWorklist.GetQueryResultForWorklistItem(context, item);
            }

            public override IList<WorklistQueryResult> GetQueryResultsHelper(IPersistenceContext context, PatientProfileSearchCriteria profileCriteria)
            {
                if (profileCriteria != null)
                    this.SearchCriteria = profileCriteria;

                if (this.SearchCriteria == null)
                    return new List<WorklistQueryResult>();

                IPatientProfileBroker broker = context.GetBroker<IPatientProfileBroker>();
                return Worklists.ConvertProfilesToQueryResults(broker.Find(this.SearchCriteria as PatientProfileSearchCriteria));            
            }
        }

        #region WorklistItem filtering & conversion functions

        public static IList<WorklistQueryResult> FilterOutCheckInResults(IList<WorklistQueryResult> queryResults, IList<WorklistQueryResult> checkInQueryResult)
        {
            IList<WorklistQueryResult> finalList = new List<WorklistQueryResult>();

            IDictionary<EntityRef<RequestedProcedure>, WorklistQueryResult> checkInDictionary = new Dictionary<EntityRef<RequestedProcedure>, WorklistQueryResult>();
            foreach (WorklistQueryResult queryResult in checkInQueryResult)
            {
                if (checkInDictionary.ContainsKey(queryResult.RequestedProcedure) == false)
                    checkInDictionary[queryResult.RequestedProcedure] = queryResult;
            }

            foreach (WorklistQueryResult queryResult in queryResults)
            {
                if (checkInDictionary.ContainsKey(queryResult.RequestedProcedure) == false)
                    finalList.Add(queryResult);
            }
            
            return finalList;
        }

        public static IList<WorklistItem> ConvertQueryResultsToWorkLists(string workClassName, IList<WorklistQueryResult> listQueryResult)
        {
            // Group the query results based on patient profile into a Registration Worklist item
            IDictionary<EntityRef<PatientProfile>, WorklistItem> worklistDictionary = new Dictionary<EntityRef<PatientProfile>, WorklistItem>();
            foreach (WorklistQueryResult queryResult in listQueryResult)
            {
                if (worklistDictionary.ContainsKey(queryResult.PatientProfile) == false)
                    worklistDictionary[queryResult.PatientProfile] = new WorklistItem(workClassName, queryResult);
            }

            // Now convert to worklist
            IList<WorklistItem> listItems = new List<WorklistItem>();
            foreach (KeyValuePair<EntityRef<PatientProfile>, WorklistItem> kvp in worklistDictionary)
            {
                listItems.Add(kvp.Value);
            }

            return listItems;
        }

        public static IList<WorklistQueryResult> ConvertProfilesToQueryResults(IList<PatientProfile> listProfile)
        {
            // Now add the worklist item 
            IList<WorklistQueryResult> items = new List<WorklistQueryResult>();
            foreach (PatientProfile profile in listProfile)
            {
                items.Add(new WorklistQueryResult(profile));
            }

            return items;
        }

        #endregion
    }
}
