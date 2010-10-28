#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageServer.Core.Validation
{
    [Flags]
    public enum StudyIntegrityValidationModes
    {
        
        /// <summary>
        /// Validate all
        /// </summary>
        Default,


        /// <summary>
        /// Do not validate
        /// </summary>
        None,
        
        /// <summary>
        /// Validate the instance count in the database and the xml.
        /// </summary>
        InstanceCount
    }
}