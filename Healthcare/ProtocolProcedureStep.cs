#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
    public abstract class ProtocolProcedureStep : ProcedureStep
    {
        private Protocol _protocol;

        #region Constructors

        public ProtocolProcedureStep(Protocol protocol)
        {
            Platform.CheckForNullReference(protocol, "protocol");
            _protocol = protocol;
        }

        /// <summary>
        /// Default no-args constructor required by nHibernate
        /// </summary>
        protected ProtocolProcedureStep()
        {
        }

        #endregion

        public virtual Protocol Protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }

		public override bool CreateInDowntimeMode
		{
			get { return false; }
		}

		public override bool IsPreStep
		{
			get { return true; }
		}

		public override TimeSpan SchedulingOffset
		{
			get { return TimeSpan.MinValue; }
		}

		public override List<Procedure> GetLinkedProcedures()
		{
			if (_protocol != null)
			{
				return CollectionUtils.Select(_protocol.Procedures,
					delegate(Procedure p) { return !Equals(p, this.Procedure); });
			}
			else
			{
				return new List<Procedure>();
			}
		}

		protected override bool IsRelatedStep(ProcedureStep step)
		{
			// can't have relatives if no protocol
			if (this.Protocol == null)
				return false;

			// relatives must be protocol steps
			if (!step.Is<ProtocolProcedureStep>())
				return false;

			// check if tied to same protocol
			ProtocolProcedureStep that = step.As<ProtocolProcedureStep>();
			return that.Protocol != null && Equals(this.Protocol, that.Protocol);
		}
	}
}
