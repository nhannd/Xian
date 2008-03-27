using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare
{
    public abstract class WorklistFilter : ValueObject
    {
        private bool _isEnabled;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }
    }

    public abstract class WorklistSingleValuedFilter<T> : WorklistFilter, IEquatable<WorklistSingleValuedFilter<T>>
    {
        private T _value;

        /// <summary>
        /// Constructor.
        /// </summary>
        public WorklistSingleValuedFilter()
        {
        }

        public T Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #region Object overrides

        public override object Clone()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Equals(WorklistSingleValuedFilter<T> that)
        {
            if (that == null) return false;
            return Equals(_value, that._value) && Equals(this.IsEnabled, that.IsEnabled);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as WorklistSingleValuedFilter<T>);
        }

        public override int GetHashCode()
        {
            return (_value != null ? _value.GetHashCode() : 0) + 29*IsEnabled.GetHashCode();
        }

        #endregion
    }

    public abstract class WorklistMultiValuedFilter<T> : WorklistFilter, IEquatable<WorklistMultiValuedFilter<T>>
    {
        private ISet<T> _values;

        /// <summary>
        /// Constructor.
        /// </summary>
        public WorklistMultiValuedFilter()
	    {
            _values = new HashedSet<T>();
	    }

        public ISet<T> Values
        {
            get { return _values; }
            // private setter for NHibernate compatibility
            private set { _values = value; }
        }

        #region Object overrides

        public override object Clone()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Equals(WorklistMultiValuedFilter<T> that)
        {
            if (that == null) return false;
            return Equals(IsEnabled, that.IsEnabled) && Equals(_values, that._values);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as WorklistMultiValuedFilter<T>);
        }

        public override int GetHashCode()
        {
            return _values.GetHashCode() + 29 * IsEnabled.GetHashCode();
        }

        #endregion
    }

    public class WorklistProcedureTypeGroupFilter : WorklistMultiValuedFilter<ProcedureTypeGroup>
    {
    }

    public class WorklistFacilityFilter : WorklistMultiValuedFilter<Facility>
    {
        private bool _includeWorkingFacility;

        public bool IncludeWorkingFacility
        {
            get { return _includeWorkingFacility; }
            set { _includeWorkingFacility = value; }
        }
    }

    public class WorklistPatientClassFilter : WorklistMultiValuedFilter<PatientClassEnum>
    {
    }

    public class WorklistOrderPriorityFilter : WorklistMultiValuedFilter<OrderPriorityEnum>
    {
    }

    public class WorklistPortableFilter : WorklistSingleValuedFilter<bool>
    {
    }

    public class WorklistTimeFilter: WorklistSingleValuedFilter<WorklistTimeRange>
    {
    }

}
