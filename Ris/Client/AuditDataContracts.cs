#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	[DataContract]
	public class PatientAuditData
	{
		public PatientAuditData(CompositeIdentifierDetail mrn, PersonNameDetail name)
		{
			Mrn = string.Format("{0} {1}", mrn.AssigningAuthority.Code, mrn.Id);
			Name = string.IsNullOrEmpty(name.MiddleName) ? 
				string.Format("{0}, {1}", name.FamilyName, name.GivenName) 
				: string.Format("{0}, {1} {2}", name.FamilyName, name.GivenName, name.MiddleName);
		}

		[DataMember]
		public string Mrn;

		[DataMember]
		public string Name;
	}

	[DataContract]
	public class OrderAuditData
	{
		public OrderAuditData(string accessionNumber)
		{
			AccessionNumber = accessionNumber;
		}

		[DataMember]
		public string AccessionNumber;
	}

	[DataContract]
	public class ProcedureAuditData
	{
		public ProcedureAuditData(string procedureName)
		{
			this.Name = procedureName;
		}

		[DataMember]
		public string Name;
	}

	[DataContract]
	public class OperationAuditData
	{
		public OperationAuditData(string operation, CompositeIdentifierDetail mrn, PersonNameDetail name)
		{
			this.Operation = operation;
			this.Patient = new PatientAuditData(mrn, name);
		}

		public OperationAuditData(string operation, CompositeIdentifierDetail mrn, PersonNameDetail name, string accessionNumber)
		{
			this.Operation = operation;
			this.Patient = new PatientAuditData(mrn, name);
			this.Order = new OrderAuditData(accessionNumber);
		}

		public OperationAuditData(string operation, CompositeIdentifierDetail mrn, PersonNameDetail name, string accessionNumber, string procedureName)
		{
			this.Operation = operation;
			this.Patient = new PatientAuditData(mrn, name);
			this.Order = new OrderAuditData(accessionNumber);
			this.Procedure = new ProcedureAuditData(procedureName);
		}

		public OperationAuditData(string operation, WorklistItemSummaryBase worklistItem)
			:this(operation, worklistItem.Mrn, worklistItem.PatientName, worklistItem.AccessionNumber, worklistItem.ProcedureName)
		{
		}

		[DataMember]
		public string Operation;

		[DataMember]
		public PatientAuditData Patient;

		[DataMember]
		public OrderAuditData Order;

		[DataMember]
		public ProcedureAuditData Procedure;
	}

	[DataContract]
	public class PreviewOperationAuditData : OperationAuditData
	{
		public PreviewOperationAuditData(string folderSystem, WorklistItemSummaryBase worklistItem)
			: base(AuditHelper.Operations.FolderItemPreview, worklistItem)
		{
			this.FolderSystem = folderSystem;
		}

		public PreviewOperationAuditData(string folderSystem, CompositeIdentifierDetail mrn, PersonNameDetail name, string accessionNumber, string procedureName)
			: base(AuditHelper.Operations.FolderItemPreview, mrn, name, accessionNumber, procedureName)
		{
			this.FolderSystem = folderSystem;
		}

		public PreviewOperationAuditData(string folderSystem, CompositeIdentifierDetail mrn, PersonNameDetail name, string accessionNumber)
			: base(AuditHelper.Operations.FolderItemPreview, mrn, name, accessionNumber)
		{
			this.FolderSystem = folderSystem;
		}

		[DataMember]
		public string FolderSystem;
	}

	[DataContract]
	public class OpenWorkspaceOperationAuditData : OperationAuditData
	{
		public OpenWorkspaceOperationAuditData(string workspace, WorklistItemSummaryBase worklistItem)
			: base(AuditHelper.Operations.DocumentWorkspaceOpen, worklistItem)
		{
			this.Workspace = workspace;
		}

		public OpenWorkspaceOperationAuditData(string workspace, CompositeIdentifierDetail mrn, PersonNameDetail name)
			: base(AuditHelper.Operations.DocumentWorkspaceOpen, mrn, name)
		{
			this.Workspace = workspace;
		}

		[DataMember]
		public string Workspace;
	}
}
