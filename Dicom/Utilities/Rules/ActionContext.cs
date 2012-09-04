#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Utilities.Command;

namespace ClearCanvas.Dicom.Utilities.Rules
{
    /// <summary>
    /// Context used with <see cref="RulesEngine{TActionContext,TTypeEnum}"/>.
    /// </summary>
    public abstract class ActionContext
    {
        #region Public Properties

        /// <summary>
        /// The message being worked against.
        /// </summary>
        public DicomMessageBase Message { get; set; }

        /// <summary>
        /// The command processor.
        /// </summary>
        public CommandProcessor CommandProcessor { get; set; }

        #endregion

    }
}
