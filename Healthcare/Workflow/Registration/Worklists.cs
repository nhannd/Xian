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
        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Scheduled : Worklist
        {
            private CheckInProcedureStepSearchCriteria checkInCriteria;
            
            public Scheduled() 
                : base()
            {
                this.SearchCriteria = new ModalityProcedureStepSearchCriteria();
                //this.SearchCriteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
                ((ModalityProcedureStepSearchCriteria)SearchCriteria).State.EqualTo(ActivityStatus.SC);

                checkInCriteria = new CheckInProcedureStepSearchCriteria();
                checkInCriteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));            
            }

            public override IList GetWorklist(IPersistenceContext context, SearchCriteria additionalCriteria)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                IList<WorklistQueryResult> queryResults = broker.GetWorklist(SearchCriteria as ModalityProcedureStepSearchCriteria, additionalCriteria as PatientProfileSearchCriteria);
                IList<WorklistQueryResult> filteredResults = Worklists.KeepScheduledAndNotCheckIn(queryResults, checkInCriteria, context);
                return (IList)Worklists.ConvertQueryResultToWorkList(this.GetType().ToString(), filteredResults);

            }

            public override IList GetQueryResultForWorklistItem(IPersistenceContext context, IWorklistItem item)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                IList<WorklistQueryResult> queryResults = broker.GetWorklist(SearchCriteria as ModalityProcedureStepSearchCriteria, new PatientProfileSearchCriteria((item as WorklistItem).PatientProfile));
                return (IList)Worklists.KeepScheduledAndNotCheckIn(queryResults, checkInCriteria, context);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class CheckIn : Worklist
        {
            private CheckInProcedureStepSearchCriteria checkInCriteria;

            public CheckIn() 
                : base()
            {
                this.SearchCriteria = new ModalityProcedureStepSearchCriteria();
                //this.SearchCriteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));

                checkInCriteria = new CheckInProcedureStepSearchCriteria();
                checkInCriteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
            }

            public override IList GetWorklist(IPersistenceContext context, SearchCriteria additionalCriteria)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                IList<WorklistQueryResult> queryResults = broker.GetWorklist(SearchCriteria as ModalityProcedureStepSearchCriteria, additionalCriteria as PatientProfileSearchCriteria);
                IList<WorklistQueryResult> filteredResults = Worklists.KeepScheduledAndCheckIn(queryResults, checkInCriteria, context);
                return (IList)Worklists.ConvertQueryResultToWorkList(this.GetType().ToString(), filteredResults);
            }

            public override IList GetQueryResultForWorklistItem(IPersistenceContext context, IWorklistItem item)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                IList<WorklistQueryResult> queryResults = broker.GetWorklist(SearchCriteria as ModalityProcedureStepSearchCriteria, new PatientProfileSearchCriteria((item as WorklistItem).PatientProfile));
                return (IList)Worklists.KeepScheduledAndCheckIn(queryResults, checkInCriteria, context);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class InProgress : Worklist
        {
            public InProgress() 
                : base()
            {
                this.SearchCriteria = new ModalityProcedureStepSearchCriteria();
                //SearchCriteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
                ((ModalityProcedureStepSearchCriteria)SearchCriteria).State.EqualTo(ActivityStatus.IP);
            }

            public override IList GetWorklist(IPersistenceContext context, SearchCriteria additionalCriteria)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return (IList)Worklists.ConvertQueryResultToWorkList(this.GetType().ToString(), broker.GetWorklist(SearchCriteria as ModalityProcedureStepSearchCriteria, additionalCriteria as PatientProfileSearchCriteria));
            }

            public override IList GetQueryResultForWorklistItem(IPersistenceContext context, IWorklistItem item)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return (IList)broker.GetWorklist(SearchCriteria as ModalityProcedureStepSearchCriteria, new PatientProfileSearchCriteria((item as WorklistItem).PatientProfile));
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Completed : Worklist
        {
            public Completed() 
                : base()
            {
                this.SearchCriteria = new ModalityProcedureStepSearchCriteria();
                //SearchCriteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
                ((ModalityProcedureStepSearchCriteria)SearchCriteria).State.EqualTo(ActivityStatus.CM);
            }

            public override IList GetWorklist(IPersistenceContext context, SearchCriteria additionalCriteria)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return (IList)Worklists.ConvertQueryResultToWorkList(this.GetType().ToString(), broker.GetWorklist(SearchCriteria as ModalityProcedureStepSearchCriteria, additionalCriteria as PatientProfileSearchCriteria));
            }

            public override IList GetQueryResultForWorklistItem(IPersistenceContext context, IWorklistItem item)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return (IList) broker.GetWorklist(SearchCriteria as ModalityProcedureStepSearchCriteria, new PatientProfileSearchCriteria( (item as WorklistItem).PatientProfile ));
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Cancelled : Worklist
        {
            public Cancelled() 
                : base()
            {
                this.SearchCriteria = new ModalityProcedureStepSearchCriteria();
                //SearchCriteria.Scheduling.StartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
                ((ModalityProcedureStepSearchCriteria)SearchCriteria).State.EqualTo(ActivityStatus.DC);
            }

            public override IList GetWorklist(IPersistenceContext context, SearchCriteria additionalCriteria)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return (IList)Worklists.ConvertQueryResultToWorkList(this.GetType().ToString(), broker.GetWorklist(SearchCriteria as ModalityProcedureStepSearchCriteria, additionalCriteria as PatientProfileSearchCriteria));
            }

            public override IList GetQueryResultForWorklistItem(IPersistenceContext context, IWorklistItem item)
            {
                IRegistrationWorklistBroker broker = context.GetBroker<IRegistrationWorklistBroker>();
                return (IList)broker.GetWorklist(SearchCriteria as ModalityProcedureStepSearchCriteria, new PatientProfileSearchCriteria((item as WorklistItem).PatientProfile));
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint))]
        public class Search : Worklist
        {
            public Search() 
                : base()
            {
                this.SearchCriteria = new PatientProfileSearchCriteria();
            }

            public override IList GetWorklist(IPersistenceContext context)
            {
                // We don't want to do a wide search, so return nothing if there is no patient search criteria
                if (SearchCriteria == null)
                    return (IList)(new List<WorklistItem>());

                IPatientProfileBroker broker = context.GetBroker<IPatientProfileBroker>();
                return (IList)Worklists.ConvertToWorkListItem(this.GetType().ToString(), broker.Find(SearchCriteria as PatientProfileSearchCriteria));
            }
        }


        #region WorklistItem filtering & conversion functions

        public static IList<WorklistQueryResult> KeepScheduledAndCheckIn(IList<WorklistQueryResult> unFilteredList, CheckInProcedureStepSearchCriteria checkInCriteria, IPersistenceContext context)
        {
            return FilterCheckInItem(unFilteredList, checkInCriteria, context, true);
        }

        public static IList<WorklistQueryResult> KeepScheduledAndNotCheckIn(IList<WorklistQueryResult> unFilteredList, CheckInProcedureStepSearchCriteria checkInCriteria, IPersistenceContext context)
        {
            return FilterCheckInItem(unFilteredList, checkInCriteria, context, false);
        }

        private static IList<WorklistQueryResult> FilterCheckInItem(IList<WorklistQueryResult> unFilteredList, CheckInProcedureStepSearchCriteria checkInCriteria, IPersistenceContext context, bool keepCheckIn)
          {
            IList<WorklistQueryResult> filteredList = new List<WorklistQueryResult>();
            IDictionary<EntityRef<RequestedProcedure>, WorklistQueryResult> dictionary = new Dictionary<EntityRef<RequestedProcedure>, WorklistQueryResult>();
            IRequestedProcedureBroker rpBroker = context.GetBroker<IRequestedProcedureBroker>();

            foreach (WorklistQueryResult queryResult in unFilteredList)
            {
                if (dictionary.ContainsKey(queryResult.RequestedProcedure) == false)
                    dictionary[queryResult.RequestedProcedure] = queryResult;
            }

            foreach (KeyValuePair<EntityRef<RequestedProcedure>, WorklistQueryResult> kvp in dictionary)
            {
                RequestedProcedure rp = rpBroker.Load(kvp.Key);
                rpBroker.LoadCheckInProcedureStepsForRequestedProcedure(rp);

                if (rp.CheckInProcedureSteps.Count > 0 && keepCheckIn == true
                    || rp.CheckInProcedureSteps.Count == 0 && keepCheckIn == false)
                {
                    filteredList.Add(kvp.Value);
                }
            }

            return filteredList;
        }

        public static IList<WorklistItem> ConvertQueryResultToWorkList(string workClassName, IList<WorklistQueryResult> listQueryResult)
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

        public static IList<WorklistItem> ConvertToWorkListItem(string workClassName, IList<PatientProfile> listProfile)
        {
            // Now add the worklist item 
            IList<WorklistItem> items = new List<WorklistItem>();
            foreach (PatientProfile profile in listProfile)
            {
                items.Add(new WorklistItem(workClassName, profile));
            }

            return items;
        }

        #endregion
    }
}
