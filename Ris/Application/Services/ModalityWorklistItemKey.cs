#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class ModalityWorklistItemKey
    {
        private readonly EntityRef _procedureStepRef;

        public ModalityWorklistItemKey(EntityRef procedureStepRef)
        {
            _procedureStepRef = procedureStepRef;
        }

        public EntityRef ProcedureStepRef
        {
            get { return _procedureStepRef; }
        }
    }
}
