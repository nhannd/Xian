#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// ProcedureType entity
    /// </summary>
	public partial class ProcedureType
    {
    	private ProcedurePlan _plan;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public ProcedureType(string id, string name)
            :this(id, name, null, null, 0)
        {
        }

        private string PlanXml { get { return _planXml; } }
        /// <summary>
        /// Sets the plan for this procedure type from the specified prototype procedure.
        /// </summary>
        /// <param name="prototype"></param>
        public virtual void SetPlanFromPrototype(Procedure prototype)
        {
			this.Plan = ProcedurePlan.CreateFromProcedure(prototype);
        }

		/// <summary>
		/// Gets or sets the procedure plan.
		/// </summary>
    	public virtual ProcedurePlan Plan
    	{
    		get
    		{
				if (_plan == null)
				{
					_plan = new ProcedurePlan(_planXml);
				}
    			return _plan;
    		}
			set
			{
				if(value == null)
					throw new InvalidOperationException("Value must not be null.");

				_plan = value;
				_planXml = value.ToString();
			}
    	}

        /// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}