#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// Used to represent a specific parameter to a stored procedure.
    /// </summary>
    /// <typeparam name="T">The type associated with the parameter.</typeparam>
    public class ProcedureParameter<T> : SearchCriteria
    {
        private T _value;
		private bool _output = false;

		/// <summary>
		/// Constructor for input parameters.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
        public ProcedureParameter(String key, T value)
            : base(key)
        {
            _value = value;
        }

		/// <summary>
		/// Contructor for output parameters.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="output"></param>
		public ProcedureParameter(String key)
			: base(key)
		{
			_output = true;
		}

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected ProcedureParameter(ProcedureParameter<T> other)
            : base(other)
        {
            _value = other._value;
            _output = other._output;
        }

        public override object Clone()
        {
            return new ProcedureParameter<T>(this);
        }

		public bool Output
		{
			get { return _output; }
		}

		public T Value
		{
			get { return _value; }
			set { _value = value; }
		}
    }
}
