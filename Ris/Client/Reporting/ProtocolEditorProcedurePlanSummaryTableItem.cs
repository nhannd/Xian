using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Reporting
{
    public class ProtocolEditorProcedurePlanSummaryTableItem
    {
        private readonly RequestedProcedureDetail _rpDetail;
        private readonly ProtocolProcedureStepDetail _protocolStepDetail;
        private readonly ProtocolDetail _protocolDetail;

        public ProtocolEditorProcedurePlanSummaryTableItem(RequestedProcedureDetail rpDetail, ProtocolProcedureStepDetail protocolStepDetail, ProtocolDetail protocolDetail)
        {
            _rpDetail = rpDetail;
            _protocolStepDetail = protocolStepDetail;
            _protocolDetail = protocolDetail;
        }

        #region Public Properties

        public RequestedProcedureDetail RequestedProcedureDetail
        {
            get { return _rpDetail; }
        }

        public ProtocolProcedureStepDetail ProtocolStepDetail
        {
            get { return _protocolStepDetail; }
        }

        public ProtocolDetail ProtocolDetail
        {
            get { return _protocolDetail; }
        }

        #endregion
    }
}