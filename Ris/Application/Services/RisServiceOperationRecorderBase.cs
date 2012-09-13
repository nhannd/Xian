#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Application.Services
{

	/// <summary>
	/// Abstract base implementation of <see cref="IServiceOperationRecorder"/> for RIS application services.
	/// </summary>
	public abstract class RisServiceOperationRecorderBase : IServiceOperationRecorder
	{
		#region Data contracts

		[DataContract]
		public class PatientData
		{
			public PatientData(PatientProfile patientProfile)
			{
				Mrn = patientProfile.Mrn.ToString();
				Name = patientProfile.Name.ToString();
			}

			[DataMember]
			public string Mrn;

			[DataMember]
			public string Name;
		}

		[DataContract]
		public class OrderData
		{
			public OrderData(Order order)
			{
				AccessionNumber = order.AccessionNumber;
			}

			[DataMember]
			public string AccessionNumber;
		}

		[DataContract]
		public class ProcedureData
		{
			public ProcedureData(Procedure procedure)
			{
				this.Name = procedure.Type.Name;
				this.Id = procedure.Type.Id;
			}

			[DataMember]
			public string Id;

			[DataMember]
			public string Name;
		}

		[DataContract]
		public class ChangeSetData
		{
			[DataMember]
			public List<object> Actions;
		}

		[DataContract]
		public class OperationData
		{
			public OperationData(string operation)
			{
				Operation = operation;
			}

			public OperationData(string operation, PatientProfile patientProfile)
			{
				Operation = operation;
				Patient = new PatientData(patientProfile);
			}

			public OperationData(string operation, PatientProfile patientProfile, Order order)
			{
				Operation = operation;
				Patient = new PatientData(patientProfile);
				Order = new OrderData(order);
			}

			public OperationData(string operation, PatientProfile patientProfile, Order order, IEnumerable<Procedure> procedures)
			{
				Operation = operation;
				Patient = new PatientData(patientProfile);
				Order = new OrderData(order);
				Procedures = procedures.Select(rp => new ProcedureData(rp)).ToList();
			}

			[DataMember]
			public string Operation;

			[DataMember]
			public PatientData Patient;

			[DataMember]
			public OrderData Order;

			[DataMember]
			public List<ProcedureData> Procedures;

			[DataMember]
			public ChangeSetData ChangeSet;
		}

		#endregion


		private const string ApplicationName = "RIS";
		private const string CategoryName = "RIS";

		private OperationData _capturedData;
		private readonly HashSet<object> _changeSetIncludes = new HashSet<object>();

		#region IServiceOperationRecorder implementation

		string IServiceOperationRecorder.Application
		{
			get { return ApplicationName; }
		}

		string IServiceOperationRecorder.Category
		{
			get { return CategoryName; }
		}

		void IServiceOperationRecorder.PreCommit(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
		{
			if (!ShouldCapture(recorderContext, persistenceContext))
				return;

			_capturedData = Capture(recorderContext, persistenceContext);
		}

		void IServiceOperationRecorder.PostCommit(IServiceOperationRecorderContext recorderContext)
		{
			if (_capturedData == null)
				return;

			Write(recorderContext);
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Determines if data should be captured for the current invocation.
		/// </summary>
		/// <param name="recorderContext"></param>
		/// <param name="persistenceContext"></param>
		/// <returns>True if data should be captured, otherwise false.</returns>
		protected virtual bool ShouldCapture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
		{
			return true;
		}

		/// <summary>
		/// Called to capture data about the current invocation.
		/// </summary>
		/// <param name="recorderContext"></param>
		/// <param name="persistenceContext"></param>
		/// <returns>An <see cref="OperationData"/> contract instance, or a subclass of it.</returns>
		protected abstract OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext);

		/// <summary>
		/// Indicates that change-set information for the specified entity instance should be included in the message.
		/// </summary>
		/// <param name="entity"></param>
		protected void IncludeChangeSetFor(Entity entity)
		{
			_changeSetIncludes.Add(entity.OID.ToString());
		}

		/// <summary>
		/// Indicates that change-set information for all instances of the specified class of entity should be included in the message.
		/// </summary>
		/// <param name="entityClass"></param>
		protected void IncludeChangeSetFor(Type entityClass)
		{
			_changeSetIncludes.Add(entityClass.FullName);
		}

		#endregion

		#region Helpers

		private void Write(IServiceOperationRecorderContext recorderContext)
		{
			if (_changeSetIncludes.Any() && recorderContext.ChangeSet != null)
			{
				var changeSetData = DefaultEntityChangeSetRecorder.WriteChangeSet(_capturedData.Operation, recorderContext.ChangeSet.Changes);
				var includedActions = from action in changeSetData.Actions
									  where action.Type == "Update" && _changeSetIncludes.Contains(action.OID) || _changeSetIncludes.Contains(action.Class)
									  select (object)action;
				_capturedData.ChangeSet = new ChangeSetData {Actions = includedActions.ToList()};
			}

			var xml = JsmlSerializer.Serialize(_capturedData, "Audit");
			recorderContext.Write(_capturedData.Operation, xml);
		}

		#endregion
	}
}
