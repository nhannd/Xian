#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// Interface for retrieving enumerated values from the database.
    /// </summary>
    /// <typeparam name="TOutput"></typeparam>
    public interface IEnumBroker<TOutput> : IPersistenceBroker
        where TOutput : ServerEnum, new()
    {
        /// <summary>
        /// Retrieves all enums.
        /// </summary>
        /// <returns></returns>
        List<TOutput> Execute();
    }
}
