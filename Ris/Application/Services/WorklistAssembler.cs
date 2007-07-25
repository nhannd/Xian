using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Ris.Application.Services.Admin;
using ClearCanvas.Ris.Application.Services.Admin.AuthenticationAdmin;

namespace ClearCanvas.Ris.Application.Services
{
    internal class WorklistAssembler
    {
        public WorklistDetail GetWorklistDetail(Worklist worklist, IPersistenceContext context)
        {
            WorklistDetail detail = new WorklistDetail(worklist.GetRef(), worklist.Name, worklist.Description, WorklistFactory.Instance.GetWorklistType(worklist));

            RequestedProcedureTypeGroupAssembler assembler = new RequestedProcedureTypeGroupAssembler();
            detail.RequestedProcedureTypeGroups = CollectionUtils.Map<RequestedProcedureTypeGroup, RequestedProcedureTypeGroupSummary, List<RequestedProcedureTypeGroupSummary>>(
                worklist.RequestedProcedureTypeGroups,
                delegate(RequestedProcedureTypeGroup rptGroup)
                {
                    return assembler.GetRequestedProcedureTypeGroupSummary(rptGroup, context);
                });

            UserAssembler userAssembler = new UserAssembler();
            detail.Users = CollectionUtils.Map<User, UserSummary, List<UserSummary>>(
                worklist.Users,
                delegate(User user)
                {
                    return userAssembler.GetUserSummary(user);
                });

            return detail;
        }

        public WorklistSummary GetWorklistSummary(Worklist worklist, IPersistenceContext context)
        {
            return new WorklistSummary(worklist.GetRef(), worklist.Name, worklist.Description, WorklistFactory.Instance.GetWorklistType(worklist));
        }

        public void UpdateWorklist(Worklist worklist, WorklistDetail detail, IPersistenceContext context)
        {
            worklist.Name = detail.Name;
            worklist.Description = detail.Description;
            
            worklist.RequestedProcedureTypeGroups.Clear();
            detail.RequestedProcedureTypeGroups.ForEach(delegate(RequestedProcedureTypeGroupSummary summary)
            {
                worklist.RequestedProcedureTypeGroups.Add(context.Load<RequestedProcedureTypeGroup>(summary.EntityRef));
            });

            worklist.Users.Clear();
            detail.Users.ForEach(delegate (UserSummary summary)
            {
                 worklist.Users.Add(context.Load<User>(summary.EntityRef));
            });
        }
    }

    internal class WorklistFactory
    {
        private readonly Dictionary <string, Type> _worklistTypeMapping;
        private static readonly object _lock = new object();
        private static WorklistFactory _theInstance;

        private WorklistFactory()
        {
            // TODO: populate dictionary from worklist classes themselves, not hard-coded
            _worklistTypeMapping = new Dictionary<string, Type>();
            _worklistTypeMapping.Add("Registration - Checked In", typeof(RegistrationCheckedInWorklist));
            _worklistTypeMapping.Add("Registration - In Progress", typeof(RegistrationInProgressWorklist));
            _worklistTypeMapping.Add("Registration - Scheduled", typeof(RegistrationScheduledWorklist));
            _worklistTypeMapping.Add("Registration - Cancelled", typeof(RegistrationCancelledWorklist));
            _worklistTypeMapping.Add("Registration - Completed", typeof(RegistrationCompletedWorklist));
            _worklistTypeMapping.Add("Technologist - Checked In", typeof(TechnologistCheckedInWorklist));
            _worklistTypeMapping.Add("Technologist - In Progress", typeof(TechnologistInProgressWorklist));
            _worklistTypeMapping.Add("Technologist - Scheduled", typeof(TechnologistScheduledWorklist));
            _worklistTypeMapping.Add("Technologist - Cancelled", typeof(TechnologistCancelledWorklist));
            _worklistTypeMapping.Add("Technologist - Completed", typeof(TechnologistCompletedWorklist));
            _worklistTypeMapping.Add("Reporting - To Be Reported", typeof(ReportingToBeReportedWorklist));
        }

        public static WorklistFactory Instance
        {
            get
            {
                if(_theInstance == null)
                {
                    lock(_lock)
                    {
                        if(_theInstance == null) 
                            _theInstance = new WorklistFactory();
                    }
                }
                return _theInstance;
            }
        }

        public ICollection<string> WorklistTypes
        {
            get { return _worklistTypeMapping.Keys; }
        }

        public Worklist GetWorklist(string type)
        {
            return (Worklist)Activator.CreateInstance(GetWorklistType(type));
        }

        public Type GetWorklistType(string type)
        {
            try
            {
                return _worklistTypeMapping[type];
            }
            catch(KeyNotFoundException)
            {
                throw new RequestValidationException("Invalid worklist type");
            }
        }

        public string GetWorklistType(Worklist worklist)
        {
            Type worklistType = worklist.GetType();
            foreach (KeyValuePair<string, Type> pair in _worklistTypeMapping)
            {
                if (pair.Value == worklistType)
                    return pair.Key;
            }
            return "";
        }
    }
}