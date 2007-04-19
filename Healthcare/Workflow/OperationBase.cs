using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common.Specifications;
using System.Collections;

namespace ClearCanvas.Healthcare.Workflow
{
    public abstract class OperationBase : IOperation
    {
        private Staff _currentUserStaff;

        class ValidInput : ISpecification
        {
            private OperationBase _owner;

            public ValidInput(OperationBase owner)
            {
                _owner = owner;
            }

            #region ISpecification Members

            public TestResult Test(object obj)
            {
                WorklistItemBase item = (WorklistItemBase)obj;
                return new TestResult(_owner.CanExecute(item));
            }

            public IEnumerable<ISpecification> SubSpecs
            {
                get { return new ISpecification[] { }; }
            }

            #endregion
        }

        #region IOperation Members

        public ISpecification InputSpecification
        {
            get { return new ValidInput(this); }
        }

        public abstract void Execute(IWorklistItem item, IList parameters, IWorkflow workflow);

        public Staff CurrentUserStaff
        {
            get { return _currentUserStaff; }
            set { _currentUserStaff = value; }
        }

        #endregion

        protected virtual bool CanExecute(IWorklistItem item)
        {
            return true;
        }
    }
}
