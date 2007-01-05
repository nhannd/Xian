using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Server.ShredHost
{
    public abstract class MarshallableList<TContainedType> : MarshalByRefObject, IEnumerable<TContainedType>
    {
        public MarshallableList()
        {
            _internalList = new List<TContainedType>();
        }

        protected ReadOnlyCollection<TContainedType> ContainedObjects
        {
            get { return _internalList.AsReadOnly(); }
        }

        public void Add(TContainedType objectToContain)
        {
            _internalList.Add(objectToContain);
        }

        public override object InitializeLifetimeService()
        {
            // I can't find any documentation yet, that says that returning null 
            // means that the lifetime of the object should not expire after a timeout
            // but the initial solution comes from this page: http://www.dotnet247.com/247reference/msgs/13/66416.aspx
            return null;
        }

        #region Private Members
        List<TContainedType> _internalList;
        #endregion

        #region IEnumerable<TContainedType> Members

        public IEnumerator<TContainedType> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        #endregion

    }
}
