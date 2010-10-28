#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// ProtocolResolutionStep entity
    /// </summary>
    public partial class ProtocolResolutionStep : ClearCanvas.Healthcare.ProtocolProcedureStep
    {
        public ProtocolResolutionStep(Protocol protocol) : base(protocol)
        {
        }

        /// <summary>
        /// This method is called from the constructor.  Use this method to implement any custom
        /// object initialization.
        /// </summary>
        private void CustomInitialize()
        {
        }

        public bool ShouldCancel
        {
            get { return this.Protocol.Status == ProtocolStatus.RJ; }
        }

        public override string Name
        {
            get { return "Protocol Resolution"; }
        }

        protected override ProcedureStep CreateScheduledCopy()
		{
			ProtocolResolutionStep newStep = new ProtocolResolutionStep(this.Protocol);
			this.Procedure.AddProcedureStep(newStep);
			return newStep;
		}
	}
}