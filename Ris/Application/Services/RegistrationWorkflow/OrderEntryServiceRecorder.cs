#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
	class OrderEntryServiceRecorder
	{
		static class Operations
		{
			public const string New = "Order:New";
			public const string Modify = "Order:Modify";
			public const string Replace = "Order:Replace";
			public const string Cancel = "Order:Cancel";
			public const string Merge = "Order:Merge";
			public const string Unmerge = "Order:Unmerge";
		}

		internal class PlaceOrder : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (PlaceOrderRequest)recorderContext.Request;
				var profile = persistenceContext.Load<PatientProfile>(request.Requisition.Patient.PatientProfileRef, EntityLoadFlags.None);
				
				var response = (PlaceOrderResponse) recorderContext.Response;
				var order = persistenceContext.Load<Order>(response.Order.OrderRef, EntityLoadFlags.None);

				return new OperationData(Operations.New, profile, order);
			}
		}

		internal class ReplaceOrder : RisServiceOperationRecorderBase
		{
			[DataContract]
			public class ReplaceOrderOperationData : OperationData
			{
				public ReplaceOrderOperationData(string operation, PatientProfile patientProfile, Order cancelledOrder, Order newOrder)
					: base(operation, patientProfile)
				{
					this.CancelledOrder = new OrderData(cancelledOrder);
					this.NewOrder = new OrderData(newOrder);
				}

				[DataMember]
				public OrderData CancelledOrder;

				[DataMember]
				public OrderData NewOrder;
			}

			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (ReplaceOrderRequest)recorderContext.Request;
				var profile = persistenceContext.Load<PatientProfile>(request.Requisition.Patient.PatientProfileRef, EntityLoadFlags.None);
				var cancelledOrder = persistenceContext.Load<Order>(request.Requisition.OrderRef, EntityLoadFlags.None);

				var response = (ReplaceOrderResponse)recorderContext.Response;
				var newOrder = persistenceContext.Load<Order>(response.Order.OrderRef, EntityLoadFlags.None);

				return new ReplaceOrderOperationData(Operations.Replace, profile, cancelledOrder, newOrder);
			}
		}

		internal class ModifyOrder : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (ModifyOrderRequest)recorderContext.Request;
				var profile = persistenceContext.Load<PatientProfile>(request.Requisition.Patient.PatientProfileRef, EntityLoadFlags.None);
				var order = persistenceContext.Load<Order>(request.Requisition.OrderRef, EntityLoadFlags.None);

				IncludeChangeSetFor(order);
				IncludeChangeSetFor(typeof(Procedure));

				return new OperationData(Operations.Modify, profile, order);
			}
		}

		internal class CancelOrder : RisServiceOperationRecorderBase
		{
			[DataContract]
			public class CancelOrderOperationData : OperationData
			{
				public CancelOrderOperationData(string operation, PatientProfile patientProfile, Order order)
					: base(operation, patientProfile, order)
				{
					this.CancelReason = EnumUtils.GetEnumValueInfo(order.CancelInfo.Reason);
				}

				[DataMember]
				public EnumValueInfo CancelReason;
			}


			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (CancelOrderRequest)recorderContext.Request;
				var order = persistenceContext.Load<Order>(request.OrderRef, EntityLoadFlags.None);
				var patientProfile = order.Procedures.First().PatientProfile;	// choose patient profile from one procedure?

				return new CancelOrderOperationData(Operations.Cancel, patientProfile, order);
			}
		}

		internal class MergeOrder : RisServiceOperationRecorderBase
		{
			[DataContract]
			public class MergeOrderOperationData : OperationData
			{
				public MergeOrderOperationData(string operation, PatientProfile patientProfile, Order newOrder, IEnumerable<Order> mergedOrders)
					: base(operation, patientProfile)
				{
					this.NewOrder = new OrderData(newOrder);
					this.MergedOrders = mergedOrders.Select(x => new OrderData(x)).ToList();
				}

				[DataMember]
				public OrderData NewOrder;

				[DataMember]
				public List<OrderData> MergedOrders;
			}

			protected override bool ShouldCapture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (MergeOrderRequest)recorderContext.Request;
				return !(request.DryRun || request.ValidationOnly);
			}

			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (MergeOrderRequest)recorderContext.Request;
				var destOrder = persistenceContext.Load<Order>(request.DestinationOrderRef, EntityLoadFlags.None);
				var patientProfile = destOrder.Procedures.First().PatientProfile;	// choose patient profile from one procedure?
				var sourceOrders = request.SourceOrderRefs.Select(persistenceContext.Load<Order>);

				return new MergeOrderOperationData(Operations.Merge, patientProfile, destOrder, sourceOrders);
			}
		}

		internal class UnmergeOrder : RisServiceOperationRecorderBase
		{
			protected override bool ShouldCapture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (UnmergeOrderRequest)recorderContext.Request;
				return !request.DryRun;
			}

			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (UnmergeOrderRequest)recorderContext.Request;
				var order = persistenceContext.Load<Order>(request.OrderRef, EntityLoadFlags.None);
				var patientProfile = order.Procedures.First().PatientProfile;	// choose patient profile from one procedure?
				return new OperationData(Operations.Unmerge, patientProfile, order);
			}
		}

	}
}
