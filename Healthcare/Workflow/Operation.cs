using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Healthcare.Workflow
{
    public abstract class Operation : IOperation
    {
        private Staff _currentUserStaff;

        class ValidInput : ISpecification
        {
            private Operation _owner;

            public ValidInput(Operation owner)
            {
                _owner = owner;
            }

            #region ISpecification Members

            public TestResult Test(object obj)
            {
                ProcedureStep ps = (ProcedureStep)obj;
                return new TestResult(_owner.CanExecute(ps));
            }

            public IEnumerable<ISpecification> SubSpecs
            {
                get { return new ISpecification[] { }; }
            }

            #endregion
        }

        protected abstract void Execute(ProcedureStep step, IWorkflow workflow);
        protected virtual bool CanExecute(ProcedureStep step)
        {
            return false;
        }

        public ISpecification InputSpecification
        {
            get
            {
                return new ValidInput(this);
            }
        }

        public void Execute(Activity input, IWorkflow workflow)
        {
            //if (this.InputSpecification.Test(input).Fail)
            //    throw new WorkflowException("operation cannot be applied to this procedure step");

            // delegate to the protected overload
            this.Execute((ProcedureStep)input, workflow);
        }

        public Staff CurrentUserStaff
        {
            get
            {
                return _currentUserStaff;
            }
            set
            {
                _currentUserStaff = value;
            }
        }
    }
}
