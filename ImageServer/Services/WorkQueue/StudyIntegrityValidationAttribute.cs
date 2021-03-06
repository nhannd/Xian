#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageServer.Core.Validation;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// Attribute to specify what type of validation must be made when a work queue entry is processed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StudyIntegrityValidationAttribute:Attribute
    {
        #region Private Memebers
        private StudyIntegrityValidationModes _validationTypes;
        private RecoveryModes _Recovery;
        #endregion

        #region Public Properties
        public StudyIntegrityValidationModes ValidationTypes
        {
            get { return _validationTypes; }
            set { _validationTypes = value; }
        }

        public RecoveryModes Recovery
        {
            get { return _Recovery; }
            set { _Recovery = value; }
        }

        #endregion
    }
}