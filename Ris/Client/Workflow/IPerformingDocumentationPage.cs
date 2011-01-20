#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
    public class ProcedurePlanChangedEventArgs : EventArgs
    {
        private readonly ProcedurePlanDetail _procedurePlanDetail;

        public ProcedurePlanChangedEventArgs(ProcedurePlanDetail procedurePlanDetail)
        {
            _procedurePlanDetail = procedurePlanDetail;
        }

        public ProcedurePlanDetail ProcedurePlanDetail
        {
            get { return _procedurePlanDetail; }
        }
    }

    /// <summary>
    /// Defines an interface to a custom documentation page.
    /// </summary>
    public interface IPerformingDocumentationPage : IExtensionPage
    {
        bool Save(bool complete);
    }
}