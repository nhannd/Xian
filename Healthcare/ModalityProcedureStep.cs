using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// ModalityProcedureStep entity
    /// </summary>
	public partial class ModalityProcedureStep : ProcedureStep
	{
        private ModalityProcedureStepType _type;
        private Modality _modality;

        public ModalityProcedureStep(RequestedProcedure procedure, ModalityProcedureStepType type, Modality modality)
            :base(procedure)
        {
            this.Type = type;
            this.Modality = modality;
        }

        /// <summary>
        /// Default no-args constructor required by NHibernate
        /// </summary>
        public ModalityProcedureStep()
        {
        }

        public override string Name
        {
            get { return _type.Name; }
        }

        public virtual ModalityProcedureStepType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public virtual Modality Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }

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