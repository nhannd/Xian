#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{

    [ExtensionOf(typeof(ProcedureStepBuilderExtensionPoint))]
    public class ProtocolAssignmentStepBuilder : ProcedureStepBuilderBase
    {

        public override Type ProcedureStepClass
        {
            get { return typeof(ProtocolAssignmentStep); }
        }

        public override ProcedureStep CreateInstance(XmlElement xmlNode, Procedure procedure)
        {
            Protocol protocol = new Protocol(procedure);
            ProtocolAssignmentStep step = new ProtocolAssignmentStep(protocol);

            //note: this is not ideal but there is no other way to save the protocol object
            PersistenceScope.CurrentContext.Lock(protocol, DirtyState.New);

            return step;
        }

        public override void SaveInstance(ProcedureStep prototype, XmlElement xmlNode)
        {
            // nothing to do
        }
    }

    /// <summary>
    /// ProtocolAssignmentStep entity
    /// </summary>
    public partial class ProtocolAssignmentStep : ProtocolProcedureStep
    {
        public ProtocolAssignmentStep(Protocol protocol)
            : base(protocol)
        {
        }

        /// <summary>
        /// This method is called from the constructor.  Use this method to implement any custom
        /// object initialization.
        /// </summary>
        private void CustomInitialize()
        {
        }

        public virtual bool CanAccept
        {
            get { return this.State == ActivityStatus.IP; }
        }

        public virtual bool CanReject
        {
            get { return this.State == ActivityStatus.IP; }
        }

        public virtual bool CanSuspend
        {
            get { return this.State == ActivityStatus.IP; }
        }

        public virtual bool CanSave
        {
            get { return this.State == ActivityStatus.IP; }
        }

        public virtual bool CanApprove
        {
            get { return (this.State == ActivityStatus.SC || this.State == ActivityStatus.IP) && this.Protocol.Status == ProtocolStatus.AA; }
        }

        public bool CanEdit(Staff staff)
        {
            return this.State == ActivityStatus.IP && this.PerformingStaff == staff;
        }

        public override string Name
        {
            get { return "Protocol Assignment"; }
        }

		protected override void LinkProcedure(Procedure procedure)
		{
			if (this.Protocol == null)
				throw new WorkflowException("This step must be associated with a Protocol before procedures can be linked.");

			this.Protocol.LinkProcedure(procedure);
		}

        protected override ProcedureStep CreateScheduledCopy()
        {
            ProtocolAssignmentStep newStep = new ProtocolAssignmentStep(this.Protocol);
            this.Procedure.AddProcedureStep(newStep);
            return newStep;
        }
    }
}