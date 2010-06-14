#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Application.Services
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class OperationEnablementAttribute : Attribute
	{
		private readonly string _enablementMethodName;

		public OperationEnablementAttribute(string enablementMethodName)
		{
			_enablementMethodName = enablementMethodName;
		}

		public string EnablementMethodName
		{
			get { return _enablementMethodName; }
		}
	}


	public abstract class WorkflowServiceBase : ApplicationServiceBase, IWorkflowService
    {
		#region ProbingWorklistQueryContext

		class ProbingWorklistQueryContext : IWorklistQueryContext
		{
			private readonly WorklistQueryContext _wqc;

			/// <summary>
			/// Gets a value indicating if the worklist depends on the executing staff.
			/// </summary>
			public bool DependsOnExecutingStaff { get; internal set; }

			/// <summary>
			/// Gets a value indicating if the worklist depends on the working facility.
			/// </summary>
			public bool DependsOnWorkingFacility { get; internal set; }

			public ProbingWorklistQueryContext(ApplicationServiceBase service, Facility workingFacility, SearchResultPage page, bool downtimeRecoveryMode)
			{
				_wqc = new WorklistQueryContext(service, workingFacility, page, downtimeRecoveryMode);
			}

			public Staff Staff
			{
				get
				{
					DependsOnExecutingStaff = true;
					return _wqc.Staff;
				}
			}

			public Facility WorkingFacility
			{
				get
				{
					DependsOnWorkingFacility = true;
					return _wqc.WorkingFacility;
				}
			}

			public bool DowntimeRecoveryMode
			{
				get { return _wqc.DowntimeRecoveryMode; }
			}

			public SearchResultPage Page
			{
				get { return _wqc.Page; }
			}

			public TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker
			{
				return _wqc.GetBroker<TBrokerInterface>();
			}
		}

		#endregion



		#region IWorkflowService implementation

		/// <summary>
		/// Obtain the list of worklists for the current user.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[ReadOperation]
		public ListWorklistsForUserResponse ListWorklistsForUser(ListWorklistsForUserRequest request)
		{
			var assembler = new WorklistAssembler();
			return new ListWorklistsForUserResponse(
				CollectionUtils.Map<Worklist, WorklistSummary>(
					PersistenceContext.GetBroker<IWorklistBroker>().Find(CurrentUserStaff, request.WorklistTokens),
					worklist => assembler.GetWorklistSummary(worklist, PersistenceContext)));
		}

		[ReadOperation]
		public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
		{
			//TODO: is this appropriate? or should we throw an exception?
			if (request.WorkItem == null)
				return new GetOperationEnablementResponse(new Dictionary<string, bool>());

			return new GetOperationEnablementResponse(GetOperationEnablement(GetWorkItemKey(request.WorkItem)));
		}


		#endregion


		#region Protected API

		/// <summary>
		/// Extracts a work-item key from the specified work-item, or returns null if the item cannot be converted to a key.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected abstract object GetWorkItemKey(object item);


		/// <summary>
		/// Helper method to query a worklist.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <typeparam name="TSummary"></typeparam>
		/// <param name="request"></param>
		/// <param name="mapCallback"></param>
		/// <returns></returns>
		protected QueryWorklistResponse<TSummary> QueryWorklistHelper<TItem, TSummary>(QueryWorklistRequest request,
            Converter<TItem, TSummary> mapCallback)
        {
            IWorklist worklist = GetWorklist(request);

            IList results = null;
            var page = new SearchResultPage(request.Page.FirstRow, request.Page.MaxRows);
			var workingFacility = GetWorkingFacility(request);
            if(request.QueryItems)
            {
                // get the first page, up to the default max number of items per page
                results = worklist.GetWorklistItems(new WorklistQueryContext(this, workingFacility, page, request.DowntimeRecoveryMode));
            }

            var count = -1;
            if(request.QueryCount)
            {
                // if the items were already queried, and the number returned is less than the max per page, and this is the first page
                // then there is no need to do a separate count query
                if (results != null && results.Count < page.MaxRows && page.FirstRow == 0)
                    count = results.Count;
                else
					count = worklist.GetWorklistItemCount(new WorklistQueryContext(this, workingFacility, null, request.DowntimeRecoveryMode));
            }

            return new QueryWorklistResponse<TSummary>(
                request.QueryItems ? CollectionUtils.Map(results, mapCallback) : null, count);
        }

		protected ResponseCachingDirective GetQueryWorklistCacheDirective(object request, object response)
		{
			var req = (QueryWorklistRequest)request;

			// items queries are never cached
			if(req.QueryItems)
				return ResponseCachingDirective.DoNotCacheDirective;

			// otherwise, check the settings to see if caching is enabled
			var settings = new WorklistSettings();
			if(!settings.WorklistItemCountCachingEnabled)
				return ResponseCachingDirective.DoNotCacheDirective;

			// check if the worklist is user-affine (dependent upon the current user),
			// in which case the response cannot be cached
			if (IsWorklistUserAffine(req))
				return ResponseCachingDirective.DoNotCacheDirective;

			// return cache directive according to settings
			return new ResponseCachingDirective(
				settings.WorklistItemCountCachingEnabled,
				TimeSpan.FromSeconds(settings.WorklistItemCountCachingTimeToLiveSeconds),
				ResponseCachingSite.Server);
		}


		/// <summary>
		/// Helper method that implements the logic for performing searches on worklists.
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <typeparam name="TSummary"></typeparam>
		/// <param name="request"></param>
		/// <param name="broker"></param>
		/// <param name="projection"></param>
		/// <param name="mapCallback"></param>
		/// <returns></returns>
		protected TextQueryResponse<TSummary> SearchHelper<TItem, TSummary>(
			WorklistItemTextQueryRequest request,
			IWorklistItemBroker broker,
			WorklistItemProjection projection,
			Converter<TItem, TSummary> mapCallback)
			where TSummary : DataContractBase
			where TItem : WorklistItem
		{
			var procedureStepClass = request.ProcedureStepClassName == null ? null
				: ProcedureStep.GetSubClass(request.ProcedureStepClassName, PersistenceContext);

			var helper = new WorklistItemTextQueryHelper<TItem, TSummary>(broker, mapCallback, procedureStepClass, projection, request.Options, PersistenceContext);

			return helper.Query(request);
		}

		#endregion

		#region Private

		private Worklist GetWorklist(QueryWorklistRequest request)
		{
			return request.WorklistRef != null ?
				this.PersistenceContext.Load<Worklist>(request.WorklistRef) :
				WorklistFactory.Instance.CreateWorklist(request.WorklistClass);
		}

		private Facility GetWorkingFacility(QueryWorklistRequest request)
		{
			return request.WorkingFacilityRef == null ? null : 
				PersistenceContext.Load<Facility>(request.WorkingFacilityRef, EntityLoadFlags.Proxy);
		}


		private bool IsWorklistUserAffine(QueryWorklistRequest req)
		{
			// check if the worklist has a dependency on the executing staff (eg current user)
			var worklist = GetWorklist(req);
			var workingFacility = GetWorkingFacility(req);
			var probingWqc = new ProbingWorklistQueryContext(this, workingFacility, req.Page, req.DowntimeRecoveryMode);

			// get the worklist to apply all of its criteria
			// (TODO: would be better to encapsulate this in the model somehow)
			worklist.GetInvariantCriteria(probingWqc);
			worklist.GetFilterCriteria(probingWqc);

			// return value indicating dependency on executing staff
			return probingWqc.DependsOnExecutingStaff;
		}

		/// <summary>
		/// Helper method to determine operation enablement for.
		/// </summary>
		/// <param name="itemKey"></param>
		/// <returns></returns>
		private Dictionary<string, bool> GetOperationEnablement(object itemKey)
		{
			var results = new Dictionary<string, bool>();
			if (itemKey == null)
				return results;

			var serviceContractType = this.GetType();
			foreach (var info in serviceContractType.GetMethods())
			{
				var attribs = info.GetCustomAttributes(typeof(OperationEnablementAttribute), true);
				if (attribs.Length < 1)
					continue;

				// Evaluate the list of enablement method in the OperationEnablementAttribute

				var enablement = true;
				foreach (var obj in attribs)
				{
					var attrib = (OperationEnablementAttribute)obj;

					var enablementHelper = serviceContractType.GetMethod(attrib.EnablementMethodName);
					if (enablementHelper == null)
						throw new EnablementMethodNotFoundException(attrib.EnablementMethodName, info.Name);

					var test = (bool)enablementHelper.Invoke(this, new [] { itemKey });
					if (test == false)
					{
						// No need to continue after any evaluation failed
						enablement = false;
						break;
					}
				}

				results.Add(info.Name, enablement);
			}

			return results;
		}

		protected void CreateLogicalHL7Event(Order order, string type)
		{
			if (new LogicalHL7EventSettings().LogicalHL7EventsEnabled)
			{
				var logicalEvent = LogicalHL7EventWorkQueueItem.CreateOrderLogicalEvent(type, order);
				this.PersistenceContext.Lock(logicalEvent.Item, DirtyState.New);
			}
		}

		protected void CreateLogicalHL7Event(Procedure procedure, string type)
		{
			if (new LogicalHL7EventSettings().LogicalHL7EventsEnabled)
			{
				var logicalEvent = LogicalHL7EventWorkQueueItem.CreateProcedureLogicalEvent(type, procedure);
				this.PersistenceContext.Lock(logicalEvent.Item, DirtyState.New);
			}
		}

		#endregion

	}
}
