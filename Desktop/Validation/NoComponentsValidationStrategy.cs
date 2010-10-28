#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Implements a validation strategy that does not consider any nodes.
    /// </summary>
    /// <remarks>
	/// This is effectively equivalent to having no validation at all.  The 
	/// container is always considered valid, regardless of the validity
	/// of contained nodes.
	/// </remarks>
    public class NoComponentsValidationStrategy : IApplicationComponentContainerValidationStrategy
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		public NoComponentsValidationStrategy()
		{
		}

    	#region IApplicationComponentContainerValidationStrategy Members

		/// <summary>
		/// Returns false.
		/// </summary>
        public bool HasValidationErrors(IApplicationComponentContainer container)
        {
            return false;
        }

		/// <summary>
		/// Does nothing.
		/// </summary>
        public void ShowValidation(IApplicationComponentContainer container, bool show)
        {
            // do nothing
        }

        #endregion
    }
}
