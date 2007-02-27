using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Abstract base class for all service layer classes.
    /// </summary>
    public abstract class ServiceLayer : IServiceLayer
    {
        private static Dictionary<Type, object> _mapEnumTables;

        static ServiceLayer()
        {
            _mapEnumTables = new Dictionary<Type, object>();
        }

        private IPersistenceContext _currentContext;

        protected ServiceLayer()
        {
        }

        /// <summary>
        /// Allows the current context to be set by the framework
        /// </summary>
        public IPersistenceContext CurrentContext
        {
            get { return _currentContext; }
            internal set { _currentContext = value; }
        }
    }
}
