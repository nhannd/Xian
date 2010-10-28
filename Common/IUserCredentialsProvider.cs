#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common
{
    /// <summary>
    /// Defines an interface that supplies user credentials.
    /// </summary>
    public interface IUserCredentialsProvider
    {
        /// <summary>
        /// Gets the user-name.
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// Gets the session token ID.
        /// </summary>
        string SessionTokenId { get; }
    }
}
