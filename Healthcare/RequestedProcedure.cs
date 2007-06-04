using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Workflow;
using System.Collections.Generic;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// RequestedProcedure entity
    /// </summary>
	public partial class RequestedProcedure : Entity
	{
        private CheckInProcedureStep _cps;

        public RequestedProcedure(Order order, RequestedProcedureType type, string index)
        {
            _order = order;
            _type = type;
            _index = index;

            _procedureSteps = new HybridSet();
        }
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        private CheckInProcedureStep CheckInStep
        {
            get
            {
                if (_cps == null)
                {
                    _cps = (CheckInProcedureStep)CollectionUtils.SelectFirst<ProcedureStep>(this.ProcedureSteps,
                        delegate(ProcedureStep step)
                        {
                            return step is CheckInProcedureStep;
                        });
                }

                return _cps;
            }
        }

        private ICollection<ProcedureStep> ModalitySteps
        {
            get
            {
                return CollectionUtils.Select<ProcedureStep>(this.ProcedureSteps,
                    delegate(ProcedureStep ps)
                    {
                        return ps is ModalityProcedureStep;
                    });
            }
        }

        #region Public Operations

        public void Start(Staff currentUserStaff)
        {
            // The CPS should be created when each RP of an order is created, but just in case it's not
            if (this.CheckInStep == null)
            {
                _cps = new CheckInProcedureStep(this);
            }

            this.CheckInStep.Start(currentUserStaff);

            if (this.Order.Status == OrderStatus.SC)
                this.Order.Status = OrderStatus.IP;
        }

        public void Complete(Staff currentUserStaff)
        {
            if (this.CheckInStep.State == ActivityStatus.IP)
            {
                this.CheckInStep.Complete();
            }
            else if (this.CheckInStep.State == ActivityStatus.SC)
            {
                this.CheckInStep.Complete(currentUserStaff);
            }

            // Order is not complete until all the Reporting Step are complete
        }

        public void Discontinue()
        {
            this.CheckInStep.Discontinue();

            if (this.Order.IsAllRequestedProcedureDiscontinued == true)
            {
                this.Order.Status = OrderStatus.DC;
            }
        }

    #endregion

        #region Public Properties

        public ActivityStatus Status
        {
            get 
            {
                return this.CheckInStep == null ? ActivityStatus.SC : this.CheckInStep.State;
            }
        }

        public bool IsMpsStarted
        {
            get
            {
                return CollectionUtils.Contains<ProcedureStep>(this.ProcedureSteps,
                    delegate(ProcedureStep ps)
                    {
                        return (ps is ModalityProcedureStep && ps.State == ActivityStatus.IP);
                    });
            }
        }

        public bool IsAllMpsDiscontinued
        {
            get
            {
                return CollectionUtils.TrueForAll<ProcedureStep>(this.ModalitySteps,
                    delegate(ProcedureStep ps)
                    {
                        return (ps.State == ActivityStatus.DC);
                    });
            }
        }

        public bool IsAllMpsCompletedOrDiscontinued
        {
            get
            {
                return CollectionUtils.TrueForAll<ProcedureStep>(this.ModalitySteps,
                    delegate(ProcedureStep ps)
                    {
                        return (ps is ModalityProcedureStep && (ps.State == ActivityStatus.DC || ps.State == ActivityStatus.CM));
                    });
            }
        }

        #endregion

        #region Object overrides

        public override bool Equals(object that)
		{
			// TODO: implement a test for business-key equality
			return base.Equals(that);
		}
		
		public override int GetHashCode()
		{
			// TODO: implement a hash-code based on the business-key used in the Equals() method
			return base.GetHashCode();
		}
		
		#endregion

	}
}
