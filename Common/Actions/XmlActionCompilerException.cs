#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common.Actions
{
    /// <summary>
	/// Exception thrown by <see cref="XmlActionCompiler{TActionContext, TSchemaContext}"/> or extensions 
	/// implementing <see cref="XmlActionCompilerOperatorExtensionPoint{TActionContext, TSchemaContext}"/>.
    /// </summary>
    [Serializable]
    public class XmlActionCompilerException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Descriptive message associated with the exception.</param>
        public XmlActionCompilerException(string message)
            : base(message)
        {
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message">Descriptive message associated with the exception.</param>
		/// <param name="innerException">The inner exception.</param>
		public XmlActionCompilerException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
    }
}