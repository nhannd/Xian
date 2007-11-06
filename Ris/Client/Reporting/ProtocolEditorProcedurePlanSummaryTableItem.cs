using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Reporting
{
    public class ProtocolEditorProcedurePlanSummaryTableItem
    {
        private readonly RequestedProcedureDetail _rpDetail;
        private readonly ProtocolProcedureStepDetail _protocolStepDetail;

        public ProtocolEditorProcedurePlanSummaryTableItem(RequestedProcedureDetail rpDetail, ProtocolProcedureStepDetail protocolStepDetail)
        {
            _rpDetail = rpDetail;
            _protocolStepDetail = protocolStepDetail;
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

        #endregion
    }
}