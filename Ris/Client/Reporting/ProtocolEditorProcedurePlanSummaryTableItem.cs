using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Reporting
{
    public class ProtocolEditorProcedurePlanSummaryTableItem
    {
        private readonly RequestedProcedureDetail _rpDetail;
        //private readonly ProtocolProcedureStepDetail _protocolStepDetail;
        private readonly ProtocolDetail _protocolDetail;
        private readonly EntityRef _protocolRef;

        //public ProtocolEditorProcedurePlanSummaryTableItem(RequestedProcedureDetail rpDetail, ProtocolProcedureStepDetail protocolStepDetail, ProtocolDetail protocolDetail)
        //{
        //    _rpDetail = rpDetail;
        //    _protocolStepDetail = protocolStepDetail;
        //    _protocolDetail = protocolDetail;
        //}


        public ProtocolEditorProcedurePlanSummaryTableItem(RequestedProcedureDetail _rpDetail, EntityRef _protocolRef, ProtocolDetail _protocolDetail)
        {
            this._rpDetail = _rpDetail;
            this._protocolDetail = _protocolDetail;
            this._protocolRef = _protocolRef;
        }

        #region Public Properties

        public RequestedProcedureDetail RequestedProcedureDetail
        {
            get { return _rpDetail; }
        }

        public EntityRef ProtocolRef
        {
            get { return _protocolRef; }
        }
        //public ProtocolProcedureStepDetail ProtocolStepDetail
        //{
        //    get { return _protocolStepDetail; }
        //}

        public ProtocolDetail ProtocolDetail
        {
            get { return _protocolDetail; }
        }

        #endregion
    }
}