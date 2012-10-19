#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	public class ProtocolEditorProcedurePlanSummaryTableItem
	{
		private readonly ProcedureDetail _rpDetail;
		private readonly ProtocolDetail _protocolDetail;

		public ProtocolEditorProcedurePlanSummaryTableItem(ProcedureDetail _rpDetail, ProtocolDetail _protocolDetail)
		{
			this._rpDetail = _rpDetail;
			this._protocolDetail = _protocolDetail;
		}

		#region Public Properties

		public ProcedureDetail ProcedureDetail
		{
			get { return _rpDetail; }
		}

		public ProtocolDetail ProtocolDetail
		{
			get { return _protocolDetail; }
		}

		#endregion
	}
}